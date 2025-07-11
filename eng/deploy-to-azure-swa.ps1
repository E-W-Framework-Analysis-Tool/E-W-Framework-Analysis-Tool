# deploy.ps1
# Deploys a pre-built Blazor WASM application to Azure Static Web Apps

param(
    [Parameter(Mandatory = $true)]
    [string]$DeployToken,

    [string]$AppLocation = "publish/wwwroot",
    
    [string]$ApiLocation = "",
    
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
if ([string]::IsNullOrWhiteSpace($DeployToken)) {
    Fail "Deploy token cannot be empty."
}

if (-not (Test-Path $AppLocation)) {
    Fail "App location not found: $AppLocation"
}

# Check for required tools
if (-not (Get-Command node -ErrorAction SilentlyContinue)) {
    Fail "Node.js is not installed or not found in PATH. Please install Node.js (https://nodejs.org)."
}

if (-not (Get-Command npx -ErrorAction SilentlyContinue)) {
    Fail "'npx' not found. Please install Node.js (https://nodejs.org) to use the Static Web Apps deploy CLI."
}

Write-Host "Node.js version: $(node --version)"
Write-Host "Deploy token: $($DeployToken.Substring(0, 10))..." # Show only first 10 chars for security
Write-Host "App location: $AppLocation"

if (-not [string]::IsNullOrWhiteSpace($ApiLocation)) {
    if (-not (Test-Path $ApiLocation)) {
        Fail "API location not found: $ApiLocation"
    }
    Write-Host "API location: $ApiLocation"
}

Write-Step "Deploying to Azure Static Web Apps"

# Show what we're about to run (without the token)
# NOTE: Production is a term specific to Azure SWAs -- this just means go live immediately. We have a separate SWA per env.
Write-Host "Running: npx @azure/static-web-apps-cli deploy --deployment-token ***TOKEN*** --app-location $AppLocation --env production --output-location .$(if($ShowVerbose){' --verbose'})"

# Execute deployment
Write-Host "`nStarting deployment..."

# Build command arguments as separate parameters
if ($ShowVerbose) {
    npx @azure/static-web-apps-cli deploy --deployment-token $DeployToken --app-location $AppLocation --env production --output-location "." --verbose
} else {
    npx @azure/static-web-apps-cli deploy --deployment-token $DeployToken --app-location $AppLocation --env production --output-location "."
}

# Check exit code
if ($LASTEXITCODE -ne 0) {
    Fail "Deployment failed with exit code $LASTEXITCODE"
}

Write-Host "`n✓ Deployment completed successfully!" -ForegroundColor Green
Write-Host "App deployed from: $AppLocation" -ForegroundColor Gray

if (-not [string]::IsNullOrWhiteSpace($ApiLocation)) {
    Write-Host "API deployed from: $ApiLocation" -ForegroundColor Gray
}

Write-Host "`nYour Static Web App should be live shortly. Check the Azure portal for the URL." -ForegroundColor Green
