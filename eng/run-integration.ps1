param(
  [string]$Configuration = "Release",
  [string]$BaseUrl = "",
  [string]$ClientId = "",
  [string]$ClientSecret = "",
  [string]$AuthUrl = ""
)

$repoRoot = Resolve-Path "$PSScriptRoot/.."
$srcRoot = Join-Path $repoRoot "src"
Push-Location $srcRoot

function Fail($message) {
  Write-Error $message
  Pop-Location
  exit 1
}

function Warning($message) {
  Write-Warning $message
  Pop-Location
  exit 0
}

# Sanity checks
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) { Fail "dotnet CLI not found." }

# Wire env vars for the tests/fixture
if ($BaseUrl) {
  $env:EWFTOOL_EDFI_BASE_URL = $BaseUrl
  Write-Host "EWFTOOL_EDFI_BASE_URL = $BaseUrl"
}
else {
  Warning "BaseUrl has not been defined and integration tests will be skipped."
}

if (-not [string]::IsNullOrEmpty($ClientId) -and -not [string]::IsNullOrEmpty($ClientSecret) ) {
    
  $env:EWFTOOL_EDFI_CLIENT_ID = $ClientId
  $env:EWFTOOL_EDFI_CLIENT_SECRET = $ClientSecret
  Write-Host "EWFTOOL_EDFI_CLIENT_ID = $ClientId"
  Write-Host "EWFTOOL_EDFI_CLIENT_SECRET = $ClientSecret"

}
else {
  Warning "Both -ClientId and -ClientSecret have not been defined, integration tests will be skipped."
}

if (-not [string]::IsNullOrEmpty($AuthUrl)) {
  $env:EWFTOOL_EDFI_AUTH_URL = $AuthUrl
  Write-Host "EWFTOOL_EDFI_AUTH_URL = $AuthUrl"
}
elseif (Test-Path Env:EWFTOOL_EDFI_AUTH_URL) {
  Remove-Item Env:EWFTOOL_EDFI_AUTH_URL
  Write-Host "EWFTOOL_EDFI_AUTH_URL removed"
}

# Run Integration tests project
$integrationProj = "EwFrameworkAnalysis.IntegrationTests\EwFrameworkAnalysis.IntegrationTests.csproj"
Write-Host "`nRunning: dotnet test $integrationProj --configuration $Configuration"
dotnet test $integrationProj --configuration $Configuration --logger "trx;LogFileName=integration.trx" || Fail "Integration tests failed."

Pop-Location
Write-Host "Integration Tests run completed."
