# Use the official .NET image from the Microsoft Container Registry
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the SDK image to build and publish the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BlogApp/BlogApp.csproj", "BlogApp/"]
RUN dotnet restore "BlogApp/BlogApp.csproj"
COPY . .
WORKDIR "/src/BlogApp"
RUN dotnet build "BlogApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlogApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY "/blog-app-443412-865e5f019c99.json" /app/
ENTRYPOINT [ "dotnet", "BlogApp.dll" ]