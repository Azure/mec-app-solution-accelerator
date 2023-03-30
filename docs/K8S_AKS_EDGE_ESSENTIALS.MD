# Deploy the application to AKS Edge Essentials

Microsoft recently released [AKS Edge Essentials](https://learn.microsoft.com/en-us/azure/aks/hybrid/aks-edge-overview) as GA (Generally Available) on February 21st 2023.

Azure Kubernetes Service Edge Essentials is an on-premises Kubernetes implementation of Azure Kubernetes Service (AKS) that automates running containerized applications at scale. AKS Edge Essentials includes a Microsoft-supported Kubernetes platform that includes a lightweight Kubernetes distribution with a small footprint and simple installation experience, making it easy for you to deploy Kubernetes on PC-class or "light" edge hardware. AKS Edge Essentials makes it easier to get started with your containerized application, bringing cloud-native best practices to your edge application.

## Install AKS Edge Essentials

You can install AKS EE by using the quick start way:

- [AKS Edge Essentials quickstart guide](https://learn.microsoft.com/en-us/azure/aks/hybrid/aks-edge-quickstart)

Alternatively, you can install it on a more 'step by step' way for more manual control of the installation:

- [AKS Edge Essentials - Single machine deployment](https://learn.microsoft.com/en-us/azure/aks/hybrid/aks-edge-howto-single-node-deployment)

**Important:** Note that with AKS EE you can install **[K3s](https://k3s.io/)** Kubernetes distribution (by default) or you can install the full Kubernetes distribution (**[K8s](https://kubernetes.io/)**), by using this parameter `-UseK8s` (if using the PowerShell Scripts). Both distributions should work propoerly for deplying this example application.

The instructions, steps and screenshots shown in this document were performed on AKS Edge Essentials with "full Kubernetes" installed. But **[K3s](https://k3s.io/)** was also tested and is compatible, too.


## (Optional) Instructions for building your own Docker Images to deploy to Kubernetes

You can directly deploy to Kubernetes and test the application Docker images we have already uploaded into Docker Hub for you, so creating your own Docker images and uploading to Docker Hub is optional, initially.

However, if you make changes in the code or you want to use your own Docker Images for any reason, you'd need to create your own Docker images and upload the images first to Docker Hub before deploying to Kubernetes. 

Use these instructions to [Buil your own Docker Images to upload to Docker Registry and deploy to Kubernetes](./BUILD_AND_PUSH_IMAGES_TO_DOCKER_HUB.MD)

## Deploy the Application to AKS EE

### Check kubectl context

If you previosly had already installed kubectl and it is pointing to some other environment, such as MiniKube, make sure the default kubectl context is pointing to AKS Edge Essentials context. 

1. Open a new command-shell and run the following command to check your current contexts:

    ```powershell
    kubectl config get-contexts
    ```

2. To set the context to point to AKS Edge Essentials:

    ```powershell
    kubectl config use-context kubernetes-admin@kubernetes
    ```

### Install and Initialize Dapr

This example application is based on **[Dapr](https://dapr.io/)** framework which is an OSS Microsoft framework specialized on microservices architecture, so you need to install it and enable in the Kubernetes cluster, first. 

3. Install or make sure you have installed `Dapr` on your machine on a Kubernetes cluster as described in the [Deploy Dapr](https://docs.dapr.io/operations/hosting/kubernetes/kubernetes-deploy/#install-with-dapr-cli). 

    Note that if you were able to run the application on plain Docker, you should have installed DAPR already, but it needs to be initialized in Kuberentes, now.

4. Initialize DAPR in the Kubernetes cluster by running this command:
5. 
    ```powershell
    dapr init -k
    ```
    
    ![image](https://user-images.githubusercontent.com/1712635/218881163-9ba81fa3-f72c-4c12-bbf6-8ec25f2dba55.png)

    **IMPORTANT:** This **[Dapr](https://dapr.io/)** installation option is okay for a dev machine, but when installing **[Dapr](https://dapr.io/)** on a "production" AKS cluster, for example in an ASE (Azure Stack Edge) server, you need to install **[Dapr](https://dapr.io/)** via AKS extension following the Doc: (https://learn.microsoft.com/en-us/azure/aks/dapr) which is how **[Dapr](https://dapr.io/)** should be installed in controlled environments since it doesn’t require cluster admin access.
    
    You can test DAPR status with:
    ```powershell
    dapr status -k
    ```
    
    If DAPR is initialized, you should get this list of Dapr pods running:
    
    ![image](https://user-images.githubusercontent.com/1712635/218881242-aa2c74ef-14a4-4a79-a149-3bbd12f4fa3d.png)

    
    Otherwise, if it's not initialized it'd be like this:
    
    ![image](https://user-images.githubusercontent.com/1712635/218880976-94b42767-40e3-4d9c-a640-2dfa029cb510.png)

    You can also check the pods running with this kubectl command:

    ```powershell
    kubectl get pods --namespace dapr-system
    ```
    
### Deploy the application's services to Kubernetes

5. Open a new command-shell and move into the `deploy/k8s` folder of this repo, as current folder of the command-shell:

    ```powershell
    cd <your_path>/deploy/k8s
    ```

6. Deploy the application in Kuberentes by running this command:

    ```powershell
    kubectl apply -f ./
    ```

    All services will be created in the specified Kubernetes namespace "mec-accelerator" for this application.
    
    ![image](https://user-images.githubusercontent.com/1712635/219480144-75f3998d-998c-464d-bc8a-7e9a1a265a0e.png)

    When you are finished trying, you can always uninstall the application pods and all related resources from your Kuberentes by running this command:

    ```powershell
    kubectl delete -f ./
    ```

### Check the application status with Kubernetes Dashboard

7. If you don't have installed/enabled the Kuberentes Dashboard, do so by running this command:

    ```powershell
    kubectl apply -f https://raw.githubusercontent.com/kubernetes/dashboard/v2.6.1/aio/deploy/recommended.yaml
    ```

8. Run `kubectl proxy` so you can access the dahboard:

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

9. You need to do this step **only one time**: Change the current folder to the `deploy/k8s/dashboard_auth` folder of this repo and run the instructions in commands.txt to configure the tokens. Basically, the following commands need to be run just once from that folder:

    ```powershell
    kubectl apply -f dashboard-adminuser.yaml 

    kubectl apply -f adminuser-cluster-role-binding.yaml
    ```
    
    ![image](https://user-images.githubusercontent.com/1712635/218881848-92de552c-a0f3-4fa8-ab87-2ca2512956f2.png)


### Generate and copy the token to provide to Kubernetes Dashboard

10. Whenever you need a token, run this command:

    ```powershell
    kubectl -n kubernetes-dashboard create token admin-user --duration=48h --output yaml
    ```
    
    Select and copy the token to the clipboard.
    
    ![image](https://user-images.githubusercontent.com/1712635/218882035-61318473-b7e1-4fe5-b253-69479e8a20b0.png)

### Provide the token to Kubernetes dashboard

11. Open http://localhost:8001/api/v1/namespaces/kubernetes-dashboard/services/https:kubernetes-dashboard:/proxy/#/login in your browser and paste the token you copied previously.

![image](https://user-images.githubusercontent.com/1712635/218882193-938f14f0-06ca-4bba-979f-da9266706f8e.png)

In the Kubernetes dashboard you should be able to explore the application's pods, services, etc.

![image](https://user-images.githubusercontent.com/1712635/219480986-86a6e67b-7fb8-41ea-8ca3-5948c1e5d358.png)


### Access the application's UI to see Alerts originated from AI model detections

12. In order to know the URL to access the front-end web app, type this kubectl command to list the services owned by the application's namespace 'mec-accelerator':

    ```powershell
    kubectl get service --namespace mec-accelerator
    ```
    Then, look at the EXTERNAL-IP used by the service 'alerts-ui' and finally type 'http://MY-IP-ADDRESS:88/' on your browser.
    
    Finally, you should be able to run the application's UI and check out the Alerts originated from the AI models.
    
    Note that the port used on the external IP might be **88**, depending on how the service port is configured in the **alerts-ui.yaml** file.
    
![image](https://user-images.githubusercontent.com/1712635/226770509-ce73881e-96ed-41c1-9965-dcf044683e02.png)

![image](https://user-images.githubusercontent.com/1712635/226770726-10fdfb87-44f1-497b-845f-fde0bfd12956.png)

#### Important known issue:

As mentioned in [AKS EE documentation verifying the services](https://learn.microsoft.com/en-us/azure/aks/hybrid/aks-edge-howto-deploy-app#3-verify-the-services), when using full K8s in AKS EE with a single node, on single machine clusters, if you created your Kubernetes cluster without specifying a **-ServiceIPRangeSize** parameter in the JSON config, you will not have allocated IPs for your workload services and you won't have an external IP address. Hence, the alerts.ui service will be stuck on "pending state", like the following:

![image](https://user-images.githubusercontent.com/1712635/220791666-14a2238e-1826-4433-b55f-c57e7f53ec36.png)

![image](https://user-images.githubusercontent.com/1712635/220791688-a814e1fb-5b16-48bf-9d0e-5d270c41fb9f.png)

**Solutions:** 

**SOLUTION A:** 

The best solution is to re-create the AKS EE cluster and specify the **-ServiceIPRangeSize** parameter in the JSON config, like here.
https://learn.microsoft.com/en-us/azure/aks/hybrid/aks-edge-howto-single-node-deployment#example-deployment-options

**SOLUTION B:** 

This is more a workaround, but you can also add `externalIPs` spec to your service YAML. You can use the IP associated with your node's primary interface. You can then access the service externally, using the external IP of the node.

**Find the external IP address:** You can find the IP address of your Linux VM by running **with Admin privileges (Run as Administrator)** the PowerShell command `Get-AksEdgeNodeAddr`, like below:

```
Get-AksEdgeNodeAddr -NodeType Linux
```

![image](https://user-images.githubusercontent.com/1712635/220792476-aa6094a4-0593-413b-a0b0-cb7f4c063b6d.png)

**Update/extend the `alerts-ui.yaml` definition:** Then, edit the `alerts-ui.yaml` deployment file and add the following after the `LoadBalancer` line:

```yaml
  externalIPs:
  - YOUR_VM_EXTERNAL_IP
```

```yaml
    apiVersion: v1
    kind: Service
    metadata:
      name: alerts-ui
      namespace: mec-accelerator
    spec:
      ports:
      - port: 80
        targetPort: 80
      selector:
        app: alerts-ui
      type: LoadBalancer
      externalIPs:
      - 192.168.0.2
```

After deploying again, it should work with no problem.

Further info:
https://learn.microsoft.com/en-us/azure/aks/hybrid/aks-edge-howto-deploy-app#3-verify-the-services

Issue related:
https://github.com/Azure/mec-app-solution-accelerator/issues/87

### Remove the application from Kubernetes 

1. Open a new command-shell and change the current folder to the `deploy/k8s` folder of this repo.

1. Run the following command to remove all related kubernetes resources for this application, since there is no stop action on kubectl.

    ```powershell
    kubectl delete -f ./
    ```




