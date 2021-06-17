FROM gitpod/workspace-full
RUN sudo curl -sL https://deb.nodesource.com/setup_14.x |  bash -
RUN sudo apt-get install -y nodejs
