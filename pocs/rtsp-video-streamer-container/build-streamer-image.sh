#!/bin/sh

tag="1.8"
image_name="mecsolutionaccelerator/rtsp-video-streamer:$tag"

docker build --tag=$image_name --file=./Dockerfiles/rtsp .

