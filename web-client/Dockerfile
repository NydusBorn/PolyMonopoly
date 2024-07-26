FROM node:22-alpine3.20 AS base

WORKDIR /app
COPY package.json ./

RUN npm install

FROM base AS build
WORKDIR /app
COPY nuxt.config.ts ./
COPY public ./public/
COPY app.vue ./
COPY theme.js ./
COPY pages ./pages/
COPY server/api ./server/api
RUN npm run build

FROM build AS final
WORKDIR /app
ENTRYPOINT node ./.output/server/index.mjs