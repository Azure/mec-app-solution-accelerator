 helm delete akri -n mec-accelerator
 
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

kubectl apply -f 15-akri.yaml