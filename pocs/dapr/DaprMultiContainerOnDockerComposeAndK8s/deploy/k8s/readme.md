# Instruction for building the Docker Images and deploying to Kubernetes

This approach uses the same Docker Images built and used by DOCKER COMPOSE UP, but for KUBERNETES, so you can be sure that the same code/containers are running on both environments. 


## Build Docker Images locally

Run this command from the root of the repo:

(PowerShell or CMD)

**/> docker compose build**

That will build the following Docker Images:

**cesardl/myfrontend**

**cesardl/mybackend**

You can check that out by running:

(PowerShell or CMD)

**/> docker images**

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

K8s installation:

**kubectl apply -f https://raw.githubusercontent.com/kubernetes/dashboard/v2.6.1/aio/deploy/recommended.yaml**

- Create a admin-user to be able to enter to the dashboard, see the Kubernetes documentation.

https://github.com/kubernetes/dashboard/blob/master/docs/user/access-control/creating-sample-user.md

- Generate a token with (You'll need to generate a new token after some time, since they expire...):

**/> kubectl -n kubernetes-dashboard create token admin-user**

You'd get a token similar to this:

eyJhbGciOiJSUzI1NiIsImtpZCI6ImpBa1kxSEFwVkdSS3ROOUU4SzhpS0U3bno3czR5aWc1SUpxcFdUTEwyTmcifQ.eyJhdWQiOlsiaHR0cHM6Ly9rdWJlcm5ldGVzLmRlZmF1bHQuc3ZjLmNsdXN0ZXIubG9jYWwiXSwiZXhwIjoxNjY3MjY5MDAwLCJpYXQiOjE2NjcyNjU0MDAsImlzcyI6Imh0dHBzOi8va3ViZXJuZXRlcy5kZWZhdWx0LnN2Yy5jbHVzdGVyLmxvY2FsIiwia3ViZXJuZXRlcy5pbyI6eyJuYW1lc3BhY2UiOiJrdWJlcm5ldGVzLWRhc2hib2FyZCIsInNlcnZpY2VhY2NvdW50Ijp7Im5hbWUiOiJhZG1pbi11c2VyIiwidWlkIjoiOTc5ZWU0YjgtMmIyYi00MTczLTg1YjMtNDA4MzM5MWMwNmYyIn19LCJuYmYiOjE2NjcyNjU0MDAsInN1YiI6InN5c3RlbTpzZXJ2aWNlYWNjb3VudDprdWJlcm5ldGVzLWRhc2hib2FyZDphZG1pbi11c2VyIn0.d9oGSzfbrPPBTp4tnDpd7i4rDMqkHujR69yHvqJntdpkMmU-fnLLQG_FJ-aJD9FCsymZWgTt_6yDxGW2DPUxuIn0AVikRfdUka5Hc3lYf7XxKVZkKn709zYlFV6W8CT8MYHnihQRGVKV98j-LXaHahanOC_TSMoHclIE_1QuYfLTowNxiZXXDeCNt0LVJH9gKCFMV7tG8UiohwjbrwJBzsuE2PS7eUJluhgQCInsm1zDTu9sgGWXbgsq5HByhV7kxpNqtUMW3NX8I1MqJGijmPlLH_PBP4Vt1kYWzck01LtbUFhhFlhegAgH4OG2d-M9sNml0XUVuX9QBP5dbXhw8Q

Login with that in the K8s dashboard.

URL: http://localhost:8001/api/v1/namespaces/kubernetes-dashboard/services/https:kubernetes-dashboard:/proxy/#/login

Select the K8s namespace:

**dapr-k8s-poc**

Go to the Services link/menu.

Look for the service "myfrontend" and you'll see a related lik as "External Endpoint" which should be the following:

http://localhost:88

Click on it (or type it on the browser URL) and you should see the web app working which is accessing the backend microservice through DAPR, under the cover.

