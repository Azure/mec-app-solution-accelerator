# Zipkin distributed tracing

The readme provides instructions on how to configure distributed tracing with Zipkin in the mec solution accelerator running on Kubernetes. 

Distributed tracing provides insights into the traffic between services involved in distributed business transactions.

Dapr’s sidecar architecture enables built-in observability features. As services communicate, Dapr 
sidecars intercept the traffic and extract tracing, metrics, and logging information. Telemetry is 
published in an open standards format. By default, Dapr supports OpenTelemetry and Zipkin

## Enable and configure tracing

To start, tracing must be enabled for the Dapr runtime using a Dapr configuration file. Here’s an 
example of a configuration file named dapr-config.yaml that enables tracing:

```YAML
apiVersion: dapr.io/v1alpha1
kind: Configuration
metadata:
  name: dapr-config
  namespace: mec-accelerator
spec:
  tracing:
    samplingRate: "1"
    zipkin:
      endpointAddress: "http://zipkin.mec-accelerator.svc.cluster.local:9411/api/v2/spans"
```

The samplingRate attribute specifies the interval used for publishing traces. The value must be 
between 0 (tracing disabled) and 1 (every trace is published).With a value of 0.5, for example, every 
other trace is published, significantly reducing published traffic.

The endpointAddress points to an endpoint on a Zipkin server running in a Kubernetes cluster. The default port for Zipkin is 9411.

The configuration must be applied to the Kubernetes cluster using the Kubernetes CLI:

```
kubectl apply -f dapr-config.yaml
```

### Configure the services to use the tracing configuration

Now everything is set up correctly to start publishing telemetry. Every Dapr sidecar that is deployed as 
part of the application must be instructed to emit telemetry when started. To do that, add a 
**dapr.io/config** annotation that references the dapr-config configuration to the deployment of 
each service. 

Here’s an example of the rules engine service’s manifest file containing 
the annotation:

```YAML
dapr.io/config: "dapr-config"
```

```YAML
apiVersion: apps/v1
kind: Deployment
metadata:
  name: rulesengine
  namespace: mec-accelerator
  labels:
    app: rulesengine
spec:
  replicas: 1
  selector:
   ...
  template:
    metadata:
      labels:
        app: rulesengine
      annotations:
        dapr.io/enabled: "true"
        dapr.io/app-id: "rulesengine"
        dapr.io/app-port: "80"
        dapr.io/config: "dapr-config"
    spec:
      ...
```

## Deploy zipkin server

When installing Dapr on a Kubernetes cluster, Zipkin must be deployed manually. Use the following 
Kubernetes manifest file __deploy/k8s/zipkin.yaml__ to deploy a standard Zipkin server to a Kubernetes 
cluster.

Kubernetes CLI to apply the Zipkin manifest file to the Kubernetes cluster and deploy the Zipkin 
server:

```
kubectl apply -f zipkin.yaml
```
If you wish to remove the zipkin server delete the deployment using kubernetes CLI:

```
kubectl delete -f zipkin.yaml
```

## Viewing Tracing Logs in the Zipkin Portal

To view tracing logs in the Zipkin portal, perform the following steps:

1. **Open the Zipkin portal:** Open the Zipkin portal by navigating to `http://localhost:9411/zipkin` in your web browser. If using minikube, use the cli to forward the service ports using `minikube service zipikin -n mec-accelerator`

![zipkin dashboard](/docs/imgs/zipkin/zipkin1.png)

2. **Filter the logs by service and time:** Select your application's service name in the `Service Name` dropdown, and set the time frame you want to filter by using the time range picker.

![zipkin service traces](/docs/imgs/zipkin/zipkin2.png)

3. **View the logs:** View the tracing logs in the Zipkin portal. You can click on each trace to see the details and logs for each request.

![zipkin tracing logs](/docs/imgs/zipkin/zipkin3.png)

3. **View service dependencies logs:** Because Dapr sidecars handle traffic between services, Zipkin can use the trace information to determine the dependencies between the services. To see it in action, go to the Dependencies tab on the Zipkin web page and select the button with the magnifying glass. Zipkin will show an overview of 
the services and their dependencies

![zipkin tracing dependencies](/docs/imgs/zipkin/zipkin4.png)