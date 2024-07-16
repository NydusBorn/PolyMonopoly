FROM node:22-alpine3.20 AS base

WORKDIR /app
COPY package.json ./

RUN npm install

FROM base AS build
WORKDIR /app
COPY tsconfig.json ./
COPY nuxt.config.ts ./
COPY public ./
COPY app.vue ./
RUN npm run build

FROM build AS final
WORKDIR /app
ENTRYPOINT node ./.output/server/index.mjs