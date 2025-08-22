#!/usr/bin/env pwsh
# Promote SQL Server image to ACR registry
param(
    [Parameter(Mandatory=$false)]
    [string]$Version,
    
    [Parameter(Mandatory=$false)]
    [string]$Registry = "ewftool.azurecr.io",
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipBackup,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipPush
)

# Generate version if not provided
if (-not $Version) {
    $Version = Get-Date -Format "yyyy.MM.dd.HHmm"
}

$imageName = "$Registry/ewftool-dev-sqlserver"

Write-Host "=== Promoting SQL Server Image ===" -ForegroundColor Green
Write-Host "Version: $Version"
Write-Host "Registry: $Registry"
Write-Host ""

Write-Host "Step 1: Building SQL Server image..." -ForegroundColor Cyan

docker build -f Dockerfile -t "ewftool-dev-sqlserver:$Version" -t "ewftool-dev-sqlserver:latest" .

if ($LASTEXITCODE -ne 0) {
    Pop-Location
    Write-Error "Failed to build SQL Server image"
    exit 1
}

Write-Host "✓ SQL Server image built successfully" -ForegroundColor Green
Write-Host ""

Write-Host "Step 2: Tagging image..." -ForegroundColor Cyan

docker tag "ewftool-dev-sqlserver:$Version" "$imageName`:$Version"
docker tag "ewftool-dev-sqlserver:latest" "$imageName`:latest"

Write-Host "✓ Image tagged successfully" -ForegroundColor Green
Write-Host ""

if (-not $SkipPush) {
    Write-Host "Step 3: Pushing image to ACR..." -ForegroundColor Cyan
    
    # Check if logged into ACR
    Write-Host "  Checking ACR login..." -ForegroundColor Yellow
    
    # First check if Azure CLI is available
    $azCliCheck = Get-Command az -ErrorAction SilentlyContinue
    if (-not $azCliCheck) {
        Write-Warning "Azure CLI not found. Please install Azure CLI and login: az acr login --name ewftool"
        Write-Host "Continuing without push..." -ForegroundColor Yellow
        $acrLoginCheck = $false
    } else {
        # Try a quick token check with timeout
        try {
            $timeoutJob = Start-Job -ScriptBlock { 
                az acr check-health --name ewftool --output json 2>$null 
            }
            $completed = Wait-Job $timeoutJob -Timeout 10
            
            if ($completed) {
                $result = Receive-Job $timeoutJob
                $acrLoginCheck = $result -and ($result | ConvertFrom-Json -ErrorAction SilentlyContinue)
                Remove-Job $timeoutJob
            } else {
                Stop-Job $timeoutJob
                Remove-Job $timeoutJob
                Write-Warning "ACR health check timed out. Please ensure you're logged in: az acr login --name ewftool"
                $acrLoginCheck = $false
            }
        } catch {
            Write-Warning "Could not verify ACR login. Please ensure you're logged in: az acr login --name ewftool"
            $acrLoginCheck = $false
        }
    }
    Write-Host "  Pushing SQL Server image..." -ForegroundColor Yellow
    docker push "$imageName`:$Version"
    docker push "$imageName`:latest"
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to push SQL Server image"
        exit 1
    }
    
    Write-Host "✓ Image pushed successfully" -ForegroundColor Green
} else {
    Write-Host "Step 3: Skipping image push" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "=== SQL Server Promotion Complete! ===" -ForegroundColor Green
Write-Host "✓ Version: $Version"
Write-Host "✓ Image: $imageName`:$Version"
Write-Host "✓ Latest tag updated"
