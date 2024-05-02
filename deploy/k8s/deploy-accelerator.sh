#!/bin/sh
if [ $# -ne 2 ]; then
    echo "Error: Please provide a first parameter for Akri kubernetes distro (k3s or k8s) and a second parameter for the MQTT broker (E4K or mosquitto)"
    exit 1
fi

if [ $1 != "k3s" ] && [ $1 != "k8s" ]; then
    echo "Akri kubernetes distro must be k3s or k8s"
    exit 1
fi

if [ $2 != "E4K" ] && [ $2 != "mosquitto" ]; then
    echo "MQTT broker must be E4K or mosquitto"
    exit 1
fi

if [ $2 = "E4K" ]; then
    if [ "$(kubectl get ns | grep 'azure-arc')" = "" ]; then
        echo "Azure Arc is not configured. Please configure it before running the script"
        exit 1
    fi

    if [ "$(kubectl get ns | grep 'azure-iot-operations')" = "" ]; then
        echo "Azure IoT Operations is not installed. Please install it before running the script"
        exit 1
    fi
    deployment="E4K"
    echo "1. Installing Mec-Accelerator with E4K MQTT broker"
else
    deployment="mosquitto"
    echo "1. Installing Mec-Accelerator with mosquitto MQTT broker"
fi

echo "2. Installing Helm"
curl -fsSL -o get_helm.sh https://raw.githubusercontent.com/helm/helm/main/scripts/get-helm-3
chmod 700 get_helm.sh
./get_helm.sh

if ! kubectl get ns | grep -q 'dapr-system'; then 
    echo "3. Installing Dapr in cluster"
    wget -q https://raw.githubusercontent.com/dapr/cli/master/install/install.sh -O - | /bin/bash
    dapr init -k
fi

echo "4. Creating Mec-Accelerator namespace"
kubectl apply -f ./00-namespace.yaml

echo "5. Installing Akri"
helm repo add akri-helm-charts https://project-akri.github.io/akri/
helm repo update
helm install akri akri-helm-charts/akri -n mec-accelerator --set kubernetesDistro=$1 --set custom.discovery.enabled=true --set custom.discovery.image.repository=mecsolutionaccelerator/akri-camera-discovery-handler --set custom.discovery.image.tag=1.8 --set custom.discovery.name=akri-camera-discovery --set custom.configuration.enabled=true --set custom.configuration.name=akri-camera --set custom.configuration.discoveryHandlerName=camera --set custom.configuration.discoveryDetails.connectionString="mongodb://control-plane-mongodb.mec-accelerator:27017" --set custom.configuration.discoveryDetails.database="ControlPlane" --set custom.configuration.discoveryDetails.collection="Cameras" --set custom.configuration.brokerPod.image.repository=mecsolutionaccelerator/framesplitter --set custom.configuration.brokerPod.image.tag=1.8

echo "6. Deploying MQTT Broker"
kubectl apply -f ./"$deployment"/

echo "7. Deploying Mec-Accelerator resources"
kubectl apply -f ./
chmod a+x ./deploy-akri-secrets.sh
./deploy-akri-secrets.sh

if [ "$(kubectl get ns | grep 'kubernetes-dashboard')" != "" ]; then
    echo "8. Deploying Kubernetes dashboards"
    kubectl apply -f https://raw.githubusercontent.com/kubernetes/dashboard/v2.7.0/aio/deploy/recommended.yaml
    kubectl apply -f ./dashboard_auth/dashboard-adminuser.yaml 
    kubectl apply -f ./dashboard_auth/adminuser-cluster-role-binding.yaml

    echo "Kubernetes dashboards installed."
    echo "Please run 'kubectl proxy' to access the dashboard at http://localhost:8001/api/v1/namespaces/kubernetes-dashboard/services/https:kubernetes-dashboard:/proxy/"
    echo "Generate a valid bearer token for the dashboard with 'kubectl -n kubernetes-dashboard create token admin-user --duration=48h --output yaml'"
fi

echo "Successfully installed MEC-Accelerator!"