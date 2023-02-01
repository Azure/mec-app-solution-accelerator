import cv2
import numpy as np
import queue
import threading
from dapr.clients import DaprClient
from dapr.ext.grpc import App, InvokeMethodRequest, InvokeMethodResponse
import json
import time
import pickle
import base64
import os
app = App()

def PublishEvent(pubsub_name: str, topic_name: str, data: json):
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
    try:
        feed=(os.getenv('FEED'))
        MY_POD_NAME=(os.getenv('MY_POD_NAME'))
        FEEDS=(os.getenv('FEEDS'))

        FEEDS=list(eval(FEEDS))

        dict_pos=MY_POD_NAME.split('-')[-1]
        feeds_dict=FEEDS[int(dict_pos)]

        feed_id=feeds_dict['id']
        feed_URL=feeds_dict['url']
        print('kubernetes execution')
        print('feed id: ' + str(feed_id))
        print('feed url: ' +feed_URL)
    except:
        print('docker-compose execution')
        feed_URL=(os.getenv('FEED'))
        feed_id=1
        print('feed url: ' +feed_URL)
    time.sleep(timer)
    
 
    print('capturing frames')
    cap = VideoCapture(feed_URL)
    timestamp_general=int(time.time()*1000)
    i=0
    while True:
        # Capture frame-by-frame
        ret,frame = cap.read()
        timestamp_init=int(time.time()*1000)
        print('feed url: ' +feed_URL)
        
        while not ret:
            print('not possible to access RTSP')
            cap.release()
            cap = VideoCapture(feed_URL)
            ret, frame = cap.read()
        # print(frame)
       
        img_encode = cv2.imencode(".jpg", frame)[1]
        resized_img_bytes = img_encode.tobytes()
        bytes_string = base64.standard_b64encode(resized_img_bytes)
        timestamp=int(time.time()*1000)
        print('Sending frame to inference')
        try:
            with DaprClient() as client:
                # Using Dapr SDK to publish a topic
                time_trace={"stepStart": timestamp_init, "stepEnd":int(time.time()*1000), "stepName": "frameSplitter"}
                
                req_data = {"source_id": 'video_'+str(feed_id), "timestamp":timestamp, "image": bytes_string.decode(), 'time_trace': time_trace}
                resp = client.invoke_method(
                    "invoke-sender-frames", "frames-receiver", data=json.dumps(req_data)
                )
                # PublishEvent(pubsub_name="pubsub", topic_name="newFrame", data=json.dumps({'frame_sent':True}))
                print('Waiting for response')
                # Print the response
                print(resp.content_type, flush=True)
                print(resp.text(), flush=True) 
        except:
            print('Inference pod busy')
        i+=1
    # cap.release()
        # cv2.destroyAllWindows()
        print('End')

if __name__ == '__main__':
    main()