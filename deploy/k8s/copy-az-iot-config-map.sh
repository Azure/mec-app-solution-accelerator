#!/bin/sh

kubectl get configmap aio-ca-trust-bundle-test-only -n azure-iot-operations -o yaml | sed 's/namespace: azure-iot-operations/namespace: mec-accelerator/' | kubectl apply --namespace mec-accelerator -f -
