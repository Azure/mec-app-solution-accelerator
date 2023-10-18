#!/usr/bin/env python3

################################################################################
# SPDX-FileCopyrightText: Copyright (c) 2020-2022 NVIDIA CORPORATION & AFFILIATES. All rights reserved.
# SPDX-License-Identifier: Apache-2.0
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
# http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
################################################################################

import sys

sys.path.append('../')
import gi


gi.require_version('Gst', '1.0')
from gi.repository import GLib, Gst
from ctypes import *
import time
import sys
import math

import numpy as np
import pyds
import cv2
import os
import os.path
from os import path
import json
import avro.schema
from avro_json_serializer import AvroJsonSerializer
from dapr.clients import DaprClient

import uuid
import base64
import logging
import itertools

if not os.path.exists('deepstream'):
    os.symlink('/opt/nvidia/deepstream/deepstream-6.3', 'deepstream')
from deepstream.sources.apps.deepstream_python_apps.apps.common.is_aarch_64 import is_aarch64
from deepstream.sources.apps.deepstream_python_apps.apps.common.bus_call import bus_call
from deepstream.sources.apps.deepstream_python_apps.apps.common.FPS import PERF_DATA
global no_display
no_display = True



perf_data = None
frame_count = {}
saved_count = {}
global PGIE_CLASS_ID_VEHICLE
PGIE_CLASS_ID_VEHICLE = 2
global PGIE_CLASS_ID_PERSON
PGIE_CLASS_ID_PERSON = 0


PGIE_CLASS_ID_VEHICLE = 2
PGIE_CLASS_ID_BICYCLE = 1
PGIE_CLASS_ID_PERSON = 0
PGIE_CLASS_ID_ROADSIGN = 3

TILED_OUTPUT_WIDTH = 1920
TILED_OUTPUT_HEIGHT = 1080


MIN_CONFIDENCE = 0.3
MAX_CONFIDENCE = 0.4
def PublishEvent(pubsub_name: str, topic_name: str, data: json):
    with DaprClient() as client:
        resp = client.publish_event(pubsub_name=pubsub_name, topic_name=topic_name, data=data, data_content_type='application/json')

