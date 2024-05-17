# Build and publish control plane ui image
tag="2.0"
image_name="mecsolutionaccelerator/alerts-api:$tag"

docker build --tag=$image_name --file=./Dockerfile.local .
docker push $image_name
