FROM microsoft/cntk:2.5-cpu-python3.5
# If GPU support intended, change cpu-python3.5 to gpu-python3.5

RUN apt-get update && apt-get install -y --no-install-recommends python-setuptools python-pip libpython3.5-dev

COPY conda_dependencies.yml /
RUN /root/anaconda3/bin/conda env update -n cntk-py35 -f /conda_dependencies.yml

COPY / /Prometheus.api

WORKDIR /Prometheus.api
ENV FLASK_APP=/Prometheus.api/app.py
CMD ["/bin/bash","-c","source /cntk/activate-cntk && python runserver.py"]