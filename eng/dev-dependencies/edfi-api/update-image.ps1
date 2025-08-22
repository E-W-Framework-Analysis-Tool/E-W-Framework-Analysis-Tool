#!/usr/bin/env pwsh
# Promote EdFi API image to ACR registry
param(
    [Parameter(Mandatory=$false)]
    [string]$Version,
    
    [Parameter(Mandatory=$false)]
    [string]$Registry = "ewftool.azurecr.io",
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipPush
)

# Generate version if not provided
if (-not $Version) {
    $Version = Get-Date -Format "yyyy.MM.dd.HHmm"
}

$imageName = "$Registry/ewftool-dev-edfi-api"

Write-Host "=== Promoting Ed-Fi API Image ===" -ForegroundColor Green
Write-Host "Version: $Version"
Write-Host "Registry: $Registry"
Write-Host ""

# Step 1: Build API image
Write-Host "Step 1: Building EdFi API image..." -ForegroundColor Cyan

docker build -t "ewftool-dev-edfi-api:$Version" -t "ewftool-dev-edfi-api:latest" .

if ($LASTEXITCODE -ne 0) {
    Pop-Location
    Write-Error "Failed to build EdFi API image"
    exit 1
}

Write-Host "✓ EdFi API image built successfully" -ForegroundColor Green
Write-Host ""

# Step 2: Tag images
Write-Host "Step 2: Tagging image..." -ForegroundColor Cyan

docker tag "ewftool-dev-edfi-api:$Version" "$imageName`:$Version"
docker tag "ewftool-dev-edfi-api:latest" "$imageName`:latest"

Write-Host "✓ Image tagged successfully" -ForegroundColor Green
Write-Host ""

# Step 3: Push image (unless skipped)
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
    # Push image
    Write-Host "  Pushing EdFi API image..." -ForegroundColor Yellow
    docker push "$imageName`:$Version"
    docker push "$imageName`:latest"
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to push EdFi API image"
        exit 1
    }
    
    Write-Host "✓ Image pushed successfully" -ForegroundColor Green
}
else {
    Write-Host "Step 3: Skipping image push" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "=== EdFi API Promotion Complete! ===" -ForegroundColor Green
Write-Host "✓ Version: $Version"
Write-Host "✓ Image: $imageName`:$Version"
Write-Host "✓ Latest tag updated"
