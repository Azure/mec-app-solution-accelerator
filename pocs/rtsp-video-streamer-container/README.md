# rtsp-video-streamer-container
A container in Python that streams any video through a RTSP uri. Can be tested with VLC media player.

# Basic Procedure:

## 1. Copy the video you want to stream as RTSP

Copy the video you want to stream as RTSP within the folder **"src"** and name your video as **"video.mp4"** (Possibly replacing the existing "video.mp4")

## 2. Build the Docker image with:

From Windows PowerShell:

```powershell
./build-streamer-image.ps1
```

## 3. Run the container with:

From Windows PowerShell:

```powershell
./run-streamer-local.ps1
```

You will see the following output:

```powershell
> .\run-streamer-local.ps1

Environment: server_ip <YOUR_IP>, server_port 8554, uri video
Example full URI: rtsp://<YOUR_IP>:8554/video
```
<img width="1171" alt="image" src="https://github.com/CESARDELATORRE/wildfire-app-solution-accelerator/assets/1712635/9afbfa94-488a-4606-aeef-2d8944be34c8">

Take note or copy the RTSP url above, like:

rtsp://<YOUR_IP>:8554/video

**IMPORTANT:** Note that if you try to use `localhost` or `127.0.0.1` from the Application in Docker, by default it wont work because the Docker containers won't be able to reach `localhost` or `127.0.0.1` but only the real IP of your network adapter in use.

## 5. Test the RTSP streaming with 

If you don't have it installed, install "VLC media player" from:

A. Microsoft Windows Apps Store:
https://apps.microsoft.com/store/detail/vlc/XPDM1ZW6815MQM

B. Videolan.org
https://www.videolan.org/vlc/index.wa.html

Run VLC media player and open the RTSP url:

<img width="1189" alt="image" src="https://github.com/CESARDELATORRE/wildfire-app-solution-accelerator/assets/1712635/1aeb8f7b-7fc7-468e-b2c9-6cedb6dce0ee">

Then, watch the video!:)

<img width="1193" alt="image" src="https://github.com/CESARDELATORRE/wildfire-app-solution-accelerator/assets/1712635/f6f6bbbe-ce90-488c-bc04-a1de36902673">

# Stop the RTSP container

It's important to stop and delete the container once you stop it with "Ctrl+C" or closes the terminal window.
Otherwise, you'll get an error when trying to start it again.

In order to stop and clean it up, run this script:

```powershell
./stop-remove-streamer-container.ps1
```
After this clean-up step, you can start the steps above again, later on.



# Deploy as a kubernetes device

# Build image MacOs

```
docker build --tag=rtsp-streamer:latest --file=./Dockerfiles/rtsp .
```