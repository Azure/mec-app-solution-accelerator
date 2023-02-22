# MEC Application Solution Accelerator (Example Reference Application for Edge/MEC)

This repo provides a reference MEC (Multi-Access Edge Compute) application example (curent state is *Alpha*), based on an event-driven microservices architecture, using Docker containers running at the Edge. 

## What is a MEC Application?

A MEC Application is basically an application composed by a set of services that in order to provide the right functionality and on-time to the users it needs to run at the EDGE (very close to the data source), so it can react real-time and instantly to events happening, while being connected to devices and related acctions with very low latency end-to-end. 

In most use cases, IoT devices, video analysis and AI/Deep-Learning models to detect/predict based on the data coming from the devices, are part of this type of applications.

This new application development paradigm targeting scenarios not possible years ago is what Microsoft defines as Modern Connected Applications:

<img width="800" alt="image" src="https://user-images.githubusercontent.com/1712635/214922412-efc15e6b-166b-450e-a0cd-d139dc7b54eb.png">

A 'MEC Application' is a subtype of application within the 'Modern Connected Applications' realm. 

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

## Goals of this MEC app Solution Accelerator (Example application)

As mentioned, the network needs are provided by the infrastructure such as a 5G network and EDGE compute.
However, aspects such as being able to create and event-driven application (Devices-->AI-->Events-->Logic-->Alerts-->Handlers) need to be implemented by your application and it's precisely the value that this example application provides:

- Recommendations on using event-driven and microservices architecture explained by this example implementation.
- Showcase of design patterns to implement (Event Pub/Sub, microservices autonomy, extensible events/alerts metadata schemas, etc.).
- Show how to inference with deep-learning models from a microservice, in this case using 'Yolo', for video/image analytics, then generate the related detection event.

The initial use case is about video analytics, but video analytics / computer vision is not the main goal of this example application but to provide a "backbone" to create your own event-driven microservice application running at the EDGE, on Kubernetes. This "backbone" is also applicable for other types of data processing such as data coming from IoT sensors that need to be automatically analyzed by AI models and events/alerts raised if needed.

> **DISCLAIMER:** This is an example application providing patterns, approaches and best practices targeting applications to be deployed at MEC/EDGE. 
> However, this is still an example application with no "production-ready" code but just for exploring architecture and implementation approaches.
> This application's code will be evolving, new features will be added and growing in the spirit of improving its quality thanks to open source contributions (PRs) from you.

## Supported deployments for this example application:

| | |
|--------|--------|
| **"Production" environment:** <img width="120" alt="image" src="https://user-images.githubusercontent.com/1712635/214693427-4d906e3a-7a1e-4623-b413-006219af1da6.png">| On any Kubernetes cluster typically deployed at Edge compute, such as on-premises AKS like AKS Edge Essentials, AKS from Azure Stack Edge server or AKS from Azure Stack HCI / Arc-Enabled. For testing purposes, is can be deployd on any Kubernetes environment, including a development machine with AKS Edge Essentials, Docker for Desktop and Kubernetes, or in the cloud, into Azure AKS. (Note: We mean a hypothetical "production" environment.) |
| **Development environment:** <img width="120" alt="image" src="https://user-images.githubusercontent.com/1712635/214693693-092921c1-7fb5-4878-87d4-559b075afc30.png"> | On any local Docker host, with 'docker compose' directly from a console command prompt (Windows/Linux/MacOS) or from Visual Studio in Windows (running Docker host with Linux, under the covers), for easy test and development in dev environments. |

A typical deployment of this application/services would be performed at the Edge, such as in a 5G Private MEC, using a wireless 5G network to connect the devices, cameras, etc. to the application's services. However, this application can be tested by itself without any specific network requirement.

## Architecture overview

This reference application proposes an event-driven and microservice oriented architecture implementation with multiple autonomous microservices. The main goal is to ingress data from client IoT/edge devices. The initial functionality for this sample application is to ingress video from wireless cameras, analyze it with AI models, detect issues, create events and submit them to a messaging broker thorugh a Publish/Subscription approach so additional microservices evaluate if the events need to be converted to Alerts and publish the alerts to the multiple event handlers related, such as an "Alerts dashboard" app or any other integrated process that needs to react in real-time thanks to the low latency provided by the Edge, as shown in the below architecture diagram.

<img width="857" alt="image" src="https://user-images.githubusercontent.com/1712635/214972301-055f9cdf-9c26-4200-beaa-45e14292e164.png">

However, this is not just about 'Video analytics'. The important value of this architecture and reference applications is based on the event-driven architecture which can be very easily customized to support different types of "input data" from IoT devices, so instead of video, it coud ingress data from IoT sensors, or manufacturing machines, analyze it with different type of AI models in the same MEC's network and again generate comparable events and derived alerts with a very low latency.

