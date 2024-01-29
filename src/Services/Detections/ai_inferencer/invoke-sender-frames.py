from dapr.ext.grpc import App, InvokeMethodRequest, InvokeMethodResponse
import torch
from src.inference import main
import json
import logging
app = App()
global model 
model = torch.hub.load("ultralytics/yolov5", "yolov5s")
@app.method(name='frames-receiver')
def framesreceiver(request: InvokeMethodRequest) -> InvokeMethodResponse:
    
    logging.info(f'frame received')
    

    image_id = json.loads(request.text())['image_id']
    source_id = json.loads(request.text())['source_id']
    timestamp = json.loads(request.text())['timestamp']
    time_trace = json.loads(request.text())['time_trace']

    detection_threshold=0.0

    path='events_schema/detections.avro'

    main(source_id,timestamp,model,image_id,detection_threshold,path,time_trace)


    return InvokeMethodResponse(b'Frame Analyzed', "text/plain; charset=UTF-8")

app.run(2060)