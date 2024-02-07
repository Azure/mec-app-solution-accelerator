# Build .NET application to update Camera configuration
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy csproj and restore any dependencies (via NuGet)
COPY *.csproj ./
RUN dotnet restore

# Copy the project files and build our release
COPY . ./
RUN dotnet publish -c Release -o out

# Use an official base image with FFmpeg installed
FROM linuxserver/ffmpeg

# Install nginx to serve the HLS files
RUN apt-get update && apt-get install -y nginx

# Install the .NET runtime dependencies
RUN apt-get update && \
    apt-get install -y wget && \
    wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && \
    apt-get update; \
    apt-get install -y apt-transport-https && \
    apt-get update && \
    apt-get install -y dotnet-runtime-8.0

# Copy the .NET Core application from the build-env image to the final image
COPY --from=build-env /app/out /app

# Copy a configuration script into the container
COPY entrypoint.sh /entrypoint.sh

# Make the entrypoint script executable
RUN chmod +x /entrypoint.sh

# Set up nginx to serve files from /var/www
RUN echo "daemon off;" >> /etc/nginx/nginx.conf
COPY nginx.conf /etc/nginx/sites-enabled/default

# Expose the port for the HLS stream
EXPOSE 3000

# Run the entrypoint script when the container starts
ENTRYPOINT ["/entrypoint.sh"]
