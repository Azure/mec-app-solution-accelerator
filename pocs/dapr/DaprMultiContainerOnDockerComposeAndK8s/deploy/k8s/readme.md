# Instruction for building the Docker Images and deploying to Kubernetes

This approach uses the same Docker Images built by "DOCKER COMPOSE BUILD" and used by "DOCKER COMPOSE UP", but in this case to deploy into KUBERNETES, so you can be sure that the same code/containers are running on both environments. (Debugging with VS/docker-compose and "production/scale" in Kubernetes.)

## Set environment variable to the Docker Hub "user" so they will be registered right

In this case I'm using the cesardl repo at Docker Hub, so. make sure you run the following to setup the environment variable:

**/> $env:DOCKER_REGISTRY = 'cesardl/'**

![image](https://user-images.githubusercontent.com/1712635/200633765-dbc5582c-1b00-4c56-acf8-6279b05fcf8c.png)

This is because in the docker-compose.yml (used when running "docker compose build" or "docker compose run") it uses that ENVIRONMENT VARIABLE in order to create the full image name:

![image](https://user-images.githubusercontent.com/1712635/200634554-fa030705-3332-42c1-a136-03772ca20de8.png)


## Build Docker Images locally

Run this command from the root of the repo:

(PowerShell or CMD)

**/> docker compose build**

![image](https://user-images.githubusercontent.com/1712635/200635253-63dac1a2-b789-449a-beb3-54e4002c6af5.png)

That will build the following Docker Images:

**cesardl/myfrontend**

**cesardl/mybackend**

You can check that out by running:

(PowerShell or CMD)

**/> docker images**

![image](https://user-images.githubusercontent.com/1712635/200630555-18704481-0e08-4bd2-bc1c-81b0b2aad5af.png)

## Push the created Docker Images to Docker Hub

The images need to be in a Docker Registry so they can be deployed into any Kubernetes node, afterwards.

To push the the created Docker Images to Docker Hub run this PowerShell:

(PowerShell)

**/> .\push-docker-images-to-docker-hub.ps1**

What that PowerShell script runs are these two commands:

(You don't need to run these again since they are run by the PowerShell script, though)

**docker push  cesardl/mybackend:latest**

**docker push  cesardl/myfrontend:latest**

You can check that these images are in fact registered in Docker Hub, here, with any browser:

https://hub.docker.com/repository/docker/cesardl/myfrontend

https://hub.docker.com/repository/docker/cesardl/mybackend


## Deploy to Kubernetes 

Run this PowerShell script:

**/> .\start.ps1**

Go to Kubernetes Dashboard:

- You need to have the K8s dashboard installed, first, see the Kubernetes documentation.

K8s dashboard installation:

**kubectl apply -f https://raw.githubusercontent.com/kubernetes/dashboard/v2.6.1/aio/deploy/recommended.yaml**

- Create a admin-user to be able to enter to the dashboard. Instructions here:

https://github.com/Azure/solution-accelerator-mec/tree/main/pocs/dapr/DaprMultiContainerOnDockerComposeAndK8s/deploy/k8s/k8s%20dashboard%20user-token

You can also read the Kubernetes dashboard documentation:

https://github.com/kubernetes/dashboard/blob/master/docs/user/access-control/creating-sample-user.md

- Generate a token with (You'll need to generate a new token after some time, since they expire...):

**/> kubectl -n kubernetes-dashboard create token admin-user --duration=48h --output yaml**

IMPORTANT: Use the --duration flag so the token won't expire in a few minutes. ;)

![image](https://user-images.githubusercontent.com/1712635/200639619-0ec3873e-a68d-4075-989e-230d13d125e4.png)

Login with your token in the K8s dashboard.

URL: http://localhost:8001/api/v1/namespaces/kubernetes-dashboard/services/https:kubernetes-dashboard:/proxy/#/login

Select the K8s namespace:

**dapr-k8s-poc**

Go to the Services link/menu.

Look for the service "myfrontend" and you'll see a related lik as "External Endpoint" which should be the following:

http://localhost:88

![image](https://user-images.githubusercontent.com/1712635/200631111-7390233a-4785-4d3a-aa09-a92ca8c1a4c3.png)

Click on it (or type it on the browser URL) and you should see the web app working which is accessing the backend microservice through DAPR, under the cover.

