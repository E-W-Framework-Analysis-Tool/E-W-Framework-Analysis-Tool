param(
    [string]$Configuration = "Release",
    [string]$PublishPath = "",
    [string]$BaseUrl = "",
    [switch]$AllBrowsers,              # if set: run FF/WebKit too
    [string]$A11yFailLevel = "serious" # critical|serious|moderate
)

$repoRoot = Resolve-Path "$PSScriptRoot/.."
$srcRoot = Join-Path $repoRoot "src"
Push-Location $srcRoot

if (-not $BaseUrl -and [string]::IsNullOrEmpty($PublishPath)) {
    $PublishPath = Join-Path $repoRoot "publish"
}

function Fail($message) {
    Write-Error $message
    Pop-Location
    exit 1
}

# Sanity checks
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) { Fail "dotnet CLI not found." }

# Wire env vars for the tests/fixture
if ($BaseUrl) {
    $env:EWFTOOL_E2E_BASE_URL = $BaseUrl
    Write-Host "EWFTOOL_E2E_BASE_URL = $BaseUrl"
}
elseif ($PublishPath) {
    if (-not (Test-Path (Join-Path $PublishPath 'wwwroot'))) {
        Fail "PublishPath missing wwwroot: $PublishPath"
    }
    $env:EWFTOOL_E2E_PUBLISH_PATH = $PublishPath
    Write-Host "EWFTOOL_E2E_PUBLISH_PATH = $PublishPath"
}
else {
    Fail "Provide either -BaseUrl or -PublishPath."
}

$env:EWFTOOL_E2E_HEADLESS = "true"
$env:EWFTOOL_E2E_ALL_BROWSERS = $(if ($AllBrowsers) { "true" } else { "false" })
$env:EWFTOOL_A11Y_FAIL_LEVEL = $A11yFailLevel

# Run E2E tests project
$e2eProj = "EwFrameworkAnalysis.Blazor.E2ETests\EwFrameworkAnalysis.Blazor.E2ETests.csproj"
Write-Host "`nRunning: dotnet test $e2eProj --configuration $Configuration"

# Temporary log file
$summaryLog = "e2e-summary.log"

dotnet test $e2eProj --configuration $Configuration `
    --logger "trx;LogFileName=e2e.trx" `
    --logger "console;verbosity=normal" `
| Select-String ">>> " ` # Clear out any error stack trace  
| Select-String -NotMatch "xUnit.net " ` # Remove duplicate printing of messages from output.WriteLine
| ForEach-Object { $_.Line -replace ">>> ", "" }` # Remove string that identifies messages to display
| Tee-Object -FilePath $summaryLog  # save console output

$testExitCode = $LASTEXITCODE

# Append to GitHub summary if running inside GitHub Actions
if ($env:GITHUB_STEP_SUMMARY) {
    Write-Host "Writing test summary to GitHub step summary..."
    Add-Content -Path $env:GITHUB_STEP_SUMMARY -Value "### E2E Test Results"    
    Get-Content $summaryLog | Add-Content -Path $env:GITHUB_STEP_SUMMARY
}
 
if ($testExitCode -ne 0) {
    Fail "Playwright E2E tests failed."
}

Pop-Location
Write-Host "E2E run completed."
