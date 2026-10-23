#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Push Ed-Fi development dependency images to Azure Container Registry

.DESCRIPTION
    This script builds (optionally) and pushes the ewftool-dev-sqlserver and 
    ewftool-dev-edfi-api images to a specified Azure Container Registry.
    
    Prerequisites:
    - Docker Desktop running
    - Azure CLI installed and logged in (az login)
    - ACR admin credentials enabled OR current user has AcrPush role

.PARAMETER RegistryName
    The name of the Azure Container Registry (e.g., "ewftdacr1a2b")
    This is the short name, not the full login server URL.

.PARAMETER RegistryLoginServer
    The full login server URL (e.g., "ewftdacr1a2b.azurecr.io")
    If not provided, will be constructed from RegistryName

.PARAMETER SkipBuild
    Skip building images locally. Use existing local images tagged as:
    - local/ewftool-dev-sqlserver:latest
    - local/ewftool-dev-edfi-api:latest

.PARAMETER Tag
    Image tag to use in ACR (default: "latest")

.EXAMPLE
    # Build and push with ACR name
    ./push-images-to-azure-registry.ps1 -RegistryName "ewftdacr1a2b"

.EXAMPLE
    # Push existing images without rebuilding
    ./push-images-to-azure-registry.ps1 -RegistryName "ewftdacr1a2b" -SkipBuild

.EXAMPLE
    # Push with custom tag
    ./push-images-to-azure-registry.ps1 -RegistryName "ewftdacr1a2b" -Tag "v1.0.0"

.EXAMPLE
    # Use full login server URL
    ./push-images-to-azure-registry.ps1 -RegistryLoginServer "ewftdacr1a2b.azurecr.io"
#>

param(
    [Parameter(Mandatory=$false)]
    [string]$RegistryName,
    
    [Parameter(Mandatory=$false)]
    [string]$RegistryLoginServer,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipBuild,
    
    [Parameter(Mandatory=$false)]
    [string]$Tag = "latest"
)

# Validate parameters
if (-not $RegistryName -and -not $RegistryLoginServer) {
    Write-Error "Either -RegistryName or -RegistryLoginServer must be provided"
    Write-Host ""
    Write-Host "Example usage:" -ForegroundColor Cyan
    Write-Host "  ./push-images-to-azure-registry.ps1 -RegistryName 'ewftdacr1a2b'" -ForegroundColor Gray
    exit 1
}

# Construct login server if not provided
if (-not $RegistryLoginServer) {
    $RegistryLoginServer = "$RegistryName.azurecr.io"
}

# Extract registry name from login server if needed
if (-not $RegistryName) {
    $RegistryName = $RegistryLoginServer -replace '\.azurecr\.io$', ''
}

Write-Host "=== Pushing Images to Azure Container Registry ===" -ForegroundColor Green
Write-Host "Registry: $RegistryLoginServer" -ForegroundColor Yellow
Write-Host "Tag: $Tag" -ForegroundColor Yellow
Write-Host ""

# Check Docker is running
Write-Host "Checking Docker..." -ForegroundColor Cyan
docker ps > $null 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Error "Docker is not running. Please start Docker Desktop."
    exit 1
}
Write-Host "Docker is running" -ForegroundColor Green

# Check Azure CLI is available
Write-Host ""
Write-Host "Checking Azure CLI..." -ForegroundColor Cyan
$azCheck = Get-Command az -ErrorAction SilentlyContinue
if (-not $azCheck) {
    Write-Error "Azure CLI not found. Install from: https://aka.ms/installazurecliwindows"
    exit 1
}
Write-Host "Azure CLI is installed" -ForegroundColor Green

# Login to ACR
Write-Host ""
Write-Host "Logging into Azure Container Registry..." -ForegroundColor Cyan
az acr login --name $RegistryName

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to login to ACR. Make sure you're logged into Azure (az login) and have access to the registry."
    exit 1
}
Write-Host "Logged into ACR" -ForegroundColor Green

# Build images if not skipped
if (-not $SkipBuild) {
    Write-Host ""
    Write-Host "Building images locally..." -ForegroundColor Cyan
    
    $env:REGISTRY = "local"
    $env:VERSION = "latest"
    
    docker compose build
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to build images"
        exit 1
    }
    
    Write-Host "Images built successfully" -ForegroundColor Green
}

# Verify local images exist
Write-Host ""
Write-Host "Verifying local images..." -ForegroundColor Cyan

$sqlServerImage = docker images -q local/ewftool-dev-sqlserver:latest
$edfiApiImage = docker images -q local/ewftool-dev-edfi-api:latest

if (-not $sqlServerImage) {
    Write-Error "Local image 'local/ewftool-dev-sqlserver:latest' not found. Run without -SkipBuild to build it."
    exit 1
}

if (-not $edfiApiImage) {
    Write-Error "Local image 'local/ewftool-dev-edfi-api:latest' not found. Run without -SkipBuild to build it."
    exit 1
}

Write-Host "Local images found" -ForegroundColor Green

# Tag images for ACR
Write-Host ""
Write-Host "Tagging images for ACR..." -ForegroundColor Cyan

$acrSqlServerImage = "$RegistryLoginServer/ewftool-dev-sqlserver:$Tag"
$acrEdfiApiImage = "$RegistryLoginServer/ewftool-dev-edfi-api:$Tag"

docker tag local/ewftool-dev-sqlserver:latest $acrSqlServerImage
docker tag local/ewftool-dev-edfi-api:latest $acrEdfiApiImage

Write-Host "Images tagged" -ForegroundColor Green

# Push images to ACR
Write-Host ""
Write-Host "Pushing images to ACR..." -ForegroundColor Cyan

Write-Host "  Pushing SQL Server image..." -ForegroundColor Gray
docker push $acrSqlServerImage

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to push SQL Server image"
    exit 1
}

Write-Host "  Pushing Ed-Fi API image..." -ForegroundColor Gray
docker push $acrEdfiApiImage

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to push Ed-Fi API image"
    exit 1
}

Write-Host "Images pushed successfully" -ForegroundColor Green

# Summary
Write-Host ""
Write-Host "=== Push Complete! ===" -ForegroundColor Green
Write-Host ""
Write-Host "Images pushed to $RegistryLoginServer :" -ForegroundColor Cyan
Write-Host "  - ewftool-dev-sqlserver:$Tag" -ForegroundColor Yellow
Write-Host "  - ewftool-dev-edfi-api:$Tag" -ForegroundColor Yellow
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "  1. Update your Container App to use these images" -ForegroundColor Gray
Write-Host "  2. Or wait ~5 minutes for auto-refresh if using :latest tag" -ForegroundColor Gray