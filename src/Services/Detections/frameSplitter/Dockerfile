FROM python:3.8.16-slim-bullseye

COPY requirements.txt .
RUN pip install --disable-pip-version-check --no-cache-dir -U -r requirements.txt \
    && rm requirements.txt

COPY ./Datasets ./Datasets
COPY ./src ./src

CMD cd src && python3 frameSplitter.py