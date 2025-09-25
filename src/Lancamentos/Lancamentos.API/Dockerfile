FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Lancamentos/Lancamentos.API/Lancamentos.API.csproj", "src/Lancamentos/Lancamentos.API/"]
COPY ["src/Lancamentos/Lancamentos.Application/Lancamentos.Application.csproj", "src/Lancamentos/Lancamentos.Application/"]
COPY ["src/Lancamentos/Lancamentos.Domain/Lancamentos.Domain.csproj", "src/Lancamentos/Lancamentos.Domain/"]
COPY ["src/Lancamentos/Lancamentos.Infrastructure/Lancamentos.Infrastructure.csproj", "src/Lancamentos/Lancamentos.Infrastructure/"]
COPY ["src/Shared/FluxoCaixa.Shared/FluxoCaixa.Shared.csproj", "src/Shared/FluxoCaixa.Shared/"]

RUN dotnet restore "src/Lancamentos/Lancamentos.API/Lancamentos.API.csproj"
COPY . .
WORKDIR "/src/src/Lancamentos/Lancamentos.API"
RUN dotnet build "Lancamentos.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Lancamentos.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

USER root
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
USER app

ENTRYPOINT ["dotnet", "Lancamentos.API.dll"]