# Backlog

## Edge scope

### Light Event Bus
- **Replace Mosquito by Microsoft E4K as the light MQTT message broker:** E4K will provide much better integration with Azure management, security, etc.

### Security

- **Authentication/Authorization:** Authentication/Authorization based on an Identity Server such as **Keycloak** or **Duende** (**aka.IdentityServer4**) 

### Automated provisioning of cameras/devices
- **Automated provisioning of cameras/devices with AKRI**

### Custom code and Kubernetes

- **Advanced Versioning of Microservices (semantic)**: Implement a more advanced versioning system based on aspnet-api-versioning or comparable system: https://github.com/Microsoft/aspnet-api-versioning
- **Migrate K8s deployment YAML to HELM charts**: Related to Open Application Model [OAM](https://oam.dev/) which uses HELM.
- **Refactor to comply with Open Application Model [OAM](https://oam.dev/)** to have the app definition decoupled from app deployment on multiple supported environments.
  -   https://oam.dev/
  -   https://github.com/oam-dev
  -   https://cloudblogs.microsoft.com/opensource/2019/10/16/announcing-open-application-model/
  -   https://learn.microsoft.com/en-us/shows/azure-friday/intro-to-rudr-a-kubernetes-implementation-of-the-open-application-model
  -   https://github.com/kubevela/kubevela
- **High coverage of Unit Tests and Functional Tests**
- **Local MEC error loging**
- **SPA web app with related API Gateway:** Such as a **.NET BLAZOR** WebAssembly SPA and **Envoy** or **OSM** as API Gateway. 
- **Shared folder in Kubernetes for images/video files:** Instead of embeded/serialized images in JSON and MongoDB database.
- **NGINX ingress in Kubernetes** (if needed)
- **Support of shared GPU across multiple pods in Kubernetes:** When supported by K8s, needed changes in implementation for inferencing the AI models with GPU.

### Alerts Rule Engine
- **Comprehensive Alerts Rule Engine based on industry/OSS solutions such as Kuiper, or Neuron**

### AI / ML / DeepLearning
- **Cascade Models support**
- **Shared GPU across K8s Pods:** Use NVIDIA DeepStream to potentially share GPUs for multiple Kuberentes Pods. References:
  - **NVIDIA GPU sharing in Kubernetes**: https://developer.nvidia.com/blog/improving-gpu-utilization-in-kubernetes/
  - **GPU time slicing in K8s** may help to have pods share GPUs: https://docs.nvidia.com/datacenter/cloud-native/gpu-operator/gpu-sharing.html
  - **Azure DeepStream architecture**, to compare code using NVIDIA DeepStream from Azure IoT Edge: https://github.com/microsoft/AzureDeepStreamAccelerator#azure-deepstream-accelerator-architecture
  - https://blog.ml6.eu/a-guide-to-gpu-sharing-on-top-of-kubernetes-6097935ababf
  - **Schedule GPUs in Kubernetes:** https://kubernetes.io/docs/tasks/manage-gpus/scheduling-gpus/

## Cloud scope

### Global aggregated Data in the cloud 
- **Application data upload to cloud for consolidation and aggregation:** Upload events and alerts through **Azure Event Hub** based on **Kafka connector from E4K** (Dependency on E4K at the EDGE).
- **Global Telemetry uploaded to cloud**. 
- **Storage:**: Application data aggregation in **Azure CosmosDB** and logs into **Azure Data Explorer**.  

### Global monitoring, observability, logging and performance/scalability analysis
- Performance analisys, execution time-spans, average per 95% percentile, etc. based on **Azure Log Analytics**. 
- Monitoring/Diagnostics of microservices based on:
  - **Azure Monitor for EDGE**
  - **Application Insights** with custom perfkeys.

### Application level in the cloud
- **Global application alerts dashboard aggregating multiple MEC:** "Single Pane of Glass" probably based on **"TRIDENT" (aka.PowerBI vNEXT)** as a global dashboard aggregating multiple MEC sites (Custom development).
- **Global Digital Twin in cloud:** Global visibility with **Azure Digital Twin** about all elements/devices handled in the multiple MEC sites. 

### Security
- **Secrets in Azure KeyVault**
