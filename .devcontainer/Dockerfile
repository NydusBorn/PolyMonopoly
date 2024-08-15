FROM mcr.microsoft.com/devcontainers/dotnet:1-8.0

RUN chsh -s /usr/bin/zsh

RUN wget -qO - 'https://proget.makedeb.org/debian-feeds/prebuilt-mpr.pub' | gpg --dearmor | sudo tee /usr/share/keyrings/prebuilt-mpr-archive-keyring.gpg 1> /dev/null
RUN echo "deb [arch=all,$(dpkg --print-architecture) signed-by=/usr/share/keyrings/prebuilt-mpr-archive-keyring.gpg] https://proget.makedeb.org prebuilt-mpr $(lsb_release -cs)" | sudo tee /etc/apt/sources.list.d/prebuilt-mpr.list

# [Optional] Uncomment this section to install additional OS packages.
RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
&& apt-get -y install direnv just \
&& apt-get clean -y \

RUN echo 'eval "$(direnv hook zsh)"' >> /home/app/.zshrc
RUN echo 'eval "$(direnv hook zsh)"' >> /home/vscode/.zshrc

# [Optional] Uncomment this line to install global node packages.
RUN su vscode -c "source /usr/local/share/nvm/nvm.sh && nvm install 22 && nvm use 22 && npm install -g typescript" 2>&1
# RUN su vscode -c "source /usr/local/share/nvm/nvm.sh && npm install -g <your-package-here>" 2>&1

RUN dotnet tool install --global dotnet-ef

RUN mkdir -p /code
WORKDIR /code
COPY ../ ./
WORKDIR /code/asp-backend
RUN dotnet restore ./asp-backend/asp-backend.csproj
WORKDIR /code/web-client
RUN npm install
WORKDIR /code
