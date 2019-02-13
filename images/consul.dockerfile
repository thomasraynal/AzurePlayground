FROM consul
RUN mkdir -p /certs
COPY ./certs /certs

RUN cp -a ./certs/. /usr/local/share/ca-certificates/
RUN update-ca-certificates