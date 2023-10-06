ARG CUDA_VERSION


FROM nvcr.io/nvidia/deepstream:6.3-triton-multiarch

RUN apt-get update


RUN mkdir -p /opt/nvidia/deepstream/deepstream-6.3/sources/apps/deepstream_python_apps && git clone https://github.com/NVIDIA-AI-IOT/deepstream_python_apps.git /opt/nvidia/deepstream/deepstream-6.3/sources/apps/deepstream_python_apps
ADD user_additional_install_runtime.sh /opt

# Execute local file
RUN chmod +x /opt/user_additional_install_runtime.sh && /opt/user_additional_install_runtime.sh

# Add local file to Docker image
ADD user_deepstream_python_apps_install.sh /opt

# Execute local file
RUN chmod +x /opt/user_deepstream_python_apps_install.sh && /opt/user_deepstream_python_apps_install.sh --version 1.1.8

WORKDIR /opt/nvidia/deepstream/deepstream
# COPY ./deepstream_python_apps ./deepstream_python_apps
COPY --chmod=777 . .

WORKDIR /opt/nvidia/deepstream/deepstream-6.3
# COPY ./deepstream_python_apps ./deepstream_python_apps
COPY --chmod=777 . .




ADD requirements.txt . 
RUN pip3 install --no-cache-dir -r requirements.txt

WORKDIR /

COPY shared /shared


COPY deepstream/Dataset ./Dataset
COPY deepstream/events_schema ./events_schema
COPY deepstream/src ./src

CMD ["/bin/bash"]
