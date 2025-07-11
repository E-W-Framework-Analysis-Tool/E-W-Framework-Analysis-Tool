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

# Check for dotnet CLI
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Fail "dotnet CLI is not installed or not found in PATH. Please install .NET SDK (https://dotnet.microsoft.com/download)."
}

# Set default publish path if not specified
if ($Publish -and [string]::IsNullOrEmpty($PublishPath)) {
    $PublishPath = Join-Path $repoRoot "publish"
}

Write-Host "Building .NET solution from: $srcRoot"
Write-Host "Configuration: $Configuration"

# Step 1: Check formatting (if requested)
if ($Check) {
    Write-Host "`nRunning: dotnet format --verify-no-changes"
    dotnet format --verify-no-changes --verbosity diagnostic || Fail ".NET code formatting check failed. Run 'dotnet format' to fix formatting issues."
} else {
    Write-Host "`nRunning: dotnet format"
    $formatOutput = dotnet format 2>&1
    
    # Show the output to console
    Write-Host $formatOutput
    
    # Check if there were unfixable issues
    if ($formatOutput -match "Unable to fix|IDE1006") {
        Write-Host "`nDetected unfixable formatting issues. Running verification to get details..." -ForegroundColor Yellow
        Write-Host "`nRunning: dotnet format --verify-no-changes --verbosity diagnostic"
        dotnet format --verify-no-changes --verbosity diagnostic || Fail ".NET code has unfixable formatting issues that must be manually resolved."
    }
}

# Step 2: Restore dependencies
Write-Host "`nRunning: dotnet restore"
dotnet restore || Fail "Failed to restore .NET dependencies."

# Step 3: Build solution
Write-Host "`nRunning: dotnet build --no-restore --configuration $Configuration"
dotnet build --no-restore --configuration $Configuration || Fail ".NET build failed."

# Step 4: Run tests
Write-Host "`nRunning: dotnet test --no-build --filter 'TestPhase=OnBuild' --configuration $Configuration"
dotnet test --no-build --filter "TestPhase=OnBuild" --configuration $Configuration || Fail ".NET tests failed."

# Step 5: Publish (if requested)
if ($Publish) {
    Write-Host "`nPublishing to: $PublishPath"
    
    # Clear and create publish directory
    if (Test-Path $PublishPath) {
        Write-Host "Clearing existing publish directory..."
        Remove-Item -Path $PublishPath -Recurse -Force
    }
    New-Item -ItemType Directory -Path $PublishPath -Force | Out-Null
    
    $webProjectPath = Join-Path $srcRoot "EwFrameworkAnalysis.Blazor\EwFrameworkAnalysis.Blazor.csproj"
    
    # Verify the project exists
    if (-not (Test-Path $webProjectPath)) {
        Fail "Web project not found at: $webProjectPath"
    }
    
    Write-Host "Running: dotnet publish $webProjectPath --no-build --configuration $Configuration --output $PublishPath"
    dotnet publish $webProjectPath --no-build --configuration $Configuration --output $PublishPath || Fail ".NET publish failed."
    
    Write-Host "Published successfully to: $PublishPath"
}

# Step 6: Run Playwright tests 
if ($Publish) {
    Write-Host "`nRunning: `nPUBLISH_PATH = $PublishPath `ndotnet test --no-build --filter 'TestPhase=PostPublish' --configuration $Configuration"
    $env:PUBLISH_PATH = $PublishPath
    dotnet test --no-build --filter "TestPhase=PostPublish" --configuration $Configuration || Fail ".NET Playwright tests failed."
}

Pop-Location
Write-Host ".NET build completed successfully."
