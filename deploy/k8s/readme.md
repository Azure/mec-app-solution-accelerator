# Deployment to Kubernetes ("Production" environment)

## Kubernetes deployment alternatives 

This sample microservices application can be deployed on most Kubernetes distributions clusters. 

Because we're targeting deployment at the Edge and also development environments, in most cases (unless you need significant process power when scaling out to many video sources/cameras) your Kubernetes clusters will be composed by a single cluster node (like a dev machine environment or light edge environment). 

Refer to the following procedure information pages to learn how to deploy to your selected Kubernetes distribution:

| | |
|--------|--------|
| <img width="130" alt="image" src="https://user-images.githubusercontent.com/1712635/214690304-eca6fc41-b4d5-4122-bf0c-47c5dc955da3.png"> | **Deploy application services to [local Kubernetes in 'Docker Desktop'](/docs/K8S_IN_DOCKER_DESKTOP_DEPLOYMENT.MD)** |
| <img width="140" alt="image" src="https://user-images.githubusercontent.com/1712635/214690383-05f86a79-3edd-4b7e-af46-4889273e9910.png"> | **Deploy application services to [local MiniKube](/docs/K8S-MINIKUBE_DEPLOYMENT.MD)** |
| <img width="140" alt="image" src="https://user-images.githubusercontent.com/1712635/214690383-05f86a79-3edd-4b7e-af46-4889273e9910.png"> | **Deploy application services to [local MiniKube](/docs/K8S-AKS-DEPLOYMENT.md)** |
| | |





