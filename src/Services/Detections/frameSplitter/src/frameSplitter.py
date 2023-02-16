import base64
import json
import logging
import os
import queue
import threading
import time

import cv2
import numpy as np
from dapr.clients import DaprClient
from dapr.ext.grpc import App


app = App()


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
            frame = self.q.get(timeout=1)
            return self.q_ret.get(timeout=.1), frame
        except:
            return False, None

    def release(self):
        self.stop = True
        self.t.join()
        self.cap.release()


def publish_event(pubsub_name: str, topic_name: str, data: dict):
    with DaprClient() as client:
        resp = client.publish_event(pubsub_name=pubsub_name, topic_name=topic_name, data=data)
        logging.debug(resp)


def main():
    logging.basicConfig(level=logging.DEBUG)

    timer = int(os.getenv('TIMEOUT'))
    feed = os.getenv('FEED')
    try:
        feeds = json.loads(os.getenv('FEEDS'))
        pod_name = os.getenv('MY_POD_NAME')
        dict_pos = pod_name.split('-')[-1]
        feed_id, feed_url = feeds[int(dict_pos)]['id'], feeds[int(dict_pos)]['url']
    except:
        feed_url, feed_id = feed, 1
    logging.info(f'feed url: {feed_url}')

    cap = VideoCapture(feed_url)
    i = 0
    while True:
        # Capture frame-by-frame
        ret, frame = cap.read()
       
