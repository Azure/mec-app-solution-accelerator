#ifndef __PARSER_YOLOAUTOML_H__
#define __PARSER_YOLOAUTOML_H__

#include <cmath>
#include <cstring>
#include <iostream>
#include "nvdsinfer_custom_impl.h"
#include "nvdsmeta.h"
#include <vector>
#include <algorithm>
#include <omp.h>

#define MIN(a,b) ((a) < (b) ? (a) : (b))
#define MAX(a,b) ((a) > (b) ? (a) : (b))
#define CLIP(a,min,max) (MAX(MIN(a, max), min))

extern "C" bool ParserCustomAutoML (std::vector<NvDsInferLayerInfo> const& outputLayersInfo, NvDsInferNetworkInfo const& networkInfo,
    NvDsInferParseDetectionParams const& detectionParams, std::vector<NvDsInferObjectDetectionInfo>& objectList)
{
  static int outputLayerIndex = -1;
  static const int NUM_CLASSES = 1;
  static bool classMismatchWarn = false;
  float scoreThreshold = 0.3;
  size_t count;
  objectList.clear();
  std::vector<NvDsInferObjectDetectionInfo> detections;

  // Find output layer index only once
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

  const NvDsInferLayerInfo *outputLayer = &outputLayersInfo[outputLayerIndex];
  count = outputLayer->inferDims.numElements / 6;
  float *data = static_cast<float*>(outputLayer->buffer);

  if (!classMismatchWarn && NUM_CLASSES != detectionParams.numClassesConfigured) {
    std::cerr << "WARNING: Num classes mismatch. Configured:" <<
      detectionParams.numClassesConfigured << ", detected by network: " <<
      NUM_CLASSES << std::endl;
    classMismatchWarn = true;
  }

  // detections.reserve(count); // Reserve space for detections

  // Parse detections
  for (size_t i = 0; i < count; ++i) {
    float* detectionPtr = data + i * 6;
    float roundedClassId = round(detectionPtr[5]);
    if (detectionPtr[4] > scoreThreshold && roundedClassId != 0) {
      detections.push_back({roundedClassId, detectionPtr[0], detectionPtr[1], 
                              detectionPtr[2], detectionPtr[3], detectionPtr[4]});
    }
  }
  // detections.erase(std::remove_if(detections.begin(), detections.end(),
  //   [](const NvDsInferObjectDetectionInfo& detection) {
  //       return detection.classId == 0; // Assuming classId is 0 for uninitialized elements
  //   }), detections.end());

  // Populate objectList
  for (const auto& detection : detections) {
    if (detection.width && detection.height) {
      NvDsInferObjectDetectionInfo object;
      object.left = CLIP(detection.left, 0, networkInfo.width - 1);
      object.top = CLIP(detection.top, 0, networkInfo.height - 1);
      object.width = CLIP(detection.width, 0, networkInfo.width - 1);
      object.height = CLIP(detection.height, 0, networkInfo.height - 1);
      object.detectionConfidence = detection.detectionConfidence;
      object.classId = detection.classId;

      objectList.push_back(object);
    }
  }

  detections.clear(); // Free up reserved space

  return true;
}

#endif