from dapr.ext.grpc import App, InvokeMethodRequest, InvokeMethodResponse
import torch
from src.inference import main
import json
import numpy
app = App()
global model 
model = torch.hub.load("ultralytics/yolov5", "yolov5s")
@app.method(name='frames-receiver')
def framesreceiver(request: InvokeMethodRequest) -> InvokeMethodResponse:
    # model = torch.hub.load("ultralytics/yolov5", "yolov5s")
    print("in")

    frame = json.loads(request.text())['image']
    source_id = json.loads(request.text())['source_id']
    timestamp = json.loads(request.text())['timestamp']
    # frame=numpy.array(image_list)
    detection_threshold=0.0
    # print(request.metadata, flush=True)
    # print(request.text(), flush=True)
    path='src/detections.avro'
    main(source_id,timestamp,model,frame,detection_threshold,path)

    return InvokeMethodResponse(b'Frame Analyzed', "text/plain; charset=UTF-8")

app.run(50060)