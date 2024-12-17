# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS dev
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Development \
    DOTNET_USE_POLLING_FILE_WATCHER=1
ENTRYPOINT ["dotnet", "watch", "run", "--project", "eBookStore/eBookStore.csproj"]

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app
COPY . .
ENTRYPOINT ["dotnet", "eBookStore/bin/Debug/net8.0/eBookStore.dll"]
