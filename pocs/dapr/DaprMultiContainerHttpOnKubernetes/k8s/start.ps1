kubectl apply `
    -f namespace.yaml `
    -f zipkin.yaml `
    -f dapr-config.yaml `
    -f mybackend.yaml `
    -f myfrontend.yaml