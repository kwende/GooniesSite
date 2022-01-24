FROM ubuntu:20.04

RUN apt-get update && DEBIAN_FRONTEND=noninteractive apt-get install build-essential git npm \
    curl wget pkg-config libpixman-1-dev libcairo2-dev libpango1.0-dev libjpeg8-dev libgif-dev -y
RUN npm install --global yarn && npm -g install create-react-app && npm install -g n && n 12.21.0

RUN mkdir /repos && git clone https://github.com/kwende/GooniesSite.git /repos/jsnes-web
RUN cd /repos/jsnes-web && yarn install

#ENTRYPOINT ["yarn", "--cwd" , "/repos/jsnes-web", "start"]