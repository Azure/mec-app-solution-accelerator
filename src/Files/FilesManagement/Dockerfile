# Indicar la plataforma para el proceso de construcción
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0.400 AS build

# Definir la arquitectura objetivo con un valor por defecto de "amd64"
ARG TARGETARCH=amd64

WORKDIR /src
COPY ["src/Files/FilesManagement/FilesManagement.csproj", "src/Files/FilesManagement/"]

# Restaurar considerando la arquitectura objetivo
RUN dotnet restore "src/Files/FilesManagement/FilesManagement.csproj" -r linux-$TARGETARCH

COPY . .
WORKDIR "/src/src/Files/FilesManagement"

# Compilar teniendo en cuenta la arquitectura objetivo
RUN dotnet build "FilesManagement.csproj" -c Release -r linux-$TARGETARCH -o /app/build

FROM build AS publish
RUN dotnet publish "FilesManagement.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Imagen final basada en el runtime de .NET
FROM mcr.microsoft.com/dotnet/aspnet:7.0.4 AS final
WORKDIR /app
EXPOSE 80
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FilesManagement.dll"]