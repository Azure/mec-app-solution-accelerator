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
import onnxruntime
import torch
import glob
from src.yolo_onnx_preprocessing_utils import preprocess,non_max_suppression, _convert_to_rcnn_output
from src.onnx_checker import get_predictions_from_ONNX


def PublishEvent(pubsub_name: str, topic_name: str, data: json):
    with DaprClient() as client:
        resp = client.publish_event(pubsub_name=pubsub_name, topic_name=topic_name, data=data, data_content_type='application/json')

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

    img_processed_list = []
    pad_list = []
    batch_size=1
    
    img_processed, pad = preprocess(img)
    img_processed_list.append(img_processed)
    pad_list.append(pad)
        
    if len(img_processed_list) > 1:
        img_data = np.concatenate(img_processed_list)
    elif len(img_processed_list) == 1:
        img_data = img_processed_list[0]
    else:
        img_data = None

    assert batch_size == img_data.shape[0]

    result = get_predictions_from_ONNX(model, img_data)
    # for val in result[0]:
    # print(val)
    
    result_final = non_max_suppression(
        torch.from_numpy(result),
        conf_thres=0.3,
        iou_thres=0.3)
    # val_to_compare_resize,_,_=img.shape
    # # dim = (720, 576)

    # if val_to_compare_resize>576:
    #     logging.info(f'Resizing to print')
    #     dim = (720,576)
    #     img= cv2.resize(img, dim, interpolation = cv2.INTER_AREA)
    #     frame_resized = cv2.imencode(".jpg", img)[1]
    #     frame_to_bytes=frame_resized.tobytes()
    #     frame = base64.standard_b64encode(frame_to_bytes)
    #     frame = frame.decode()
    #     image_id_str = str(image_id)
    #     minioClient.upload_bytes(bucket, image_id_str+'.jpg', frame_to_bytes)

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
    # results = model(img)
    schema = avro.schema.Parse(open(path, "rb").read())
    serializer = AvroJsonSerializer(schema)

    # detections = json.loads(results.pandas().xyxy[0].to_json())
    
    if result_final!=[]:
        logging.info(f'Objects Detected')
        
        for idx,result in enumerate(result_final):
            result=result.numpy()
            BoundingBoxes=[]
            
            if result[:, 4:5].item() > detection_threshold:
                
                xmin = result[:, 0:1].item()
                xmax = result[:, 2:3].item()
                ymin = result[:, 1:2].item()
                ymax = result[:, 3:4].item()
                BoundingBoxes.append({"x": xmin, "y":ymin})
                BoundingBoxes.append({"x": xmin, "y":ymax})
                BoundingBoxes.append({"x": xmax, "y":ymin})
                BoundingBoxes.append({"x": xmax, "y":ymax})

                data["Classes"].append({"EventType": "smoke", "Confidence":result[:, 4:5].item(), "BoundingBoxes": BoundingBoxes})

        data['time_trace'].append({"stepStart": timestamp_init, "stepEnd":int(time.time()*1000), "stepName": "ai_inferencer"})
        stepEnd=int(time.time()*1000)

        json_str = serializer.to_json(data)

        

        PublishEvent(pubsub_name="pubsub", topic_name="newDetection", data=json_str)
        logging.info("Time per frame: "+str(stepEnd-timestamp)+" s")
        logging.info(f'Event published')


    return 




