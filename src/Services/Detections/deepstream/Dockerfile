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
ARG CUDA_VERSION=\
    $(if [ "${TARGETPLATFORM}" = "linux/amd64" ]; then \
        echo "12.1"; \
    elif ["${TARGETPLATFORM}" = "linux/arm64"] then \
        echo "11.4"; \
    fi)

# Create binaries to run yolo in deepstream

RUN git clone https://github.com/Javipercor/DeepStream-Yolo.git Deepstream-Yolo && cd Deepstream-Yolo && run CUDA_VER=${CUDA_VERSION} make -C nvdsinfer_custom_impl_Yolo
#if not present
#RUN mkdir /src/model/nvdsinfer_custom_impl_Yolo/layers


COPY nvdsinfer_custom_impl_Yolo/*.o /src/model/nvdsinfer_custom_impl_Yolo/
COPY nvdsinfer_custom_impl_Yolo/*.so /src/model/nvdsinfer_custom_impl_Yolo/
COPY nvdsinfer_custom_impl_Yolo/layers/*.o /src/model/nvdsinfer_custom_impl_Yolo/layers/

RUN rm -rf /Deepstream-Yolo

# WORKDIR /opt/nvidia/deepstream/deepstream
# # COPY ./deepstream_python_apps ./deepstream_python_apps
# COPY --chmod=777 . .

# WORKDIR /opt/nvidia/deepstream/deepstream-6.3
# # COPY ./deepstream_python_apps ./deepstream_python_apps
# COPY --chmod=777 . .




ADD deepstream/requirements.txt . 
RUN pip3 install --no-cache-dir -r requirements.txt

WORKDIR /

COPY shared /shared


COPY deepstream/Dataset ./Dataset
COPY deepstream/events_schema ./events_schema
COPY deepstream/src ./src

# RUN cd src/models/model_yolo_automl/ && CUDA_VER=${CUDA_VERSION} make -C nvdsinfer_custom_impl_YoloAutoML
# RUN cd /

# ENTRYPOINT ["sh", "-c", "cd /src && python3 init_samples.py no /frames"]
ENTRYPOINT ["sh", "-c", "cd /src && python3 init_native_yolo.py no /frames"]
# ENTRYPOINT ["sh", "-c", "cd /src && python3 init_samples.py file:///Dataset/sample_720p.mp4 local frames"]

# CMD ["/bin/bash"]
