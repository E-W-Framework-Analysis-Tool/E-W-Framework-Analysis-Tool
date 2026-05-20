param(
    [switch]$Check,
    [switch]$Publish,
    [string]$PublishPath = "",
    [string]$Configuration = "Release"
)

$repoRoot = Resolve-Path "$PSScriptRoot/.."
$srcRoot = Join-Path $repoRoot "src"
Push-Location $srcRoot

function Fail($message) {
    Write-Error $message
    Pop-Location
    exit 1
}

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Fail "dotnet CLI is not installed or not found in PATH. Install .NET SDK."
}

# Check for npm and run install if needed
$uiProjectDir = Join-Path $srcRoot "EwFrameworkAnalysis.UI"
if (Test-Path $uiProjectDir) {
    Push-Location $uiProjectDir
    
    if (-not (Get-Command npm -ErrorAction SilentlyContinue)) {
        Write-Warning "npm not found in PATH. Skipping npm install."
    } else {
        # Check if package.json exists
        if (Test-Path "package.json") {
            # Check if node_modules exists and is up to date
            $needsInstall = $false
            if (-not (Test-Path "node_modules")) {
                $needsInstall = $true
                Write-Host "node_modules directory not found."
            } elseif ((Get-Item "package.json").LastWriteTime -gt (Get-Item "node_modules").LastWriteTime) {
                $needsInstall = $true
                Write-Host "package.json is newer than node_modules."
            }
            
            if ($needsInstall) {
                Write-Host "`nRunning: npm install"
                npm install || Fail "npm install failed."
            } else {
                Write-Host "npm dependencies are up to date."
            }
        } else {
            Write-Host "No package.json found in UI project, skipping npm install."
        }
    }
    
    Pop-Location
}

if ($Publish -and [string]::IsNullOrEmpty($PublishPath)) {
    $PublishPath = Join-Path $repoRoot "publish"
}

Write-Host "Building .NET solution from: $srcRoot"
Write-Host "Configuration: $Configuration"

# Step 1: format
if ($Check) {
    Write-Host "`nRunning: dotnet format --verify-no-changes"
    dotnet format --verify-no-changes --verbosity diagnostic || Fail ".NET code formatting check failed."
} else {
    Write-Host "`nRunning: dotnet format"
    $formatOutput = dotnet format 2>&1
    Write-Host $formatOutput
    if ($formatOutput -match "Unable to fix|IDE1006") {
        Write-Host "`nDetected unfixable formatting issues; verifying..." -ForegroundColor Yellow
        dotnet format --verify-no-changes --verbosity diagnostic || Fail ".NET code has unfixable formatting issues."
    }
}

# Step 2: restore
Write-Host "`nRunning: dotnet restore"
dotnet restore || Fail "Failed to restore .NET dependencies."

# Step 3: build
Write-Host "`nRunning: dotnet build --no-restore --configuration $Configuration"
dotnet build --no-restore --configuration $Configuration || Fail ".NET build failed."

# Step 4: unit tests (non-E2E)
Write-Host "`nRunning: dotnet test EwFrameworkAnalysis.UI.UnitTests\EwFrameworkAnalysis.UI.UnitTests.csproj --no-build --configuration $Configuration"
dotnet test EwFrameworkAnalysis.UI.UnitTests\EwFrameworkAnalysis.UI.UnitTests.csproj --no-build --configuration $Configuration || Fail ".NET tests failed."

Write-Host "`nRunning: dotnet test EwFrameworkAnalysis.Common.UnitTests\EwFrameworkAnalysis.Common.UnitTests.csproj --no-build --configuration $Configuration"
dotnet test EwFrameworkAnalysis.Common.UnitTests\EwFrameworkAnalysis.Common.UnitTests.csproj --no-build --configuration $Configuration || Fail ".NET tests failed."

# Step 5: publish (optional)
if ($Publish) {
    Write-Host "`nPublishing to: $PublishPath"
    if (Test-Path $PublishPath) {
        Write-Host "Clearing existing publish directory..."
        Remove-Item -Path $PublishPath -Recurse -Force
    }
    New-Item -ItemType Directory -Path $PublishPath -Force | Out-Null

    $webProjectPath = Join-Path $srcRoot "EwFrameworkAnalysis.UI\EwFrameworkAnalysis.UI.csproj"
    if (-not (Test-Path $webProjectPath)) {
        Fail "Web project not found at: $webProjectPath"
    }

    Write-Host "Running: dotnet publish $webProjectPath --no-build --configuration $Configuration --output $PublishPath"
    dotnet publish $webProjectPath --no-build --configuration $Configuration --output $PublishPath || Fail ".NET publish failed."
    Write-Host "Published successfully to: $PublishPath"

    # Fingerprint CSS/JS files so browsers fetch fresh assets after each deployment
    $publishedWwwRoot = Join-Path $PublishPath "wwwroot"
    $fingerprintScript = Join-Path $PSScriptRoot "fingerprint-static-assets.ps1"
    Write-Host "`nFingerprinting static assets in: $publishedWwwRoot"
    & $fingerprintScript -WwwRootPath $publishedWwwRoot || Fail "Static asset fingerprinting failed."
}

Pop-Location
Write-Host ".NET build completed successfully."
