#!/bin/sh

# Build and publish rtps converter
tag="2.0"
image_name="mecsolutionaccelerator/rtsp-converter:$tag"

docker build --tag=$image_name --file=./Dockerfile .
docker push $image_name
