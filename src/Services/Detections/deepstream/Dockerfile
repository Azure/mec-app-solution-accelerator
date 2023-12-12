ARG CUDA_VERSION



FROM nvcr.io/nvidia/deepstream:6.3-triton-multiarch

# BEGIN: get-arch

ARG TARGETPLATFORM

# END: get-arch
ARG VERSION=1.1.8

RUN apt-get update


RUN mkdir -p /opt/nvidia/deepstream/deepstream-6.3/sources/apps/deepstream_python_apps && git clone https://github.com/NVIDIA-AI-IOT/deepstream_python_apps.git /opt/nvidia/deepstream/deepstream-6.3/sources/apps/deepstream_python_apps
ADD deepstream/user_additional_install_runtime.sh /opt

# Execute local file
RUN chmod +x /opt/user_additional_install_runtime.sh && /opt/user_additional_install_runtime.sh

# Add local file to Docker image
ADD deepstream/user_deepstream_python_apps_install.sh /opt

# Execute local file
RUN chmod +x /opt/user_deepstream_python_apps_install.sh && /opt/user_deepstream_python_apps_install.sh --version ${VERSION} --arch ${TARGETPLATFORM}

ARG CUDA_VERSION

# Set the value of another variable based on the value of CUDA_VERSION
RUN echo $TARGETPLATFORM
WORKDIR /

# -------------- To set up for running YOLO natively with deepstream Yolo --------------------
# RUN if [ ${TARGETPLATFORM} = linux/amd64 ]; then \
#         git clone https://github.com/Javipercor/DeepStream-Yolo.git /Deepstream-Yolo && cd /Deepstream-Yolo &&  CUDA_VER=12.1 make --always-make -C nvdsinfer_custom_impl_Yolo  &&\
#         mkdir -p /src/model/nvdsinfer_custom_impl_Yolo/layers && \
#         mv nvdsinfer_custom_impl_Yolo/*.o /src/model/nvdsinfer_custom_impl_Yolo/ && \
#         mv nvdsinfer_custom_impl_Yolo/*.so /src/model/nvdsinfer_custom_impl_Yolo/ && \
#         mv nvdsinfer_custom_impl_Yolo/layers/*.o /src/model/nvdsinfer_custom_impl_Yolo/layers/; \
#     elif [ ${TARGETPLATFORM} = linux/arm64 ]; then \
#         git clone https://github.com/Javipercor/DeepStream-Yolo.git Deepstream-Yolo && cd Deepstream-Yolo && CUDA_VER=11.4 make --always-make -C nvdsinfer_custom_impl_Yolo; \
#     fi 

# RUN rm -rf /Deepstream-Yolo






ADD deepstream/requirements.txt . 
RUN pip3 install --no-cache-dir -r requirements.txt



COPY shared /shared


COPY deepstream/Dataset ./Dataset
COPY deepstream/events_schema ./events_schema
COPY deepstream/src ./src


# -------------- To set up for running Automl yolo onxx ; partially reusable for other onnx models --------------------
RUN if [ ${TARGETPLATFORM} = linux/amd64 ]; then \
        cd src/models/model_yolo_automl/ && CUDA_VER=12.1 make --always-make -C nvdsinfer_custom_impl_YoloAutoML; \
    elif [ ${TARGETPLATFORM} = linux/arm64 ]; then \
        cd src/models/model_yolo_automl/ && CUDA_VER=11.4 make --always-make -C nvdsinfer_custom_impl_YoloAutoML; \
    fi 
RUN cd /


# ----------------- TO run samples -----------------
# ENTRYPOINT ["sh", "-c", "cd /src && python3 init_samples.py no /frames"]

# ----------------- TO run  YOLO natively with deepstream Yolo-----------------
# ENTRYPOINT ["sh", "-c", "cd /src && python3 init_native_yolo.py no /frames"]

# ----------------- TO run  Automl yolo onxx-----------------
ENTRYPOINT ["sh", "-c", "cd /src && python3 init_onnx_yolo_automl.py no /frames"]


# ----------------- TO locally without docker compose nor kubernetes -----------------
# ENTRYPOINT ["sh", "-c", "cd /src && python3 init_samples.py file:///Dataset/sample_720p.mp4 local frames"]

# CMD ["/bin/bash"]
