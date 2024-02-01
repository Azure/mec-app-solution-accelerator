$tags="1.8"
$imageName="mecsolutionaccelerator/rtsp-video-streamer:${tags}"

docker build --tag=${imageName} --file=./Dockerfiles/rtsp .
