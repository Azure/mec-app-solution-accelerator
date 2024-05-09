# How to provision a new RTSP stream feed for a Docker only deployment

The current solution deployed in a plain Docker host supports a single one RTSP feed since Docker is not multi-instance, as Kubernetes.

## Docker Compose configuration

In docker-compose.override.yml find the framesplitter service.

![plot](./imgs/new-feed/step1-dc.png)

Comment the current feed line which is using a by default video:

```
- FEED=../Datasets/PerimeterDemo.mp4
```

Uncomment and modify the rtsp url with your ip and port.

That RTSP Uri can be the RTSP of any working IP camera. If the camera RTSP needs a user and password, you can provide it as part of the RTSP Uri, as well.

This is an example of a real IP camera:

```
- FEED=rtsp://admin:MyPassword@192.160.0.55:554/cam/realmonitor?channel=1&subtype=0
```

** Changing the by default video stream embeded in the container image**

If you want to change the by default video stream embeded in the container image (.mp4 file), it is also possible by copying your own mp4 file into the datasets folder withn the FrameSplitter container, and reference the .mp4 file name correctly from docker compose. 

Note that in this case you need to build your own Docker Image of the FrameSplitter.

![plot](./imgs/new-feed/step2-dc.png)

```
- FEED=../Datasets/YOURFILE.mp4
```

However, in general, the goal is to use RTSP of real video cameras or to consume a RTSP stream from a camera simulator from a container.




















