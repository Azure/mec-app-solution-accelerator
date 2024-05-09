# How to provision a new RTSP stream feed for a Docker only deployment

The current solution deployed in a plain Docker host supports a single one RTSP feed since Docker is not multi-instance, as Kubernetes.

## Docker Compose configuration

In docker-compose.override.yml find the framesplitter service.

![plot](./imgs/new-feed/step1-dc.png)

Comment the current feed line which is using a by default video:

```
- FEED=../Datasets/PerimeterDemo.mp4
```

Uncomment and modify the rtsp url with your ip and port

```
- FEED=rtsp://20.42.105.26:8554/mystream
```

It is also possible to run your own videos using compose, if you add your video to datasets folder, and reference it correctly

![plot](./imgs/new-feed/step2-dc.png)

```
- FEED=../Datasets/YOURFILE.mp4
```




