def tiler_sink_pad_buffer_probe(pad, info, u_data):
    timestamp_init=int(time.time()*1000)
    
    if debug != 'local':
        import shared.minio_utils as minio
        endpoint = os.getenv('MINIOURL')
        bucket = 'images'
        minioClient = minio.MinIOClient(endpoint, 'minio', 'minio123')
    
    path='../events_schema/detections.avro'
    
    
    frame_number = 0
    num_rects = 0
    gst_buffer = info.get_buffer()
    schema = avro.schema.Parse(open(path, "rb").read())
    serializer = AvroJsonSerializer(schema)
    if not gst_buffer:
        print("Unable to get GstBuffer ")
        return

    # Retrieve batch metadata from the gst_buffer
    # Note that pyds.gst_buffer_get_nvds_batch_meta() expects the
    # C address of gst_buffer as input, which is obtained with hash(gst_buffer)
    batch_meta = pyds.gst_buffer_get_nvds_batch_meta(hash(gst_buffer))
    
    l_frame = batch_meta.frame_meta_list
    
    while l_frame is not None:
        
        try:
            # Note that l_frame.data needs a cast to pyds.NvDsFrameMeta
            # The casting is done by pyds.NvDsFrameMeta.cast()
            # The casting also keeps ownership of the underlying memory
            # in the C code, so the Python garbage collector will leave
            # it alone.
            frame_meta = pyds.NvDsFrameMeta.cast(l_frame.data)
        except StopIteration:
            print("Unable to get frame meta")
            break

        frame_number = frame_meta.frame_num
        l_obj = frame_meta.obj_meta_list
        num_rects = frame_meta.num_obj_meta
        is_first_obj = True
        
        
        obj_counter = {
            PGIE_CLASS_ID_VEHICLE: 0,
            PGIE_CLASS_ID_PERSON: 0,
            PGIE_CLASS_ID_BICYCLE: 0,
            PGIE_CLASS_ID_ROADSIGN: 0
        }
        obj_list = []
        obj_json = {}
        timestamp=int(time.time()*1000)
        data = { "SourceId":"source_id",
        "UrlVideoEncoded": "1.0",
        "Frame": 'frame_'+str(frame_number)+'.jpg',
        "EventName": "ObjectDetection",
        "OriginModule": "Ai inference detection",
        "Information": "Test message",
        "EveryTime": int(timestamp),
        "Classes": [],
        "time_trace":[]
        }

        BoundingBoxes=[]
        
        frame_copy=None
        print(frame_number,l_obj,num_rects)
        while l_obj is not None:
            try:
                # Casting l_obj.data to pyds.NvDsObjectMeta
                obj_meta = pyds.NvDsObjectMeta.cast(l_obj.data)

            except StopIteration:
                print("Casting problem")
                break
            
            obj_dict = {
                "class_id": obj_meta.class_id,
                "class_label": obj_meta.obj_label,
                "confidence": obj_meta.confidence,
                "left": int(obj_meta.rect_params.left),
                "top": int(obj_meta.rect_params.top),
                "width": int(obj_meta.rect_params.width),
                "height": int(obj_meta.rect_params.height)
            }

            print('obj_meta')
            print(obj_meta.class_id)
            print(obj_meta.obj_label)
            
            xmin = int(obj_meta.rect_params.left)
            ymin = int(obj_meta.rect_params.top)
            xmax = int(obj_meta.rect_params.left + obj_meta.rect_params.width)
            ymax = int(obj_meta.rect_params.top + obj_meta.rect_params.height)
            obj_list.append(obj_dict)
            BoundingBoxes.append({"x": xmin, "y":ymin})
            BoundingBoxes.append({"x": xmin, "y":ymax})
            BoundingBoxes.append({"x": xmax, "y":ymin})
            BoundingBoxes.append({"x": xmax, "y":ymax})
            data["Classes"].append({"EventType": obj_meta.obj_label, "Confidence":obj_meta.confidence, "BoundingBoxes": BoundingBoxes})
            # obj_counter[obj_meta.class_id] += 1
            
            if True:
                if is_first_obj:
                    is_first_obj = False
                    # Getting Image data using nvbufsurface
                    # the input should be address of buffer and batch_id
                    n_frame = pyds.get_nvds_buf_surface(hash(gst_buffer), frame_meta.batch_id)
                    # n_frame = draw_bounding_boxes(n_frame, obj_meta, obj_meta.confidence)
                    # save output json
                    
                    # convert python array into numpy array format in the copy mode.
                    frame_copy = np.array(n_frame, copy=True, order='C')
                    # convert the array into cv2 default color format
                    frame_copy = cv2.cvtColor(frame_copy, cv2.COLOR_RGBA2BGRA)
                    if is_aarch64(): # If Jetson, since the buffer is mapped to CPU for retrieval, it must also be unmapped 
                        pyds.unmap_nvds_buf_surface(hash(gst_buffer), frame_meta.batch_id) # The unmap call should be made after operations with the original array are complete.
                                                                                            #  The original array cannot be accessed after this call.

            try:
                l_obj = l_obj.next
            except StopIteration:
                break
        if frame_copy is not None:
            obj_json["frame_id"] = 'frame_'+str(frame_number)+'.jpg'
            obj_json['Detections'] = obj_list
            

            print("Frame Number=", frame_number, "Number of Objects=", num_rects, "Vehicle_count=",
                obj_counter[PGIE_CLASS_ID_VEHICLE], "Person_count=", obj_counter[PGIE_CLASS_ID_PERSON])
            # update frame rate through this probe
            stream_index = "stream{0}".format(frame_meta.pad_index)
            global perf_data
            perf_data.update_fps(stream_index)
            #UNCOMMENT for local testing purposes
            # if debug == 'local':
            #     img_path = "{}/stream_{}/frame_{}.jpg".format(folder_name, frame_meta.pad_index, frame_number)
            #     cv2.imwrite(img_path, frame_copy)
            #     json_path = "{}/stream_{}/detections_{}.json".format(folder_name, frame_meta.pad_index, frame_number)
            #     with open(json_path, 'w') as f:
            #         json.dump(data, f)
            # img_encode = cv2.imencode(".jpg", frame_copy)[1]
            # resized_img_bytes = img_encode.tobytes()
            # bytes_string = base64.standard_b64encode(resized_img_bytes).decode()
            saved_count["stream_{}".format(frame_meta.pad_index)] += 1
            image_id = uuid.uuid4()
            image_id_str = str(image_id)
            logging.info(f'Image uploaded with ID: {image_id}')
            if debug != 'local':
                minioClient.upload_bytes(bucket, image_id_str+'.jpg', resized_img_bytes)
            
            try:
                l_frame = l_frame.next
            except StopIteration:
                break
            time_trace={"stepStart": timestamp_init, "stepEnd":int(time.time()*1000), "stepName": "deepstream"}
            data['time_trace'].append(time_trace)
            json_str = serializer.to_json(data)

            if debug != 'local':
                PublishEvent(pubsub_name="pubsub", topic_name="newDetection", data=json_str)

            logging.info(f'Event published')
        else:
            print('No detections found')
            logging.info('No detections found')

        return Gst.PadProbeReturn.OK
        



