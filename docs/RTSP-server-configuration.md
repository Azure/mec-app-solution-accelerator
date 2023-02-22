# RTSP Server set-up

Eventhough there are different ways to provide a new video to the system, in this document we will focus on the rtsp provisioning

## Virtual Machine Creation - Optional

This step is optional if using docker compose and you can jump to RTSP Server Run.

At first an Azure Subscription is needed.
Then we proceed to create our Azure Virtual Machine (first one in the list)

![plot](./imgs/RTSP-server/step1.png)

Next, machine selection is needed. In this example we have opted for a Standard_B4ms and Windows 11 system, as binaries for RTSP server are preprared to work with windows.

![plot](./imgs/RTSP-server/step2.png)

Afterward, selection of ports is nedeed. In this example we have selected app ports and RDP port in the creation step

![plot](./imgs/RTSP-server/step3.png)

Once the VM is created, go to the networking section and add new port as depicted in the following image

![plot](./imgs/RTSP-server/step4.png)

After all this proccess you will be ready to connect to your VM using Windows Remote Desktop

![plot](./imgs/RTSP-server/step5.png)

## RTSP Server Run

Navigate to RTSP-server folder or download it in your VM

![plot](./imgs/RTSP-server/step6.png)

Navigate to rtsp folder in terminal and execute 
```
.\rtsp-simple-server.exe rtsp-simple-server.yml
```

The output should be similar to the portrayed in the following image

![plot](./imgs/RTSP-server/step7.png)


Open a new terminal window, navigate to the RTSP-server older , unzip de ffmpeg.zip and execute

```
ffmpeg\bin\ffmpeg.exe -re -stream_loop -1 -i .\videos\YOURDESIREDVIDEO.mp4 -c copy -f rtsp -rtsp_transport tcp rtsp://myuser:mypass@localhost:8554/mystream
```

![plot](./imgs/RTSP-server/step8.png)

## Expose server to public internet

In case you are not using an Azure VM configured as explained in this document, or you want to have an external computer to run your rtsp, you can expose the RTSP using different services such as ngrok. Visit https://ngrok.com/ and get your authtoken and register it. 

```
./ngrok authtoken yourauthtoken
```

After this navigate to de ngrok folder inside de RTSP-server and execute:

```
./ngrok tcp 8554
```

![plot](./imgs/RTSP-server/step9.png)

finally you will have a URL as:

rtsp://20.42.105.26:8554/mystream

or if you are using an authenticated RTSP URL

rtsp://myuser:mypass@localhost:8554/mystream

Replacing in both cases de ip or localhost