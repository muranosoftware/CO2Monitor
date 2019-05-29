FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
RUN pwd
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY CO2Monitor.sln ./
COPY src/CO2Monitor.Controller/CO2Monitor.Controller.csproj ./src/CO2Monitor.Controller/
COPY src/CO2Monitor.Core/CO2Monitor.Core.csproj ./src/CO2Monitor.Core/
COPY src/CO2Monitor.Infrastructure/CO2Monitor.Infrastructure.csproj ./src/CO2Monitor.Infrastructure/
COPY src/CO2Monitor.SlackBot/CO2Monitor.SlackBot.csproj ./src/CO2Monitor.SlackBot/

RUN dotnet restore 
COPY . .
WORKDIR /src/src/CO2Monitor.Core
RUN dotnet build CO2Monitor.Core.csproj -c Release -o /app

WORKDIR /src/src/CO2Monitor.Infrastructure
RUN dotnet build CO2Monitor.Infrastructure.csproj -c Release -o /app

WORKDIR /src/src/CO2Monitor.Controller
RUN dotnet build CO2Monitor.Controller.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CO2Monitor.Controller.dll"]