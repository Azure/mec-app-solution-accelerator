# Backlog

## Edge scope

### Light Event Bus
- **Replace Mosquito by Microsoft E4K as the light MQTT message broker:** E4K will provide much better integration with Azure management, security, etc.

### Security

- **Authentication/Authorization:** Authentication/Authorization based on an Identity Server such as **Keycloak** or **Duende** (**aka.IdentityServer4**) 

### Automated provisioning of cameras/devices
- **Automated provisioning of cameras/devices with AKRI**

### Custom code and Kubernetes

- **Versioning Microservices (semantic)**
- **Migrate K8s deployment YAML to HELM charts**
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

## Cloud scope

### Global aggregated Data in the cloud 
- **Application data upload to cloud for consolidation and aggregation:** Upload events and alerts through **Azure Event Hub** based on **Kafka connector from E4K** (Dependency on E4K at the EDGE).
- **Global Telemetry uploaded to cloud**. 
- **Storage:**: Application data aggregation in **Azure CosmosDB** and logs into **Azure Data Explorer**.  

### Global observability, logging and performance/scalability analysis
- Performance analisys, execution time-spans, average per 95% percentile, etc. based on **Azure Log Analytics**. 

### Application level in the cloud
- **Global application alerts dashboard aggregating multiple MEC:** "Single Pane of Glass" as a global dashboard aggregating multiple MEC sites (Custom development).
- **Global Digital Twin in cloud:** Global visibility with **Azure Digital Twin** about all elements/devices handled in the multiple MEC sites. 

### Security
- **Secrets in Azure KeyVault**
