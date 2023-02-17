FROM python:3.8.16-slim-bullseye

#RUN apt-get update
#RUN apt-get install libgl1-mesa-glx libglib2.0-0 git -y
#Run apt install ffmpeg -y
COPY requirements.txt .
RUN pip install --disable-pip-version-check --no-cache-dir -U -r requirements.txt \
    && rm requirements.txt


COPY ./Datasets ./Datasets
COPY ./src ./src
CMD cd src && python3 frameSplitter.py