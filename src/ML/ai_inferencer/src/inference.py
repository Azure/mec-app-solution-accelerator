from dapr.clients import DaprClient
import time
import glob
import torch
import json
import os
import base64
import cv2
import numpy as np


def PublishEvent(pubsub_name: str, topic_name: str, data: str):
    with DaprClient() as client:
        resp = client.publish_event(pubsub_name=pubsub_name, topic_name=topic_name, data=data, data_content_type='application/json')
        print(resp)

def main(model,frame):
    print('i')
    backToBytes = base64.standard_b64decode(frame)
    img = cv2.imdecode(np.frombuffer(backToBytes, np.uint8), cv2.IMREAD_COLOR)

    results = model(img)
    print(results)
    detections = []
    for detection in json.loads(results.pandas().xyxy[0].to_json())["name"].values():
        detections.append(detection)
    detections=list(set(detections))
    data_str = {"information":"Yolo detected: " + ', '.join(detections)}
    print(json.dumps(data_str))
    PublishEvent(pubsub_name="pubsub", topic_name="newDetection", data=json.dumps(data_str))

    # results.print()

        

    


if __name__ == '__main__':
    os.system("python3 invoke-sender-frames.py")
    main()
