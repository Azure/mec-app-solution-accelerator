# Instruction for building the Docker Images and deploying to Kubernetes
## Set environment variable to the Docker Hub "user" so they will be registered right
**--> Please, add instructions and example commands ----**
## Build Docker Images locally
**--> Please, add instructions and example commands ----**
## Push the created Docker Images to Docker Hub
**--> Please, add instructions and example commands ----**

WIP, no extra steps for image building. Hardocded image references for the moment until compose build contexts work for both docker compose up and k8,


## Deploy to Kubernetes 

1. Install minikube or equivalent [Minicube, dapr documentation](https://docs.dapr.io/operations/hosting/kubernetes/cluster/setup-minikube/).

1. Start minikube `minikube start`

1. Make sure you have installed Dapr on your machine on a Kubernetes cluster as described in the [Deploy dapr](https://docs.dapr.io/operations/hosting/kubernetes/kubernetes-deploy/#install-with-dapr-cli).

1. Open a new command-shell and change the current folder to the `deploy/k8s` folder of this repo.

1. Run `minikube ssh docker pull pabravil/aiinferencer:latest` command to pull a large docker image into the cluster local repo. Otherwise minikube docker pull will timeout. WIP

1. Run `kubectl apply -f ./` All services will be created in the `default` namespace.

1. To interact with kubernetes and dapr dashboards run `minikube dashboard` and `dapr dashboard -k` in new terminals 

1. Run `minikube service myfrontend` to create a tunnel to the myfrontend service. (one way of doing it with minikube)

## Stop/delete 

1. Run `minikube stop` to stop the minikube node

1. Open a new command-shell and change the current folder to the `deploy/k8s` folder of this repo.

1. Run `kubectl delete -f ./` to remove related kubernetes resources

1. To remove the minikube node, run `minikube delete`

