param(
    [string]$kubernetesDistro,
    [string]$mqttBroker
)

if ($kubernetesDistro -ne "k3s" -and $kubernetesDistro -ne "k8s") {
    Write-Host "Akri kubernetes distro must be k3s or k8s"
    exit 1
}

if ($mqttBroker -ne "E4K" -and $mqttBroker -ne "mosquitto") {
    Write-Host "MQTT broker must be E4K or mosquitto"
    exit 1
}

if ($mqttBroker -eq "E4K") {
    $azureArcNS = kubectl get ns | Select-String 'azure-arc'
    $azureIoTOperationsNS = kubectl get ns | Select-String 'azure-iot-operations'

    if (-not $azureArcNS) {
        Write-Host "Azure Arc is not configured. Please configure it before running the script"
        exit 1
    }

    if (-not $azureIoTOperationsNS) {
        Write-Host "Azure IoT Operations is not installed. Please install it before running the script"
        exit 1
    }

    $deployment = "E4K"
    Write-Host "1. Installing Mec-Accelerator with E4K MQTT broker"
} else {
    $deployment = "mosquitto"
    Write-Host "1. Installing Mec-Accelerator with mosquitto MQTT broker"
}

Write-Host "2. Creating Mec-Accelerator namespace"
kubectl apply -f .\00-namespace.yaml

Write-Host "3. Installing Akri"
helm repo add akri-helm-charts https://project-akri.github.io/akri/
helm install akri akri-helm-charts/akri -n mec-accelerator --set kubernetesDistro=$kubernetesDistro --set custom.discovery.enabled=true --set custom.discovery.image.repository=mecsolutionaccelerator/akri-camera-discovery-handler --set custom.discovery.image.tag=1.8 --set custom.discovery.name=akri-camera-discovery --set custom.configuration.enabled=true --set custom.configuration.name=akri-camera --set custom.configuration.discoveryHandlerName=camera --set custom.configuration.discoveryDetails.connectionString="mongodb://control-plane-mongodb.mec-accelerator:27017" --set custom.configuration.discoveryDetails.database="ControlPlane" --set custom.configuration.discoveryDetails.collection="Cameras" --set custom.configuration.brokerPod.image.repository=mecsolutionaccelerator/framesplitter --set custom.configuration.brokerPod.image.tag=1.8

Write-Host "4. Deploying MQTT Broker"
kubectl apply -f .\$deployment\

Write-Host "5. Deploying Mec-Accelerator resources"
kubectl apply -f .\
.\deploy-akri-secrets.sh