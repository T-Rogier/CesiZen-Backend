# syntax=docker/dockerfile:1

# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copie solution et csproj pour profiter du cache
COPY CesiZen-Backend.sln ./
COPY CesiZen-Backend/CesiZen-Backend.csproj CesiZen-Backend/

RUN dotnet restore CesiZen-Backend/CesiZen-Backend.csproj

# Copie du reste et publish
COPY . .
RUN dotnet publish CesiZen-Backend/CesiZen-Backend.csproj -c Release -o /app/publish /p:UseAppHost=false

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

# L'API écoute sur 8080 (cohérent avec Traefik/Swarm)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "CesiZen-Backend.dll"]
