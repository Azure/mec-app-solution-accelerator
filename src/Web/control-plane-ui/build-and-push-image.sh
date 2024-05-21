#!/bin/sh

# Build and publish control plane ui image
tag="2.0"
image_name="mecsolutionaccelerator/control-plane-ui:$tag"

docker build --tag=$image_name --file=./Dockerfile .
docker push $image_name
