from dapr.clients import DaprClient
import time
import json
import base64
import cv2
import numpy as np
import avro.schema
from avro_json_serializer import AvroJsonSerializer
import logging



def PublishEvent(pubsub_name: str, topic_name: str, data: json):
    with DaprClient() as client:
        resp = client.publish_event(pubsub_name=pubsub_name, topic_name=topic_name, data=data, data_content_type='application/json')
        


def main(source_id,timestamp,model,frame,detection_threshold,path,time_trace):
    timestamp_init=int(time.time()*1000)
    logging.basicConfig(level=logging.DEBUG)
    logging.info(source_id)
    

    backToBytes = base64.standard_b64decode(frame)
    print('yes')

    img = cv2.imdecode(np.frombuffer(backToBytes, np.uint8), cv2.IMREAD_COLOR)
    print(img.shape)
    cv2.imwrite('filename.jpg', img)
    # print(img)
    dim = (720, 1280)
    frame_resized= cv2.resize(img, dim, interpolation = cv2.INTER_AREA)
    print(frame_resized.shape)
    cv2.imwrite('frame_resized.jpg', frame_resized)
    # frame_to_bytes=frame_resized.tobytes()
    # frame_to_send = base64.standard_b64encode(frame_to_bytes)
    
    # data = { "SourceId":source_id,
    # "UrlVideoEncoded": "1.0",
    # "Frame": frame_to_send.decode(),
    # "EventName": "ObjectDetection",
    # "OriginModule": "Ai inference detection",
    # "Information": "Test message",
    # "EveryTime": int(timestamp),
    # "Classes": [],
    # "time_trace":[]
    # }
    # data['time_trace'].append(time_trace)
    # results = model(img)
    # schema = avro.schema.Parse(open(path, "rb").read())
    # serializer = AvroJsonSerializer(schema)

    # detections = json.loads(results.pandas().xyxy[0].to_json())
    
    # if detections["name"]!={}:
    #     logging.info(f'Objects Detected')
        
    #     for idx,detection in enumerate(detections["name"].values()):
            
    #         BoundingBoxes=[]
            
    #         if list(detections["confidence"].values())[idx] > detection_threshold:
                
    #             xmin=list(detections["xmin"].values())[idx]
    #             xmax=list(detections["xmax"].values())[idx]
    #             ymin=list(detections["ymin"].values())[idx]
    #             ymax=list(detections["ymax"].values())[idx]
    #             BoundingBoxes.append({"x": xmin, "y":ymin})
    #             BoundingBoxes.append({"x": xmin, "y":ymax})
    #             BoundingBoxes.append({"x": xmax, "y":ymin})
    #             BoundingBoxes.append({"x": xmax, "y":ymax})

    #             data["Classes"].append({"EventType": detection, "Confidence":list(detections["confidence"].values())[idx], "BoundingBoxes": BoundingBoxes})

    #     data['time_trace'].append({"stepStart": timestamp_init, "stepEnd":int(time.time()*1000), "stepName": "ai_inferencer"})
        

    #     json_str = serializer.to_json(data)

        

    #     PublishEvent(pubsub_name="pubsub", topic_name="newDetection", data=json_str)
    #     logging.info(f'Event published')

    return 


        


