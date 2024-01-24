FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ControlPlane.API.csproj .
COPY API.sln .

RUN dotnet restore API.sln

COPY . .
RUN dotnet build ControlPlane.API.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish ControlPlane.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ControlPlane.API.dll"]