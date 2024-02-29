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
import uuid
import shared.minio_utils as minio

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

def main():
    endpoint = os.getenv('MINIOURL')
    bucket = 'images'
    minioClient = minio.MinIOClient(endpoint, 'minio', 'minio123')
    logging.basicConfig(level=logging.INFO)
    
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
        timestamp = int(time.time() * 1000)
        img_encode = cv2.imencode(".jpg", frame)[1]
        resized_img_bytes = img_encode.tobytes()
        bytes_string = base64.standard_b64encode(resized_img_bytes).decode()
        
        logging.info(f'Sending frame to inference')
        try:
            image_id = uuid.uuid4()
            image_id_str = str(image_id)
            logging.info(f'Image uploaded with ID: {image_id}')
            minioClient.upload_bytes(bucket, image_id_str+'.jpg', resized_img_bytes)
            with DaprClient() as client:
            # Create data to send to AI inference service
                time_trace = {"stepStart": timestamp_init, "stepEnd": int(time.time() * 1000), "stepName": "frameSplitter"}
                req_data = {"source_id": 'video_' + str(feed_id), "timestamp": timestamp, "image_id": image_id_str, 'time_trace': time_trace}

            # Invoke the AI Model inference service
                time_check=int(time.time()*1000)
                resp = client.invoke_method(
                    "invoke-sender-frames", "frames-receiver", data=json.dumps(req_data)
                )

                logging.info(f'Waiting for response')
                response = resp.text()
                logging.info(response)
                time_check_end=int(time.time()*1000)
                logging.info(f'time to process frame: {time_check_end-time_check}')    
        except Exception as e:
            logging.error(f'Error encountered: {e}')     

if __name__ == '__main__':
    main()
