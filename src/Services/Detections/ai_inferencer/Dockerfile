FROM python:3.8.16-slim-bullseye

COPY requirements.txt .
RUN pip3 install --upgrade pip
RUN pip3 install torch==1.13.1+cpu torchvision==0.14.1+cpu --extra-index-url https://download.pytorch.org/whl/cpu
RUN pip3 install -r requirements.txt

COPY . .

CMD python3 invoke-sender-frames.py
