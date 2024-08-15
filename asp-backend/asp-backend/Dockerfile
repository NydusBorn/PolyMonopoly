FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS deps
RUN dotnet tool install --global dotnet-ef
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ./asp-backend.csproj ./
RUN dotnet restore asp-backend.csproj

FROM deps AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY Controllers ./Controllers
COPY Properties ./Properties
COPY appsettings.json ./
COPY appsettings.Development.json ./
COPY Program.cs ./
COPY Classes ./Classes
COPY Contexts ./Contexts
RUN /root/.dotnet/tools/dotnet-ef migrations add Initial
RUN dotnet build "asp-backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "asp-backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish /src/Migrations ./Migrations
ENTRYPOINT ["dotnet", "asp-backend.dll"]
