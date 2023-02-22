## Deploy the Application to Kubernetes using 'Minikube'

Make sure you have installed ["Docker Desktop"](https://docs.docker.com/desktop/install/windows-install/) and ["Minikube"](https://minikube.sigs.k8s.io/docs/start/)

Minikube is local Kubernetes, focusing on making it easy to learn and develop for Kubernetes. The Kubernetes server runs locally within your Docker instance, is not configurable, and is a single-node cluster. It runs within a Docker container on your local system, and is only for local development and testing.

1. Once minikube is installed, pen a new command-shell and run:

    ```powershell
    minikube start
    ```

2. If you have already installed kubectl and it is pointing to some other environment, such a cloud AKS cluster, ensure you change the context so that kubectl is pointing to minikube. Open a new command-shell and run:

    To check your current contexts:

    ```powershell
    kubectl config get-contexts
    ```

    If required, set the context to point to Kuberentes from Minikube:

    ```powershell
    kubectl config use-context minikube
    ```
    
    ![kubectl minikube context](/docs/imgs/deploy/minikube1.png)

### Install and Initialize DAPR

3. Install or make sure you have installed `Dapr` on your machine on a Kubernetes cluster as described in the [Deploy dapr](https://docs.dapr.io/operations/hosting/kubernetes/kubernetes-deploy/#install-with-dapr-cli). 

Note that if you were able to run the application on plain Docker, you should have installed DAPR already, but it needs to be initialized in Kuberentes, now.

4. Initialize DAPR in the Kubernetes cluster by running this command:

    ```powershell
    dapr init -k
    ```
    
    ![image](https://user-images.githubusercontent.com/1712635/218881163-9ba81fa3-f72c-4c12-bbf6-8ec25f2dba55.png)

    **IMPORTANT:** This DAPR installation is okay for a dev machine, but when installing DAPR on a "production" AKS cluster, for example in an ASE (Azure Stack Edge) server, you need to install DAPR via AKS extension following the Doc: (https://learn.microsoft.com/en-us/azure/aks/dapr) which is how DAPR should be installed in AKS and it doesn’t require cluster admin access.
    
    You can test DAPR status with:
    ```powershell
    dapr status -k
    ```
    
    If DAPR is initialized, you should get this list of Dapr pods running:
    
    ![image](https://user-images.githubusercontent.com/1712635/218881242-aa2c74ef-14a4-4a79-a149-3bbd12f4fa3d.png)

    
    Otherwise, if it's not initialized it'd be like this:
    
    ![image](https://user-images.githubusercontent.com/1712635/218880976-94b42767-40e3-4d9c-a640-2dfa029cb510.png)


## Deploy the application's services to Minikube

5. Run `minikube ssh docker pull mecsolutionaccelerator/aiinferencer:latest` command to pull the ai inferencer docker image into the cluster local repository. Otherwise minikube docker pull may timeout. This is an issue with minikube and large docker images or slow connections

    ```powershell
    minikube ssh docker pull mecsolutionaccelerator/aiinferencer:latest
    ```

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

    When you are finsihed trying, you can always uninstall the application pods and all related resources from your Kuberentes by running this command:

    ```powershell
    kubectl delete -f ./
    ```

### Check the application status with Kubernetes Dashboard

8. Run `minikube dashboard` so you can access the dahboard:

    ```powershell
    /> minikube dashboard
    ```

![minikube dashboard](/docs/imgs/deploy/minikube2.png)

### Access the application's UI to see Alerts originated from AI model detections

8. To access the front-end, run `minikube service alerts-ui -n mec-accelerator` to open the application's UI in a new browser tab and check out the Alerts originated from the AI models:

    ```powershell
    /> minikube service alerts-ui -n mec-accelerator
    ```

![image](https://user-images.githubusercontent.com/1712635/218885207-5d720a2d-f5a6-4e29-bfd1-bad384803805.png)

### Remove the application from Minikube 

1. Open a new command-shell and change the current folder to the `deploy/k8s` folder of this repo.

2. Run the following command to remove all related kubernetes resources for this application, since there is no stop action on kubectl.

    ```powershell
    kubectl delete -f ./
    ```









