param (
    [string]$kubernetesDistro = "",
    [string]$mqttBroker = "",
    [switch]$uninstall
)

if ($uninstall) {
    if (-not (kubectl get ns | Select-String 'mec-accelerator')) {
        exit 1
    }
    Write-Host "Uninstalling MEC accelerator"
    if (kubectl get pods -n azure-iot-operations | Select-String 'mec-listener') {
      kubectl delete -f ./E4K/
    }
    if (kubectl get pods -n mec-accelerator | Select-String 'mosquitto') {
      kubectl delete -f ./mosquitto/
    }

    kubectl delete clusterrole akri-webhook-configuration-patch
    kubectl delete clusterrolebinding akri-webhook-configuration-patch
    kubectl delete ValidatingWebhookConfiguration akri-webhook-configuration
    kubectl delete -f ./00-namespace.yaml
    exit 1
}

if (-not $kubernetesDistro -or -not $mqttBroker) {
    Write-Host "Expecting parameters --kubernetesDistro and --mqttBroker"
    exit 1
}

if ($kubernetesDistro -ne "k3s" -and $kubernetesDistro -ne "k8s") {
    Write-Host "Akri kubernetes distro must be k3s or k8s"
    exit 1
}

if ($mqttBroker -ne "E4K" -and $mqttBroker -ne "mosquitto") {
    Write-Host "MQTT broker must be E4K or mosquitto"
    exit 1
}

if ($mqttBroker -eq "E4K") {
    if (-not (kubectl get ns | Select-String 'azure-arc')) {
        Write-Host "Azure Arc is not configured. Please configure it before running the script"
        exit 1
    }

    if (-not (kubectl get ns | Select-String 'azure-iot-operations')) {
        Write-Host "Azure IoT Operations is not installed. Please install it before running the script"
        exit 1
    }
    Write-Host "Installing Mec-Accelerator with E4K MQTT broker"
}
else {
    Write-Host "Installing Mec-Accelerator with mosquitto MQTT broker"
}

Write-Host ""
Write-Host "----------------------------------------------------------------------------------"    
Write-Host "1. Installing Helm"
winget install Helm.Helm

if (-not (kubectl get ns | Select-String 'dapr-system')) {
    Write-Host ""
    Write-Host "----------------------------------------------------------------------------------"    
    Write-Host "2. Installing Dapr in cluster"
    powershell -Command "iwr -useb https://raw.githubusercontent.com/dapr/cli/master/install/install.ps1 | iex"
    dapr init -k
}

Write-Host ""
Write-Host "----------------------------------------------------------------------------------"    
Write-Host "3. Creating Mec-Accelerator namespace"
kubectl apply -f ./00-namespace.yaml

Write-Host ""
Write-Host "----------------------------------------------------------------------------------"    
Write-Host "4. Installing Akri"
helm repo add akri-helm-charts https://project-akri.github.io/akri/
helm repo update
helm install akri akri-helm-charts/akri -n mec-accelerator --set kubernetesDistro=$kubernetesDistro --set custom.discovery.enabled=true --set custom.discovery.image.repository=mecsolutionaccelerator/akri-camera-discovery-handler --set custom.discovery.image.tag=2.0 --set custom.discovery.name=akri-camera-discovery --set custom.configuration.enabled=true --set custom.configuration.name=akri-camera --set custom.configuration.discoveryHandlerName=camera --set custom.configuration.discoveryDetails.connectionString="mongodb://control-plane-mongodb.mec-accelerator:27017" --set custom.configuration.discoveryDetails.database="ControlPlane" --set custom.configuration.discoveryDetails.collection="Cameras" --set custom.configuration.brokerPod.image.repository=mecsolutionaccelerator/framesplitter --set custom.configuration.brokerPod.image.tag=2.0
Start-Sleep -Seconds 30

Write-Host ""
Write-Host "----------------------------------------------------------------------------------"    
Write-Host "5. Deploying MQTT Broker"
kubectl apply -f ./"$mqttBroker"/

Write-Host ""
Write-Host "----------------------------------------------------------------------------------"    
Write-Host "6. Deploying Mec-Accelerator resources"
kubectl apply -f ./
Unblock-File ./deploy-akri-secrets.ps1
./deploy-akri-secrets.ps1

if (-not (kubectl get ns | Select-String 'kubernetes-dashboard')) {
    Write-Host ""
    Write-Host "----------------------------------------------------------------------------------"    
    Write-Host "7. Deploying Kubernetes dashboards"
    kubectl apply -f https://raw.githubusercontent.com/kubernetes/dashboard/v2.7.0/aio/deploy/recommended.yaml
    kubectl apply -f ./dashboard_auth/dashboard-adminuser.yaml 
    kubectl apply -f ./dashboard_auth/adminuser-cluster-role-binding.yaml
    Write-Host ""
    Write-Host "----------------------------------------------------------------------------------"    
    Write-Host "Kubernetes dashboards installed."
    Write-Host "Please run 'kubectl proxy' to access the dashboard at http://localhost:8001/api/v1/namespaces/kubernetes-dashboard/services/https:kubernetes-dashboard:/proxy/"
    Write-Host "Generate a valid bearer token for the dashboard with 'kubectl -n kubernetes-dashboard create token admin-user --duration=48h --output yaml'"
    Write-Host "----------------------------------------------------------------------------------"    
}

Write-Host ""
Write-Host "----------------------------------------------------------------------------------"    
Write-Host "Successfully installed MEC-Accelerator!"
Write-Host ""
Write-Host "Alerts-UI and Control-Plane-UI services deployed in:"
Write-Host "----------------------------------------------------------------------------------"
kubectl get service -n mec-accelerator control-plane-ui-service -o jsonpath='| Control Plane web app URL (control-plane-ui-service pod) | http://{.status.loadBalancer.ingress[*].ip}:{.spec.ports[*].port} |{"\n"}'
Write-Host "----------------------------------------------------------------------------------"
kubectl get service -n mec-accelerator alerts-ui -o jsonpath='| Alerts Dashboard web app (alerts-ui pod)                 | http://{.status.loadBalancer.ingress[*].ip}:{.spec.ports[*].port} |{"\n"}'
Write-Host "----------------------------------------------------------------------------------"