FROM python:3.8

RUN apt-get update
RUN apt-get install libgl1-mesa-glx libglib2.0-0 git -y
Run apt install ffmpeg -y
COPY requirements.txt .
RUN pip install --disable-pip-version-check --no-cache-dir -U -r requirements.txt \
    && rm requirements.txt
COPY . .


CMD python3 invoke-sender-frames.py
