# Build and publish control plane ui image
tag="1.8.1"
image_name="mecsolutionaccelerator/rulesengine:$tag"

docker build --tag=$image_name --file=./Dockerfile.local .
docker push $image_name
