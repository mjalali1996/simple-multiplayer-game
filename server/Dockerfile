FROM ubuntu:22.04

RUN apt-get update \
    && apt-get install -y curl \
    && apt-get install -y htop

RUN apt-get install -y dotnet-runtime-7.0

RUN mkdir "nakama-files" && cd "nakama-files" && curl -L https://github.com/heroiclabs/nakama/releases/download/v3.32.0/nakama-3.32.0-linux-amd64.tar.gz | tar xvz

