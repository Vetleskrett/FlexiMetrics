FROM python:3.13-slim

WORKDIR /app

COPY fleximetrics.py ./

COPY script.py ./

COPY requirements.txt ./

RUN pip install --disable-pip-version-check --root-user-action=ignore --no-cache-dir dataclass-wizard

RUN pip install --disable-pip-version-check --root-user-action=ignore --no-cache-dir -r requirements.txt

CMD ["python", "script.py"]