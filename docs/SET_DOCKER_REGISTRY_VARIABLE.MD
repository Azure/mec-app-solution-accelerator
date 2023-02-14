# Set the DOCKER_REGISTRY environment variable

## Why you need the DOCKER_REGISTRY environment variable

The current solution's docker-compose files use the 'DOCKER_REGISTRY' environment variable to specify the Docker Registry name to use.

It's the same mechanism and environment variable that Docker projects created by Visual Studio use, as well. 

Basically, if the image's name for one of the application's service is, for instance, 'alertsapi', the Docker registry user should be appended as a prefix, such as 'myuser/alertsapi' so when uploading the image to Docker Hub or any other Docker Registry such as Azure Container Registry (ACR) the image can be automatically related to a specific Docker Registry repository.

This is important because when deploying the images to Kubernetes you will need to deploy te images from a Docker Registry such as Docker Hub. You cannot deploy images to Kuberentes directly from the local images you have in your dev machine but it needs to go through a Docker Registry. 

For instance, since we want this solution to have all the Docker images uploaded/registered in Docker Hub within the reposiory/user name "mecsolutionaccelerator", in that case the 'DOCKER_REGISTRY' must be initialized to "mecsolutionaccelerator/", so when those images are uploaded to Docker Hub, they will be like the following:

![image](https://user-images.githubusercontent.com/1712635/218830882-29ece9ca-6fb7-4212-b5ca-06227f81fc60.png)


If you'd like to use your own repository/user name (namespace) in Docker Hub or any Docker Registry, you can initialize the 'DOCKER_REGISTRY' environment variable with the name you prefer, such as "mydockeruser/".

