## 0 - (Optional) - start vm

## 1 - Download folder RTSP-server

## 2 - go to folder rtsp and execute ./rtsp-simple-server.exe rtsp-simple-server.yml on powershell

## 3 - execute '\ffmpeg\bin\ffmpeg.exe -re -stream_loop -1 -i .\demo.mp4 -c copy -f rtsp -rtsp_transport tcp rtsp://myuser:mypass@localhost:8554/mystream'

## 4a - Replace the url in the docker-compose.override.yml in framesplitter with rtsp://myuser:mypass@localhost:8554/mystream' to FEED. 
## 4b - Replace the url in the \deploy\k8s\feed-configmap.yaml in data-tag url with rtsp://myuser:mypass@localhost:8554/mystream'.

## 5- (Optional) -  Navigate to ngrok folder and execute './ngrok authtoken yourauthtoken'. It is needed to create an account in ngrok

## 6 - (Optional) - Execute './ngrok tcp 8554'

## 7 - (Optional) - replace localhost by the direction appearing in the screen '0.tcp.ngrok.io:13571' in your docker compose or framesplitter configuration yaml
