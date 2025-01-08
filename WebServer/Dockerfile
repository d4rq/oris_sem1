FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8888

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WebServer/WebServer.csproj", "WebServer/"]
COPY ["HttpServerLibrary/HttpServerLibrary.csproj", "HttpServerLibrary/"]
COPY ["OrmLibrary/OrmLibrary.csproj", "OrmLibrary/"]
COPY ["TemplateEngine/TemplateEngine.csproj", "TemplateEngine/"]
RUN dotnet restore "WebServer/WebServer.csproj"
COPY . .
WORKDIR "/src/WebServer"
RUN dotnet build "WebServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WebServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebServer.dll"]
