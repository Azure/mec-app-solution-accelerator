FROM python:3.8.16-slim-bullseye

COPY frameSplitter/requirements.txt .
RUN pip install --disable-pip-version-check --no-cache-dir -U -r requirements.txt \
    && rm requirements.txt

COPY ./frameSplitter/Datasets ./Datasets
COPY ./frameSplitter/src ./src
COPY shared ./src/shared

CMD cd src && python3 frameSplitter.py