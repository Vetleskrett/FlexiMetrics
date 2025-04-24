FROM python:3.13

WORKDIR /app

COPY fleximetrics.py ./

COPY requirements.txt ./

COPY packages-list.txt ./

RUN apt update

RUN apt upgrade -y

RUN xargs apt install -y < packages-list.txt

RUN pip install --disable-pip-version-check --root-user-action=ignore --no-cache-dir dataclass-wizard

RUN pip install --disable-pip-version-check --root-user-action=ignore --no-cache-dir -r requirements.txt

CMD ["python", "script.py"]