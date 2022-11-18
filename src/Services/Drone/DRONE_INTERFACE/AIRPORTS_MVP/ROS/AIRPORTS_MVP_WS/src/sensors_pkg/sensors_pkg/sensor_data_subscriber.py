import rclpy
from rclpy.node import Node

from geometry_msgs.msg import Vector3Stamped
from sensor_msgs.msg import Range, CompressedImage

import cv2
import numpy as np

import json
from dapr.clients import DaprClient


################################################################
# ----------------           Service          ---------------- #
################################################################

class Service():

    def __init__(self, service_name: str, msg_type, topic_name: str, expected_msg_frequency: float) -> None:

        """
        - service name: string specifying the name of the service.
        - msg_type: ROS2 message type the service publishes.
        - topic_name: name of the topic the service publishes to.
        - expected_msg_frequency: publishing frequency [Hz] the manager should expect to consider the service is online.

        """
        
        # Service details
        self.service_name: str = service_name
        self.msg_type = msg_type
        self.topic_name : str = topic_name
        self.expected_msg_frequency: float = expected_msg_frequency
        
        self.is_online: bool = False

        # Time management - time in [s]
        self._clock = None
        self.last_callback_time: float = 0.0

        # Service data
        self.msg: msg_type = None


    def assign_clock(self, clock) -> None:

        '''
        Assigns the specified ROS2 clock as the service clock
        '''

        self._clock = clock
    

    def subscriber_callback(self, new_msg):

        '''
        Callback function, called when the services recieves new data.
        - Keeps track of the last time it was called.
        - Keeps track of the service's online status.
        - Stores the last message recieved.
        '''
        
        # Time management
        last_callback_time = self._clock.now().to_msg()
        self.last_callback_time = last_callback_time.sec + last_callback_time.nanosec * 10 ** -9

        # Service online check
        self.is_online = True

        # Data management
        self.msg = new_msg


    def log(self, msg: str, msg_type: int = 0) -> None:

        """
        msg_type
        0: Debug
        1: Warning
        2: Error
        """

        msg_types = ["Debug", "Warning", "Error"]

        print(f'[{msg_types[msg_type]}] [{self.service_name} service]: {msg}')
    

    def __str__(self) -> str:

        string = (f'\n---------------- Service ---------------- \
                    \nIs online: {self.is_online} \
                    \nService name: {self.service_name} \
                    \nMessage type: {self.msg_type} \
                    \nTopic name: {self.topic_name} \
                    \nMessage frequency [Hz]: {self.expected_msg_frequency} \
                    \n------------------------------------------ \
                    \n')

        return string


################################################################
# ----------------       Service Manager      ---------------- #
################################################################

