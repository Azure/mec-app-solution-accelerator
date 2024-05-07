# MEC Application Solution Accelerator (Example Reference Application for Edge/MEC)

Get started developing apps for 5G Private MEC (Multi-Access Edge Compute) & other Edge scenarios also including Wi-Fi, with this 'MEC Application Solution Accelerator' (example app) based on an event-driven microservices architecture, using Docker containers, Kubernetes, Dapr and Azure IoT Operations including a MQTT message broker with Publish/Subscription to handle the events generated by AI DeepLearning models running at the edge.

The selected use case is a live video analytics (computer vision) at the edge with alerts dashboards and dynamic provisioning of any number of IP video cameras at the MEC (Multi-access Edge Compute through 5G, Wi-Fi or Ethernet), to provide real-time alerts on issues detected by AI models analyzing the video streams.

# Introduction and high level architecture

## What is a MEC Application?

A MEC Application is basically an application composed by a set of services that in order to provide the right functionality and on-time to the users it needs to run at the EDGE (very close to the data source), so it can react real-time and instantly to events happening, while being connected to devices and related acctions with very low latency end-to-end. 

In most use cases, IoT devices, video analysis and AI/Deep-Learning models to detect/predict based on the data coming from the devices, are part of this type of applications.

This new application development paradigm targeting scenarios not possible years ago is what Microsoft defines as Modern Connected Applications:

<img width="800" alt="image" src="https://github.com/Azure/mec-app-solution-accelerator/assets/1712635/f950fe8f-3a2f-410b-b531-6cadb5a28bea">

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

The network related needs are usually provided by environments such as 5G Private MEC and 5G Public MEC, but Wi-Fi and cabled network are also possible and complementary.

Check out this blog post for additional explanations: https://aka.ms/buildwith5G


## Goals for the 'MEC app solution accelerator' (This example application)

As mentioned, the network needs are provided by the infrastructure such as a 5G network or Wi-Fi and eEdge compute. 

However, aspects such as being able to create an event-driven application (Devices-->AI-->Events-->Logic-->Alerts-->Handlers) need to be implemented by your application and it's precisely the value that this example application provides:

- Recommendations on using event-driven and microservices architecture explained by this example implementation.
- Showcase of design patterns to implement (Event Pub/Sub, microservices autonomy, extensible events/alerts metadata schemas, etc.).
- Show how to inference with deep-learning models from a microservice, in this case using 'Yolo', for video/image analytics, then generate the related detection event.

The initial use case is about video analytics, but video analytics / computer vision is not the main goal of this example application but to provide a "backbone" to create your own event-driven microservice application running at the EDGE, on Kubernetes. This "backbone" is also applicable for other types of data processing such as data coming from IoT sensors that need to be automatically analyzed by AI models and events/alerts raised if needed.

> **DISCLAIMER:** This is an example application providing patterns, approaches and best practices targeting applications to be deployed at MEC/EDGE. 
> However, this is still an example application with no "production-ready" code but just for exploring architecture and implementation approaches.
> This application's code will be evolving, new features will be added and growing in the spirit of improving its quality thanks to open source contributions (PRs) from you.


# Supported application deployment alternatives 

| | |
|--------|--------|
| **"Production" environment:** <img width="120" alt="image" src="https://user-images.githubusercontent.com/1712635/214693427-4d906e3a-7a1e-4623-b413-006219af1da6.png">| On any Kubernetes cluster typically deployed at Edge compute, such as on-premises AKS Edge Essentials, AKS from Azure Stack HCI / Arc-Enabled or K3s on Ubuntu Linux or even in AKS in Azure cloud for a testing environment. (Note: We mean a hypothetical "production" environment, since this is an example app.) |
| **Development environment:** <img width="120" alt="image" src="https://user-images.githubusercontent.com/1712635/214693693-092921c1-7fb5-4878-87d4-559b075afc30.png"> | On any local Docker host, with 'docker compose' directly from a console command prompt (Windows/Linux/MacOS) or from Visual Studio in Windows (running Docker host with Linux, under the covers), for easy testing and development in dev environments. |