def cb_newpad(decodebin, decoder_src_pad, data):
    print("In cb_newpad\n")
    caps = decoder_src_pad.get_current_caps()
    gststruct = caps.get_structure(0)
    gstname = gststruct.get_name()
    source_bin = data
    features = caps.get_features(0)

    # Need to check if the pad created by the decodebin is for video and not
    # audio.
    if (gstname.find("video") != -1):
        # Link the decodebin pad only if decodebin has picked nvidia
        # decoder plugin nvdec_*. We do this by checking if the pad caps contain
        # NVMM memory features.
        if features.contains("memory:NVMM"):
            # Get the source bin ghost pad
            bin_ghost_pad = source_bin.get_static_pad("src")
            if not bin_ghost_pad.set_target(decoder_src_pad):
                sys.stderr.write("Failed to link decoder src pad to source bin ghost pad\n")
        else:
            sys.stderr.write(" Error: Decodebin did not pick nvidia decoder plugin.\n")


def decodebin_child_added(child_proxy, Object, name, user_data):
    print("Decodebin child added:", name, "\n")
    if name.find("decodebin") != -1:
        Object.connect("child-added", decodebin_child_added, user_data)

    if not is_aarch64() and name.find("nvv4l2decoder") != -1:
        # Use CUDA unified memory in the pipeline so frames
        # can be easily accessed on CPU in Python.
        Object.set_property("cudadec-memtype", 2)

    if "source" in name:
        source_element = child_proxy.get_by_name("source")
        if source_element.find_property('drop-on-latency') != None:
            Object.set_property("drop-on-latency", True)

def create_source_bin(index, uri):
    print("Creating source bin")

    # Create a source GstBin to abstract this bin's content from the rest of the
    # pipeline
    bin_name = "source-bin-%02d" % index
    print(bin_name)
    nbin = Gst.Bin.new(bin_name)
    if not nbin:
        sys.stderr.write(" Unable to create source bin \n")

    # Source element for reading from the uri.
    # We will use decodebin and let it figure out the container format of the
    # stream and the codec and plug the appropriate demux and decode plugins.
    uri_decode_bin = Gst.ElementFactory.make("uridecodebin", "uri-decode-bin")
    if not uri_decode_bin:
        sys.stderr.write(" Unable to create uri decode bin \n")
    # We set the input uri to the source element
    uri_decode_bin.set_property("uri", uri)
    # Connect to the "pad-added" signal of the decodebin which generates a
    # callback once a new pad for raw data has beed created by the decodebin
    uri_decode_bin.connect("pad-added", cb_newpad, nbin)
    uri_decode_bin.connect("child-added", decodebin_child_added, nbin)

    # We need to create a ghost pad for the source bin which will act as a proxy
    # for the video decoder src pad. The ghost pad will not have a target right
    # now. Once the decode bin creates the video decoder and generates the
    # cb_newpad callback, we will set the ghost pad target to the video decoder
    # src pad.
    Gst.Bin.add(nbin, uri_decode_bin)
    bin_pad = nbin.add_pad(Gst.GhostPad.new_no_target("src", Gst.PadDirection.SRC))
    if not bin_pad:
        sys.stderr.write(" Failed to add ghost pad in source bin \n")
        return None
    return nbin

