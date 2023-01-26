FROM ubuntu:20.04

ENV TZ=Europe/Madrid
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone
RUN apt update
RUN apt-get update && apt-get -y install --no-install-recommends python3.8-dev python3-pip
RUN update-alternatives --install /usr/bin/python3 python3 /usr/bin/python3.8 1
RUN apt-get install wget libgl1-mesa-glx libglib2.0-0 git tzdata -y
RUN apt-get install ffmpeg -y
COPY requirements.txt .
RUN pip3 install --upgrade pip
RUN pip3 install torch==1.13.1+cpu torchvision==0.14.1+cpu --extra-index-url https://download.pytorch.org/whl/cpu
RUN pip3 install -r requirements.txt
COPY . .


CMD python3 invoke-sender-frames.py
