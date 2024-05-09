## Try the 'Control Plain app' to provision cameras

In order to know the URL for the 'Control Plain web app' (IP and port to use), type the following command:

```powershell
kubectl get services --namespace mec-accelerator
```

<img width="1000" alt="image" src="./imgs/deploy_06.png">

Then search for the service with name **"control-plane-ui-service"** and related IP and external port, so you write a URL similar to the following in a browser:

`http://<your-IP>:90/`

<img width="1000" alt="image" src="./imgs/deploy_02.png">

### Provision a camera simulator with the out-of-the-box RTSP stream container

As a first try, before trying with any real IP camera, you can test the solution with our out-of-the-box camera simulator which is a Docker container that is streaming a video through RTSP.

In order to provision such "camera simulator", click on the "Cameras Provisioning" menu option, then "Add new Camera".

In that dialog, write a name for that camera simulator, select "Container" as the Type and finally select "Default RTSP Stream Container" as the Model.
With that, the app will automatically select the internal IP used in Kubernetes for the RTSP pod (IP = "rtsp-video-streamer") and it will also automatically generate its related RTSP Uri:

![image](https://github.com/Azure/mec-app-solution-accelerator/assets/1712635/2cf4de62-392b-441d-b997-2701fcc28669)

After that you can check that the video stream is working by clicking to the "Cameras Dashboard" menu option so you should see it like the following:

<img width="806" alt="image" src="https://github.com/Azure/mec-app-solution-accelerator/assets/1712635/62043726-6202-4983-9470-f2350b6727d6">

**Important:** Note that the cameras dashboard intended use is just to check that the camera streams are working properly. This dashboard is not working with the object detection AI model. The object detections and alerts are shown in the "Alerts Dashboard".


### Access the Alerts dashboard UI with Alerts originated from AI model detections

To access the  front-end, 
In order to know the URL for the 'Alerts dashboard UI' front-end (IP and port to use), type the following command:

```powershell
kubectl get services --namespace mec-accelerator
```

<img width="1000" alt="image" src="./docs/imgs/deploy_07.png">

Then search for the service with name **"alerts-ui"** and related IP and external port, so you write a URL similar to the following in a browser:

`http://<your-IP>:88/`

Alternatively, you can also click on the link "Alerts Dashboard" from the Control-Plane app here:

<img width="1000" alt="image" src="./docs/imgs/deploy_03.png">

Following any of those ways you should be able to run the 'Alerts dashboard UI' and check out the Alerts originated from the AI models when analyzing the video:

<img width="1000" alt="image" src="./docs/imgs/deploy_05.png">
