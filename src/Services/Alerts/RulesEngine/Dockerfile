# Use a specific platform for building the image
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Define the target architecture as argument.
ARG TARGETARCH

WORKDIR /src
COPY ["src/Services/Alerts/RulesEngine/RulesEngine.csproj", "src/Services/Alerts/RulesEngine/"]

# Restore dependencies
RUN dotnet restore "src/Services/Alerts/RulesEngine/RulesEngine.csproj" -r linux-$TARGETARCH

COPY . .
WORKDIR "/src/src/Services/Alerts/RulesEngine"

# Compile dependencies
RUN dotnet build "RulesEngine.csproj" -c Release -r linux-$TARGETARCH -o /app/build

FROM build AS publish
RUN dotnet publish "RulesEngine.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image based on .NET runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RulesEngine.dll"]
