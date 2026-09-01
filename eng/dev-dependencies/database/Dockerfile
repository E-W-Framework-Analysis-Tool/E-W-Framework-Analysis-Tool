# Multi-stage build to download packages first
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS package-downloader
WORKDIR /src

# Step 1: Install dependencies
RUN apt-get update && apt-get install -y unzip

# Step 2: Add the NuGet source and download database packages
RUN dotnet new console -n TempProject
WORKDIR /src/TempProject
RUN dotnet nuget add source "https://pkgs.dev.azure.com/ed-fi-alliance/Ed-Fi-Alliance-OSS/_packaging/EdFi%40Release/nuget/v3/index.json" --name "EdFi-Release"

# Download each database package. Versions are pinned: Ed-Fi republishes under the
# same package ids, so an unpinned restore makes this image non-reproducible.
RUN dotnet add package EdFi.Database.Security.Standard.5.2.0 --version 7.3.20023
RUN dotnet add package EdFi.Database.Admin.Standard.5.2.0 --version 7.3.20021
RUN dotnet add package EdFi.Suite3.Ods.Populated.Template.Standard.5.2.0 --version 7.3.20054
RUN dotnet add package EdFi.Suite3.Ods.Minimal.Template.Standard.5.2.0 --version 7.3.20051

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
            echo "Found: $file"; \
        else \
            echo "Missing: $file"; \
            echo "Available files:"; \
            ls /app/backups/; \
            exit 1; \
        fi; \
    done

# Main SQL Server image
#
# SQL Server images are published for linux/amd64 only. On Apple Silicon this layer
# is built and run under emulation (Rosetta), which 2022 tolerates far better than 2019.
FROM mcr.microsoft.com/mssql/server:2022-latest

ENV ACCEPT_EULA=Y
ENV SA_PASSWORD=P@ssw0rd123
ENV MSSQL_PID=Developer

USER root

# Every startup script and the compose healthcheck shell out to sqlcmd. Fail the build
# with a clear message if a future base image drops the bundled tools.
RUN test -x /opt/mssql-tools18/bin/sqlcmd \
    || (echo "ERROR: /opt/mssql-tools18/bin/sqlcmd is missing from the base image." && exit 1)

# Keep everything outside /var/opt/mssql so it is not shadowed by the data volume
RUN mkdir -p /opt/backups /opt/scripts/init-scripts /var/opt/mssql/backup

# Ed-Fi core databases and templates extracted from the NuGet packages above
COPY --from=package-downloader /app/backups/*.bak /opt/backups/

# Additional backups checked into the repo (updated Admin DB, scenarios, CEDS Data
# Warehouse, etc.). Empty except for .gitkeep on a clean checkout.
COPY backups/ /opt/backups/

COPY restore-all-backups.sh /opt/scripts/
COPY update-connection-strings.sh /opt/scripts/
COPY entrypoint.sh /opt/scripts/
COPY init-scripts/ /opt/scripts/init-scripts/

# Normalize CRLF -> LF (this repo is Windows-oriented and git may check these out with
# CRLF, which bash rejects) and set executable bits.
RUN set -eux; \
    find /opt/scripts -type f \( -name "*.sh" -o -name "*.sql" \) -print0 \
      | xargs -0 -r sed -i 's/\r$//'; \
    find /opt/scripts -type f -name "*.sh" -print0 \
      | xargs -0 -r chmod +x; \
    if grep -RIl $'\r' /opt/scripts; then \
      echo "ERROR: CRLF line endings detected in these files (must be LF):"; \
      grep -RIl $'\r' /opt/scripts; \
      exit 1; \
    fi

RUN chown -R mssql:root /opt/backups /opt/scripts /var/opt/mssql/backup

USER mssql

ENTRYPOINT ["/opt/scripts/entrypoint.sh"]
