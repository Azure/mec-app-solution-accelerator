#!/bin/sh

tag="2.0"
image_name="mecsolutionaccelerator/rtsp-video-streamer:$tag"

docker build --tag=$image_name --file=./Dockerfiles/rtsp .

