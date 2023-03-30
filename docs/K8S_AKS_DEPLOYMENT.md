## Deploy the application to AKS in Azure cloud 

Check thhe official [Dapr doc guide](https://docs.dapr.io/operations/hosting/kubernetes/cluster/setup-aks/) and [Microsoft Docs guide](https://learn.microsoft.com/en-us/azure/aks/learn/quick-kubernetes-deploy-cli)

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

    ![kubectl minikube context](/docs/imgs/deploy/aks1.png)


    Alternatively, check and set your current contexts using kubectl config:

        ```powershell
        kubectl config get-contexts
        ```

        ```powershell
        kubectl config use-context DESIRED_CLUSTER
        ```

### Install and Initialize DAPR

The recommended approach for installing Dapr on AKS is to use the AKS Dapr extension. The extension offers support for all native Dapr configuration capabilities through command-line arguments via the Azure CLI and offers the option of opting into automatic minor version upgrades of the Dapr runtime.

This process is also described in the [Install Dapr using the AKS Dapr extension 
docs](https://docs.dapr.io/operations/hosting/kubernetes/kubernetes-deploy/#install-with-dapr-cli). 

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
    az k8s-extension create --cluster-type managedClusters \
    --cluster-name myAKSCluster \
    --resource-group myResourceGroup \
    --name myDaprExtension \
    --extension-type Microsoft.Dapr
    ``
    
    If DAPR is initialized, you should the list of Dapr pods running:

    ```powershell
    kubectl get pods -n dapr-system
    ```

### Deploy the application's services to Kubernetes

6. Open a new command-shell and move into the `deploy/k8s` folder of this repo, as current folder of the command-shell:

    ```powershell
    cd <your_path>/deploy/k8s
    ```

7. Deploy the application in Kuberentes by running this command:

    ```powershell
    kubectl apply -f ./
    ```

    All services will be created in the specified Kubernetes namespace "mec-accelerator" for this application.
    
    ![image](https://user-images.githubusercontent.com/1712635/219480144-75f3998d-998c-464d-bc8a-7e9a1a265a0e.png)


### Access the application's UI to see Alerts originated from AI model detections

8. To access the front-end, go to the Azure portal, find the AKS resource and navigate to the services sections. In there the alerts-ui service of type NodePort will have public IP assigned.

Note that the port used on the external IP might be **88**, depending on how the service port is configured in the alerts-ui.yaml file.

![image](https://user-images.githubusercontent.com/1712635/220746544-90e6e492-fd44-4f0b-8dc4-07661fc72558.png)

### Remove the application from Kubernetes 

When you are finsihed trying, you can always uninstall the application pods and all related resources from your Kuberentes by running this command.

9. Open a new command-shell and change the current folder to the `deploy/k8s` folder of this repo.

10. Run the following command to remove all related kubernetes resources for this application, since there is no stop action on kubectl.

    ```powershell
    kubectl delete -f ./
    ```



