/*
 * Copyright (c) 2021, NVIDIA CORPORATION. All rights reserved.
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

#include <string.h>

#include <algorithm>
#include <iostream>
#include <map>
#include <mutex>
#include <numeric>
#include <sstream>
#include <string>

#include "infer_custom_process.h"
#include "nvdsinfer_custom_impl.h"
#include "nvbufsurface.h"
#include "nvdsmeta.h"
#include "parser_YoloAutoML.h"
#include <chrono>



typedef struct _GstBuffer GstBuffer;

/** This is a example how DeepStream Triton plugin(gst-nvinferserver) do
 * custom extra input preprocess and custom postprocess on triton based models.
 *
 * CustomProcess can support loop for LSTM-alike loop purpose.
 * To enable it, update REQUIRE_LOOP true
 * FasterRCNN does not require loop, this is just a sample to show how to use the
 * loop.
 */

// sample show how to use loop process pre stream
#define REQUIRE_LOOP false

// enable debug log
#define ENABLE_DEBUG 0

namespace dsis = nvdsinferserver;

#if ENABLE_DEBUG
#define LOG_DEBUG(fmt, ...) fprintf(stdout, "%s:%d" fmt "\n", __FILE__, __LINE__, ##__VA_ARGS__)
#else
#define LOG_DEBUG(fmt, ...)
#endif

#define LOG_ERROR(fmt, ...) fprintf(stderr, "%s:%d" fmt "\n", __FILE__, __LINE__, ##__VA_ARGS__)

