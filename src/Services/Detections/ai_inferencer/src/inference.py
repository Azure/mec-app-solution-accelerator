from dapr.clients import DaprClient
import time
import json
import base64
import cv2
import numpy as np
import avro.schema
from avro_json_serializer import AvroJsonSerializer
import logging
import os
import shared.minio_utils as minio
import paho.mqtt.client as mqtt

def PublishEvent(pubsub_name: str, topic_name: str, data: json):
    unacked_publish = set()
    mqttc = mqtt.Client(mqtt.CallbackAPIVersion.VERSION2)
    mqttc.on_publish = on_publish

    mqttc.user_data_set(unacked_publish)
    mqttc.connect(os.getenv('Mqtt__ConnectionString'), int(os.getenv('Mqtt__Port')))
    mqttc.loop_start()

    msg_info = mqttc.publish(topic_name, data, qos=1)
    unacked_publish.add(msg_info.mid)

    # Wait for all message to be published
    while len(unacked_publish):
        time.sleep(0.1)

    # Due to race-condition described above, the following way to wait for all publish is safer
    msg_info.wait_for_publish()

    mqttc.disconnect()
    mqttc.loop_stop()

def on_publish(client, userdata, mid, reason_code, properties):
    # reason_code and properties will only be present in MQTTv5. It's always unset in MQTTv3
    try:
        userdata.remove(mid)
    except KeyError:
        print("on_publish() is called with a mid not present in unacked_publish")
        print("This is due to an unavoidable race-condition:")
        print("* publish() return the mid of the message sent.")
        print("* mid from publish() is added to unacked_publish by the main thread")
        print("* on_publish() is called by the loop_start thread")
        print("While unlikely (because on_publish() will be called after a network round-trip),")
        print(" this is a race-condition that COULD happen")
        print("")
        print("The best solution to avoid race-condition is using the msg_info from publish()")
        print("We could also try using a list of acknowledged mid rather than removing from pending list,")
        print("but remember that mid could be re-used !")

def main(source_id,timestamp,model,image_id,detection_threshold,path,time_trace):
    timestamp_init=int(time.time()*1000)
    logging.basicConfig(level=logging.DEBUG)
    logging.info(source_id)

    endpoint = os.getenv('MINIOURL')
    bucket = 'images'
    
    minioClient = minio.MinIOClient(endpoint, 'minio', 'minio123')
    image_stream = minioClient.download_stream(bucket, image_id + ".jpg",)
    if image_stream is not None:
        img = cv2.imdecode(np.frombuffer(image_stream, np.uint8), cv2.IMREAD_COLOR)
    else:
        logging.error('Failed to download image')
        return
    val_to_compare_resize,_,_=img.shape
    # dim = (720, 576)

    if val_to_compare_resize>576:
        logging.info(f'Resizing to print')
        dim = (720,576)
        img= cv2.resize(img, dim, interpolation = cv2.INTER_AREA)
        frame_resized = cv2.imencode(".jpg", img)[1]
        frame_to_bytes=frame_resized.tobytes()
        frame = base64.standard_b64encode(frame_to_bytes)
        frame = frame.decode()
        image_id_str = str(image_id)
        minioClient.upload_bytes(bucket, image_id_str+'.jpg', frame_to_bytes)

    data = { "SourceId":source_id,
    "UrlVideoEncoded": "1.0",
    "Frame": image_id,
    "EventName": "ObjectDetection",
    "OriginModule": "Ai inference detection",
    "Information": "Test message",
    "EveryTime": int(timestamp),
    "Classes": [],
    "time_trace":[]
    }
    data['time_trace'].append(time_trace)
    results = model(img)
    schema = avro.schema.Parse(open(path, "rb").read())
    serializer = AvroJsonSerializer(schema)

    detections = json.loads(results.pandas().xyxy[0].to_json())
    
    if detections["name"]!={}:
        logging.info(f'Objects Detected')
        
        for idx,detection in enumerate(detections["name"].values()):
            
            BoundingBoxes=[]
            
            if list(detections["confidence"].values())[idx] > detection_threshold:
                
                xmin = list(detections["xmin"].values())[idx]
                xmax = list(detections["xmax"].values())[idx]
                ymin = list(detections["ymin"].values())[idx]
                ymax = list(detections["ymax"].values())[idx]
                BoundingBoxes.append({"x": xmin, "y":ymin})
                BoundingBoxes.append({"x": xmin, "y":ymax})
                BoundingBoxes.append({"x": xmax, "y":ymin})
                BoundingBoxes.append({"x": xmax, "y":ymax})

                data["Classes"].append({"EventType": detection, "Confidence":list(detections["confidence"].values())[idx], "BoundingBoxes": BoundingBoxes})

        data['time_trace'].append({"stepStart": timestamp_init, "stepEnd":int(time.time()*1000), "stepName": "ai_inferencer"})
        

        json_str = serializer.to_json(data)

        

        PublishEvent(pubsub_name="pubsub", topic_name="newDetection", data=json_str)
        logging.info(f'Event published')


    return 




