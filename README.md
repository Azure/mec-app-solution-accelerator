# MEC Application Solution Accelerator (Example Reference Application for Edge/MEC)

Example MEC (Multi-Access Edge Compute) reference application, powered by Microsoft, based on a microservices architecture and Docker containers running at the Edge (Kubernetes cluster at Edge compute).

A typical deployment of this application/services would be performed at the Edge, and using a wireless 5G network to connect the devices, cameras, etc. to the application's services.

## Getting Started

Make sure you have [installed](https://docs.docker.com/desktop/install/windows-install/) and configured docker in your environment. After that, you can run the below commands from the solution's root directory and get started trying it.

```powershell
docker-compose build
docker-compose up
```
You should see docker compose up starting like in the following screenshot:
<img width="1126" alt="image" src="https://user-images.githubusercontent.com/1712635/212741292-4396cc66-3ce9-451b-8d2f-bb3e6ec8e8b2.png">

You should be able to browse different components of the application by using the below URLs :

**TBD - PLEASE ADD URLs to use when using "docker compose up" here:**

**UI web app:**
https://tbd

(Add screenshot for UI web app)

**Alerts.API service Swagger page:**
https://tbd

(Add screenshot for Alerts.API service Swagger page)



### Possible deployments

This sample microservice application can run locally using a local Kubernetes cluster such as "Kubernetes in Docker for Desktop" or any other Kubernetes distribution.
For development purposes, you can also run it on plain Docker with "docker compose up" or Visual Studio.
Refer to these additional procedure information pages to Get Started on each environment:


**TBD - PLEASE ADD PAGES and URLs here:**

- [Local Kubernetes](https://tbd-url)
- [Visual Studio (F5 experience)](https://tbd-url)
- [Docker compose on windows](https://tbd-url)
- [Docker compose on macOS](https://tbd-url)


### URLs when using Visual Studio (Move to VS page to create)

**Temporal for MyFrontEnd web app:**
https://localhost:52710/
or
https://host.docker.internal:52710/

**Alerts.API service Swagger page:**
http://localhost:52708/swagger/index.html

<img width="481" alt="image" src="https://user-images.githubusercontent.com/1712635/212743730-a75e728c-8d6d-4267-9847-1f141d7ad7f9.png">


## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft 
trademarks or logos is subject to and must follow 
[Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general).
Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship.
Any use of third-party trademarks or logos are subject to those third-party's policies.
