# Set the DOCKER_REGISTRY environment variable

## Why you need the DOCKER_REGISTRY environment variable

The current solution's docker-compose files use the 'DOCKER_REGISTRY' environment variable to specify the Docker Registry namespace to use.

It's the same mechanism and same environment variable that Docker projects created by Visual Studio use, as well. 

Basically, let's suppose that an image's name for one of the application's service is, for instance, `alertsapi`, the Docker registry user (or namespace) should be appended as a prefix, such as `myuser/alertsapi` so when uploading the image to Docker Hub or any other Docker Registry such as Azure Container Registry (ACR) the image can be automatically related to a specific Docker Registry repository namespace.

This is important because when deploying the images to Kubernetes you will need to deploy the images from a Docker Registry such as Docker Hub or Azure Container Registry. You cannot deploy images to Kuberentes directly from the local images you have in your dev machine but it needs to go through a Docker Registry, and for that, you need to use a namespace as part of the Docker Image's name. 

For instance, since we want this solution to have all the Docker images uploaded/registered in Docker Hub within the reposiory/namespace `mecsolutionaccelerator`, in that case the `DOCKER_REGISTRY` environment variable must be initialized to `mecsolutionaccelerator/`, so when those images are uploaded to Docker Hub, they will show up as the following:

![image](https://user-images.githubusercontent.com/1712635/218830882-29ece9ca-6fb7-4212-b5ca-06227f81fc60.png)

If you'd like to use your own repository/user name (namespace) in Docker Hub or any Docker Registry, you can initialize the 'DOCKER_REGISTRY' environment variable with the name you prefer, such as "mydockeruser/".

# How to set the DOCKER_REGISTRY environment variable

You set this environment variable like you do with any other OS environment vairable.
For instance, in Windows, you can do it from `System --> About --> Advanced System Settings --> Environment Variables` as shown in the following screenshot:

![image](https://user-images.githubusercontent.com/1712635/218831802-a1a3a522-ea43-4294-aa8a-99f078859d18.png)

After that, from a CMD console or PowerShell yu should be able to check that it's correctly set up:

**From CMD console:**

Type `echo %DOCKER_REGISTRY%`

![image](https://user-images.githubusercontent.com/1712635/218832133-195d5aa4-5c59-49c3-b3ed-d8e73067ae7d.png)

**From PowerShell console:**

Type `Get-ChildItem -Path Env:\DOCKER_REGISTRY`

![image](https://user-images.githubusercontent.com/1712635/218832228-47e55c9d-c581-4d5a-8205-b247ebd857ff.png)

Now, when using `docker compose build` to build your Docker images, they will be using the prefix specified in that environment variable, as you can see in this screenshot by running `docker images` after building the images:

![image](https://user-images.githubusercontent.com/1712635/218833081-0f5708ac-4391-468c-a37b-cbad5f95b535.png)

Then, you'd be ready to upload/push your Docker images to Docker hub with `docker push`, such as:

`docker push mecsolutionaccelerator/alertshandler`



