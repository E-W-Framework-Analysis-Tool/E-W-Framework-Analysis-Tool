#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Capture current database state and build SQL Server image for local use or pushing to ACR

.DESCRIPTION
    This script captures the current state of databases in the running SQL Server container,
    builds a new SQL Server image with those databases, and optionally tags it for pushing to ACR.
    
    IMPORTANT: This script only builds the image locally. Use push-images-to-azure-registry.ps1
    to push the built images to Azure Container Registry.
    
    Prerequisites:
    - Docker Desktop running
    - SQL Server container running (docker compose up -d)
    - Databases in desired state (scenarios created, data modified, etc.)

.PARAMETER Tag
    Image tag to use (default: "latest"). Also tagged as "local/ewftool-dev-sqlserver:{tag}"

.PARAMETER SkipBackup
    Skip backing up databases. Only use if backups are already up-to-date in ./backups/

.EXAMPLE
    # Backup all databases and build image
    ./update-image.ps1

.EXAMPLE
    # Build image without backing up (use existing backups)
    ./update-image.ps1 -SkipBackup

.EXAMPLE
    # Build with custom tag
    ./update-image.ps1 -Tag "v1.0.0"
#>

param(
    [Parameter(Mandatory=$false)]
    [string]$Tag = "latest",
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipBackup
)

Write-Host "=== Capturing Database State and Building Image ===" -ForegroundColor Green
Write-Host "Tag: $Tag" -ForegroundColor Yellow
Write-Host ""

# Check if in correct directory
$currentDir = Get-Location
if (-not (Test-Path "Dockerfile")) {
    Write-Error "Dockerfile not found. Please run this script from eng/dev-dependencies/database/"
    exit 1
}

# Step 1: Backup databases (unless skipped)
if (-not $SkipBackup) {
    Write-Host "Step 1: Backing up all databases..." -ForegroundColor Cyan
    Write-Host "This captures the current state of your databases (including any scenarios you created)" -ForegroundColor Yellow
    Write-Host ""
    
    # Run backup script
    & "$PSScriptRoot/backup-database.ps1"
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to backup databases"
        exit 1
    }
    
    Write-Host ""
    Write-Host "✓ Database backups captured" -ForegroundColor Green
} else {
    Write-Host "Step 1: Skipping database backup (using existing backups)" -ForegroundColor Yellow
}

# Step 2: Build SQL Server image
Write-Host ""
Write-Host "Step 2: Building SQL Server image..." -ForegroundColor Cyan
Write-Host "This creates a new image with all backed up databases" -ForegroundColor Yellow

docker build -f Dockerfile -t "local/ewftool-dev-sqlserver:$Tag" -t "local/ewftool-dev-sqlserver:latest" .

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to build SQL Server image"
    exit 1
}

Write-Host "✓ SQL Server image built successfully" -ForegroundColor Green

# Summary
Write-Host ""
Write-Host "=== Image Build Complete! ===" -ForegroundColor Green
Write-Host ""
Write-Host "Local image created:" -ForegroundColor Cyan
Write-Host "  • local/ewftool-dev-sqlserver:$Tag" -ForegroundColor Yellow
Write-Host "  • local/ewftool-dev-sqlserver:latest" -ForegroundColor Yellow
Write-Host ""
Write-Host "What was captured:" -ForegroundColor Cyan
$backupFiles = Get-ChildItem -Path "backups/*.bak" | Select-Object -ExpandProperty Name
if ($backupFiles) {
    foreach ($file in $backupFiles) {
        Write-Host "  • $($file -replace '\.bak$', '')" -ForegroundColor Yellow
    }
} else {
    Write-Host "  (no backup files found in ./backups/)" -ForegroundColor DarkGray
}
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "  1. Test locally:" -ForegroundColor Gray
Write-Host "     cd ../.." -ForegroundColor Gray
Write-Host "     docker compose down -v" -ForegroundColor Gray
Write-Host "     docker compose up -d" -ForegroundColor Gray
Write-Host ""
Write-Host "  2. Push to Azure Container Registry:" -ForegroundColor Gray
Write-Host "     cd ../.." -ForegroundColor Gray
Write-Host "     ./push-images-to-azure-registry.ps1 -RegistryName '<acr-name>'" -ForegroundColor Gray
Write-Host ""
Write-Host "  (Get ACR name from infrastructure team or run prepare-to-push-images.ps1 in infra repo)" -ForegroundColor DarkGray
