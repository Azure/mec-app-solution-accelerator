# Multi-Arch .NET Docker Sample

Basic HELLO WORLD updated so the Docker image is multi-arch (multiple architectures, such as x64, ARM, etc. in the same image).

This allows to have a single image published in a Docker container that will work on any CPU architecture (x64, ARM, etc.)

## Try a pre-built version of the sample

```console
docker run --rm -it mecsolutionaccelerator/dotnetapp
```
The following is a screenshot from a Docker host on x64 machine (It's a Windows 11 machine with Docker for desktop):

![image](https://github.com/Azure/mec-app-solution-accelerator/assets/1712635/d529ffe4-3751-4e19-82c1-219d06e1a3cd)

Now, this is from a Docker host on ARM machine (It's an NVIDIA Orin with ARM processor) and using exaclty the same Docker image name and tag!:

![image](https://github.com/Azure/mec-app-solution-accelerator/assets/1712635/268fdd75-c80a-4647-aa01-a4d97f57c299)


## The Dockerfile

This is the Dockerfile where you can see special commands for MULTI-ARCH:

https://github.com/Azure/mec-app-solution-accelerator/blob/main/pocs/multi-arch-docker/multi-arch-dotnetapp/Dockerfile

```
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0.400 AS build
ARG TARGETARCH

WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.csproj .

RUN dotnet restore -a $TARGETARCH

# copy and publish app and libraries
COPY . .

RUN dotnet publish -a $TARGETARCH --self-contained false --no-restore -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:7.0.4
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["./dotnetapp"]
```


## Build the MULTI-ARCH .NET Docker image with "docker buildx"

[Buildx](https://www.docker.com/blog/how-to-rapidly-build-multi-architecture-images-with-buildx/) is a nice addition to Docker tools. I think of it as “full BuildKit”. For our purposes, it enables specifying multiple platforms to build at once and to package them all up as a multi-platform tag. It will even push them to your registry, all with a single command.

We first need to setup buildx:

```console
docker buildx create --use
```

Example:

![image](https://github.com/Azure/mec-app-solution-accelerator/assets/1712635/001704b0-dccb-4dba-a7c5-0e67402c28ef)

We can now build a multi-platform image for our app.

```console
docker buildx build -f Dockerfile --platform linux/arm64,linux/arm,linux/amd64 -t mecsolutionaccelerator/dotnetapp --push .
```

![image](https://github.com/Azure/mec-app-solution-accelerator/assets/1712635/e73a466d-96a8-42f7-9a14-daffd65834aa)

Here, we’re building for three architectures, and finally pushing/registering the image in the "mecsolutionaccelerator" repo at Docker Hub.

That command (with --push) pushed 3 images and 1 tag to the Docker registry.

I can now try pulling the image and running the container from any machine with Docker Linux and x64 or ARM:

```console
docker run --rm -it mecsolutionaccelerator/dotnetapp
```
Again, this is from a Docker host on ARM machine (It's an NVIDIA Orin with ARM processor) and using a single Docker image name and tag!:

![image](https://github.com/Azure/mec-app-solution-accelerator/assets/1712635/268fdd75-c80a-4647-aa01-a4d97f57c299)

## Disabling buildx

In order to disable buildx from your Docker host, run this command:

```console
docker buildx stop
```

# Enabling buildx in "docker compose"

To use buildx in docker-compose, you can set the environment variable *COMPOSE_DOCKER_CLI_BUILD=1* and *DOCKER_BUILDKIT=1*. 
If buildx is not set as default, you should add DOCKER_DEFAULT_PLATFORM=linux/amd64

Here’s an example of how to use buildx in docker-compose file:

```console
version: '2.4'
services:
  testbuild:
    build: ../testbuild
    image: testbuild
    platform: linux/arm64/v8
```
