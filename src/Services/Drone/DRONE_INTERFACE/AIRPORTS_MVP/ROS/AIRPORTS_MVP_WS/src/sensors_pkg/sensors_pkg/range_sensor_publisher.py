from tracemalloc import start
import rclpy
from rclpy.node import Node

from sensor_msgs.msg import Range


class RangeSensorPublisher(Node):

    def __init__(self):

        super().__init__('range_sensor_publisher')

        self.publisher = self.create_publisher(Range, '/range_sensor_topic', 10)

        timer_period = 0.01  # 0.01 seconds -> 100 Hz
        self.timer = self.create_timer(timer_period, self.timer_callback)


    def timer_callback(self):

        range_sensor_data = Range()

        range_sensor_data.header.stamp = self.get_clock().now().to_msg()

        time = range_sensor_data.header.stamp.sec + range_sensor_data.header.stamp.nanosec * 10 ** -9

        range_sensor_data.range = float(time)

        self.publisher.publish(range_sensor_data)

        print(f'Time: {time}, r: {range_sensor_data.range}')



def main(args=None):

    rclpy.init(args=args)

    range_sensor_publisher = RangeSensorPublisher()

    rclpy.spin(range_sensor_publisher)

    range_sensor_publisher.destroy_node()
    rclpy.shutdown()
