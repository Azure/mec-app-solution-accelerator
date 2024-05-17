$tags="2.0"
$imageName="mecsolutionaccelerator/rtsp-video-streamer:${tags}"

docker build --tag=${imageName} --file=./Dockerfiles/rtsp .

docker push ${imageName}
