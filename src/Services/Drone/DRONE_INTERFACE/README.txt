ESTRUCTURA DEL MICROSERVICIO
- AIRPORTS_MVP: contiene los archivos de ROS necesarios para la comunicacion RTSP
- Datasets: contiene el archivo ".mp4" que alimenta el feed de video
- Dockerfile
- requirements: lista de dependencias de python (se instalan automaticamente en la imagen)

FUNCIONAMIENTO DEL MICROSERVICIO
- camera_publisher: nodo de ROS que lee el video de un archivo y manda cada frame por un ROS topic codificado en formato ".jpg"
- sensor_data_subscriber: nodo de ROS que se suscribe al topic anterior, decodifica y trata cada frame, y publica una alerta por un pubsub de Dapr cada 10 frames

LANZAMIENTO DEL MICROSERVICIO
- Automaticamente se inicia "sensor_data_subscriber", que espera a recibir datos del RTSP de ROS
- Para iniciar la publicacion de stream de video mediante ROS, se debe correr el siguiente comando en el container:
source opt/ros/humble/setup.bash && source AIRPORTS_MVP/ROS/AIRPORTS_MVP_WS/install/setup.bash && ros2 run sensors_pkg camera_publisher

* La publicación mediante Dapr Sidecar está comentada, se encuentra en la linea 162 de "sensor_data_subscriber".
* sensor_data_subscriber realiza un print cada vez que manda una alerta por Dapr