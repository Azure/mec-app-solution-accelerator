import cv2
import numpy as np
import queue
import threading
from dapr.clients import DaprClient
import json
import time
import pickle
import base64
import os

def PublishEvent(pubsub_name: str, topic_name: str, data: str):
    with DaprClient() as client:
        resp = client.publish_event(pubsub_name=pubsub_name, topic_name=topic_name, data=data)
        print(resp)


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
                #self.release()
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
        print('c viene el frame')
        print(frame)
        # if frame==None:
        #     self.q_ret.get()
        #     return False,None
        # else:
        return self.q_ret.get(timeout=.1),frame

    def release(self):
        self.stop = True
        self.t.join()
        self.cap.release()

def main():
    print('main')
    timer=0
    timer= int(os.getenv('TIMEOUT'))
    feed= (os.getenv('FEED'))
    time.sleep(timer)

 
    print('dentro')
    cap = VideoCapture(feed)
    timestamp_general=int(time.time()*1000)
    i=0
    while True:
        # Capture frame-by-frame
        print('in')
        ret,frame = cap.read()
        print(ret)
        
        while not ret:
            print('not ret')
            cap.release()
            cap = VideoCapture(feed)
            ret, frame = cap.read()
        # print(frame)
       
        img_encode = cv2.imencode(".jpg", frame)[1]
        resized_img_bytes = img_encode.tobytes()
        bytes_string = base64.standard_b64encode(resized_img_bytes)
        timestamp=int(time.time()*1000)
        with DaprClient() as client:
            # Using Dapr SDK to publish a topic
            req_data = {"source_id": 'video_'+str(timestamp_general), "timestamp":timestamp, "image": bytes_string.decode()}
            resp = client.invoke_method(
                "invoke-sender-frames", "frames-receiver", data=json.dumps(req_data)
            )
            print('Esperando respuesta')
            # Print the response
            print(resp.content_type, flush=True)
            print(resp.text(), flush=True) 
        i+=1
    # cap.release()
        # cv2.destroyAllWindows()
        print('fin')

if __name__ == '__main__':
    main()