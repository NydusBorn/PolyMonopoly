FROM mcr.microsoft.com/devcontainers/dotnet:1-8.0

# [Optional] Uncomment this section to install additional OS packages.
# RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
#     && apt-get -y install --no-install-recommends <your-package-list-here>

# [Optional] Uncomment this line to install global node packages.
RUN su vscode -c "source /usr/local/share/nvm/nvm.sh && nvm install 22 && nvm use 22 && npm install -g typescript" 2>&1
# RUN su vscode -c "source /usr/local/share/nvm/nvm.sh && npm install -g <your-package-here>" 2>&1

RUN mkdir -p /code
WORKDIR /code

COPY ../asp-backend ./asp-backend/
WORKDIR /code/asp-backend
RUN dotnet restore ./asp-backend/asp-backend.csproj
WORKDIR /code
COPY ../web-client ./web-client/
WORKDIR /code/web-client
RUN npm install
WORKDIR /code
