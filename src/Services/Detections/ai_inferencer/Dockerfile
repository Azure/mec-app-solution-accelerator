FROM python:3.8.16-slim-bullseye

COPY ai_inferencer/requirements.txt .

# Add the system packages required for psutil
RUN apt-get update && apt-get install -y gcc python3-dev

# Upgrade pip and setuptools
RUN pip3 install --upgrade pip setuptools

RUN pip3 install torch==1.13.1 torchvision==0.14.1 --extra-index-url https://download.pytorch.org/whl/cpu
RUN pip3 install -r requirements.txt
RUN pip3 uninstall opencv-python opencv-python-headless -y
RUN pip3 install opencv-python-headless

COPY shared /shared

COPY ai_inferencer/ .

CMD python3 invoke-sender-frames.py
