# MEC Application Solution Accelerator (Example Reference Application for Edge/MEC)

This repo provides a reference MEC (Multi-Access Edge Compute) application example, powered by Microsoft and based on an event-driven microservices architecture, using Docker containers running at the Edge. 

## What is a MEC Application?

A MEC Application is basically an application composed by a set of services that in order to provide the right functionality and on-time to the users it needs to run at the EDGE (very close to the data source), so it can react real-time and instantly to events happening, while being connected to devices and related acctions with very low latency end-to-end. 

In most use cases, IoT devices, video analysis and AI/Deep-Learning models to detect/predict based on the data coming from the devices, are part of this type of applications.

This new application development paradigm targeting scenarios not possible years ago is what Microsoft defines as Modern Connected Applications:

<img width="800" alt="image" src="https://user-images.githubusercontent.com/1712635/214922412-efc15e6b-166b-450e-a0cd-d139dc7b54eb.png">

Therefore, the main needs for this kind of applications are:

- Event-Driven based on light message brokers
- AI/ML models
- Low latency (Instant reaction is a must)
- High bandwidth for heavy communication (i.e. video)
- High network reliability for mission-critical
- Support for massive number of IoT devices wirelesly communicated
- Able to cover broad/large areas, wirelessly
- Dynamic and on-demand network QoS 

The network related needs are usually provided by environments such as 5G Private MEC and 5G Public MEC, but Wi-Fi and cabled network are also complementary.

However, aspects such as application low latency and event-driven are provided by this app. 

## Supported deployments for this example application:

| | |
|--------|--------|
| **"Production" environment:** <img width="120" alt="image" src="https://user-images.githubusercontent.com/1712635/214693427-4d906e3a-7a1e-4623-b413-006219af1da6.png">| On any Kubernetes cluster typically deployed at Edge compute, such as on-premises AKS on an Azure Stack Edge server or any Azure Stack HCI / Arc-Enabled. For testing purposes, is can be deployd on any Kubernetes environment, including a development PC with Docker for Desktop and Kubernetes, or in the cloud, into Azure AKS. (Note: Hypothetical "production" environment.) |
| **Development environment:** <img width="120" alt="image" src="https://user-images.githubusercontent.com/1712635/214693693-092921c1-7fb5-4878-87d4-559b075afc30.png"> | On any local Docker host, with 'docker compose' directly from a console command prompt (Windows/Linux/MacOS) or from Visual Studio in Windows (running Docker host with Linux, under the covers), for easy test and development in dev environments. |

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

| | |
|--------|--------|
| <img width="40" alt="image" src="https://user-images.githubusercontent.com/1712635/214689990-47bd981b-c756-444e-84f1-140a63d3ca7f.png"> | **Deployment on [Visual Studio (F5 experience)](./VS_DOCKER_DEPLOY.MD)** |
| <img width="100" alt="image" src="https://user-images.githubusercontent.com/1712635/214690304-eca6fc41-b4d5-4122-bf0c-47c5dc955da3.png"> | **Deployment on [Local Kubernetes by Docker for Desktop](https://tbd-url)** |
| <img width="120" alt="image" src="https://user-images.githubusercontent.com/1712635/214690383-05f86a79-3edd-4b7e-af46-4889273e9910.png"> | **Deployment on [Local MiniKube](https://tbd-url)** |


## Architecture overview

This reference application proposes an event-driven and microservice oriented architecture implementation with multiple autonomous microservices. The main goal is to ingress data from client IoT/edge devices. The initial functionality for this sample application is to ingress video from wireless cameras, analyze it with AI models, detect issues, create events and submit them to a messaging broker thorugh a Publish/Subscription approach so additional microservices evaluate if the events need to be converted to Alerts and publish the alerts to the multiple event handlers related, such as an "Alerts dashboard" app or any other integrated process that needs to react in real-time thanks to the low latency provided by the Edge, as shown in the below architecture diagram.

![image](https://user-images.githubusercontent.com/1712635/214716588-cc19eb48-4aa8-44e3-a9a6-340a4cc5b7aa.png)

However, this is not just about 'Video analytics'. The important value of this architecture and reference applications is based on the event-driven architecture which can be very easily customized to support different types of "input data" from IoT devices, so instead of video, it coud ingress data from IoT sensors, or manufacturing machines, analyze it with different type of AI models in the same MEC's network and again generate comparable events and derived alerts with a very low latency.

Therefore, the importance of this example applications is about the event-driven design patterns implemented by using light MQTT messaging brokers and effective dedicated microservices leveraging DAPR (Microsoft's framework specialized on microservices patters) and deployed on Kubernetes so the solution can be deployed on most EDGE environments supporting Kubernetes.   

## Example deployment on 5G Private MEC

A typical deployment of this type of solution would be to deploy it into a **5G Private MEC** such as the one supported by **Azure Private MEC** solution and **Azure Private 5G Core (AP5GC)**, as shown in the diagram below which would define a sinple 5G Lab for a 5G Private MEC.

This MEC application Solution Accelerator (example application) would be deployed into the highlighted in yellow server/s in the following 5G Private MEC diagram:

![image](https://user-images.githubusercontent.com/1712635/214719118-1191fac7-b424-44eb-b81c-f8ec10e18124.png)

Only what's highlighted in yellow is purely related to this MEC application Solution Accelerator (example application). The rest of the elements in the shown diagram are part of the needed 5G network infrastructure plus Azure cloud services for infrastructure management plus other optional Azure services that could be used for long term analysis and aggregation, such as Azure Data Explorerm Digital Twins and Azure Machine Learning for training new AI models, etc. 

When moving to production you would need to scale out the number of 5G RANs depending on how large is the area to cover, the number of servers for Azure Private 5G Core depending on the number of 5G network sites and the number of servers for application compute depending on the compute requirements demanded by AI models and application process which can vary depending on the number of video cameras and/or IoT devices to handle.

## Backlog

tbd

## Roadmap

tbd

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