#ifndef INFER_ASSERT
#define INFER_ASSERT(expr)                                                     \
    do {                                                                       \
        if (!(expr)) {                                                         \
            fprintf(stderr, "%s:%d ASSERT(%s) \n", __FILE__, __LINE__, #expr); \
            std::abort();                                                      \
        }                                                                      \
    } while (0)
#endif

// constant values definition
constexpr float kIouThreshold = 0.3f;
constexpr float kScoreThreshold = 0.5f;
constexpr uint32_t kInferWidth = 800;
constexpr uint32_t kInferHeight = 800;
constexpr uint32_t kInferChannel = 3;
static const std::vector<float> kDefaultImageInfo{(float)kInferHeight, (float)kInferWidth, 1.0f};
static const std::vector<std::string> kClassLabels = {
    "smoke"};

/** Define a function for custom processor for DeepStream Triton plugin(nvinferserver)
 * do custom extra input preprocess and custom postprocess on triton based models.
 * The sysmbol is loaded through
 *   infer_config {
 *     custom_lib {  path: "path/to/custom_impl_process.so" }
 *     extra {
 *       custom_process_funcion: "CreateInferServerCustomProcess"
 *     }}
 */
extern "C" dsis::IInferCustomProcessor*
CreateInferServerCustomProcess(const char* config, uint32_t configLen);

namespace {
// return buffer description string
std::string
strOfBufDesc(const dsis::InferBufferDescription& desc)
{
    std::stringstream ss;
    ss << "*" << desc.name << "*, shape: ";
    for (uint32_t i = 0; i < desc.dims.numDims; ++i) {
        if (i != 0) {
            ss << "x";
        } else {
            ss << "[";
        }
        ss << desc.dims.d[i];
        if (i == desc.dims.numDims - 1) {
            ss << "]";
        }
    }
    ss << ", dataType:" << (int)desc.dataType;
    ss << ", memType:" << (int)desc.memType;
    return ss.str();
}

// non-maximum suppression for bbox
std::vector<NvDsInferObjectDetectionInfo>
nms(std::vector<NvDsInferObjectDetectionInfo>& objs)
{

    std::vector<NvDsInferObjectDetectionInfo> final;
    final.reserve(objs.size());
    std::stable_sort(objs.begin(), objs.end(), [](const auto& a, const auto& b) {
        return a.detectionConfidence > b.detectionConfidence;
    });
    
    auto iou = [](const auto& a, const auto& b) {
        float x0 = std::max<float>(a.left, b.left);
        float x1 = std::min<float>(a.left + a.width, b.left + b.width);
        float y0 = std::max<float>(a.top, b.top);
        float y1 = std::min<float>(a.top + a.height, b.top + b.height);
        float overlap = (x1 - x0) * (y1 - y0);
        float unionArea = a.width * a.height + b.width * b.height - overlap;
        
        if (unionArea < 0.1f)
            return 0.0f;
        return overlap / unionArea;
    };
    for (size_t i = 0; i < objs.size(); ++i) {
        const auto& ref = objs[i];
        if (ref.detectionConfidence < kScoreThreshold)
            continue;
        final.push_back(ref);
        for (size_t j = i + 1; j < objs.size(); ++j) {
            if (ref.detectionConfidence < kScoreThreshold) {
                continue;
            }
            if (objs[j].classId == ref.classId && iou(ref, objs[j]) >= kIouThreshold) {
                objs[j].detectionConfidence = 0;
            }
        }
    }
    return final;
}

}  // namespace

/** Example of a Custom process instance for deepstream-triton(gst-nvinferserver) plugin
 * It is derived from nvdsinferserver::IInferCustomProcessor
 * If should be loaded through
 * config_triton_inferserver_primary_fasterRCNN.txt:
 *   infer_config {
 *     custom_lib {  path: "path/to/custom_impl_process.so" }
 *     extra {
 *       custom_process_funcion: "CreateInferServerCustomProcess"
 *     }
 *   }
 */
class NvInferServerCustomProcess : public dsis::IInferCustomProcessor {
private:
    std::map<uint64_t, std::vector<float>> _streamFeedback;
    std::mutex _streamMutex;

public:
    ~NvInferServerCustomProcess() override = default;
    /** override function
     * Specifies supported extraInputs memtype in extraInputProcess()
     */
    void supportInputMemType(dsis::InferMemType& type) override { type = dsis::InferMemType::kCpu; }

    /** override function
     * check whether custom loop process needed.
     * If return True, extraInputProcess() and inferenceDone() runs in order per stream_ids
     * This is usually for LSTM loop purpose. FasterRCNN does not need it.
     * The code for requireInferLoop() conditions just sample when user has
     * a LSTM-like Loop model and requires loop custom processing.
     * */
    bool requireInferLoop() const override { return REQUIRE_LOOP; }

    /**
     * override function
     * Do custom processing on extra inputs.
     * @primaryInput is already preprocessed. DO NOT update it again.
     * @extraInputs, do custom processing and fill all data according the tensor shape
     * @options, it has most of the common Deepstream metadata along with primary data.
     *           e.g. NvDsBatchMeta, NvDsObjectMeta, NvDsFrameMeta, stream ids...
     *           see infer_ioptions.h to see all the potential key name and structures
     *           in the key-value table.
     */
    NvDsInferStatus extraInputProcess(
        const std::vector<dsis::IBatchBuffer*>& primaryInputs,
        std::vector<dsis::IBatchBuffer*>& extraInputs, const dsis::IOptions* options) override
    {
        INFER_ASSERT(primaryInputs.size() > 0);

        // INFER_ASSERT(extraInputs.size() == 1);
        dsis::InferBufferDescription input0Desc = primaryInputs[0]->getBufDesc();
        // dsis::InferBufferDescription input1Desc = extraInputs[0]->getBufDesc();
        // INFER_ASSERT(input1Desc.dataType == dsis::InferDataType::kFp32);
        // INFER_ASSERT(input1Desc.elementSize == sizeof(float));  // bytes per element

        INFER_ASSERT(!strOfBufDesc(input0Desc).empty());
        LOG_DEBUG("extraInputProcess: primary input %s", strOfBufDesc(input0Desc).c_str());

        // LOG_DEBUG("extraInputProcess: extra input %s", strOfBufDesc(input1Desc).c_str());

        int batchSize = input0Desc.dims.d[0];

        INFER_ASSERT(batchSize >= 1);
        // INFER_ASSERT(extraInputs[0]->getTotalBytes() >= (uint32_t)batchSize * sizeof(float));
        // std::vector<uint64_t> streamIds;
        // if (options) {
        //     INFER_ASSERT(
        //         options->getValueArray(OPTION_NVDS_SREAM_IDS, streamIds) == NVDSINFER_SUCCESS);
        //     INFER_ASSERT(streamIds.size() == (uint32_t)batchSize);
        // }
        

        // float* input1 = (float*)extraInputs[0]->getBufPtr(0);
        // if (!requireInferLoop()) {
        //     // if loop not required, just copy the default image size
        //     for (int i = 0; i < batchSize; ++i) {
        //         memcpy(&input1[i * 3], kDefaultImageInfo.data(), sizeof(float) * 3);
        //     }
        // } else {
        //     /** if loop required, check stream feedbacks from last inference result.
        //      * It is a just sample purpose how to customize loop, not required
        //      *  for FasterRCNN.
        //      */
        //     std::unique_lock<std::mutex> locker(_streamMutex);
        //     for (int i = 0; i < batchSize; ++i) {
        //         if (streamIds.size() &&
        //             _streamFeedback.find(streamIds[i]) != _streamFeedback.end()) {
        //             memcpy(&input1[i * 3], _streamFeedback[streamIds[i]].data(), sizeof(float) * 3);
        //         } else {
        //             memcpy(&input1[i * 3], kDefaultImageInfo.data(), sizeof(float) * 3);
        //         }
        //     }
        // }
        return NVDSINFER_SUCCESS;
    }

    /** override function
     * Custom processing for inferenced output tensors.
     * output memory types is controlled by gst-nvinferserver config file
     *     config_triton_inferserver_primary_fasterRCNN.txt:
     *       infer_config {
     *         backend {  output_mem_type: MEMORY_TYPE_CPU }
     *     }
     * User can even attach parsed metadata into GstBuffer from this function
     */
    NvDsInferStatus inferenceDone(
        const dsis::IBatchArray* outputs, const dsis::IOptions* inOptions) override
    {
        
        if (requireInferLoop()) {
            feedbackStreamInput(outputs, inOptions);
        }
        std::vector<uint64_t> streamIds;
        INFER_ASSERT(
            inOptions->getValueArray(OPTION_NVDS_SREAM_IDS, streamIds) == NVDSINFER_SUCCESS);
        INFER_ASSERT(!streamIds.empty());
        uint32_t batchSize = streamIds.size();
        
        for (uint32_t iBatch = 0; iBatch < batchSize; ++iBatch) {

            std::vector<NvDsInferObjectDetectionInfo> parsedBboxes;
            INFER_ASSERT(parseObjBbox(outputs, parsedBboxes, iBatch) == NVDSINFER_SUCCESS);
            INFER_ASSERT(attachObjMetas(inOptions, parsedBboxes, iBatch) == NVDSINFER_SUCCESS);

        }

        return NVDSINFER_SUCCESS;
    }

    /** override function
     * Receiving errors if anything wrong inside lowlevel lib
     */
    void notifyError(NvDsInferStatus s) override
    {
        std::unique_lock<std::mutex> locker(_streamMutex);
        _streamFeedback.clear();
    }

private:
    /** function for loop processing only. not requried for fasterRCNN
     */
    NvDsInferStatus feedbackStreamInput(
        const dsis::IBatchArray* outputs, const dsis::IOptions* inOptions);

    /** parse all bounding boxes from inferenced tensors
     */
    NvDsInferStatus parseObjBbox(
        const dsis::IBatchArray* tensors, std::vector<NvDsInferObjectDetectionInfo>& outObjs,
        uint32_t batchIdx);
    /**
     * attach bounding boxes into NvDsBatchMeta and NvDsFrameMeta
     */
    NvDsInferStatus attachObjMetas(
        const dsis::IOptions* inOptions, const std::vector<NvDsInferObjectDetectionInfo>& objs,
        uint32_t batchIdx);
};

/** Implementation to Create a custom processor for DeepStream Triton
 * plugin(nvinferserver) to do custom extra input preprocess and custom
 * postprocess on triton based models.
 */
extern "C" {
dsis::IInferCustomProcessor*
CreateInferServerCustomProcess(const char* config, uint32_t configLen)
{
    return new NvInferServerCustomProcess();
}
}


/** parse all bounding boxes from inferenced tensors
 */
NvDsInferStatus
NvInferServerCustomProcess::parseObjBbox(
    const dsis::IBatchArray* tensors, std::vector<NvDsInferObjectDetectionInfo>& outObjs,
    uint32_t batchIdx)
{
    
    const NvDsInferNetworkInfo networkInfo = {kInferWidth, kInferHeight, kInferChannel};
    NvDsInferParseDetectionParams params = {(uint32_t)kClassLabels.size()};
    params.perClassPreclusterThreshold.resize(kClassLabels.size(), kScoreThreshold);
    /** class 0 is backgroup, set a confidence value larger than 1.0f to
     *  ignore background in NvDsInferParseCustomFasterRCNN
     */
    params.perClassPreclusterThreshold[0] = 1.1f;
    std::vector<NvDsInferLayerInfo> outputLayers;
    for (uint32_t iBuf = 0; iBuf < tensors->getSize(); ++iBuf) {
        const dsis::IBatchBuffer* out = tensors->getBuffer(iBuf);
        INFER_ASSERT(out);
        NvDsInferLayerInfo capiLayer = {FLOAT, {{0, {0}, 0}}};
        const dsis::InferBufferDescription& outDesc = out->getBufDesc();
        capiLayer.dataType =
            (outDesc.dataType == dsis::InferDataType::kFp32 ? FLOAT : (NvDsInferDataType)-1);
        capiLayer.layerName = outDesc.name.c_str();
        // Triton output tensor shape is full dimensions NCHW, batch-size is dims.d[0]
        INFER_ASSERT(out->getBatchSize() == 0);
        // NvDsInferLayerInfo CAPI requires CHW rather than NCHW
        capiLayer.inferDims.numDims = outDesc.dims.numDims - 1;
        uint32_t batchSize = outDesc.dims.d[0];
        // std::cout << "batchSize: " << batchSize << std::endl;
        // std::cout << "batchIdx: " << batchIdx << std::endl;
        INFER_ASSERT(batchIdx < batchSize);
        std::copy(outDesc.dims.d + 1, outDesc.dims.d + outDesc.dims.numDims, capiLayer.inferDims.d);
        uint32_t perBatchSize = std::accumulate(
            capiLayer.inferDims.d, capiLayer.inferDims.d + capiLayer.inferDims.numDims, 1,
            [](int s, int i) { return s * i; });
        capiLayer.inferDims.numElements=perBatchSize;
        /// sample model's data type is kFp32


        capiLayer.buffer = (float*)out->getBufPtr(0) + perBatchSize * batchIdx;
        outputLayers.push_back(capiLayer);
    }

    // call ds nvinfer custom parse to get all objects
    std::vector<NvDsInferObjectDetectionInfo> detectObjs;
    // detectObjs=NvDsInferParseCustomAutoML(outputLayers, networkInfo, params, detectObjs);
    if (!ParserCustomAutoML(outputLayers, networkInfo, params, detectObjs)) {
        
        LOG_ERROR("DS-triton inferserver custom tensor parse failed");
        return NVDSINFER_CUSTOM_LIB_FAILED;
    }

    // filter overlapped bounding boxes via Non-maximum suppression
    detectObjs = nms(detectObjs);
    outObjs.swap(detectObjs);
    return NVDSINFER_SUCCESS;
}

/**
 * attach bounding boxes into NvDsBatchMeta and NvDsFrameMeta
 */
NvDsInferStatus
NvInferServerCustomProcess::attachObjMetas(
    const dsis::IOptions* inOptions, const std::vector<NvDsInferObjectDetectionInfo>& detectObjs,
    uint32_t batchIdx)
{
    INFER_ASSERT(inOptions);
    GstBuffer* gstBuf = nullptr;
    NvDsBatchMeta* batchMeta = nullptr;
    std::vector<NvDsFrameMeta*> frameMetaList;
    NvBufSurface* bufSurf = nullptr;
    std::vector<NvBufSurfaceParams*> surfParamsList;
    int64_t unique_id = 0;

    // get GstBuffer
    if (inOptions->hasValue(OPTION_NVDS_GST_BUFFER)) {
        INFER_ASSERT(inOptions->getObj(OPTION_NVDS_GST_BUFFER, gstBuf) == NVDSINFER_SUCCESS);
    }
    INFER_ASSERT(gstBuf);

    // get NvBufSurface
    if (inOptions->hasValue(OPTION_NVDS_BUF_SURFACE)) {
        INFER_ASSERT(inOptions->getObj(OPTION_NVDS_BUF_SURFACE, bufSurf) == NVDSINFER_SUCCESS);
    }
    INFER_ASSERT(bufSurf);

    // get NvDsBatchMeta
    if (inOptions->hasValue(OPTION_NVDS_BATCH_META)) {
        INFER_ASSERT(inOptions->getObj(OPTION_NVDS_BATCH_META, batchMeta) == NVDSINFER_SUCCESS);
    }
    INFER_ASSERT(batchMeta);

    // get all frame meta list into vector<NvDsFrameMeta*>
    if (inOptions->hasValue(OPTION_NVDS_FRAME_META_LIST)) {
        INFER_ASSERT(
            inOptions->getValueArray(OPTION_NVDS_FRAME_META_LIST, frameMetaList) ==
            NVDSINFER_SUCCESS);
    }
    INFER_ASSERT(batchIdx < frameMetaList.size());  // batchsize

    // get unique_id
    if (inOptions->hasValue(OPTION_NVDS_UNIQUE_ID)) {
        INFER_ASSERT(inOptions->getInt(OPTION_NVDS_UNIQUE_ID, unique_id) == NVDSINFER_SUCCESS);
    }

    // get all surface params list into vector<NvBufSurfaceParams*>
    if (inOptions->hasValue(OPTION_NVDS_BUF_SURFACE_PARAMS_LIST)) {
        INFER_ASSERT(
            inOptions->getValueArray(OPTION_NVDS_BUF_SURFACE_PARAMS_LIST, surfParamsList) ==
            NVDSINFER_SUCCESS);
    }
    INFER_ASSERT(batchIdx < surfParamsList.size());  // batchsize


    // attach object's boundingbox
    for (const auto& obj : detectObjs) {

        NvDsObjectMeta* objMeta = nvds_acquire_obj_meta_from_pool(batchMeta);
        objMeta->unique_component_id = unique_id;
        objMeta->confidence = obj.detectionConfidence;

        /* This is an untracked object. Set tracking_id to -1. */
        objMeta->object_id = UNTRACKED_OBJECT_ID;
        objMeta->class_id = obj.classId;

        NvOSD_RectParams& rect_params = objMeta->rect_params;
        // NvOSD_TextParams& text_params = objMeta->text_params;
        
        float orig_width=static_cast<double>(surfParamsList[batchIdx]->width);
        float orig_height=static_cast<double>(surfParamsList[batchIdx]->height);

        
        float width = obj.width * (orig_width/kInferWidth);
        float height = obj.height * (orig_height/kInferHeight);
        /* Assign bounding box coordinates. */
        // strange output from triton automl onnx model
        rect_params.left = (obj.left -(obj.width/2)) * (orig_width/kInferWidth);
        rect_params.top = (obj.top-(obj.height/2)) * (orig_height/kInferHeight);
        rect_params.width = width;
        rect_params.height = height;



        // /* Border of width 3. */
        // rect_params.border_width = 3;
        // rect_params.has_bg_color = 0;
        // rect_params.border_color = (NvOSD_ColorParams){1, 0, 0, 1};

        // /* display_text requires heap allocated memory. */
        // if (obj.classId < kClassLabels.size()) {
        //     text_params.display_text = g_strdup(kClassLabels[obj.classId].c_str());
        //     strncpy(objMeta->obj_label, kClassLabels[obj.classId].c_str(), MAX_LABEL_SIZE - 1);
        //     objMeta->obj_label[MAX_LABEL_SIZE - 1] = 0;
        // }
        // /* Display text above the left top corner of the object. */
        // text_params.x_offset = rect_params.left;
        // text_params.y_offset = rect_params.top - 10;
        // /* Set black background for the text. */
        // text_params.set_bg_clr = 1;
        // text_params.text_bg_clr = (NvOSD_ColorParams){0, 0, 0, 1};
        // /* Font face, size and color. */
        // text_params.font_params.font_name = (gchar*)"Serif";
        // text_params.font_params.font_size = 11;
        // text_params.font_params.font_color = (NvOSD_ColorParams){1, 1, 1, 1};

        nvds_acquire_meta_lock(batchMeta);
        nvds_add_obj_meta_to_frame(frameMetaList[batchIdx], objMeta, NULL);
        frameMetaList[batchIdx]->bInferDone = TRUE;
        nvds_release_meta_lock(batchMeta);
    }

    return NVDSINFER_SUCCESS;
}
