# Use a specific platform for building the image
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Define the target architecture as argument.
ARG TARGETARCH

WORKDIR /src
COPY ["src/Web/alerts-ui/Alerts.UI.csproj", "src/Web/alerts-ui/"]

# Restore dependencies
RUN dotnet restore "src/Web/alerts-ui/Alerts.UI.csproj" -r linux-$TARGETARCH

COPY . .
WORKDIR "/src/src/Web/alerts-ui"

# Compile dependencies
RUN dotnet build "Alerts.UI.csproj" -c Release -r linux-$TARGETARCH -o /app/build

FROM build AS publish
RUN dotnet publish "Alerts.UI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image based on .NET runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Alerts.UI.dll"]