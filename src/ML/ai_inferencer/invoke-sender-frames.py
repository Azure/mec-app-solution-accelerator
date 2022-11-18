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
    image_list = json.loads(request.text())['image']
    frame=numpy.array(image_list)
    main(model,frame)
    # print(request.metadata, flush=True)
    # print(request.text(), flush=True)
    return InvokeMethodResponse(b'Frame Analyzed', "text/plain; charset=UTF-8")

app.run(50051)