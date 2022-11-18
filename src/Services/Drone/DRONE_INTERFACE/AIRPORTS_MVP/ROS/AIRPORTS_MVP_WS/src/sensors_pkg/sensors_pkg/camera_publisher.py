import rclpy
from rclpy.node import Node

from sensor_msgs.msg import CompressedImage

import cv2
import numpy as np


class CameraPublisher(Node):

    def __init__(self):

        super().__init__('camera_publisher')

        self.publisher = self.create_publisher(CompressedImage, '/camera_topic', 10)

        timer_period = 0.05  # 0.05 seconds -> 20 Hz
        self.timer = self.create_timer(timer_period, self.timer_callback)

        self.capture = cv2.VideoCapture("./Datasets/demo.mp4")


    def timer_callback(self):

        camera_data = CompressedImage()

        if self.capture.isOpened():
            
            hasFrames, frame = self.capture.read()
            
            if hasFrames == True:

                camera_data.header.stamp = self.get_clock().now().to_msg()

                _, frame = cv2.imencode('.jpg', frame)
                camera_data.data = frame.tobytes()
                self.publisher.publish(camera_data)

            else:
                self.capture.release()
                cv2.destroyAllWindows()
            


def main(args=None):

    rclpy.init(args=args)

    camera_publisher = CameraPublisher()

    rclpy.spin(camera_publisher)

    camera_publisher.destroy_node()
    rclpy.shutdown()