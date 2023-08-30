import cv2
import queue
import threading
from dapr.clients import DaprClient
import json
import time
import base64
import os
import logging
import requests
import boto3
from botocore.exceptions import NoCredentialsError
from botocore.client import Config
import uuid

class VideoCapture:
    def __init__(self, name):
        self.cap = cv2.VideoCapture(name)
        self.q = queue.Queue()
        self.q_ret = queue.Queue()
        self.stop = False
        self.t = threading.Thread(target=self._reader)
        self.t.daemon = True
        self.t.start()

    def _reader(self):
        while not self.stop:
            ret, frame = self.cap.read()
            if not ret:
                break
            if not self.q.empty():
                try:
                    self.q.get_nowait()
                    self.q_ret.get_nowait()    # discard previous (unprocessed) frame
                except queue.Empty:
                    pass
            self.q.put(frame)
            self.q_ret.put(ret)

    def read(self):
        try:
            frame=self.q.get(timeout=1)
            return self.q_ret.get(timeout=.1),frame
        except:
            return False,None

    def release(self):
        self.stop = True
        self.t.join()
        self.cap.release()

def upload_bytes_to_minio(bucket_name, object_name, data_bytes, endpoint_url, access_key, secret_key):
    s3 = boto3.client(
        's3',
        endpoint_url=endpoint_url,
        aws_access_key_id=access_key,
        aws_secret_access_key=secret_key,
        config=Config(signature_version='s3v4'),
        region_name='us-east-1'
    )

    try:
        s3.put_object(Body=data_bytes, Bucket=bucket_name, Key=object_name)
        print(f"Data has been uploaded to {bucket_name} as {object_name}.")
    except NoCredentialsError:
        print("Credentials not available.")
    except Exception as e:
        print(f"An error occurred: {e}")

def main():
    endpoint = 'http://minio:9000'  # ej. 'http://localhost:9000'
    access_key = 'minio'
    secret_key = 'minio123'
    bucket = 'images'
    
    logging.basicConfig(level=logging.DEBUG)
    
    timer=0
    timer=int(os.getenv('TIMEOUT'))
    try:
        MY_POD_NAME=(os.getenv('MY_POD_NAME'))
        FEEDS=(os.getenv('FEEDS'))

        FEEDS=list(eval(FEEDS))

        dict_pos=MY_POD_NAME.split('-')[-1]
        feeds_dict=FEEDS[int(dict_pos)]

        feed_id=feeds_dict['id']
        feed_URL=feeds_dict['url']
    except:
        feed_URL=(os.getenv('FEED'))
        feed_id=1

    logging.info(f'feed url: {feed_URL}')
        
    time.sleep(timer)
    
 
    cap = VideoCapture(feed_URL)
    while True:
        # Capture frame-by-frame
        ret, frame = cap.read()
        timestamp_init = int(time.time() * 1000)

        while not ret:
            logging.info(f'not possible to access feed')
            cap.release()
            cap = VideoCapture(feed_URL)
            ret, frame = cap.read()

        img_encode = cv2.imencode(".jpg", frame)[1]
        resized_img_bytes = img_encode.tobytes()
        bytes_string = base64.standard_b64encode(resized_img_bytes).decode()
        timestamp = int(time.time() * 1000)
        logging.info(f'Sending frame to inference')
        try:
            image_id = uuid.uuid4()
            image_id_str = str(image_id)
            logging.info(f'Image uploaded with ID: {image_id}')
            upload_bytes_to_minio(bucket, image_id_str+'.jpg', resized_img_bytes, endpoint, access_key, secret_key)
            with DaprClient() as client:
            # Create data to send to AI inference service
                time_trace = {"stepStart": timestamp_init, "stepEnd": int(time.time() * 1000), "stepName": "frameSplitter"}
                req_data = {"source_id": 'video_' + str(feed_id), "timestamp": timestamp, "image_id": image_id_str, 'time_trace': time_trace}

            # Invoke the AI Model inference service
                resp = client.invoke_method(
                    "invoke-sender-frames", "frames-receiver", data=json.dumps(req_data)
                )

                logging.info(f'Waiting for response')
                response = resp.text()
                logging.info(response)
        except Exception as e:
            logging.error(f'Error encountered: {e}')


        

if __name__ == '__main__':
    main()
