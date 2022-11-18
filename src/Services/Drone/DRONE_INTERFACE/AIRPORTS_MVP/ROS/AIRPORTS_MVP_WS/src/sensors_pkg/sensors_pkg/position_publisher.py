import rclpy
from rclpy.node import Node

from geometry_msgs.msg import Vector3Stamped


import numpy as np

class PositionPublisher(Node):

    def __init__(self):

        super().__init__('position_publisher')

        self.publisher = self.create_publisher(Vector3Stamped, '/position_topic', 10)

        timer_period = 0.2  # 0.2 seconds -> 5 Hz
        self.timer = self.create_timer(timer_period, self.timer_callback)


    def timer_callback(self):

        position_data = Vector3Stamped()

        position_data.header.stamp = self.get_clock().now().to_msg()

        time = position_data.header.stamp.sec + position_data.header.stamp.nanosec * 10 ** -9

        position_data.vector.x = float(np.sin(time + 0))
        position_data.vector.y = float(np.sin(time + 1))
        position_data.vector.z = float(np.sin(time + 2))

        self.publisher.publish(position_data)
        
        print(f'Time: {time}, x: {position_data.vector.x}, y: {position_data.vector.y}, z: {position_data.vector.z}')


def main(args=None):

    rclpy.init(args=args)

    position_publisher = PositionPublisher()

    rclpy.spin(position_publisher)

    position_publisher.destroy_node()
    rclpy.shutdown()
