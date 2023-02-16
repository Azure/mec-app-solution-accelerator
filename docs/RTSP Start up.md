## 0 - (Optional) - start vm

## 1 - Download folder RTSP-server

## 2 - go to folder rtsp and execute ./rtsp-simple-server.exe rtsp-simple-server.yml on powershell

## 3 - execute '\ffmpeg\bin\ffmpeg.exe -re -stream_loop -1 -i .\demo.mp4 -c copy -f rtsp -rtsp_transport tcp rtsp://myuser:mypass@localhost:8554/mystream'

## 4 - (Optional) -  Navigate to ngrok folder and execute './ngrok authtoken yourauthtoken'. It is needed to create an account in ngrok

## 5 - (Optional) - Execute './ngrok tcp 8554'

## 6 - (Optional) - replace localhost by the direction appearing in the screen '0.tcp.ngrok.io:13571' in your docker compose or framesplitter configuration yaml
