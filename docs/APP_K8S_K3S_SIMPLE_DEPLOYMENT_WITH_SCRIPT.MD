# MEC-Accelerator with Control-Plane-App (v1.9) - Deployment to AKS-EE-Windows or k3s-Ubuntu-Linux with script

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

## Setup Azure IoT Operations into your cluster (only if deploying with E4K)

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

#### Install HELM in your client machine

If you don't have HELM installed in your client system, you need to have it since AKRI-OSS is deployed through HELM. 

Make sure you have install [helm](https://helm.sh/) in the machine you are using with KUBECTL accessing the Kubernetes cluster.
By default, that machine can be a Windows machine with KUBECTL installed and pointing to the right Kubernetes cluster context. 

Full installation instructions of Helm can be found [here](https://helm.sh/docs/intro/install/).

### Update the application control plane configuration (OPTIONAL for Azure Private 5G network)
Update the config map that contains essential information for connecting to our Azure Mobile Network. You'll find the configuration in the file `deploy/k8s/13-control-plane-api-config-map.yaml`.

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

### Execute script
Inside the `deploy/k8s` folder theres a script called `deploy-accelerator` that handles of the needed operations to deploy the MEC-Application.

The script needs two parameters as arguments:
- Kubernetes distro - k8s and k3s are supported
- MQTT broker - E4K (Azure IoT E4K broker) or mosquitto are supported

Note that when using E4K MQTT broker, Azure Arc + Azure IoT operations must be installed in the cluster.

The script with make the following deployments:
- Initialize mec-accelerator namespace in the cluster where all the resources will be deployed
- Install Akri with the Kubernetes distro provided
- Deploy selected MQTT broker
- Deploy the rest of the accelerator resources

#### Executing script in Windows
Execute the script `deploy-accelerator.ps1` while providing the following parameters:
- kubernetesDistro - for the Kubernetes distro (k3s or k8s)
- mqttBroker - for the MQTT Broker (E4K or mosquitto)

Example of a K3S + E4K deployment:
```
.\deploy-accelerator.ps1 -kubernetesDistro k3s -mqttBroker E4K
```

Example of a K3S + mosquitto deployment:
```
.\deploy-accelerator.ps1 -kubernetesDistro k3s -mqttBroker mosquitto
```

#### Executing script in Linux
Execute the script `deploy-accelerator.sh` while providing the parameters for the Kubernetes distro and MQTT Broker.

Example of a K3S + E4K deployment will be:
```
./deploy-accelerator.sh --kubernetesDistro k3s --mqttBroker E4K
```

Example of a K3S + mosquitto deployment:
```
./deploy-accelerator.sh --kubernetesDistro k3s --mqttBroker mosquitto
```

### Access the 'Control Plain app' to provision cameras (and 5G network SIMs if using 5G cameras in a Private 5G network)

- In order to know the URL for the 'Control Plain web app', using a browser go to the Kubernetes dashboard, select the Kuberentes namespace where the application is deployed ("mec-accelerator"), go to the "Services" menu in the left tab and click on the url to the right on the **"control-plane-ui-service" service** row.

![image](https://github.com/Azure/mec-app-solution-accelerator/assets/1712635/1df800f5-f5e8-4d40-8d2f-911197a7678b)

You can also directly access the URL for the Control Plane App, but if not using LOCALHOST, the service IP will vary:

`http://<your-IP>:90/`

<img width="599" alt="image" src="https://github.com/Azure/mec-app-solution-accelerator/assets/1712635/9e84321e-ef1f-40a9-872e-9c39d8e2f362">

### Access the Alerts dashboard UI with Alerts from AI model detections

- To access the Alerts dashboard UI front-end, you can either click on the link from the Control-Plane app here:

<img width="617" alt="image" src="https://github.com/Azure/mec-app-solution-accelerator/assets/1712635/53f28434-c200-42e7-917e-e3118ee0257f">

Or you can find out the real URL from the K8s dashboard, from the Alerts-UI service config, select the Kuberentes namespace where the application is deployed (MEC-Accelerator), go to the services menu in the left tab and click on the url to the right on the Alerts-UI service row:

<img width="814" alt="image" src="https://github.com/Azure/mec-app-solution-accelerator/assets/1712635/76ca5777-65a3-40de-b5c2-6d9975fca968">

Either way, you should be able to run the application's UI and check out the Alerts originated from the AI models:

<img width="465" alt="image" src="https://github.com/Azure/mec-app-solution-accelerator/assets/1712635/e2d8ddbf-036e-4b9a-8faa-c30d3018a7fa">


### Remove the MEC-Application from Kubernetes 

- Open a new command-shell and change the current folder to the `deploy/k8s` folder of this repo.

- Run the `deploy-accelerator` command with the `-uninstall` parameter to remove all related kubernetes resources for this application, since there is no stop action on kubectl.

Windows:
```
.\deploy-accelerator.ps1 -uninstall
```

Linux:
```
.\deploy-accelerator.sh -- uninstall
```
