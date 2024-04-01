# Deploy the application on Jetson AGX Orin using k3s

Using a NVIDIA DeepStream is an AI-based video analytics platform that enables real-time processing and understanding of video streams. Leveraging the capabilities of Jetson AGX Orin, this deployment allows you to harness the potential of DeepStream for video analytics tasks.

## Setup Jetson AGX Orin

Follow the official documentation to set up your Jetson AGX Orin. Make sure to install the necessary drivers and dependencies provided by the JetPack components.  [Getting Started with Jetson AGX Orin Developer Kit](https://developer.nvidia.com/embedded/learn/get-started-jetson-agx-orin-devkit).

> **_NOTE:_**  JetPack **5.1.2** comes with Jetson Linux **35.4.1** BSP with Linux Kernel **5.10** and Ubuntu **20.04** based root file system.  This Linux kernel supports VXLAN out of the box and provides the right cgroups settings that makes Kubernetes (K3s) works out of the box.
If newer version of JetPack is used, you should consider add VXLAN extra packages or use different K3s network configuration.

## Install and configure Docker Engine to use the nvida-container-toolkit

1. Install using the convenience script.

    ```bash
    curl -fsSL https://get.docker.com -o get-docker.sh
    sudo sh ./get-docker.sh
    ```

    Add your user to the docker group and then log out and log back in so that your group membership is re-evaluated.

    ```bash
    sudo usermod -aG docker $USER
    logout
    ```

    When running ``kubernetes`` with ``docker``, edit the config file which is usually present at ``/etc/docker/daemon.json`` to set up nvidia-container-runtime (JetPack installs nvidia-container-runtime) as the default low-level runtime:

    ```bash
    {
        "default-runtime": "nvidia",
        "runtimes": {
            "nvidia": {
                "path": "/usr/bin/nvidia-container-runtime",
                "runtimeArgs": []
            }
        }
    }
    ```

    And then restart docker:

    ```bash
    sudo systemctl restart docker
    ```

## Install k3s with docker as container runtime

2. Use the QuickStart script

    ```bash
    curl -sfL https://get.k3s.io | sh -s - --docker --write-kubeconfig-mode 644 --write-kubeconfig $HOME/.kube/config
    ```


### Check kubectl context

3. If you previosly had already installed kubectl and it is pointing to some other environment, make sure the default kubectl context is pointing to your new k3s installation.

    ```bash
    kubectl config get-contexts
    ```

4. To set the context to point to k3s defualt context:

    **If using K3s:**

    ```bash
    kubectl config use-context default
    ```

### Install and Initialize Dapr

This example application is based on **[Dapr](https://dapr.io/)** framework which is an OSS Microsoft framework specialized on microservices architecture, so you need to install it and enable in the Kubernetes cluster, first.

5. Install or make sure you have installed `Dapr` on your machine on a Kubernetes cluster as described in the [Deploy Dapr](https://docs.dapr.io/operations/hosting/kubernetes/kubernetes-deploy/#install-with-dapr-cli).

    Note that if you were able to run the application on plain Docker, you should have installed DAPR already, but it needs to be initialized in Kuberentes, now.

6. Initialize DAPR in the Kubernetes cluster by running this command:

    ```powershell
    dapr init -k
    ```

    ![image](https://user-images.githubusercontent.com/1712635/218881163-9ba81fa3-f72c-4c12-bbf6-8ec25f2dba55.png)

    **IMPORTANT:** This **[Dapr](https://dapr.io/)** installation option is okay for a dev machine, but when installing **[Dapr](https://dapr.io/)** on a "production" cluster, you need to install **[Dapr](https://dapr.io/)** via AKS extension following the Doc: (https://learn.microsoft.com/en-us/azure/aks/dapr) which is how **[Dapr](https://dapr.io/)** should be installed in controlled environments since it doesn’t require cluster admin access.

7. You can test DAPR status with:
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

8. Move into the `deploy/k8s` folder of this repo, as current folder of the command-shell:

    ```bash
    cd <your_path>/deploy/k8s
    ```

9. Deploy the application in Kuberentes by running common deployments with gpu deployments

    ```bash
    kubectl apply -f ./common/
    ```
    No GPU support
    ```bash
    kubectl apply -f ./cpu/
    ```

    GPU support (NVIDIA Deepstream implementation)

    > **Note:** The DeepStream docker image size is more than **10GB**, ``kubelet`` could fail pulling big images if it is not configured for this purpose. Instead of change the ``kubelet`` default configuration you can pull the DeepStream image with ``Docker CLI`` and then execute the gpu deployment.

    ```bash
    docker pull mecsolutionaccelerator/deepstream:0.2_arm64
    ```

    ```bash
    kubectl apply -f ./gpu/arm64_arch
    ```

    All services will be created in the specified Kubernetes namespace "mec-accelerator" for this application.

    You can also check the pods running with this kubectl command:

    ```bash
    kubectl get pods --namespace mec-accelerator
    ```

    When you are finished trying the application, you can always uninstall the application pods and all related resources from your Kuberentes by running this command:

    ```bash
    kubectl delete -f ./cpu/ && kubectl delete -f ./common/
    ```

    Or

    ```bash
    kubectl delete -f ./gpu/arm64_arch && kubectl delete -f ./common/
    ```
