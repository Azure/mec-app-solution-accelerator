# Buil your own Docker Images to upload to Docker Registry and deploy to Kubernetes

You can deploy to Kubernetes and test the images we have already uploaded into Docker Hub, so creating the images and uploading to Docker Hub is optional, initially.

However, if you make changes to the code and later deploy your own Docker Images to Kubernetes, you'd need to create the images locally and upload the images first into a Docker registry. Finally, from the Docker registry you can deploy the images into Kubernetes.

## Set the DOCKER_REGISTRY environment variable 

Before creating the Docker images it's important to setup the DOCKER_REGISTRY environment variable in your dev system (Windows / Linux) so the images will be created with the right prefix (i.e. Docker hub user). This is important in order to be able to upload the Docker images into Docker Hub or any other Docker registry from where you will deploy the images to Kubernetes.

Please, read here why and [How to setup the DOCKER_REGISTRY environment variable](./docs/SET_DOCKER_REGISTRY_VARIABLE.MD).

## Build the Docker images locally

In order to make sure you have built the Docker Images with tha latest code, build the Docker images with `docker compose build` from the root folder of this solution, such as:

```powershell
docker compose build
```

<img width="800" alt="image" src="https://user-images.githubusercontent.com/1712635/214674622-c404aa17-8b16-4df8-b958-ff8423995d67.png"> 

You could also create the Docker Images with a Script and a line using `docker build` for each Image. But using `docker compose build` is more consistent so you make sure you create and use the same images with the same image's name in Docker (dev time) and later in Kubernetes ("production" time). Note that using a different script with multiple `docker build` lines you could specify different image's names.

## Push the created Docker images to Docker Hub (Or your preferred registry)

You can directly run this PowerShell script from the `deploy\k8s` folder.

Of course, before doing so, you'd need to change the namespace/user for Docker registry, since "mecsolutionaccelerator" is reserve for our team and secured with credentials. You will also need to authenticate first against your selected Docker registry (Docker Hub, ACR, etc.) with `docker login`.

```powershell
./push-docker-images-to-docker-hub.ps1
```

Internally, that script is pushing the multiple Docker Images into Docker Hub, like here:

```powershell
docker push mecsolutionaccelerator/alerts-ui:latest
docker push mecsolutionaccelerator/framesplitter:latest
docker push mecsolutionaccelerator/aiinferencer:latest
docker push mecsolutionaccelerator/alerts-api:latest
docker push mecsolutionaccelerator/rulesengine:latest
```

So the Docker images are now uploaded into Docker Hub:

![image](https://user-images.githubusercontent.com/1712635/219430508-84358f3a-b1e6-4d8c-9da2-2f5c308bf5b0.png)

Once you have your own images in your selectec Docker registry you can then deploy to Kubernetes.