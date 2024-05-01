# MEC-Accelerator with Control-Plane-App (v1.9) - Deployment to AKS-EE-Windows or k3s-Ubuntu-Linux

## Supported environment infrastructure at the edge

This version of MEC-App-Accelerator supports the following Kubernetes distributions and host operating system.

- **AKS Edge Essentials on Windows**
- **K3s on Ubuntu Linux**

Supported environment on top of the Kubernetes/k3s cluster:

- **Azure IoT Operations alternative**
    - IoT MQ as messaging broker
    - AKRI for dynamic cameras provissioning (OSS version)

- **Plain OSS/Mosquitto alternative**
    - Eclipse Mosquitto as messaging broker
    - AKRI for dynamic cameras provissioning (OSS version)

**Doker-Desktop-Kubernetes** is not supported by Azure IoT Operations, however, if using the OSS/Mosquito alternative, it should also work for you, but we recommend the above environments since those are the tested environments by our team.



## Install the Kubernetes/K3s cluster at the edge

Install the AKS-EE-Windows cluster or the K3s-Ubuntu cluster by following the instructions in the below Microsoft official doc.

Even when the below documentation link is the supported baseline environment for an Azure IoT Operations cluster, you can also follow this AKS-EE/K3s cluster installation procedure documentation even if you won't use the Azure IoT Operations alternative but the plain OSS/Mosquitto alternative:

**Install AKS-EE-Windows alternative:** 

