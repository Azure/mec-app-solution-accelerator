from dapr.clients import DaprClient
import time
import glob
import torch
import json
import os
import base64
import cv2
import numpy as np
import avro.schema
from avro_json_serializer import AvroJsonSerializer
from avro.datafile import DataFileReader, DataFileWriter
from avro.io import DatumReader, DatumWriter
import io


def PublishEvent(pubsub_name: str, topic_name: str, data: json):
    with DaprClient() as client:
        resp = client.publish_event(pubsub_name=pubsub_name, topic_name=topic_name, data=data, data_content_type='application/json')
        print(resp)

# def main(model,frame):
def main(source_id,timestamp,model,frame,detection_threshold,path):
    print(source_id)
    print(timestamp)
    # print('i')
    # "Frame": backToBytes.decode()
    backToBytes = base64.standard_b64decode(frame)

    img = cv2.imdecode(np.frombuffer(backToBytes, np.uint8), cv2.IMREAD_COLOR)
    # print(img)
    data = { "SourceId":source_id,
    "UrlVideoEncoded": "1.0",
    "Frame": frame,
    "EventName": "ObjectDetection",
    "EventType": "class",
    "OriginModule": "Ai inference detection",
    "Information": "Test message",
    "EveryTime": int(timestamp),
    "BoundingBoxes": []
    }
    results = model(img)
    schema = avro.schema.Parse(open(path, "rb").read())
    serializer = AvroJsonSerializer(schema)
    # writer = avro.io.DatumWriter(schema)
    # bytes_writer = io.BytesIO()
    # encoder = avro.io.BinaryEncoder(bytes_writer)
    detections = json.loads(results.pandas().xyxy[0].to_json())
    for idx,detection in enumerate(detections["name"].values()):
        data["BoundingBoxes"]=[]
        
        if list(detections["confidence"].values())[idx] > detection_threshold:
            data["EventType"]=detection
            xmin=list(detections["xmin"].values())[idx]
            xmax=list(detections["xmax"].values())[idx]
            ymin=list(detections["ymin"].values())[idx]
            ymax=list(detections["ymax"].values())[idx]
            data["BoundingBoxes"].append({"x": xmin, "y":ymin})
            data["BoundingBoxes"].append({"x": xmin, "y":ymax})
            data["BoundingBoxes"].append({"x": xmax, "y":ymin})
            data["BoundingBoxes"].append({"x": xmax, "y":ymax})
            json_str = serializer.to_json(data)
            # print(json_str)
            # writer.write(data, encoder)
            # bbytes = bytes_writer.getvalue()
            # print(bbytes)
            PublishEvent(pubsub_name="pubsub", topic_name="newDetection", data=json_str)
    return
    # results.print()

        

    


if __name__ == '__main__':
    #os.system("python3 invoke-sender-frames.py")
    model = torch.hub.load("ultralytics/yolov5", "yolov5s", pretrained=True,force_reload=True )
    img = cv2.imread('coches2.jpg')
    retval, buffer = cv2.imencode('.jpg', img) 
    frame_out=base64.b64encode(buffer)
    path='detection.avro'
    detection_threshold=0.7
    main(model, frame_out, detection_threshold,path)
