param(
  [string]$Configuration = "Release",
  [string]$BaseUrl = "",
  [string]$ClientId = "",
  [string]$ClientSecret = "",
  [string]$AccessToken = "",
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

# Sanity checks
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) { Fail "dotnet CLI not found." }

# Wire env vars for the tests/fixture
if ($BaseUrl) {
  $env:EWFTOOL_EDFI_BASE_URL = $BaseUrl
  Write-Host "EWFTOOL_EDFI_BASE_URL = $BaseUrl"
}
else {
  Fail "Provide -BaseUrl."
}

if (-not [string]::IsNullOrEmpty($ClientId) -and -not [string]::IsNullOrEmpty($ClientSecret) ) {
    
  $env:EWFTOOL_EDFI_CLIENT_ID = $ClientId
  $env:EWFTOOL_EDFI_CLIENT_SECRET = $ClientSecret
  Write-Host "EWFTOOL_EDFI_CLIENT_ID = $ClientId"
  Write-Host "EWFTOOL_EDFI_CLIENT_SECRET = $ClientSecret"

}
elseif (-not [string]::IsNullOrEmpty($AccessToken)) {
  
  $env:EWFTOOL_EDFI_ACCESS_TOKEN = $AccessToken
  Write-Host "EWFTOOL_EDFI_ACCESS_TOKEN = $AccessToken"

}
else {
  Fail "Provide -ClientId and -ClientSecret or -AccessToken."
}


# Run Integration tests project
$integrationProj = "EwFrameworkAnalysis.Blazor.IntegrationTests\EwFrameworkAnalysis.Blazor.IntegrationTests.csproj"
Write-Host "`nRunning: dotnet test $integrationProj --configuration $Configuration"
dotnet test $integrationProj --configuration $Configuration --logger "trx;LogFileName=integration.trx" || Fail "Integration tests failed."

Pop-Location
Write-Host "Integration Tests run completed."
