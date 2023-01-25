# MEC Application Solution Accelerator (Example Reference Application for Edge/MEC)

This repo provides a reference MEC (Multi-Access Edge Compute) application example, powered by Microsoft and based on an event-driven microservices architecture, using Docker containers running at the Edge. 

## Supported deployments:

- **"Production" environment:** On any Kubernetes cluster typically deployed at Edge compute, such as on-premises AKS on an Azure Stack Edge server or any Azure Stack HCI / Arc-Enabled. For testing purposes, is can be deployd on any Kubernetes environment, including a development PC with Docker for Desktop and Kubernetes, or in the cloud, into Azure AKS.
- **Development environment:** On any local Docker host, with 'docker compose' or Visual Studio, for easy test and development in dev environments.

A typical deployment of this application/services would be performed at the Edge, such as in a 5G Private MEC, using a wireless 5G network to connect the devices, cameras, etc. to the application's services. However, this application can be tested by itself without any specific network requirement.

## Getting Started

> **IMPORTANT NOTE:** For getting the latest version of the code, **use the DEV branch of the repo**. 
> Until March 2023, the MAIN branch can be significantly outdated compared to code in the DEV branch.

### Grab the GitHub repo code

Get the GitHub repo's code:

```powershell
git clone git@github.com:Azure/mec-app-solution-accelerator.git
```

Select the 'dev' branch with git:
```powershell
cd <your local path>
git checkout dev
```

### Deploy on Docker ('docker compose')

Make sure you have [installed](https://docs.docker.com/desktop/install/windows-install/) and configured docker in your environment. After that, you can run the below commands from the solution's root directory and get started trying it.

**Build the Docker images:**
```powershell
docker-compose build
```
<img width="800" alt="image" src="https://user-images.githubusercontent.com/1712635/214674622-c404aa17-8b16-4df8-b958-ff8423995d67.png">

**Run the solution:**

```powershell
docker-compose up
```
You should see 'docker compose up' starting like in the following screenshot:
<img width="800" alt="image" src="https://user-images.githubusercontent.com/1712635/212741292-4396cc66-3ce9-451b-8d2f-bb3e6ec8e8b2.png">

Wait until all containers are up and running and you start seeing traces related to detections performed by the AI model and events raised because of them, like in the following screenshot:
<img width="796" alt="image" src="https://user-images.githubusercontent.com/1712635/214675605-954ceeb1-70b0-40a4-9a9a-138313cc9b86.png">

At this point, you should be able to run the "Alerts Dashboard" web app with the following URL in any browser:

**Alerts Dashboard UI web app:**
```code
https://localhost:50058
```
<img width="800" alt="image" src="https://user-images.githubusercontent.com/1712635/214684282-2aa3739e-cd61-47b5-a7e9-2a01e9d040ae.png">

### Additional supported deployments

This sample microservice application can run locally using a local Kubernetes cluster such as "Kubernetes in Docker for Desktop" or any other Kubernetes distribution.
For development purposes, you can also run it on plain Docker with "docker compose up" or Visual Studio.
Refer to these additional procedure information pages to Get Started on each environment:

https://user-images.githubusercontent.com/1712635/214689990-47bd981b-c756-444e-84f1-140a63d3ca7f.png

| <img width="50" alt="image" src="https://user-images.githubusercontent.com/1712635/214689990-47bd981b-c756-444e-84f1-140a63d3ca7f.png"> | [Visual Studio (F5 experience)](./VS_DOCKER_DEPLOY.MD) |
|--------|--------|

| <img width="50" alt="image" src="https://user-images.githubusercontent.com/1712635/214690304-eca6fc41-b4d5-4122-bf0c-47c5dc955da3.png"> | [Local Kubernetes by Docker for Desktop](https://tbd-url) |
| <img width="50" alt="image" src="https://user-images.githubusercontent.com/1712635/214690383-05f86a79-3edd-4b7e-af46-4889273e9910.png"> | [Local MiniKube](https://tbd-url) |

### URLs when using Visual Studio (Move to VS page to create)

**Temporal for MyFrontEnd web app:**
https://localhost:52710/
or
https://host.docker.internal:52710/

**Alerts.API service Swagger page:**
http://localhost:52708/swagger/index.html

<img width="481" alt="image" src="https://user-images.githubusercontent.com/1712635/212743730-a75e728c-8d6d-4267-9847-1f141d7ad7f9.png">


## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft 
trademarks or logos is subject to and must follow 
[Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general).
Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship.
Any use of third-party trademarks or logos are subject to those third-party's policies.
