# Build Frontend
FROM node:14 AS build
WORKDIR /usr/src/app
COPY EnoLandingPageBackend/ClientApp/package.json EnoLandingPageBackend/ClientApp/package-lock.json ./
RUN npm install
COPY EnoLandingPageBackend/ClientApp .
RUN npm run build


# Setup Static File Host
FROM nginx:1.21-alpine

WORKDIR /etc/nginx
RUN rm /usr/share/nginx/html/index.html
RUN rm -r /docker-entrypoint.d/*
RUN rm -r ./*

ENV STATIC_HOSTING=true
ENV SUBSTITUTE_PATH=/usr/share/nginx/html

COPY ./substitute_variables.sh /docker-entrypoint.d/substitute_variables.sh
RUN chmod +x /docker-entrypoint.d/substitute_variables.sh

COPY nginx/ /etc/nginx/
COPY scoreboard/ /usr/share/nginx/html/api/scoreboard/

COPY --from=build /usr/src/app/dist/ClientApp/index.html /template/index.html
COPY --from=build /usr/src/app/dist/ClientApp /usr/share/nginx/html
COPY ./customization /usr/share/nginx/html/assets/customization