[Create your Azure Arc-enabled Kubernetes cluster on Windows](https://learn.microsoft.com/en-us/azure/iot-operations/deploy-iot-ops/howto-prepare-cluster?tabs=aks-edge-essentials)

**Install K3s-Ubuntu-Linux alternative:** 

[Create your Azure Arc-enabled K3s cluster on Ubuntu-Linux](https://learn.microsoft.com/en-us/azure/iot-operations/get-started/quickstart-deploy?tabs=linux#connect-a-kubernetes-cluster-to-azure-arc)

**IMPORTANT** If you are NOT going to use Azure IoT Operations, when following the previous setup doc, stop before the section named *"Deploy Azure IoT Operations Preview"* so the last secion to follow should be *"Verify cluster"*.

At this point, and after verifying the cluster from the command-line, you should also be able to check it out from the Azure portal because even when it's deployed on a machine on-premises or at the edge, it's connected to Azure through Azure Arc, so you can monitor the cluster from there, like in the following screenshot:

<img width="725" alt="image" src="https://github.com/Azure/mec-app-solution-accelerator/assets/1712635/e3637858-32bd-433a-8143-f27d0ac87b83">

However, in order to check out your cluster's Workloads (pods, deployments, etc.), you need to generate and provide a *"Service account bearer token"*, by following this procedure [Service account token authentication option](https://learn.microsoft.com/en-us/azure/azure-arc/kubernetes/cluster-connect?tabs=azure-powershell%2Cagent-version#service-account-token-authentication-option) and finally providing the token like in this other screenshot:

<img width="551" alt="image" src="https://github.com/Azure/mec-app-solution-accelerator/assets/1712635/7a490b36-755e-440d-9eab-4c395574c58f">

So then, you can monitor your AKS EE workloads in your cluster on-premises/edge from Azure cloud portal, thanks to Azure Arc connection to your cluster:

<img width="551" alt="image" src="https://github.com/Azure/mec-app-solution-accelerator/assets/1712635/99734c5e-894b-4a31-8d26-842fa6c6aa45">

*Azure Arc support:* For most cases, it's interesting to follow the above setup instructions instead of a plain K8s/K3s setup because then you will also have your cluster as an Azure Arc-enabled cluster, so you can monitor the cluster at the edge from anywhere through Azure portal in the cloud.

*Eventual removal of AKS Edge Essentials:* Eventually, if you need to install the AKS-EE, follow this procedure:

[Uninstall AKS Edge Essentials](https://learn.microsoft.com/en-us/azure/aks/hybrid/aks-edge-howto-uninstall#uninstall-aks-edge-essentials)

## Setup Azure IoT Operations into your cluster

### Prerequisites for Azure IoT Operations

It's important to check and install the prerequisites for Azure IoT Operations explained here:

[Prerequisites for Azure IoT Operations](https://learn.microsoft.com/en-us/azure/iot-operations/get-started/quickstart-deploy?tabs=windows#prerequisites)

### Deploy Azure IoT Operations Preview

Install Azure IoT Operations by following the official "simplified/automated approach" documentation here:

[Deploy Azure IoT Operations Preview](https://learn.microsoft.com/en-us/azure/iot-operations/get-started/quickstart-deploy?tabs=windows#deploy-azure-iot-operations-preview)

When deploying Aio by following the above documentation link, you should see a console app similar to the following:

![image](https://github.com/Azure/mec-app-solution-accelerator/assets/1712635/6a7be373-265c-4698-96f8-59575b4f7a0e)

Note that there's an alternative "Detailed step-by-step AKS Edge-Essentials plus Azure IoT Operations deployment" available in Microsoft's docs, but we initially recommend to go the easy path explained above instead of the following docs which require significantly more time due to the many step-by-step instructions, especially on what's related to the AKS-EE cluster setup:

**(OPTIONAL LEARNING) Detailed Azure IoT Operations setup** 
- [Prepare your Azure Arc-enabled Kubernetes cluster](https://learn.microsoft.com/en-us/azure/iot-operations/deploy-iot-ops/howto-prepare-cluster?tabs=aks-edge-essentials)
- [Deploy Azure IoT Operations Preview extensions to a Kubernetes cluster](https://learn.microsoft.com/en-us/azure/iot-operations/deploy-iot-ops/howto-deploy-iot-operations)
- [Manage secrets for your Azure IoT Operations Preview deployment](https://learn.microsoft.com/en-us/azure/iot-operations/deploy-iot-ops/howto-manage-secrets)

However, in case you want to know what's going on under the covers, it's interesting to take a look to those alternative and detailed setup docs for Azure IoT Operations.


## Deploy the MEC-Application into the cluster at the edge

### Check your kubectl context

2. If you previosly had already installed kubectl and it is pointing to some other environment, such as MiniKube, make sure the default kubectl context is pointing to the right cluster. 

    Open a new command-shell and run the following command to check your current contexts:

    ```powershell
    kubectl config get-contexts
    ```

    To set the context to point to the right Kuberentes cluster, use kubectl and provide the name of the context to use:

    ```powershell
    kubectl config use-context [my-cluster-context]
    ```

### Deploy Dapr into the cluster

This example application is based on **[Dapr](https://dapr.io/)** framework which is an OSS Microsoft framework specialized on microservices architecture, so you need to install it and enable in the Kubernetes cluster, first. 

#### Install and Initialize DAPR

- Install or make sure you have installed `Dapr` on your machine on a Kubernetes cluster as described in the [Deploy dapr](https://docs.dapr.io/operations/hosting/kubernetes/kubernetes-deploy/#install-with-dapr-cli). 

Note that if you were able to run the application on plain Docker, you should have installed DAPR already, but it needs to be initialized in Kuberentes, now.

- Initialize DAPR in the Kubernetes cluster by running this command:
 
    ```powershell
    dapr init -k
    ```
    
    ![image](https://user-images.githubusercontent.com/1712635/218881163-9ba81fa3-f72c-4c12-bbf6-8ec25f2dba55.png)

    **IMPORTANT:** This DAPR installation is okay for a dev machine with Kubernetes in AKS Edge Essentials or a single node k3s, but when installing DAPR on a "production" AKS cluster, for example in an ASE (Azure Stack Edge) server on in Azure AKS cluster in the cloud, you need to install DAPR via AKS extension following the Doc: (https://learn.microsoft.com/en-us/azure/aks/dapr) which is how DAPR should be installed in production environments and it doesn’t require cluster admin access.
    
    You can test DAPR status with:
    ```powershell
    dapr status -k
    ```
    
    If DAPR is initialized, you should get this list of Dapr pods running:
    
    ![image](https://github.com/Azure/mec-app-solution-accelerator/assets/1712635/e5d54136-caf1-4211-84d5-f466b5768e13)

    
    Otherwise, if it's not initialized it'd be like this:
    
    ![image](https://user-images.githubusercontent.com/1712635/218880976-94b42767-40e3-4d9c-a640-2dfa029cb510.png)


### (Optional) Instructions for building your own Docker Images to deploy to Kubernetes

This step is not needed if you directly deploy to Kubernetes and test the images we have already uploaded into Docker Hub, so creating your own Docker images and uploading to Docker Hub is optional, initially.

However, if you make changes in the .NET or Python code or you want to use your own Docker Images for any other reason, you'd need to create your own Docker images and upload the images first to Docker Hub before deploying to Kubernetes. 

Use these instructions to [Buil your own Docker Images to upload to Docker Registry and deploy to Kubernetes](./BUILD_AND_PUSH_IMAGES_TO_DOCKER_HUB.MD)


### Deploy the application namespace (custom K8s namespace)
1. Open a new command-shell and move into the `deploy/k8s` folder of this repo, as current folder of the command-shell:

    ```powershell
    cd <your_path>/deploy/k8s
    ```

2. Deploy the mec-accelerator namespace in Kuberentes by running this command:

    ```powershell
    kubectl apply -f ./00-namespace.yaml
    ```

![image](https://github.com/Azure/mec-app-solution-accelerator/assets/1712635/2407d56b-0b3e-4467-bdc3-59dce3527398)

### Update the application control plane configuration

3. **(OPTIONAL for Azure Private 5G network)** Update the config map that contains essential information for connecting to our Azure Mobile Network. You'll find the configuration in the file `deploy/k8s/13-control-plane-api-config-map.yaml`.

Replace the following values in the file:

```
  MobileNetwork__SubscriptionId: "your_subscription_id"
  MobileNetwork__ResourceGroup: "your_resource_group"
  MobileNetwork__Name: "your_network_name"
  MobileNetwork__AttachedDataNetwork: "your_attachedDataNetwork_fullID"
  MobileNetwork__Slice: "your_slice_fullID"
  ClientCredentials__TenantId: "your_tenant_id"
  ClientCredentials__ClientId: "your_client_id"
  ClientCredentials__ClientSecret: "your_client_secret"
```

*Note:* This step is required if you want to manage SIMs provisioning from this app against AP5GC (Azure Private 5G Core) in Azure. 
For just getting alerts and camera management/provisioning (i.e. Wi-Fi or Ethernet cameras provisioning), you can skip this part if you don't have an AP5GC Mobile Network and packet core deployed into an ASE, RAN, 5G UEs, etc.


### Install AKRI and related application discovery handler

#### Install HELM in your client machine

If you don't have HELM installed in your client system, you need to have it since AKRI-OSS is deployed through HELM. 

Make sure you have install [helm](https://helm.sh/) in the machine you are using with KUBECTL accessing the Kubernetes cluster.
By default, that machine can be a Windows machine with KUBECTL installed and pointing to the right Kubernetes cluster context. 

Full installation instructions of Helm can be found [here](https://helm.sh/docs/intro/install/).

#### Install AKRI

Install akri repository.

```powershell
helm repo add akri-helm-charts https://project-akri.github.io/akri/
```

Install AKRI with the application's "camera discovery handler" AKRI Helm chart and related configuration:

**IMPORTANT:** If you are installing agains a K3s or K8s (Kubernetes on Docker Desktop should be "k8s"), you need to specify that distro in the `kubernetesDistro` parameter below as:

`--set kubernetesDistro=k8s`

or 

`--set kubernetesDistro=k3s`

This is the whole command to run, but make sure of the `kubernetesDistro` parameter first depending on your target cluster:

```powershell
 helm install akri akri-helm-charts/akri `
 -n mec-accelerator `
 --set kubernetesDistro=k8s `
 --set custom.discovery.enabled=true `
 --set custom.discovery.image.repository=mecsolutionaccelerator/akri-camera-discovery-handler `
 --set custom.discovery.image.tag=1.8 `
 --set custom.discovery.name=akri-camera-discovery `
 --set custom.configuration.enabled=true `
 --set custom.configuration.name=akri-camera `
 --set custom.configuration.discoveryHandlerName=camera `
 --set custom.configuration.discoveryDetails.connectionString="mongodb://control-plane-mongodb.mec-accelerator:27017" `
 --set custom.configuration.discoveryDetails.database="ControlPlane" `
 --set custom.configuration.discoveryDetails.collection="Cameras" `
 --set custom.configuration.brokerPod.image.repository=mecsolutionaccelerator/framesplitter `
 --set custom.configuration.brokerPod.image.tag=1.8
```

Here's an example execution when targeting k3s:

![image](https://github.com/Azure/mec-app-solution-accelerator/assets/1712635/2ed6995f-e8d3-477e-857b-08d3ba8fca64)

### Deploy MQTT Broker resources to Kubernetes
Before continuing with the deployment, decide wether to use Azure IoT Operations E4K or Mosquitto MQTT broker.

#### Deploy Azure IoT MQ broker listener pod (aka. E4K)
  
  1. Open a new command-shell and move into the `deploy/k8s/E4K` folder of this repo, as current folder of the command-shell:
    ```powershell
    cd <your_path>/deploy/k8s/E4K
    ```
  2. Deploy E4K broker listener and the required configurations by running this command:
    ```powershell
    kubectl apply -f ./
    ```

#### Deploy Mosquitto pod
  1. Open a new command-shell and move into the `deploy/k8s/mosquitto` folder of this repo, as current folder of the command-shell:
    ```powershell
    cd <your_path>/deploy/k8s/mosquitto
    ```
  2. Deploy mosquitto and the required configurations by running this command:
    ```powershell
    kubectl apply -f ./
    ```

### Deploy the application's services into the Kubernetes cluster
8. Open a new command-shell and move into the `deploy/k8s` folder of this repo, as current folder of the command-shell:

    ```powershell
    cd <your_path>/deploy/k8s
    ```

9. Deploy the application in Kuberentes by running this command:

    ```powershell
    kubectl apply -f ./
    ```

    All services will be created in the specified Kubernetes namespace "mec-accelerator" for this application.
    
    ![image](https://github.com/Azure/mec-app-solution-accelerator/assets/1712635/3d54066f-1d77-46b6-85c3-df9c9b1b1c62)


## Deploy AKRI configuration

10. Open a new command-shell and move into the `deploy/k8s` folder of this repo, as current folder of the command-shell:

```powershell
./deploy-akri-secrets.ps1
```

![image](https://github.com/Azure/mec-app-solution-accelerator/assets/1712635/26175770-b114-4c9a-9c1b-01e87ffb4d3d)

### Check the application status with Kubernetes Dashboard

11. If you don't have installed/enabled the Kuberentes Dashboard, do so by running this command:

    ```powershell
    kubectl apply -f https://raw.githubusercontent.com/kubernetes/dashboard/v2.7.0/aio/deploy/recommended.yaml
    ```

12. Run `kubectl proxy` so you can access the dahboard:

    ```powershell
    kubectl proxy
    ```
    
    ![image](https://user-images.githubusercontent.com/1712635/218886875-0f4f1c1f-1791-4bdb-93e0-cc3ab437750a.png)

    Kubectl will make Dashboard available at  
http://localhost:8001/api/v1/namespaces/kubernetes-dashboard/services/https:kubernetes-dashboard:/proxy/

![image](https://user-images.githubusercontent.com/1712635/218886986-d838bc24-b103-437c-8f60-ebe5c2cc1095.png)

However, at this point you need to provide a security token to athenticate and enter the dashboard.
Let's do that in the following steps.

### Configure Bearer Tokens from Kuberentes to access the Dashboard

13. You need to do this step **only one time**: Change the current folder to the `deploy/k8s/dashboard_auth` folder of this repo and run the instructions in commands.txt to configure the tokens. Basically, the following commands need to be run just once from that folder:

    ```powershell
    kubectl apply -f dashboard-adminuser.yaml 

    kubectl apply -f adminuser-cluster-role-binding.yaml
    ```
    
    ![image](https://user-images.githubusercontent.com/1712635/218881848-92de552c-a0f3-4fa8-ab87-2ca2512956f2.png)


### Generate and copy the token to provide to Kubernetes Dashboard

14. Whenever you need a token, run this command:

    ```powershell
    kubectl -n kubernetes-dashboard create token admin-user --duration=48h --output yaml
    ```
    
    Select and copy the token to the clipboard.
    
    ![image](https://user-images.githubusercontent.com/1712635/218882035-61318473-b7e1-4fe5-b253-69479e8a20b0.png)

### Provide the token to Kubernetes dashboard

15. Open http://localhost:8001/api/v1/namespaces/kubernetes-dashboard/services/https:kubernetes-dashboard:/proxy/#/login in your browser and paste the token you copied previously.

![image](https://user-images.githubusercontent.com/1712635/218882193-938f14f0-06ca-4bba-979f-da9266706f8e.png)

In the Kubernetes dashboard you should be able to explore the application's pods, services, etc.

![image](https://github.com/Azure/mec-app-solution-accelerator/assets/1712635/63af4277-e4f7-413a-b0c5-abc0080ac955)


### Access the 'Control Plain app' to provision cameras (and 5G network SIMs if using 5G cameras in a Private 5G network)

16. In order to know the URL for the 'Control Plain web app', using a browser go to the Kubernetes dashboard, select the Kuberentes namespace where the application is deployed ("mec-accelerator"), go to the "Services" menu in the left tab and click on the url to the right on the **"control-plane-ui-service" service** row.

![image](https://github.com/Azure/mec-app-solution-accelerator/assets/1712635/1df800f5-f5e8-4d40-8d2f-911197a7678b)

You can also directly access the URL for the Control Plane App, but if not using LOCALHOST, the service IP will vary:

`http://<your-IP>:90/`

![image](https://github.com/Azure/mec-app-solution-accelerator/assets/1712635/f3957480-d9f4-4155-a5ff-fda03da1c0a2)


### Access the Alerts dashboard UI with Alerts from AI model detections

XX. To access the front-end, go to 'http://localhost/' on your browser or using the Kubernetes dashboard, select the Kuberentes namespace where the application is deployed, go to the services menu in the left tab and click on the url to the right on the UI service row.

![image](https://user-images.githubusercontent.com/1712635/218883329-5641f19c-f3d9-402f-a75e-2fe44aa6c9eb.png)

Either way, you should be able to run the application's UI and check out the Alerts originated from the AI models:

![image](https://user-images.githubusercontent.com/1712635/218885207-5d720a2d-f5a6-4e29-bfd1-bad384803805.png)

### Access control plane application's UI to configure and create Cameras

XX. To access the front-end, go to 'http://localhost:90/' on your browser or using the Kubernetes dashboard, select the Kuberentes namespace where the application is deployed, go to the services menu in the left tab and click on the url to the right on the control-plane-ui-service row.


### Remove the application from Kubernetes 

XX. Open a new command-shell and change the current folder to the `deploy/k8s` folder of this repo.

XX. Run the following command to remove all related kubernetes resources for this application, since there is no stop action on kubectl.

    ```powershell
    kubectl delete -f ./
    ```
XX. If Azure IoT Operations E4K broker has been used, run the following commands to remove kubernetes resouces
    ```powershell
    cd .\deploy\k8s\E4K\
    kubectl delete -f ./
    ```