Therefore, the importance of this example applications is about the event-driven design patterns implemented by using light MQTT messaging brokers and effective dedicated microservices leveraging DAPR (Microsoft's framework specialized on microservices patters) and deployed on Kubernetes so the solution can be deployed on most EDGE environments supporting Kubernetes.   

## Example deployment on 5G Private MEC

A typical deployment of this type of solution would be to deploy it into a **5G Private MEC** such as the one supported by **Azure Private MEC** solution and **Azure Private 5G Core (AP5GC)**, as shown in the diagram below which would define a sinple 5G Lab for a 5G Private MEC.

This MEC application Solution Accelerator (example application) would be deployed into the highlighted in yellow server/s in the following 5G Private MEC diagram:

![image](https://user-images.githubusercontent.com/1712635/214719118-1191fac7-b424-44eb-b81c-f8ec10e18124.png)

Only what's highlighted in yellow is purely related to this MEC application Solution Accelerator (example application). The rest of the elements in the shown diagram are part of the needed 5G network infrastructure plus Azure cloud services for infrastructure management plus other optional Azure services that could be used for long term analysis and aggregation, such as Azure Data Explorerm Digital Twins and Azure Machine Learning for training new AI models, etc. 

When moving to production you would need to scale out the number of 5G RANs depending on how large is the area to cover, the number of servers for Azure Private 5G Core depending on the number of 5G network sites and the number of servers for application compute depending on the compute requirements demanded by AI models and application process which can vary depending on the number of video cameras and/or IoT devices to handle.

### "Heavy EDGE" as the selected approach for this example application

It's important to highlight that the selected approach for this example application is "centralized per EDGE site", which is called **"Heavy Edge"**, so you have a single or few sets of EDGE compute servers/appliances on-premises in central places, versus **"Heavy User Equipment Edge"** which needs one compute-machine (such as NVDIA ORIN) per camera or cluster of IoT devices. 

With **"Heavy User Equipment Edge"** because you are placing the AI models compute besides the data source, then network bandwidth needed can be much lighter in the MEC's wireless network.

However, in this project we wanted to target the "global approach" because in most cases allows a lower cost (TCO) because of a enabling a centralized management of the compute with a smaller number of compute appliances to manage because of sharing the compute with Kubernetes scalability (compute shared across many Kubernetes pods).

  
Both approaches are good approaches depending on the needs and shown below. 
"Heavy EDGE" is therefore the selected approach for this example application.

<img width="976" alt="image" src="https://user-images.githubusercontent.com/1712635/214955088-2d15fbfb-1548-4665-8145-59f301b0d70a.png">

## Getting Started (Development environment)

> **IMPORTANT NOTE:** Get the stable code from the **main branch**. But for  getting the latest version of the code, **use the DEV branch** of the repo. However, the DEV branch might still be in testing phase and could be less stable. 

### Grab the GitHub repo code

Get the GitHub repo's code:

```powershell
git clone git@github.com:Azure/mec-app-solution-accelerator.git
```

## Local Docker host deployment alternatives

When developing, testing and debugging the MEC application it's easier and more straightforward to deploy into Docker so, for instance, you can debug code with Visual Studio and can easily test the application with just Docker installed without further setup steps as required by Kubernetes.

| | |
|--------|--------|
| <img width="100" alt="image" src="https://user-images.githubusercontent.com/1712635/214690304-eca6fc41-b4d5-4122-bf0c-47c5dc955da3.png"> | **Deployment on [Docker for Desktop with 'docker compose up'](./docs/DOCKER_COMPOSE_DEPLOYMENT.MD)** |
| <img width="40" alt="image" src="https://user-images.githubusercontent.com/1712635/214689990-47bd981b-c756-444e-84f1-140a63d3ca7f.png"> | **Deployment on Docker with [Visual Studio (F5 experience)](./docs/VS_DOCKER_DEPLOYMENT.MD)** |
| | |



## Kubernetes deployment alternatives ("Production" environment)

This sample microservices application can be deployed on most Kubernetes distributions clusters. 

Because we're targeting deployment at the Edge and also development environments, in most cases (unless you need significant process power when scaling out to many video sources/cameras) your Kubernetes clusters will be composed by a single cluster node (like a dev machine environment or light edge environment). 

Refer to the following procedure information pages to learn how to deploy to your selected Kubernetes distribution:

| | |
|--------|--------|
| <img width="100" alt="image" src="https://user-images.githubusercontent.com/1712635/214690304-eca6fc41-b4d5-4122-bf0c-47c5dc955da3.png"> | **Deployment on [local Kubernetes in 'Docker Desktop'](/docs/K8S_IN_DOCKER_DESKTOP_DEPLOYMENT.MD)** |
| <img width="120" alt="image" src="https://user-images.githubusercontent.com/1712635/214690383-05f86a79-3edd-4b7e-af46-4889273e9910.png"> | **Deployment on [local MiniKube](/docs/K8S-MINIKUBE_DEPLOYMENT.MD)** |
| | |


## Other useful configurations

In order to test your own scenarios you might want to try the following operations even before customizing or forking the application's code:

| | |
|--------|--------|
| <img width="100" alt="image" src="TBD.png"> | **[How to provision your own video RTSP feed in the app with configuration](/docs/RTSP_START_UP.MD)** |
| <img width="120" alt="image" src="TBD.png"> | **[How to create your own VM in Azure with a RTSP faking a camera](/docs/HOW_TO_USE_OWN_MODEL_CLASSES.MD)** |
| <img width="120" alt="image" src="TBD.png"> | **[How to use your own 'classes' to be detected by the AI model](/docs/HOW_TO_USE_OWN_MODEL_CLASSES.MD)** |
| <img width="120" alt="image" src="TBD.png"> | **[How to scale up the number of pods in Kubernetes deployment configuration](/docs/HOW_TO_SCALE_UP_K8S_PODS.MD)** |
| | |


## Backlog

The backlog is defined in detail here: [Backlog](BACKLOG.MD)


## Contributing

> PLEASE Read our [branch guide](BRANCH-GUIDE.MD) to know about our branching policy when contributing with PRs.
> **Note for Pull Requests (PRs):** We accept pull requests from the community. When doing it, please do it onto the DEV branch which is the consolidated work-in-progress branch. Do not request it onto MAIN branch. 

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
