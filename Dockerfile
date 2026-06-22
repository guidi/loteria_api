FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY src/Loteria.API.csproj src/
RUN dotnet restore "src/Loteria.API.csproj"

COPY src/. src/
WORKDIR /src/src
RUN dotnet build "Loteria.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Loteria.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:${PORT:-8080}
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Loteria.API.dll"]
