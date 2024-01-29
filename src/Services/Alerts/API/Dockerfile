# Usar una plataforma específica para el proceso de construcción
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0.400 AS build

# Definir la arquitectura objetivo como un argumento con un valor por defecto de "amd64"
ARG TARGETARCH=amd64

WORKDIR /src
COPY ["src/Services/Alerts/API/Alerts.API.csproj", "src/Services/Alerts/API/"]

# Restaurar considerando la arquitectura objetivo
RUN dotnet restore "src/Services/Alerts/API/Alerts.API.csproj" -r linux-$TARGETARCH

COPY . .
WORKDIR "/src/src/Services/Alerts/API"

# Compilar considerando la arquitectura objetivo
RUN dotnet build "Alerts.API.csproj" -c Release -r linux-$TARGETARCH -o /app/build

FROM build AS publish
RUN dotnet publish "Alerts.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Imagen final basada en el runtime de .NET
FROM mcr.microsoft.com/dotnet/aspnet:7.0.4 AS final
WORKDIR /app
EXPOSE 80
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Alerts.API.dll"]