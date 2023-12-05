/*
 * Copyright (c) 2018-2019, NVIDIA CORPORATION. All rights reserved.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a
 * copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 */

#include <cmath>
#include <cstring>
#include <iostream>
#include "nvdsinfer_custom_impl.h"
#include "nvdsmeta.h"

// #include "nvdssample_fasterRCNN_common.h"

#define MIN(a,b) ((a) < (b) ? (a) : (b))
#define MAX(a,b) ((a) > (b) ? (a) : (b))
#define CLIP(a,min,max) (MAX(MIN(a, max), min))

/* This is a sample bounding box parsing function for the sample FasterRCNN
 * detector model provided with the TensorRT samples. */
#include <vector>
#include <algorithm>

/* C-linkage to prevent name-mangling */





extern "C" bool NvDsInferParseCustomAutoML (std::vector<NvDsInferLayerInfo> const& outputLayersInfo, NvDsInferNetworkInfo const& networkInfo,
    NvDsInferParseDetectionParams const& detectionParams, std::vector<NvDsInferObjectDetectionInfo>& objectList)
{
  return true;
  static int outputLayerIndex = -1;
  static const int NUM_CLASSES = 1;
  std::vector<NvDsInferObjectDetectionInfo> detections;
  static bool classMismatchWarn = false;
  float scoreThreshold = 0.3;
  size_t count;
  objectList.clear();

  int numClassesToParse;
  for (unsigned int i = 0; i < outputLayersInfo.size(); i++) {
    
  }
  if (outputLayerIndex == -1) {
    for (unsigned int i = 0; i < outputLayersInfo.size(); i++) {
      if (strcmp(outputLayersInfo[i].layerName, "output") == 0) {
        outputLayerIndex = i;
        break;
      }
    }
    if (outputLayerIndex == -1) {
    std::cerr << "Could not find output layer buffer while parsing" << std::endl;
    return false;
    }
  }

	



  if (!classMismatchWarn) {
    if (NUM_CLASSES !=
        detectionParams.numClassesConfigured) {
      std::cerr << "WARNING: Num classes mismatch. Configured:" <<
        detectionParams.numClassesConfigured << ", detected by network: " <<
        NUM_CLASSES << std::endl;
    }
    classMismatchWarn = true;
  }

  numClassesToParse = MIN (NUM_CLASSES,
      detectionParams.numClassesConfigured);



  float *data = (float*) outputLayersInfo[outputLayerIndex].buffer;
  std::cout << "OLI" << outputLayerIndex << std::endl;
  count = outputLayersInfo[outputLayerIndex].inferDims.numElements / 6;
  std::cout << "count" << std::endl;
  std::cout << count << std::endl;
  // // Llenar el vector de detecciones a partir del buffer
  for (size_t i = 0; i < count; ++i) {
    float* detectionPtr = data + i * 6; // Asumiendo 6 valores por detección
    if (detectionPtr[4]>scoreThreshold && round(detectionPtr[5])!=0){
      detections.push_back({round(detectionPtr[5]), detectionPtr[0], 
                              detectionPtr[1], detectionPtr[2], detectionPtr[3], detectionPtr[4]});
      

      }
    }
  
  for (int k = 0; k < detections.size(); k++){
    NvDsInferObjectDetectionInfo object;
    object.left = CLIP(detections[k].left, 0, networkInfo.width - 1);
    object.top = CLIP(detections[k].top, 0, networkInfo.height - 1);
    object.width = CLIP((detections[k].width), 0, networkInfo.width - 1);
    object.height = CLIP((detections[k].height), 0, networkInfo.height - 1);
    object.detectionConfidence = detections[k].detectionConfidence;
    object.classId = (detections[k].classId);
    if (object.width && object.height)
		{
    std::cout << "detection" << std::endl;
    objectList.push_back(object); }
  }
  std::cout << "out" << std::endl;
  return true;
  
  
}

/* Check that the custom function has been defined correctly */
// CHECK_CUSTOM_PARSE_FUNC_PROTOTYPE(NvDsInferParseCustomFasterRCNN);
CHECK_CUSTOM_PARSE_FUNC_PROTOTYPE(NvDsInferParseCustomAutoML);
