# Multi-stage build to download packages first
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS package-downloader
WORKDIR /src

# Step 1: Install dependencies
RUN apt-get update && apt-get install -y unzip

# Step 2: Add the NuGet source and download database packages
RUN dotnet new console -n TempProject
WORKDIR /src/TempProject
RUN dotnet nuget add source "https://pkgs.dev.azure.com/ed-fi-alliance/Ed-Fi-Alliance-OSS/_packaging/EdFi%40Release/nuget/v3/index.json" --name "EdFi-Release"

# Download each database package
RUN dotnet add package EdFi.Database.Security.Standard.5.2.0
RUN dotnet add package EdFi.Database.Admin.Standard.5.2.0
RUN dotnet add package EdFi.Suite3.Ods.Populated.Template.Standard.5.2.0
RUN dotnet add package EdFi.Suite3.Ods.Minimal.Template.Standard.5.2.0

# Step 3: Extract all packages and find .bak files
RUN mkdir -p /app/packages
RUN for pkg in "edfi.database.security.standard.5.2.0" "edfi.database.admin.standard.5.2.0" "edfi.suite3.ods.populated.template.standard.5.2.0" "edfi.suite3.ods.minimal.template.standard.5.2.0"; do \
        echo "Processing package: $pkg"; \
        find ~/.nuget/packages -name "$pkg" -type d -exec find {} -name "*.nupkg" \; | head -1 | xargs -I {} unzip {} -d /app/packages/$pkg/; \
    done

# Step 4: Find and collect all .bak files
RUN mkdir -p /app/backups
RUN find /app/packages -name "*.bak" -type f -exec cp {} /app/backups/ \;

# Rename backups for templates if needed
RUN cd /app/backups && \
    if [ -f "EdFi.Ods.Minimal.Template.bak" ]; then \
        mv "EdFi.Ods.Minimal.Template.bak" "EdFi_Ods_Minimal_Template.bak"; \
        echo "Renamed EdFi.Ods.Minimal.Template.bak to EdFi_Ods_Minimal_Template.bak"; \
    fi && \
    if [ -f "EdFi.Ods.Populated.Template.bak" ]; then \
        mv "EdFi.Ods.Populated.Template.bak" "EdFi_Ods_Populated_Template.bak"; \
        echo "Renamed EdFi.Ods.Populated.Template.bak to EdFi_Ods_Populated_Template.bak"; \
    fi

# Step 5: Verify we have the expected .bak files
RUN echo "=== Found backup files ===" && \
    ls -la /app/backups/ && \
    echo "=== Checking for required files ===" && \
    for file in "EdFi_Admin.bak" "EdFi_Security.bak" "EdFi_Ods_Minimal_Template.bak" "EdFi_Ods_Populated_Template.bak"; do \
        if [ -f "/app/backups/$file" ]; then \
            echo "✓ Found: $file"; \
        else \
            echo "✗ Missing: $file"; \
            echo "Available files:"; \
            ls /app/backups/; \
            exit 1; \
        fi; \
    done

# Main SQL Server image
FROM mcr.microsoft.com/mssql/server:2019-latest

ENV ACCEPT_EULA=Y
ENV SA_PASSWORD=P@ssw0rd123
ENV MSSQL_PID=Developer

# Create directories - keep everything outside volume mount path
RUN mkdir -p /opt/backups
RUN mkdir -p /opt/scripts

# Copy the downloaded .bak files to /opt/backups (outside volume mount)
COPY --from=package-downloader /app/backups/*.bak /opt/backups/

# Copy additional backups from local backups directory (if any exist)
# This allows for updated Admin DB, scenarios, CEDS Data Warehouse, etc.
COPY backups/ /opt/backups/

# Copy the scripts to /opt/scripts (outside volume mount path)
COPY restore-all-backups.sh /opt/scripts/
COPY entrypoint.sh /opt/scripts/

ENTRYPOINT ["/opt/scripts/entrypoint.sh"]