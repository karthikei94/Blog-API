# Use the official .NET image from the Microsoft Container Registry
# FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
# WORKDIR /app
# EXPOSE 8080
# EXPOSE 8081

# Use the SDK image to build and publish the project
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
ARG RUNTIME=linux-musl-x64
WORKDIR /src

COPY ["BlogApp/BlogApp.csproj", "BlogApp/"]

RUN dotnet restore \
            "BlogApp/BlogApp.csproj" \
            -r "$RUNTIME"

COPY . .

WORKDIR "/src/BlogApp"
RUN dotnet build "BlogApp.csproj" \
    -c "$BUILD_CONFIGURATION" \
    -o /app/build
    
RUN dotnet publish "BlogApp.csproj" \
    -c "$BUILD_CONFIGURATION" \
    -r "$RUNTIME" \
    --self-contained false \
    -o /app/publish \
    /p:UseAppHost=false \
    /p:PublishReadyToRun=true

# Final Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
RUN apk add --no-cache icu-libs tzdata

WORKDIR /app
USER app
EXPOSE 8080
EXPOSE 8081
COPY --from=build /app/publish .
COPY "/blog-app-443412-865e5f019c99.json" /app/
ENTRYPOINT [ "dotnet", "BlogApp.dll" ]