$rtsp_name="rtsp-streamer-container"
$rtsp_port="8554"
$rtsp_host="localhost"
$rtsp_uri="video"

$containterImage="wildfireaccelerator/rtsp-video-streamer:1.0"

docker run -it --name ${rtsp_name} -p ${rtsp_port}:${rtsp_port} -e RTSP_HOST=${rtsp_host} -e RTSP_PORT=${rtsp_port} -e RTSP_URI=${rtsp_uri} ${containterImage}


######################################################################################################################################################################
# Example with no variables:
# docker run -it --name rtsp-streamer-container -p 8554:8554 -e RTSP_HOST=localhost -e RTSP_PORT=8554 -e RTSP_URI=video wildfireaccelerator/rtsp-video-streamer:1.0


######################################################################################################################wildfireaccelerator################################################
# (CDLTLL) Example full URI to use:
# rtsp://192.168.1.233:8554/video