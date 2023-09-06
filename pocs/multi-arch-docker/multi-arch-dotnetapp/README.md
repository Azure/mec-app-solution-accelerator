# Multi-Arch .NET Docker Sample

Basic HELLO WORLD updated so the Docker image is multi-arch (multiple architectures, such as x64, ARM, etc. in the same image).

This allows to have a single image published in a Docker container that will work on any CPU architecture (x64, ARM, etc.)

## Try a pre-built version of the sample

```console
docker run --rm -it mecsolutionaccelerator/dotnetapp
```

## Build the MULTI-ARCH .NET Docker image with "docker buildx"

[Buildx](https://www.docker.com/blog/how-to-rapidly-build-multi-architecture-images-with-buildx/) is a nice addition to Docker tools. I think of it as “full BuildKit”. For our purposes, it enables specifying multiple platforms to build at once and to package them all up as a multi-platform tag. It will even push them to your registry, all with a single command.

We first need to setup buildx:

```console
docker buildx create --use
```

We can now build a multi-platform image for our app.

```console
docker buildx build -f Dockerfile --platform linux/arm64,linux/arm,linux/amd64 -t mecsolutionaccelerator/dotnetapp --push .
```

Here, we’re building for three architectures, and finally pushing/registering the image in the "mecsolutionaccelerator" repo at Docker Hub.

That command (with --push) pushed 3 images and 1 tag to the Docker registry.

I can now try pulling the image and running the container:

```console
docker run --rm -it mecsolutionaccelerator/dotnetapp
```

From a Docker host on x64 machine (It's a Windows 11 machine with Docker for desktop):


From a Docker host on ARM machine (It's an NVIDIA Orin with ARM processor):