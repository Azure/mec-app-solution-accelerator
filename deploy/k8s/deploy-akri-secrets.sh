#!/bin/sh

# We need to copy the dapr certificates to start the framesplitter broker
kubectl get secret dapr-trust-bundle --namespace=dapr-system -oyaml | grep -v '^\s*namespace:\s' | kubectl apply --namespace=mec-accelerator -f -