class ServiceManager(Node):

    def __init__(self, global_timer_frequency: float, services_list=None):

        '''
        - global_timer_frequency: defines the frequency [Hz] for the main callback function.
        - services_list: a list containing all the services for the manager to keeptrack of.

        '''
        
        super().__init__('service_manager')

        # Services setup
        self._services_list = services_list if services_list is not None else list()
        [service.assign_clock(self.get_clock()) for service in self._services_list]

        self._subscriptions_list = [self.create_subscription(service.msg_type, service.topic_name, service.subscriber_callback, 1) for service in self._services_list]
        
        self._is_service_online_list = [False for _ in self._services_list]

        # Global timer setup
        self._timer = self.create_timer(1 / global_timer_frequency, self.global_timer_callback)

        # Display service status
        self.display_services()

        # Daper events
        self.i = 0


    def global_timer_callback(self):

        '''
        Main function for the service manager.
        - Updates current time.
        - Checks the online status of each service.
        - Displays each service's latest data.

        Callback frequency defined by the "global_timer_frequency" parameter specified in the manager initialization.
        '''
        
        current_time = self._clock.now().to_msg()
        self.current_time = current_time.sec + current_time.nanosec * 10 ** -9
        
        # Services online check
        self.services_online_check()

        # Image streaming
        self.display_current_data()

        # Dapr publish
        self.i += 1

        if self.i % 10 == 0:
            print(f"Sending frame {self.i}")
            orderId = self.i
            PUBSUB_NAME = 'pubsub'
            TOPIC_NAME = 'newAlertPython'
            with DaprClient() as client:
                #Using Dapr SDK to publish a topic
                result = client.publish_event(
                    pubsub_name=PUBSUB_NAME,
                    topic_name=TOPIC_NAME,
                    data=json.dumps(orderId),
                    data_content_type='application/json',
                )

    def display_services(self):

        '''
        Displays the current status and parameters of each service specified in the manager initialization.
        '''

        [print(service) for service in self._services_list]
    
    def services_online_check(self) -> None:

        '''
        Checks the online status of each service specified in the manager initialization.
        - Raises a warning when the service skips 10 messages at the expected message frequency.
        - Raises a debug message when the service is back online.
        '''

        for index, service in enumerate(self._services_list):

            if service.is_online and service.last_callback_time + 10.0 / service.expected_msg_frequency < self.current_time:
                service.is_online = False

            if service.is_online != self._is_service_online_list[index]:
                if service.is_online:
                    service.log('Service online', 0)
                else:
                    service.log('Service offline', 1)

            self._is_service_online_list[index] = service.is_online

    def display_current_data(self):

        '''
        Displays each service's latest data.
        '''

        service_indexes = {'position': 0,
                           'camera': 1,}

        #if self._services_list[service_indexes['camera']].is_online and self._services_list[service_indexes['position']].is_online:
        if self._services_list[service_indexes['camera']].is_online:

            frame = np.frombuffer(self._services_list[service_indexes['camera']].msg.data, dtype=np.uint8)
            frame = cv2.imdecode(frame, cv2.IMREAD_COLOR)
                
            ## Frame editing

            '''            
            time_text = str(self.current_time)

            position_x = self._services_list[service_indexes['position']].msg.vector.x
            position_y = self._services_list[service_indexes['position']].msg.vector.y
            position_z = self._services_list[service_indexes['position']].msg.vector.z

            position_text = f'x: {position_x}, y: {position_y}, z: {position_z}'

            position_delta_time_text = str(self.current_time - self._services_list[service_indexes['position']].last_callback_time)
            camera_delta_time_text = str(self.current_time - self._services_list[service_indexes['camera']].last_callback_time)

            frame = cv2.putText(frame, time_text, (20, 20), cv2.FONT_HERSHEY_SIMPLEX, 0.6, (32, 32, 32), 2)
            frame = cv2.putText(frame, position_delta_time_text, (20, 40), cv2.FONT_HERSHEY_SIMPLEX, 0.6, (32, 32, 64), 2)
            frame = cv2.putText(frame, camera_delta_time_text, (20, 60), cv2.FONT_HERSHEY_SIMPLEX, 0.6, (32, 32, 64), 2)
            frame = cv2.putText(frame, position_text, (20, 450), cv2.FONT_HERSHEY_SIMPLEX, 0.6, (32, 32, 32), 2)
            '''

            
            cv2.imshow('ARIPORTS_MVP', frame)
            if cv2.waitKey(25) & 0xFF == ord('q'):
                exit()
            



    def log(self, msg: str, msg_type: int = 0) -> None:

        """
        msg_type
        0: Debug
        1: Warning
        2: Error
        """

        msg_types = ["Debug", "Warning", "Error"]

        print(f'[{msg_types[msg_type]}] {msg}')



def main(args=None):

    rclpy.init(args=args)

    position_service = Service(service_name = 'position',
                               msg_type = Vector3Stamped,
                               topic_name = '/position_topic',
                               expected_msg_frequency = 5.0)

    camera_service = Service(service_name = 'camera',
                             msg_type = CompressedImage,
                             topic_name = '/camera_topic',
                             expected_msg_frequency = 15.0)

    range_sensor_service = Service(service_name = 'range_sensor',
                                   msg_type = Range,
                                   topic_name = '/range_sensor_topic',
                                   expected_msg_frequency = 100.0)

    services_list = list()
    services_list.append(position_service)
    services_list.append(camera_service)
    services_list.append(range_sensor_service)

    global_timer_frequency = 20.0

    data_subscriber = ServiceManager(global_timer_frequency, services_list)

    rclpy.spin(data_subscriber)

    data_subscriber.destroy_node()
    rclpy.shutdown()
