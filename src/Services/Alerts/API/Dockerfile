# Use a specific platform for building the image
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Define the target architecture as argument.
ARG TARGETARCH

WORKDIR /src
COPY ["src/Services/Alerts/API/Alerts.API.csproj", "src/Services/Alerts/API/"]

# Restore dependencies
RUN dotnet restore "src/Services/Alerts/API/Alerts.API.csproj" -r linux-$TARGETARCH

COPY . .
WORKDIR "/src/src/Services/Alerts/API"

# Compile dependencies
RUN dotnet build "Alerts.API.csproj" -c Release -r linux-$TARGETARCH -o /app/build

FROM build AS publish
RUN dotnet publish "Alerts.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image based on .NET runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Alerts.API.dll"]