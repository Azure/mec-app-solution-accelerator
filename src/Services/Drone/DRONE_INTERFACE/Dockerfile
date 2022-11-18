FROM osrf/ros:humble-desktop

# ENTRYPOINT ["tail", "-f", "/dev/null"]

# Setup GUI
ENV DISPLAY=host.docker.internal:0.0

# Install additional packages
RUN sudo apt update \
    && apt install -y pip \
    && apt install -y pcmanfm lxtask \
    # Clean up
    && apt autoremove -y \
    && apt clean -y \
    && rm -rf /var/lib/apt/lists/*

COPY requirements.txt .
RUN pip install --disable-pip-version-check --no-cache-dir -U -r requirements.txt \
    && rm requirements.txt

# Setup project
ARG PROJECT_NAME=AIRPORTS_MVP
RUN mkdir ${PROJECT_NAME}

# ROS setup
## Create a ROS workspace
ARG WORKSPACE_NAME=AIRPORTS_MVP_WS
ARG WORKSPACE_PATH=${PROJECT_NAME}/ROS/${WORKSPACE_NAME}
SHELL ["/bin/bash", "-c"]

RUN mkdir -p ${WORKSPACE_PATH}/src

## Create ROS packages
### Sensor package
ARG PACKAGE_NAME=sensors_pkg
ARG PACKAGE_PATH=${WORKSPACE_PATH}/src/${PACKAGE_NAME}

RUN source opt/ros/humble/setup.bash \
    && cd ${WORKSPACE_PATH}/src \
    && ros2 pkg create --build-type ament_python ${PACKAGE_NAME}

COPY ${PACKAGE_PATH}/setup.py ${PACKAGE_PATH}/setup.py
COPY ${PACKAGE_PATH}/package.xml ${PACKAGE_PATH}/package.xml
COPY ${PACKAGE_PATH}/${PACKAGE_NAME} ${PACKAGE_PATH}/${PACKAGE_NAME}


### Unity TCP endpoint package
ARG PACKAGE_NAME=unity_tcp_endpoint
ARG PACKAGE_PATH=${WORKSPACE_PATH}/src/${PACKAGE_NAME}

RUN source opt/ros/humble/setup.bash \
    && cd ${WORKSPACE_PATH}/src \
    && ros2 pkg create --build-type ament_python ${PACKAGE_NAME}

COPY ${PACKAGE_PATH}/setup.py ${PACKAGE_PATH}/setup.py
COPY ${PACKAGE_PATH}/package.xml ${PACKAGE_PATH}/package.xml
COPY ${PACKAGE_PATH}/${PACKAGE_NAME} ${PACKAGE_PATH}/${PACKAGE_NAME}


## Colcon build
RUN cd ${WORKSPACE_PATH} \
    && colcon build

COPY Datasets Datasets

CMD source opt/ros/humble/setup.bash && source AIRPORTS_MVP/ROS/AIRPORTS_MVP_WS/install/setup.bash && ros2 run sensors_pkg sensor_data_subscriber







