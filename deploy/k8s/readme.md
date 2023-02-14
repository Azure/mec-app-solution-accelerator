# Instructions for building the Docker Images to deploy to Kubernetes

## Set environment variable to the Docker Hub "user/namespace" so they will be registered right

Before creating the Docker images it's important to setup the DOCKER_REGISTRY environment variable in your dev system (Windows / Linux) so the images will be created with the right prefix (i.e. Docker hub user). This is important in order to be able to upload the Docker images into Docker Hub or any other Docker Registry from where you will deploy the images to Kuberentes.

Please, read here why and [How to setup the DOCKER_REGISTRY environment variable](./docs/SET_DOCKER_REGISTRY_VARIABLE.MD).

## Build Docker Images locally

In order to make sure you have built the Docker Images with tha latest code, build the Docker images with `docker compose build` from the root folder of this solution, such as:

```powershell
docker compose build
```

<img width="800" alt="image" src="https://user-images.githubusercontent.com/1712635/214674622-c404aa17-8b16-4df8-b958-ff8423995d67.png"> 

You could also create the Docker Images with a Script and a line using `docker build` for each Image. But using `docker compose build` is more consistent so you make sure you create and use the same images with the same image's name in Docker (dev time) and later in Kubernetes ("production" time). Note that using a different script with multiple `docker build` lines you could specify different image's names.

## Push the created Docker Images to Docker Hub

You can directly run this PowerShell script from the `deploy\k8s` folder:

```powershell
./push-docker-images-to-docker-hub.ps1
```

Internally, it's pushing the multiple Docker Images into Docker Hub, like here:

```powershell
docker push mecsolutionaccelerator/alerts-ui:latest
docker push mecsolutionaccelerator/framesplitter:latest
docker push mecsolutionaccelerator/aiinferencer:latest
docker push mecsolutionaccelerator/alertshandler:latest
docker push mecsolutionaccelerator/rulesengine:latest
```

**--> WIP: The names of those images need to be consistent to the images in docker-compose.yml ----**


# Deploy to Kubernetes from 'Docker Desktop'

Make sure you have installed ["Docker Desktop"](https://docs.docker.com/desktop/install/windows-install/).

Docker Desktop includes a standalone Kubernetes server and client, as well as Docker CLI integration that runs on your machine.

The Kubernetes server runs locally within your Docker instance, is not configurable, and is a single-node cluster. It runs within a Docker container on your local system, and is only for local development and testing.

## Enable Kubernetes in Docker Desktop

1. In order to Enable Kubernetes in Docker Desktop, check out the instructions from Docker to [Enable Kuberentes in Docker Desktop](https://docs.docker.com/desktop/kubernetes/#enable-kubernetes)

2. If you have already installed kubectl and it is pointing to some other environment, such as minikube, ensure you change the context so that kubectl is pointing to docker-desktop. Open a new command-shell and run:

    To check your current contexts:

    ```powershell
    kubectl config get-contexts
    ```

    To set the context to point to Kuberentes from Docker Desktop:

    ```powershell
    kubectl config use-context docker-desktop
    ```

## Install and Initialize DAPR

3. Install or make sure you have installed `Dapr` on your machine on a Kubernetes cluster as described in the [Deploy dapr](https://docs.dapr.io/operations/hosting/kubernetes/kubernetes-deploy/#install-with-dapr-cli). 

Note that if you were able to run the application on plain Docker, you should have installed DAPR already, but it needs to be initialized in Kuberentes, now.

4. Initialize DAPR by running:
    ```powershell
    dapr init -k
    ```

    You can test DAPR status with:
    ```powershell
    dapr status -k
    ```
    

## Deploy the application's services to Kubernetes

5. Open a new command-shell and move into the `deploy/k8s` folder of this repo, as current folder of the command-shell:

    ```powershell
    cd <your_path>/deploy/k8s
    ```

6. Deploy the application in Kuberentes by running this command:

    ```powershell
    kubectl apply -f ./
    ```

    All services will be created in the specified Kubernetes namespace for this application.

    When you are finsihed trying, you can always uninstall the application pods and all related resources from your Kuberentes by running this command:

    ```powershell
    kubectl delete -f ./
    ```

## Check the application status with Kubernetes Dashboard

7. If you don't have installed/enabled the Kuberentes Dashboard, do so by running this command:

    ```powershell
    kubectl apply -f https://raw.githubusercontent.com/kubernetes/dashboard/v2.6.1/aio/deploy/recommended.yaml
    ```

8. Run `kubectl proxy` so you can access the dahboard:

    ```powershell
    /> kubectl proxy
    ```

    Kubectl will make Dashboard available at  
http://localhost:8001/api/v1/namespaces/kubernetes-dashboard/services/https:kubernetes-dashboard:/proxy/


## Configure Bearer Tokens from Kuberentes to access the Dashboard

9. You need to do this step **only one time**: Change the current folder to the `deploy/k8s/dashboard_auth` folder of this repo and run the instructions in commands.txt to configure the tokens. Basically, the following commands need to be run just once from that folder:

    ```powershell
    kubectl apply -f dashboard-adminuser.yaml 

    kubectl apply -f adminuser-cluster-role-binding.yaml
    ```

## Generate and copy the token to provide to Kubernetes Dashboard

10. Whenever you need a token, run this command:

    ```powershell
    kubectl -n kubernetes-dashboard create token admin-user --duration=48h --output yaml
    ```

    Copy the token to the clipboard.
    

## Provide the token to Kubernetes dashboard

11. Open 'http://localhost:8001/api/v1/namespaces/kubernetes-dashboard/services/https:kubernetes-dashboard:/proxy/#/login' in your browser and paste the token you copied previously.

    In Kubernetes dashboard you should be able to explore the application's pods, services, etc.

## Access the application's UI to see Alerts originated from AI model detections

12. To access the front-end, go to 'http://localhost/' on your borwser or using the Kubernetes dashboard, select the Kuberentes namespace where the application is deployed, go to the services menu in the left tab and click on the url near the UI service.

## Stop/delete 

1. Open a new command-shell and change the current folder to the `deploy/k8s` folder of this repo.

1. Run the following command to remove all related kubernetes resources for this application, since there is no stop action on kubectl.

    ```powershell
    kubectl delete -f ./
    ```




