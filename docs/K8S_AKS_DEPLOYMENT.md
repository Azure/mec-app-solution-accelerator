# Deploy the application to AKS in Azure cloud or Azure Public MEC (Azure Edge Zone)

- Deploying into **Public MEC** datacenters (**Azure Edge Zones**) makes sese when the 5G network used with the devices is a public 5G network such as AT&T. These Public MEC datacenters (Azure Edge Zones) still provide very low latency if your devices are placed on the same area (i.e. city) where the Azure Edge Zone is based. Low latency is provided thanks to the fact that for instance, a 5G AT&T network accesses directly the related Azure Edge Zones without going through the Internet.  See [What is Azure public MEC?](https://learn.microsoft.com/en-us/azure/public-multi-access-edge-compute-mec/overview) for further details.

- Deploying into a regular **Azure Region datacenter** makes sense when very low latency is not critical and the application services can run in the cloud. This environment could also be used for a QoS environment or testing environment.

This procedure, deploying into AKS in Azure applies to both cases since deploying and using an AKS cluster is similar in a regular Azure Region or in an Azure Edge Zone (Public MEC). The only difference when creating the AKS cluster is that you need to specify an "Azure Edge Zone" such as Atlanta or Dallas.

## Create an AKS cluster in Azure

For creating an AKS cluster in a regular Azure Region datacenter (cloud), follow the official documentation on how to [Deploy an Azure Kubernetes Service (AKS) cluster with the UI](https://learn.microsoft.com/en-us/azure/aks/learn/quick-kubernetes-deploy-portal?tabs=azure-cli#create-an-aks-cluster) or how to [Deploy an Azure Kubernetes Service (AKS) cluster with the CLI](https://learn.microsoft.com/en-us/azure/aks/tutorial-kubernetes-deploy-cluster?tabs=azure-cli)

**Important:** When creating the AKS cluster, if it's just for testing, you can create a "Dev/Test" cluster configuration which by default starts using a single node/VM. However, for better performance with the AI model inference, we recommend to use a node size (VM type) such as **"Standard F8s V2"** (8 vcpus, 16 GiB memory), which is available in East US region, for instance.

If you choose a different VM/node type, make sure it's not an ARM procesor based, since the Docker Images we use don't currently support ARM.

See below an example of the AKS cluster creation:

![image](https://user-images.githubusercontent.com/1712635/229197486-e2e326a9-d5e5-4156-93c9-30c6a5ebc5f1.png)


### (Optional) Create the AKS cluster in Azure Edge zone (Public MEC)

Edge Zones are unique solution offering with small, localized footprints of Azure in a metropolitan area designed to provide low latency connectivity for applications that require the highest level of performance, providing low latency connectivity that is tailored to the needs of an enterprise. The benefits of this solution can be enjoyed by various industries and use cases such as live media streaming, real-time analytics, inferencing with AI/ML algorithms, smart cities, retail, automotive, and healthcare. One such solution offering, Azure public multi-access edge compute (MEC) solution are a type of Edge Zone that are placed in or near mobile operators' data centers in metro areas that are accessed from mobile devices connected to the mobility network. To learn more details on Azure public MEC, please refer to Azure public MEC documentation: [Azure public MEC Overview](https://msazure.visualstudio.com/NEC/_wiki/wikis/NEC.wiki?wikiVersion=GBwikiMaster&pagePath=/AKS%20for%20Azure%20Edge%20Zone%20and%20Azure%20public%20MEC/overview). With this solution, enterprises are able to enjoy the advantages of low latency at the edge

Creating an AKS cluster in an Azure Edge zone is very similar, but you need to explicetely select the Azure Edge zone.

Before you can deploy an AKS cluster in the Edge Zone, your subscription needs to have access to the Edge Zone location. The access to the Edge Zone is provided through the onboarding process. To onboard to the Edge Zone you can either create a support request through the Azure portal or Sign Up for preview at aka.ms/AzurepublicMEC

The only different steps are:

- Select Deploy to an edge zone under the region locator for the AKS cluster.

![image](https://user-images.githubusercontent.com/1712635/228965879-8c8a4157-88cf-4c74-9f01-6834b50548e1.png)

- Select the Edge Zone targeted for deployment, and leave the default value selected for Kubernetes version.

![image](https://user-images.githubusercontent.com/1712635/228966018-3bf38cad-013b-4fc3-a111-ebc0bff63bd4.png)

You can see that Edge zones (aka Public MEC) are related to operators, such as in the above cases, with AT&T.

Note that when you create an AKS cluster in an Edge zone, under the covers, the Kubernetes cluster is not the same as a regular AKS, but a more lightweigth AKS cluster, also less scalable, especially designed for the edge. However, when deploying and working with it, the experience is similar.

## Use Azure CLI to target your AKS cluster in your Azure subscription

1. Use Azure CLI to log in and set the apropiate subscription where the AKS cluster is located

    To check your current contexts:

    ```powershell
    az login
    az account set -s SUBSCRIPTION_ID
    ```

2. Get credentials and set the kubectl context using az `aks get-credentials -r -n`:

    To check your current contexts:

    ```powershell
    az aks get-credentials --resource-group YOUR_RG_GROUP --name YOUR_AKS_NAME
    ```

    <img width="940" alt="image" src="https://user-images.githubusercontent.com/1712635/228974631-223a9e2d-e877-4fc9-b654-a6e9ac4c6a53.png">

    Additionally, check and set your current contexts using kubectl config:

        ```powershell
        kubectl config get-contexts
        ```

        ```powershell
        kubectl config use-context DESIRED_CLUSTER
        ```

## Install and Initialize DAPR

The recommended approach for installing Dapr on AKS is to use the AKS Dapr extension. The extension offers support for all native Dapr configuration capabilities through command-line arguments via the Azure CLI and offers the option of opting into automatic minor version upgrades of the Dapr runtime.

This process is also described in the [Install Dapr using the AKS Dapr extension](https://docs.dapr.io/developing-applications/integrations/azure/azure-kubernetes-service-extension/#install-dapr-using-the-aks-dapr-extension) document.

4. Extension prerequisites

    ```powershell
    az feature register --namespace "Microsoft.ContainerService" --name "AKS-ExtensionManager"
    az feature register --namespace "Microsoft.ContainerService" --name "AKS-Dapr"
    ```

    After a few minutes, check the status to show Registered. Confirm the registration status by using the az feature list command:

    ```powershell
    az feature list -o table --query "[?contains(name, 'Microsoft.ContainerService/AKS-ExtensionManager')].{Name:name,State:properties.state}"
    az feature list -o table --query "[?contains(name, 'Microsoft.ContainerService/AKS-Dapr')].{Name:name,State:properties.state}"
    ```
    Next, refresh the registration of the Microsoft.KubernetesConfiguration and Microsoft.ContainerService resource providers by using the az provider register command:

    ```powershell
    az feature list -o table --query "[?contains(name, 'Microsoft.ContainerService/AKS-ExtensionManager')].{Name:name,State:properties.state}"
    az feature list -o table --query "[?contains(name, 'Microsoft.ContainerService/AKS-Dapr')].{Name:name,State:properties.state}"
    ```

    ![kubectl minikube context](/docs/imgs/deploy/aks2.png)


5. Enable the Azure CLI extension for cluster extensions and install Dapr on AKS

    You will also need the k8s-extension Azure CLI extension. Install this by running the following commands:

    ```powershell
    az extension add --name k8s-extension
    ```

    After your subscription is registered to use Kubernetes extensions, install Dapr on your cluster by creating the Dapr extension. For example:

    ```powershell
    az k8s-extension create --cluster-type managedClusters --cluster-name myAKSCluster --resource-group myResourceGroup --name myDaprExtension --extension-type Microsoft.Dapr
    ```

    If DAPR is properly initialized, you should the list of Dapr pods running:

    ```powershell
    kubectl get pods -n dapr-system
    ```

    ![image](https://user-images.githubusercontent.com/1712635/229188179-59c07888-b515-4406-b7c5-13db4eb60ec0.png)

   Check [Dapr doc guide](https://docs.dapr.io/operations/hosting/kubernetes/cluster/setup-aks/) for further information.

## (Optional - DeepStream capability) Add GPU node pool to Kubernetes

There are several ways to create a node pool with GPU support. A complete guide is available at this link [Use GPUs for compute-intensive workloads on Azure Kubernetes Service (AKS)](https://learn.microsoft.com/en-us/azure/aks/gpu-cluster). For ease we will describe the method [Using the AKS GPU image](https://learn.microsoft.com/en-us/azure/aks/gpu-cluster#update-your-cluster-to-use-the-aks-gpu-image-preview)

6. Install the [aks-preview](https://learn.microsoft.com/en-us/cli/azure/extension#az-extension-add) Azure CLI extension using the az extension add command.

    ```powershell
    az extension add --name aks-preview
    ```

7. Register the GPUDedicatedVHDPreview feature flag using the [az feature register](https://learn.microsoft.com/en-us/cli/azure/feature#az-feature-register) command.

    ```powershell
    az feature register --namespace "Microsoft.ContainerService" --name "GPUDedicatedVHDPreview"
    ```

    It takes a few minutes for the status to show Registered. You can check the status using the [az feature show](https://learn.microsoft.com/en-us/cli/azure/feature#az-feature-show) command.

    ```powershell
    az feature show --namespace "Microsoft.ContainerService" --name "GPUDedicatedVHDPreview"
    ```

8. Add a node pool with a GPU node.

    You should provide a VM type that supports **GPU**, consider [NC-series](https://learn.microsoft.com/en-us/azure/virtual-machines/nc-series) (--node-vm-size). Additionally you should mark the new node pool with an specific **taint** (--node-taints) that will be tolerated by the DeepStream pod
    ![DeepStream pod toleration](/docs/imgs/deploy/deepstream_pod.png)

    ```powershell
    az aks nodepool add --resource-group myResourceGroup --cluster-name myAKSCluster --name gpunp --node-count 1 --node-vm-size Standard_NC6s_v3 --node-taints sku=gpu:NoSchedule --aks-custom-headers UseGPUDedicatedVHD=true --enable-cluster-autoscaler --min-count 1 --max-count 3
    ```

## Deploy the application's services to Kubernetes

9. Open a new command-shell and move into the `deploy/k8s` folder of this repo, as current folder of the command-shell:

    ```powershell
    cd <your_path>/deploy/k8s
    ```

10. Deploy the application in Kuberentes by running common deployments with cpu **or** gpu deployments

    ```powershell
    kubectl apply -f ./common/
    ```
    No GPU support
    ```powershell
    kubectl apply -f ./cpu/
    ```
    GPU support (NVIDIA Deepstream implementation)
    ```powershell
    kubectl apply -f ./gpu/
    ```

    All services will be created in the specified Kubernetes namespace "mec-accelerator" for this application.

    ![image](https://user-images.githubusercontent.com/1712635/219480144-75f3998d-998c-464d-bc8a-7e9a1a265a0e.png)


## Access the application's UI to see Alerts originated from AI model detections

11. To access the front-end, go to the Azure portal, find the AKS resource and navigate to the services sections. In there the alerts-ui service of type NodePort will have public IP assigned.

Note that the port used on the external IP is **88**, depending on how the service port is configured in the alerts-ui.yaml file.

![image](https://user-images.githubusercontent.com/1712635/229188900-6425d250-4513-4b44-924e-eb8192db97d2.png)

## Remove the application from Kubernetes

When you are finsihed trying, you can always uninstall the application pods and all related resources from your Kuberentes by running this command.

12. Open a new command-shell and change the current folder to the `deploy/k8s` folder of this repo.

13. Run the following command to remove all related kubernetes resources for this application, since there is no stop action on kubectl.

    ```powershell
    kubectl delete -f ./
    ```
