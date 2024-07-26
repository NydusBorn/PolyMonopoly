#!/usr/bin/env just --justfile

export BACKEND_HOST := "http://localhost:5220"

# build web client for production
web-build:
    cd web-client && \
    npm run build

# start web client in dev mode
web-dev:
    cd web-client && \
    npm run dev

# start web client in production mode, requires building
web-start:
    cd web-client/.output/server && \
    node index.mjs

# run backend in dev mode
backend-dev:
    cd asp-backend/asp-backend && \
    dotnet run asp-backend.dll

# publish backend for production
backend-publish:
    cd asp-backend/asp-backend && \
    dotnet publish -c Release -o ./.output

# run backend in production mode, requires publishing
backend-start:
    cd asp-backend/asp-backend && \
    dotnet .output/asp-backend.dll