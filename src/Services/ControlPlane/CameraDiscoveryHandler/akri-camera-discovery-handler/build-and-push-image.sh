#!/bin/sh
tag="2.0"
image_name="mecsolutionaccelerator/akri-camera-discovery-handler:$tag"

docker build --tag=$image_name --file=./Dockerfile.discovery-handler .
docker push $image_name
