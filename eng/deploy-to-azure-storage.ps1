# Deploys a pre-built Blazor WASM application to Azure Storage Account Static Website

param(
    [Parameter(Mandatory = $true)]
    [string]$StorageAccountName,

    [Parameter(Mandatory = $true)]
    [string]$ResourceGroupName,

    [string]$AppLocation = "publish/wwwroot",
    
    [switch]$ShowVerbose
)

function Fail($message) {
    Write-Error $message
    exit 1
}

function Write-Step($message) {
    Write-Host "`n=== $message ===" -ForegroundColor Cyan
}

# Validate inputs
if ([string]::IsNullOrWhiteSpace($StorageAccountName)) {
    Fail "Storage account name cannot be empty."
}

if ([string]::IsNullOrWhiteSpace($ResourceGroupName)) {
    Fail "Resource group name cannot be empty."
}

if (-not (Test-Path $AppLocation)) {
    Fail "App location not found: $AppLocation"
}

# Check for Azure CLI
if (-not (Get-Command az -ErrorAction SilentlyContinue)) {
    Fail "Azure CLI is not installed or not found in PATH. Please install Azure CLI (https://docs.microsoft.com/cli/azure/install-azure-cli)."
}

Write-Host "Azure CLI version: $(az version --query '\"azure-cli\"' -o tsv)"
Write-Host "Storage account: $StorageAccountName"
Write-Host "Resource group: $ResourceGroupName"
Write-Host "App location: $AppLocation"

Write-Step "Verifying Azure login"

# Check if logged in
$accountInfo = az account show 2>$null
if ($LASTEXITCODE -ne 0) {
    Fail "Not logged in to Azure. Please run 'az login' first."
}

$accountName = az account show --query name -o tsv
Write-Host "Logged in as: $accountName" -ForegroundColor Green

Write-Step "Verifying storage account"

# Verify storage account exists
$storageExists = az storage account show --name $StorageAccountName --resource-group $ResourceGroupName 2>$null
if ($LASTEXITCODE -ne 0) {
    Fail "Storage account '$StorageAccountName' not found in resource group '$ResourceGroupName'."
}

Write-Host "✓ Storage account verified" -ForegroundColor Green

Write-Step "Deploying files to `$web container"

# Get files to upload
$files = Get-ChildItem -Path $AppLocation -Recurse -File
Write-Host "Found $($files.Count) files to upload"

# Upload all files to $web container
# Using --overwrite to replace existing files
# Using --content-cache-control for proper caching headers
Write-Host "`nUploading files..."

# Note: We need to call az directly with proper escaping, not use splatting
# because PowerShell array splatting doesn't work well with external commands
if ($ShowVerbose) {
    az storage blob upload-batch `
        --account-name $StorageAccountName `
        --auth-mode login `
        --destination "`$web" `
        --source $AppLocation `
        --overwrite `
        --pattern "*" `
        --content-cache-control "public, max-age=31536000, immutable" `
        --verbose
} else {
    az storage blob upload-batch `
        --account-name $StorageAccountName `
        --auth-mode login `
        --destination "`$web" `
        --source $AppLocation `
        --overwrite `
        --pattern "*" `
        --content-cache-control "public, max-age=31536000, immutable" `
        --no-progress
}

if ($LASTEXITCODE -ne 0) {
    Fail "File upload failed with exit code $LASTEXITCODE"
}

# Upload index.html with no-cache headers (separate to override caching)
Write-Host "`nUpdating cache headers for entry point files..."
az storage blob upload `
    --account-name $StorageAccountName `
    --auth-mode login `
    --container-name "`$web" `
    --file "$AppLocation/index.html" `
    --name "index.html" `
    --content-cache-control "no-cache, no-store, must-revalidate" `
    --overwrite `
    $(if($ShowVerbose){"--verbose"})

if ($LASTEXITCODE -ne 0) {
    Write-Warning "Failed to update cache headers for index.html"
}

Write-Step "Purging CDN cache (if configured)"

# Attempt to find and purge CDN endpoint
# This is optional - if no CDN exists, we just skip it
$cdnProfiles = az cdn profile list --resource-group $ResourceGroupName --query "[].name" -o tsv 2>$null

if ($cdnProfiles -and $LASTEXITCODE -eq 0) {
    foreach ($profile in $cdnProfiles -split "`n") {
        if ([string]::IsNullOrWhiteSpace($profile)) { continue }
        
        $endpoints = az cdn endpoint list --profile-name $profile --resource-group $ResourceGroupName --query "[?originHostHeader=='$StorageAccountName.z13.web.core.windows.net'].name" -o tsv 2>$null
        
        if ($endpoints -and $LASTEXITCODE -eq 0) {
            foreach ($endpoint in $endpoints -split "`n") {
                if ([string]::IsNullOrWhiteSpace($endpoint)) { continue }
                
                Write-Host "Purging CDN endpoint: $endpoint"
                az cdn endpoint purge `
                    --resource-group $ResourceGroupName `
                    --profile-name $profile `
                    --name $endpoint `
                    --content-paths "/*" `
                    --no-wait
                
                if ($LASTEXITCODE -eq 0) {
                    Write-Host "✓ CDN purge initiated for $endpoint" -ForegroundColor Green
                }
            }
        }
    }
} else {
    Write-Host "No CDN profiles found or configured - skipping cache purge" -ForegroundColor Gray
}

Write-Step "Deployment Summary"

# Get the static website endpoint
$webEndpoint = az storage account show `
    --name $StorageAccountName `
    --resource-group $ResourceGroupName `
    --query "primaryEndpoints.web" `
    -o tsv

Write-Host "`n✓ Deployment completed successfully!" -ForegroundColor Green
Write-Host "Files deployed from: $AppLocation" -ForegroundColor Gray
Write-Host "Storage account: $StorageAccountName" -ForegroundColor Gray

if ($webEndpoint) {
    Write-Host "`nWebsite URL: $webEndpoint" -ForegroundColor Cyan
    Write-Host "Your app should be live at the URL above." -ForegroundColor Green
} else {
    Write-Host "`nUnable to retrieve website URL. Check Azure portal for the static website endpoint." -ForegroundColor Yellow
}

Write-Host "`nNote: If you have a CDN configured, changes may take a few minutes to propagate." -ForegroundColor Gray