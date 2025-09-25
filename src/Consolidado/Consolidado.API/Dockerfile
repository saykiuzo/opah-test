FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Consolidado/Consolidado.API/Consolidado.API.csproj", "src/Consolidado/Consolidado.API/"]
COPY ["src/Consolidado/Consolidado.Application/Consolidado.Application.csproj", "src/Consolidado/Consolidado.Application/"]
COPY ["src/Consolidado/Consolidado.Domain/Consolidado.Domain.csproj", "src/Consolidado/Consolidado.Domain/"]
COPY ["src/Consolidado/Consolidado.Infrastructure/Consolidado.Infrastructure.csproj", "src/Consolidado/Consolidado.Infrastructure/"]
COPY ["src/Shared/FluxoCaixa.Shared/FluxoCaixa.Shared.csproj", "src/Shared/FluxoCaixa.Shared/"]

RUN dotnet restore "src/Consolidado/Consolidado.API/Consolidado.API.csproj"
COPY . .
WORKDIR "/src/src/Consolidado/Consolidado.API"
RUN dotnet build "Consolidado.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Consolidado.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

USER root
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
USER app

ENTRYPOINT ["dotnet", "Consolidado.API.dll"]