A typical deployment of this application/services would be performed at the Edge, such as in a 5G Private MEC, using a wireless 5G network to connect the devices, cameras, etc. to the application's services. However, this application can be tested by itself without any specific network requirement (you can try it on a single laptop/computer!).

## Deployment alternatives for Kubernetes ("Production" environment)

This sample microservices application is supported on the following deployment combination alternatives for different distros and host operating system:

<img width="1000" alt="image" src="https://github.com/Azure/mec-app-solution-accelerator/assets/1712635/152b170c-6443-4141-ac5c-fa2b0e5b06ee">

We have grouped the multiple combination alternatives in two different setup procedures docs:





| | | |
|--------|--------|--------|
| <img width="140" alt="Kubernetes logo" src="https://github.com/Azure/mec-app-solution-accelerator/assets/1712635/62ffa845-b8ed-49f2-9aed-81047d236f85"> | [Simplified setup with application deployment script]() | The infrastructure installation is the same but the application components deployment is simplified with a single Script (For Windows PowerShell of for Linux Bash) |
| <img width="140" alt="Kubernetes logo" src="https://github.com/Azure/mec-app-solution-accelerator/assets/1712635/62ffa845-b8ed-49f2-9aed-81047d236f85"> | [Step-by-step application components deployment]() | The infrastructure installation is the same but in this case the application components are deployed one by one into Kubernetes so developers can know what's going on under the covers |
| | | |

## Local Docker deployment alternatives (Development and Test environment)

When developing, testing and debugging the MEC application it's easier and more straightforward to deploy into Docker so, for instance, you can debug code with Visual Studio and can easily test the application with just Docker installed without further setup steps as required by Kubernetes.

| | |
|--------|--------|
| <img width="140" alt="image" src="https://user-images.githubusercontent.com/1712635/220490921-dc521a14-3f0a-481f-8179-7233a744dbc1.png"> | **Deploy application services to [Docker for Desktop with 'docker compose up'](./docs/DOCKER_COMPOSE_DEPLOYMENT.MD)** |
| <img width="130" alt="image" src="https://user-images.githubusercontent.com/1712635/220490972-9140e540-3000-47f0-a3c4-4e64b4976266.png"> | **Deploy application services to Docker with [Visual Studio (F5 experience)](./docs/VS_DOCKER_DEPLOYMENT.MD)** |
| | |




# Configurations for easy customization

In order to test your own scenarios you might want to try the following operations even before customizing or forking the application's code:

| | |
|--------|--------|
| <img width="70" alt="Camera icon" src="https://user-images.githubusercontent.com/1712635/220493758-47ec3c24-7a29-4e85-8f20-ee141e2f538a.png"> | **[How to provision your own video RTSP feed in the app with configuration](/docs/HOW_TO_PROVISION_NEW_FEED.MD)** |
| <img width="70" alt="VM icon" src="https://user-images.githubusercontent.com/1712635/220493850-b6391852-78d4-4b53-9dac-841ecdad2551.png"> | **[How to create your own VM in Azure with a RTSP faking a camera](/docs/HOW_TO_CREATE_RTSP_SERVER.MD)** |
| <img width="80" alt="Model's classes cat dog" src="https://user-images.githubusercontent.com/1712635/220493891-de118a4c-c228-4078-8536-efd5680227a8.png"> | **[How to use your own 'classes' to be detected by the AI model](/docs/HOW_TO_USE_OWN_MODEL_CLASSES.MD)** |
| <img width="70" alt="Kubernetes scalability icon" src="https://user-images.githubusercontent.com/1712635/220494004-d638e5e0-41e6-4aa7-a004-b85e73418022.png"> | **[How to scale up the number of pods in Kubernetes deployment configuration](/docs/HOW_TO_SCALE_UP_K8S_PODS.MD)** |
| | |


# Backlog

The backlog is defined in detail here: [Backlog](BACKLOG.MD)


# Contributing

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

# Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft 
trademarks or logos is subject to and must follow 
[Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general).
Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship.
Any use of third-party trademarks or logos are subject to those third-party's policies.
