# FHIR Query Builder Dockerfile
# This creates a container image for the FHIR Query Builder application
# Note: This is primarily for development/testing as the app is a Windows Forms application

# Use the official .NET runtime image
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY ["FHIRQueryBuilder.csproj", "FHIRQueryBuilder/"]
COPY ["../FhirServerHelper/FhirServerAgent.csproj", "FhirServerHelper/"]

# Restore dependencies
RUN dotnet restore "FHIRQueryBuilder/FHIRQueryBuilder.csproj"

# Copy source code
COPY . FHIRQueryBuilder/
COPY ../FhirServerHelper/ FhirServerHelper/

# Build the application
WORKDIR "/src/FHIRQueryBuilder"
RUN dotnet build "FHIRQueryBuilder.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "FHIRQueryBuilder.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage - runtime image
FROM base AS final
WORKDIR /app

# Install dependencies for GUI applications (if needed for testing)
RUN apt-get update && apt-get install -y \
    libx11-6 \
    libxext6 \
    libxrender1 \
    libxtst6 \
    libxi6 \
    libxrandr2 \
    libasound2 \
    libpangocairo-1.0-0 \
    libatk1.0-0 \
    libcairo-gobject2 \
    libgtk-3-0 \
    libgdk-pixbuf2.0-0 \
    && rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=publish /app/publish .

# Create directories for application data
RUN mkdir -p /app/QueryHistory /app/Exports /app/ConfigBackups

# Set environment variables
ENV DOTNET_ENVIRONMENT=Production
ENV FHIR_QUERY_BUILDER_DEFAULT_SERVER_URL=https://server.fire.ly
ENV FHIR_QUERY_BUILDER_ENABLE_LOGGING=true

# Expose any ports if needed (not typically needed for Windows Forms apps)
# EXPOSE 80

# Set the entry point
ENTRYPOINT ["dotnet", "FHIRQueryBuilder.dll"]

# Health check (optional)
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD dotnet --info || exit 1

# Labels for metadata
LABEL maintainer="FHIR Query Builder Team"
LABEL version="1.0"
LABEL description="FHIR Query Builder - A comprehensive tool for building and testing FHIR queries"
LABEL org.opencontainers.image.source="https://github.com/sjvann/FhirServerHelper"
LABEL org.opencontainers.image.documentation="https://github.com/sjvann/FhirServerHelper/blob/main/FHIRQueryBuilder/README.md"
LABEL org.opencontainers.image.licenses="MIT"
