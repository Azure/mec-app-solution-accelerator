#!/bin/sh

# Build and publish framesplitter
tag="2.0"
image_name="mecsolutionaccelerator/framesplitter:$tag"

docker build --tag=$image_name --file=./frameSplitter/Dockerfile .
docker push $image_name