def main(args):
    # Check input arguments
    logging.basicConfig(level=logging.INFO)
    logging.getLogger('is_aarch64').setLevel(logging.INFO)
    logging.getLogger('bus_call').setLevel(logging.INFO)
    logging.getLogger('PERF_DATA').setLevel(logging.INFO)
    logging.getLogger('GST').setLevel(logging.INFO)
    logging.getLogger('GLib').setLevel(logging.INFO)
    logging.info(int(time.time()*1000))
    # os.getenv("URLS")
    if len(args) < 2:
        sys.stderr.write("usage: %s <uri1> [uri2] ... [uriN] <folder to save frames>\n" % args[0])
        sys.exit(1)

    global folder_name
    folder_name = args[-1]
    global debug
    debug = args[-2]
    # if path.exists(folder_name):
    #     sys.stderr.write("The output folder %s already exists. Please remove it first.\n" % folder_name)
    #     sys.exit(1)

    # Standard GStreamer initialization
    Gst.init(None)
    if debug != 'local':
        logging.info(args)
        feeds = os.getenv("FEEDS").split(',')
        input_info=[]
        input_info.append([args[0]])
        input_info.append(feeds)
        input_info.append([args[-2]])
        input_info.append([args[-1]])
        
        flattened_input_info = list(itertools.chain.from_iterable(input_info))
        logging.info(flattened_input_info)
        args=flattened_input_info
        logging.info(args)
    global perf_data
    perf_data = PERF_DATA(len(args) - 3)
    number_sources = len(args) - 3
    # Create gstreamer elements */
    # Create Pipeline element that will form a connection of other elements
    print("Creating Pipeline \n ")
    pipeline = Gst.Pipeline()
    is_live = False

    if not pipeline:
        sys.stderr.write(" Unable to create Pipeline \n")
    print("Creating streamux \n ")

    # Create nvstreammux instance to form batches from one or more sources.
    streammux = Gst.ElementFactory.make("nvstreammux", "Stream-muxer")
    if not streammux:
        sys.stderr.write(" Unable to create NvStreamMux \n")

    pipeline.add(streammux)
    for i in range(number_sources):
        folder_path = folder_name + "/stream_" + str(i)
        if not os.path.exists(folder_path) and debug == 'local':
            os.makedirs(folder_path)
        elif debug != 'local':
            print("No Local Debugging.")
        else:
            print("Folder already exists. Continuing...")
            print("Detections will be saved in ", folder_path)
        frame_count["stream_" + str(i)] = 0
        saved_count["stream_" + str(i)] = 0
        print("Creating source_bin ", i, " \n ")
        uri_name = args[i + 1]
        if uri_name[0]=='/':
            uri_name = 'file://'+uri_name
        if uri_name.find("rtsp://") == 0:
            is_live = True
        source_bin = create_source_bin(i, uri_name)
        if not source_bin:
            sys.stderr.write("Unable to create source bin \n")
        pipeline.add(source_bin)
        padname = "sink_%u" % i
        sinkpad = streammux.get_request_pad(padname)
        if not sinkpad:
            sys.stderr.write("Unable to create sink pad bin \n")
        srcpad = source_bin.get_static_pad("src")
        if not srcpad:
            sys.stderr.write("Unable to create src pad bin \n")
        srcpad.link(sinkpad)
    print("Creating Pgie \n ")
    pgie = Gst.ElementFactory.make("nvinfer", "primary-inference")
    if not pgie:
        sys.stderr.write(" Unable to create pgie \n")
    # Add nvvidconv1 and filter1 to convert the frames to RGBA
    # which is easier to work with in Python.
    print("Creating nvvidconv1 \n ")
    nvvidconv1 = Gst.ElementFactory.make("nvvideoconvert", "convertor1")
    if not nvvidconv1:
        sys.stderr.write(" Unable to create nvvidconv1 \n")
    print("Creating filter1 \n ")
    caps1 = Gst.Caps.from_string("video/x-raw(memory:NVMM), format=RGBA")
    filter1 = Gst.ElementFactory.make("capsfilter", "filter1")
    if not filter1:
        sys.stderr.write(" Unable to get the caps filter1 \n")
    filter1.set_property("caps", caps1)
    print("Creating tiler \n ")
    tiler = Gst.ElementFactory.make("nvmultistreamtiler", "nvtiler")
    if not tiler:
        sys.stderr.write(" Unable to create tiler \n")
    print("Creating nvvidconv \n ")
    nvvidconv = Gst.ElementFactory.make("nvvideoconvert", "convertor")
    if not nvvidconv:
        sys.stderr.write(" Unable to create nvvidconv \n")
    print("Creating nvosd \n ")
    nvosd = Gst.ElementFactory.make("nvdsosd", "onscreendisplay")
    if not nvosd:
        sys.stderr.write(" Unable to create nvosd \n")
    if no_display:
        print("Creating Fakesink \n")
        sink = Gst.ElementFactory.make("fakesink", "fakesink")
        sink.set_property('enable-last-sample', 0)
        sink.set_property('sync', 0)
    else:
        if is_aarch64():
            print("Creating nv3dsink \n")
            sink = Gst.ElementFactory.make("nv3dsink", "nv3d-sink")
            if not sink:
                sys.stderr.write(" Unable to create nv3dsink \n")
        else:
            print("Creating EGLSink \n")
            sink = Gst.ElementFactory.make("nveglglessink", "nvvideo-renderer")
            if not sink:
                sys.stderr.write(" Unable to create egl sink \n")

    if is_live:
        print("Atleast one of the sources is live")
        streammux.set_property('live-source', 1)

    streammux.set_property('width', 1920)
    streammux.set_property('height', 1080)
    streammux.set_property('batch-size', number_sources)
    streammux.set_property('batched-push-timeout', 4000000)
    # pgie.set_property('config-file-path', "dstest_imagedata_config.txt")
    pgie.set_property('config-file-path', "config_infer_primary_yoloV5.txt")
    pgie_batch_size = pgie.get_property("batch-size")
    if (pgie_batch_size != number_sources):
        print("WARNING: Overriding infer-config batch-size", pgie_batch_size, " with number of sources ",
              number_sources, " \n")
        pgie.set_property("batch-size", number_sources)
    tiler_rows = int(math.sqrt(number_sources))
    tiler_columns = int(math.ceil((1.0 * number_sources) / tiler_rows))
    tiler.set_property("rows", tiler_rows)
    tiler.set_property("columns", tiler_columns)
    tiler.set_property("width", TILED_OUTPUT_WIDTH)
    tiler.set_property("height", TILED_OUTPUT_HEIGHT)

    sink.set_property("sync", 0)
    sink.set_property("qos", 0)

    if not is_aarch64():
        # Use CUDA unified memory in the pipeline so frames
        # can be easily accessed on CPU in Python.
        mem_type = int(pyds.NVBUF_MEM_CUDA_UNIFIED)
        streammux.set_property("nvbuf-memory-type", mem_type)
        nvvidconv.set_property("nvbuf-memory-type", mem_type)
        nvvidconv1.set_property("nvbuf-memory-type", mem_type)
        tiler.set_property("nvbuf-memory-type", mem_type)

    print("Adding elements to Pipeline \n")
    pipeline.add(pgie)
    pipeline.add(tiler)
    pipeline.add(nvvidconv)
    pipeline.add(filter1)
    pipeline.add(nvvidconv1)
    pipeline.add(nvosd)
    pipeline.add(sink)

    print("Linking elements in the Pipeline \n")
    streammux.link(pgie)
    pgie.link(nvvidconv1)
    nvvidconv1.link(filter1)
    filter1.link(tiler)
    tiler.link(nvvidconv)
    nvvidconv.link(nvosd)
    nvosd.link(sink)

    # create an event loop and feed gstreamer bus mesages to it
    loop = GLib.MainLoop()
    bus = pipeline.get_bus()
    bus.add_signal_watch()
    bus.connect("message", bus_call, loop)

    tiler_sink_pad = tiler.get_static_pad("sink")
    if not tiler_sink_pad:
        sys.stderr.write(" Unable to get src pad \n")
    else:
        tiler_sink_pad.add_probe(Gst.PadProbeType.BUFFER, tiler_sink_pad_buffer_probe, 0)
        # perf callback function to print fps every 5 sec
        GLib.timeout_add(5000, perf_data.perf_print_callback)

    # List the sources
    print("Now playing...")
    for i, source in enumerate(args[:-2]):
        if i != 0:
            print(i, ": ", source)

    print("Starting pipeline \n")
    # start play back and listed to events		
    pipeline.set_state(Gst.State.PLAYING)
    try:
        loop.run()
    except:
        pass
    # cleanup
    print("Exiting app\n")
    pipeline.set_state(Gst.State.NULL)


if __name__ == '__main__':
    
    sys.exit(main(sys.argv))
