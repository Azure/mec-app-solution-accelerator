# Deploy on Docker ('docker compose')

Make sure you have [installed](https://docs.docker.com/desktop/install/windows-install/) and configured docker in your environment. After that, you can run the below commands from the solution's root directory and get started trying it.

## Grab the GitHub repo code

Get the GitHub repo's code:

```powershell
git clone git@github.com:Azure/mec-app-solution-accelerator.git
```

## (Optional step) Set the DOCKER_REGISTRY environment variable

Before creating the Docker images it's important to setup the DOCKER_REGISTRY environment variable in your dev system (Windows / Linux) so the images will be created with the right prefix (i.e. Docker hub user). This is important if you later want to use the same Docker images to upload them into Docker Hub or any other Docker Registry from where you will deploy the images to Kuberentes.

Here you have additional information on why and [How to setup the DOCKER_REGISTRY environment variable](./docs/SET_DOCKER_REGISTRY_VARIABLE.MD).

But if you just want to try the solution with `docker compose up` or Visual Studio, this step is optional, as it should work locally, anyways.

## Build the Docker images

Now, build the Docker images with `docker compose build`:

```powershell
docker compose build
```

for deepstream the command is

```powershell
docker compose -f docker-compose_deepstream_arm64.yml -f  docker-compose_deepstream_arm64.override.yml build
```

<img width="800" alt="image" src="https://user-images.githubusercontent.com/1712635/214674622-c404aa17-8b16-4df8-b958-ff8423995d67.png">

You could directly have run `docker compose up` (the next step), so if the images were not built already they will be built in the first place, but this step using `docker compose build` is intentional so you learn how you can just create the Docker images for other deployments such as Kubernetes, without habing to run it first into a Docker host.

## Run the solution

You start the containers and run the solution in yoour local docker host with this command:

```powershell
docker compose up
```
for deepstream the command is
```powershell
docker-compose -f docker-compose_deepstream_amd64.yml -f  docker-compose_deepstream_amd64.override.yml up
docker compose -f docker-compose_deepstream_arm64.yml -f  docker-compose_deepstream_arm64.override.yml up
```

You should see 'docker compose up' starting like in the following screenshot:
<img width="800" alt="image" src="https://user-images.githubusercontent.com/1712635/212741292-4396cc66-3ce9-451b-8d2f-bb3e6ec8e8b2.png">

Wait until all containers are up and running and you start seeing traces related to detections performed by the AI model and events raised because of them, like in the following screenshot:
<img width="796" alt="image" src="https://user-images.githubusercontent.com/1712635/214675605-954ceeb1-70b0-40a4-9a9a-138313cc9b86.png">

## Whats going on under the covers?

When the containers are running, the process starts with the <code>FrameSplitter</code> service/s which is ingesting the video feed from the video sources. In the "out-of-the-box" example code, the video comes from a local video file, but you can change that video source, with the configuration in <code>docker-compose.override.yml</code> and use any RTSP feed uri from real video cameras or from a fake RTSP provided by a VM simulating a video camera.

Then the image frames are sent to the AI model microservice which is detecting the target issues/objects. When that happens, events are published through the MQTT broker (pub/sub pattern) and the subscribers get it in order to evaluate if alerts should be raised depending on alerts rules. If alerts are raised thorugh Pub/Sub, then hte alerts will appear in the Alerts Dashboard UI. 

## Alerts Dashboard UI web app

At this point, you should be able to run the "Alerts Dashboard" web app with the following URL in any browser:

```code
https://localhost:50058
```
<img width="800" alt="image" src="https://user-images.githubusercontent.com/1712635/214684282-2aa3739e-cd61-47b5-a7e9-2a01e9d040ae.png">

Note that the same alerts could be reaching to other systems such as additional IoT devices, machinery to react or any other alert systems, all of them getting the same alert events based on the Publish/Subscription pattern using the MQTT message broker.
