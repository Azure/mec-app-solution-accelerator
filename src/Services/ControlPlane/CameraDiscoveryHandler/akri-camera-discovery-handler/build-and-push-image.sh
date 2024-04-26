#!/bin/sh
tag="1.8"
image_name="mecsolutionaccelerator/akri-camera-discovery-handler:$tag"

docker build --tag=$image_name --file=./Dockerfile.discovery-handler .
docker push $image_name
