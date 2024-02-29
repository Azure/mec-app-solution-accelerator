import json
import onnxruntime
import torch

# labels_file = "YoloAutoML/labels.json"
# with open(labels_file) as f:
#     classes = json.load(f)
# classes=['smoke']
# print(classes)
# try:
#     session = onnxruntime.InferenceSession('YoloAutoML/1/model.onnx')
#     print("ONNX model loaded...")
# except Exception as e: 
#     print("Error loading ONNX file: ", str(e))

# sess_input = session.get_inputs()
# sess_output = session.get_outputs()
# print(f"No. of inputs : {len(sess_input)}, No. of outputs : {len(sess_output)}")

# for idx, input_ in enumerate(range(len(sess_input))):
#     input_name = sess_input[input_].name
#     input_shape = sess_input[input_].shape
#     input_type = sess_input[input_].type
#     print(f"{idx} Input name : { input_name }, Input shape : {input_shape}, \
#     Input type  : {input_type}")  

# for idx, output in enumerate(range(len(sess_output))):
#     output_name = sess_output[output].name
#     output_shape = sess_output[output].shape
#     output_type = sess_output[output].type
#     print(f" {idx} Output name : {output_name}, Output shape : {output_shape}, \
#     Output type  : {output_type}")


# import glob
# import numpy as np
# from PIL import Image

# from yolo_onnx_preprocessing_utils import preprocess

# # use height and width based on the generated model
# test_images_path = "automl_models_od_yolo/test_images_dir/*" # replace with path to images
# image_files = ['1.jpeg']
# img_processed_list = []
# pad_list = []
# batch_size=1
# for i in range(batch_size):
#     img_processed, pad = preprocess(image_files[i])
#     img_processed_list.append(img_processed)
#     pad_list.append(pad)
    
# if len(img_processed_list) > 1:
#     img_data = np.concatenate(img_processed_list)
# elif len(img_processed_list) == 1:
#     img_data = img_processed_list[0]
# else:
#     img_data = None

# assert batch_size == img_data.shape[0]

# img = Image.open('1.jpeg')

# img_data = preprocess(img, 800, 800)

def get_predictions_from_ONNX(onnx_session,img_data):
    """perform predictions with ONNX Runtime
    
    :param onnx_session: onnx model session
    :type onnx_session: class InferenceSession
    :param img_data: pre-processed numpy image
    :type img_data: ndarray with shape 1xCxHxW
    :return: boxes, labels , scores 
    :rtype: list
    """
    sess_input = onnx_session.get_inputs()
    sess_output = onnx_session.get_outputs()
    # predict with ONNX Runtime
    output_names = [ output.name for output in sess_output]
    pred = onnx_session.run(output_names=output_names,\
                                               input_feed={sess_input[0].name: img_data})
    return pred[0]

# result = get_predictions_from_ONNX(session, img_data)


# from yolo_onnx_preprocessing_utils import non_max_suppression, _convert_to_rcnn_output
# # print(result)
# for val in result[0]:
#     # print(val)
    
#     if val[4]>0.3:
#         print(int(val[0]),int(val[1]),int(val[2]),int(val[3]),(val[4]),round(val[5]))
# result_final = non_max_suppression(
#     torch.from_numpy(result),
#     conf_thres=0.1,
#     iou_thres=0.5)


# # print(len(result[0]))
# # print(result_final[0])
# for result in result_final:
#     result=result.numpy()
#     rcnn_label = {"boxes": [], "labels": [], "scores": []}
#     rcnn_label["boxes"] = result[:, :4]
#     rcnn_label["labels"] = result[:, 5:6]
#     rcnn_label["scores"] = result[:, 4:5]
# print(rcnn_label)



# import cv2
# import numpy as np

# img_np = cv2.imread('1.jpeg')  # replace with desired image index
# img_np=cv2.resize(img_np, (800, 800))

# img = Image.fromarray(img_np.astype('uint8'), 'RGB')

# x, y = img.size
# print(img.size)

# # Create a copy of the image to draw on
# output_img = img_np.copy()

# # Draw box and label for each detection
# label = rcnn_label["labels"]
# boxes = rcnn_label["boxes"]

# xmin, ymin, xmax, ymax = boxes[0] / 800
# xmin, ymin, xmax, ymax = xmin * x, ymin * y, xmax * x, ymax * y
# print(xmin, ymin, xmax, ymax)
# width, height = xmax - xmin, ymax - ymin

# # Draw rectangle on the image
# cv2.rectangle(output_img, (int(xmin), int(ymin)), (int(xmax), int(ymax)), (0, 255, 0), 1)

# # Save the output image
# cv2.imwrite('output.jpg', output_img)

# def _get_box_dims(image_shape, box):
#     box_keys = ['topX', 'topY', 'bottomX', 'bottomY']
#     height, width = image_shape[0], image_shape[1]

#     box_dims = dict(zip(box_keys, [coordinate.item() for coordinate in box]))

#     box_dims['topX'] = box_dims['topX'] * 1.0 / width
#     box_dims['bottomX'] = box_dims['bottomX'] * 1.0 / width
#     box_dims['topY'] = box_dims['topY'] * 1.0 / height
#     box_dims['bottomY'] = box_dims['bottomY'] * 1.0 / height

#     return box_dims

# def _get_prediction(boxes, labels, scores, image_shape, classes):
#     bounding_boxes = []
#     for box, label_index, score in zip(boxes, labels, scores):
#         box_dims = _get_box_dims(image_shape, box)

#         box_record = {'box': box_dims,
#                       'label': classes[label_index],
#                       'score': score.item()}

#         bounding_boxes.append(box_record)

#     return bounding_boxes

# # Filter the results with threshold.
# # Please replace the threshold for your test scenario.
# score_threshold = 0.8
# filtered_boxes_batch = []
# for batch_sample in range(0, batch_size*3, 3):
#     # in case of retinanet change the order of boxes, labels, scores to boxes, scores, labels
#     # confirm the same from order of boxes, labels, scores output_names 
#     boxes, labels, scores = predictions[batch_sample], predictions[batch_sample + 1], predictions[batch_sample + 2]
#     bounding_boxes = _get_prediction(boxes, labels, scores, (height_onnx, width_onnx), classes)
#     filtered_bounding_boxes = [box for box in bounding_boxes if box['score'] >= score_threshold]
#     filtered_boxes_batch.append(filtered_bounding_boxes)