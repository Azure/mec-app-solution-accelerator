# Control Plane Deployment to Kubernetes in 'Docker Desktop'

## Building Local Images
The control plane is make up of two components: a Next.js application for the frontend and a .NET Web API for the backend. To deploy these, we need to create local images for each component.

Below are the commands to build both images. Ensure you have a terminal open in the project's root directory:

Control Plane API image:

```
docker build -t control-plane-api:v1 -f src/Services/ControlPlane/API/Dockerfile src/Services/ControlPlane/API/
```

Control Plane UI image:

```
docker build -t control-plane-ui:v1 -f src/Web/control-plane-ui/Dockerfile src/Web/control-plane-ui/
```

## Deploy Control Plane Services

First, update the config map that contains essential information for connecting to our Azure Mobile Network. You'll find the configuration in the file `deploy/k8s/control-plane/02-control-plane-api-config-map.yaml`.

Replace the following values in the file:

```
  MobileNetwork__SubscriptionId: "your_subscription_id"
  MobileNetwork__ResourceGroup: "your_resource_group"
  MobileNetwork__Name: "your_network_name"
  MobileNetwork__DataNetwork: "your_datanetwork"
  MobileNetwork__Slice: "your_slice"
  ClientCredentials__TenantId: "your_tenant_id"
  ClientCredentials__ClientId: "your_client_id"
  ClientCredentials__ClientSecret: "your_client_secret"
```

Finally, deploy the application with this command:


```
kubectl apply -f ./
```

This deployment will expose two endpoints:

- localhost:89 -> WebAPI
- localhost:90 -> Front End Application

*Note: If deploying to an environment other than localhost, remember to update the `API_URL` environment variable in `control-plane-ui` to point to the correct endpoint.

