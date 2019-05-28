FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["CO2Monitor.Controller/CO2Monitor.Controller.csproj", "CO2Monitor.Controller/"]
RUN dotnet restore "CO2Monitor.Controller/CO2Monitor.Controller.csproj"
COPY . .
WORKDIR "/src/CO2Monitor.Controller"
RUN dotnet build "CO2Monitor.Controller.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "CO2Monitor.Controller.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CO2Monitor.Controller.dll"]