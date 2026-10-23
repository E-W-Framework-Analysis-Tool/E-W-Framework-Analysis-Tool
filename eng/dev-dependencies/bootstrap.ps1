#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Stand up a local Ed-Fi ODS API and SQL Server for E-W Framework Analysis Tool development.

.DESCRIPTION
    Brings the dev dependencies up from nothing to a verified, usable Ed-Fi API:

      1. Preflight checks (Docker running, architecture, Azure CLI availability)
      2. Resolves images - pulls from the team's Azure Container Registry when it is
         reachable, otherwise builds them locally from the Ed-Fi NuGet packages
      3. Starts the containers and waits for SQL Server to finish restoring the
         Ed-Fi core databases and templates
      4. Creates the default scenarios (GrandBend and Empty)
      5. Verifies the API end to end by requesting an OAuth token and reading data

    Safe to re-run. Existing scenarios are left alone.

.PARAMETER UseACR
    Force pulling images from the Azure Container Registry. Fails if unavailable
    instead of falling back to a local build.

.PARAMETER Build
    Force building images locally, skipping the registry entirely.

.PARAMETER SkipScenarios
    Start the containers but do not create scenario databases.

.PARAMETER SkipVerification
    Do not exercise the API after startup.

.PARAMETER RemoveConflicting
    Remove containers holding this stack's names that belong to a different compose
    project (for example an older copy of this tooling). Containers only; volumes are
    left untouched.

.PARAMETER RegistryName
    Azure Container Registry short name. Default: ewftool

.PARAMETER ApiPort
    Host port for the Ed-Fi API. Default: 5000. On macOS this collides with the AirPlay
    Receiver; either disable that or pass a different port here. A port passed here is
    saved to .env and reused by later runs and by docker compose.

.PARAMETER SqlPort
    Host port for SQL Server. Default: 14333. Persisted the same way as -ApiPort.

.EXAMPLE
    ./bootstrap.ps1
    Auto-detects the registry, falls back to a local build, verifies the result.

.EXAMPLE
    ./bootstrap.ps1 -Build
    Builds everything locally. No Azure access needed. Takes 10-15 minutes on a cold cache.
#>

param(
    [Parameter(Mandatory = $false)]
    [switch]$UseACR,

    [Parameter(Mandatory = $false)]
    [switch]$Build,

    [Parameter(Mandatory = $false)]
    [switch]$SkipScenarios,

    [Parameter(Mandatory = $false)]
    [switch]$SkipVerification,

    [Parameter(Mandatory = $false)]
    [switch]$RemoveConflicting,

    [Parameter(Mandatory = $false)]
    [string]$RegistryName = "ewftool",

    [Parameter(Mandatory = $false)]
    [int]$ApiPort = 5000,

    [Parameter(Mandatory = $false)]
    [int]$SqlPort = 14333
)

$ErrorActionPreference = "Stop"

# Port selection is per-machine (macOS runs AirPlay on 5000), so it is persisted to
# .env rather than passed on every invocation. Docker Compose loads that file
# automatically, which keeps a bare `docker compose up -d` on the same ports as
# bootstrap. .env is gitignored; .env.example documents the keys.
$envFile = Join-Path $PSScriptRoot ".env"

function Read-EnvFile {
    $values = @{}
    if (Test-Path $envFile) {
        foreach ($line in Get-Content $envFile) {
            if ($line -match '^\s*([A-Za-z_][A-Za-z0-9_]*)\s*=\s*(.*?)\s*$') {
                $values[$Matches[1]] = $Matches[2]
            }
        }
    }
    return $values
}

function Write-EnvFile($values) {
    $lines = @(
        "# Local overrides for docker-compose.yml. Written by bootstrap.ps1.",
        "# Gitignored - this is per-machine configuration."
    )
    foreach ($key in ($values.Keys | Sort-Object)) {
        $lines += "$key=$($values[$key])"
    }
    Set-Content -Path $envFile -Value $lines
}

$envValues = Read-EnvFile

# An explicitly passed port wins and is remembered; otherwise fall back to .env, then
# to the parameter default.
if ($PSBoundParameters.ContainsKey("ApiPort")) {
    $envValues["EDFI_API_PORT"] = $ApiPort
}
elseif ($envValues.ContainsKey("EDFI_API_PORT")) {
    $ApiPort = [int]$envValues["EDFI_API_PORT"]
}

if ($PSBoundParameters.ContainsKey("SqlPort")) {
    $envValues["SQL_PORT"] = $SqlPort
}
elseif ($envValues.ContainsKey("SQL_PORT")) {
    $SqlPort = [int]$envValues["SQL_PORT"]
}

if ($envValues.Count -gt 0) {
    Write-EnvFile $envValues
}

# Also set them in-process so compose sees them even before .env is written.
$env:EDFI_API_PORT = $ApiPort
$env:SQL_PORT = $SqlPort

# Demo/development credentials. Intentionally checked in - this stack holds no real data.
$API_CLIENT_ID = "RvcohKz9zHI4"
$API_CLIENT_SECRET = "E1676E88-4D3B-4E4E-B7B7-7C3F8E5D2A9C"
$SA_PASSWORD = "P@ssw0rd123"
$SQL_CONTAINER = "ewftool-dev-sqlserver"
$API_BASE_URL = "http://localhost:$ApiPort"
$DEFAULT_SCENARIO = "GrandBend"

$SCENARIOS = @(
    @{ Name = "GrandBend"; Template = "populated" }
    @{ Name = "Empty"; Template = "minimal" }
)

Push-Location $PSScriptRoot

function Write-Step($message) { Write-Host "`n$message" -ForegroundColor Cyan }
function Write-Ok($message) { Write-Host "  $message" -ForegroundColor Green }
function Write-Detail($message) { Write-Host "  $message" -ForegroundColor DarkGray }

function Fail($message) {
    Pop-Location
    # Not Write-Error: under $ErrorActionPreference = "Stop" it throws a terminating
    # error and the exit below never runs, leaving the process exit code at 0.
    [Console]::Error.WriteLine("ERROR: $message")
    exit 1
}

function Invoke-Sqlcmd-InContainer($query) {
    docker exec $SQL_CONTAINER /opt/mssql-tools18/bin/sqlcmd `
        -S localhost -U sa -P $SA_PASSWORD -C -Q $query -h -1 2>$null
}

# ---------------------------------------------------------------------------
# 1. Preflight
# ---------------------------------------------------------------------------
Write-Host "=== Bootstrapping Ed-Fi Dev Dependencies ===" -ForegroundColor Green

Write-Step "Checking prerequisites..."

if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    Fail "Docker not found. Install Docker Desktop: https://www.docker.com/products/docker-desktop"
}

docker info 2>&1 | Out-Null
if ($LASTEXITCODE -ne 0) {
    Fail "Docker is not running. Start Docker Desktop and try again."
}
Write-Ok "Docker is running"

# SQL Server ships linux/amd64 images only. On Apple Silicon the container runs under
# emulation, which needs Rosetta enabled in Docker Desktop to perform acceptably.
$isAppleSilicon = $IsMacOS -and ((uname -m) -eq "arm64")
if ($isAppleSilicon) {
    Write-Host "  Apple Silicon detected. SQL Server runs under linux/amd64 emulation." -ForegroundColor Yellow
    Write-Host "  Enable Docker Desktop > Settings > General > 'Use Rosetta for x86_64/amd64 emulation'" -ForegroundColor Yellow
    Write-Host "  if startup is slow or SQL Server crashes on launch." -ForegroundColor Yellow
}

function Test-PortInUse($port) {
    $client = New-Object System.Net.Sockets.TcpClient
    try {
        $connect = $client.BeginConnect("127.0.0.1", $port, $null, $null)
        $inUse = $connect.AsyncWaitHandle.WaitOne(500) -and $client.Connected
        return $inUse
    }
    catch { return $false }
    finally { $client.Close() }
}

# Check host ports before compose does, so the failure names the actual culprit.
$ourContainers = docker ps --filter "name=ewftool-dev" --format "{{.Names}}" 2>$null
foreach ($p in @(@{ Port = $ApiPort; What = "Ed-Fi API"; Param = "-ApiPort" },
                 @{ Port = $SqlPort; What = "SQL Server"; Param = "-SqlPort" })) {
    if ((Test-PortInUse $p.Port) -and -not $ourContainers) {
        Write-Host "`nPort $($p.Port) ($($p.What)) is already in use." -ForegroundColor Red

        if ($IsMacOS -and $p.Port -eq 5000) {
            Write-Host "  On macOS this is usually the AirPlay Receiver. Either turn it off in" -ForegroundColor Yellow
            Write-Host "  System Settings > General > AirDrop & Handoff > AirPlay Receiver," -ForegroundColor Yellow
            Write-Host "  or run this script with a different port." -ForegroundColor Yellow
        }

        Write-Host "`n  Identify the listener:" -ForegroundColor Gray
        if ($IsWindows) {
            Write-Host "    netstat -ano | findstr :$($p.Port)" -ForegroundColor Gray
        }
        else {
            Write-Host "    lsof -nP -iTCP:$($p.Port) -sTCP:LISTEN" -ForegroundColor Gray
        }
        Write-Host "`n  Or pick another port - it is saved to .env and reused from then on," -ForegroundColor Gray
        Write-Host "  by this script and by plain docker compose commands alike:" -ForegroundColor Gray
        Write-Host "    ./bootstrap.ps1 $($p.Param) 5010" -ForegroundColor Gray

        Fail "Port $($p.Port) is not available."
    }
}

# The containers use fixed names, so a stack started from a different directory (an
# older copy of this tooling, say) holds those names and blocks compose from starting.
$thisProject = Split-Path -Leaf $PSScriptRoot
$conflicts = @()
foreach ($name in @($SQL_CONTAINER, "ewftool-dev-edfi-api")) {
    $owner = docker inspect $name --format '{{ index .Config.Labels "com.docker.compose.project" }}' 2>$null
    if ($LASTEXITCODE -eq 0 -and $owner -and $owner.Trim() -ne $thisProject) {
        $conflicts += [PSCustomObject]@{ Name = $name; Project = $owner.Trim() }
    }
}

if ($conflicts.Count -gt 0) {
    if ($RemoveConflicting) {
        Write-Host "  Removing containers from another compose project..." -ForegroundColor Yellow
        foreach ($c in $conflicts) {
            docker rm -f $c.Name | Out-Null
            Write-Detail "removed $($c.Name) (was owned by '$($c.Project)')"
        }
        Write-Ok "Conflicting containers removed"
    }
    else {
        Write-Host "`nContainer names are already taken by another compose project:" -ForegroundColor Red
        foreach ($c in $conflicts) {
            Write-Host "  $($c.Name) belongs to project '$($c.Project)'" -ForegroundColor Red
        }
        Write-Host "`nRe-run with -RemoveConflicting, or remove them yourself:" -ForegroundColor Yellow
        Write-Host "  docker rm -f $(($conflicts.Name) -join ' ')" -ForegroundColor Gray
        Write-Host "`nThis removes the containers only. Their data volumes are left alone." -ForegroundColor DarkGray
        Fail "Cannot start while those container names are in use."
    }
}

if ($UseACR -and $Build) {
    Fail "-UseACR and -Build are mutually exclusive."
}

# ---------------------------------------------------------------------------
# 2. Resolve images: registry when reachable, local build otherwise
# ---------------------------------------------------------------------------
$env:VERSION = "latest"
$usedRegistry = $false

function Test-AcrAvailable {
    if (-not (Get-Command az -ErrorAction SilentlyContinue)) {
        Write-Detail "Azure CLI not installed."
        return $false
    }

    az account show 2>&1 | Out-Null
    if ($LASTEXITCODE -ne 0) {
        Write-Detail "Not logged into Azure (run 'az login')."
        return $false
    }

    az acr login --name $RegistryName 2>&1 | Out-Null
    if ($LASTEXITCODE -ne 0) {
        Write-Detail "Cannot log into registry '$RegistryName' (no access, or it does not exist)."
        return $false
    }

    return $true
}

if ($Build) {
    Write-Step "Building images locally (-Build specified)..."
}
else {
    Write-Step "Resolving images..."
    if (Test-AcrAvailable) {
        $env:REGISTRY = "$RegistryName.azurecr.io"
        Write-Ok "Registry $env:REGISTRY is reachable"
        Write-Host "  Pulling images..." -ForegroundColor Yellow

        docker compose pull
        if ($LASTEXITCODE -eq 0) {
            $usedRegistry = $true
            Write-Ok "Images pulled"
        }
        else {
            Write-Warning "Pull failed. Falling back to a local build."
        }
    }
    elseif ($UseACR) {
        Fail "-UseACR specified but registry '$RegistryName' is not reachable. See messages above."
    }
    else {
        Write-Detail "Registry unavailable, building locally instead."
    }
}

if (-not $usedRegistry) {
    $env:REGISTRY = "local"

    Write-Host "  Building images. First run takes 10-15 minutes" -ForegroundColor Yellow
    if ($isAppleSilicon) {
        Write-Host "  (longer on Apple Silicon - the SQL Server image builds under emulation)." -ForegroundColor Yellow
    }

    docker compose build
    if ($LASTEXITCODE -ne 0) {
        Fail "Image build failed. See the output above."
    }
    Write-Ok "Images built"
}

# ---------------------------------------------------------------------------
# 3. Start containers and wait for the databases to be restored
# ---------------------------------------------------------------------------
Write-Step "Starting containers..."
docker compose up -d
if ($LASTEXITCODE -ne 0) {
    Fail "docker compose up failed. See the output above."
}
Write-Ok "Containers started"

Write-Step "Waiting for SQL Server to accept connections..."
$timeout = 300
$elapsed = 0
$healthy = $false

while ($elapsed -lt $timeout) {
    $health = docker inspect $SQL_CONTAINER --format '{{.State.Health.Status}}' 2>$null
    if ($health -eq "healthy") {
        $healthy = $true
        break
    }
    if ($health -eq "unhealthy") {
        Fail "SQL Server reported unhealthy. Inspect with: docker compose logs sqlserver"
    }
    Start-Sleep -Seconds 5
    $elapsed += 5
    if ($elapsed % 30 -eq 0) { Write-Detail "still waiting... ($elapsed/$timeout s)" }
}

if (-not $healthy) {
    Fail "Timed out waiting for SQL Server. Inspect with: docker compose logs sqlserver"
}
Write-Ok "SQL Server is accepting connections"

# The healthcheck passes as soon as the engine answers, which is well before the
# entrypoint has finished restoring the Ed-Fi databases and seeding the API client.
Write-Step "Waiting for Ed-Fi databases to finish restoring..."
$requiredDatabases = @(
    "EdFi_Admin",
    "EdFi_Security",
    "EdFi_Ods_Populated_Template",
    "EdFi_Ods_Minimal_Template"
)
$dbList = "'" + ($requiredDatabases -join "','") + "'"
$timeout = 600
$elapsed = 0
$restored = $false

while ($elapsed -lt $timeout) {
    $count = Invoke-Sqlcmd-InContainer "SET NOCOUNT ON; SELECT COUNT(*) FROM sys.databases WHERE name IN ($dbList)"
    if ($null -ne $count -and $count.Trim() -eq "$($requiredDatabases.Count)") {
        $restored = $true
        break
    }
    Start-Sleep -Seconds 5
    $elapsed += 5
    if ($elapsed % 30 -eq 0) { Write-Detail "still restoring... ($elapsed/$timeout s)" }
}

if (-not $restored) {
    Fail "Ed-Fi databases were not restored within $timeout s. Inspect with: docker compose logs sqlserver"
}
Write-Ok "Ed-Fi core databases and templates restored"

Write-Step "Waiting for the default API client..."
$timeout = 120
$elapsed = 0
$bootstrapped = $false

while ($elapsed -lt $timeout) {
    $count = Invoke-Sqlcmd-InContainer "SET NOCOUNT ON; SELECT COUNT(*) FROM EdFi_Admin.dbo.ApiClients WHERE [Key] = '$API_CLIENT_ID'"
    if ($null -ne $count -and $count.Trim() -eq "1") {
        $bootstrapped = $true
        break
    }
    Start-Sleep -Seconds 5
    $elapsed += 5
}

if (-not $bootstrapped) {
    Fail "The default API client was not created. Inspect with: docker compose logs sqlserver"
}
Write-Ok "API client '$API_CLIENT_ID' is present"

# ---------------------------------------------------------------------------
# 4. Create scenarios
# ---------------------------------------------------------------------------
if ($SkipScenarios) {
    Write-Step "Skipping scenario creation (-SkipScenarios specified)"
}
else {
    Write-Step "Creating scenarios..."

    foreach ($scenario in $SCENARIOS) {
        $name = $scenario.Name
        $exists = Invoke-Sqlcmd-InContainer "SET NOCOUNT ON; SELECT COUNT(*) FROM EdFi_Admin.dbo.OdsInstances WHERE [Name] = '$name'"

        if ($null -ne $exists -and $exists.Trim() -ne "0") {
            Write-Detail "$name already exists, leaving it as is"
            continue
        }

        & "./database/create-edfi-scenario.ps1" -ScenarioName $name -Template $scenario.Template -ApiPort $ApiPort
        if ($LASTEXITCODE -ne 0) {
            Fail "Failed to create scenario '$name'."
        }
        Write-Ok "$name created"
    }

    # The API caches the ODS instance configuration it read at startup.
    Write-Step "Restarting the API to pick up the new scenarios..."
    docker compose restart webapi | Out-Null
    Write-Ok "API restarted"
}

# ---------------------------------------------------------------------------
# 5. Verify the API end to end
# ---------------------------------------------------------------------------
$tokenUrl = "$API_BASE_URL/$DEFAULT_SCENARIO/oauth/token"
$dataUrl = "$API_BASE_URL/$DEFAULT_SCENARIO/data/v3"

if ($SkipVerification -or $SkipScenarios) {
    Write-Step "Skipping API verification"
}
else {
    Write-Step "Verifying the API..."

    # The API needs a moment after restart before it will serve requests.
    $timeout = 180
    $elapsed = 0
    $token = $null

    while ($elapsed -lt $timeout) {
        try {
            $response = Invoke-RestMethod -Uri $tokenUrl -Method Post -Body @{
                grant_type    = "client_credentials"
                client_id     = $API_CLIENT_ID
                client_secret = $API_CLIENT_SECRET
            } -ContentType "application/x-www-form-urlencoded" -TimeoutSec 15

            if ($response.access_token) {
                $token = $response.access_token
                break
            }
        }
        catch {
            # API still starting, or the scenario route is not registered yet.
        }

        Start-Sleep -Seconds 5
        $elapsed += 5
        if ($elapsed % 30 -eq 0) { Write-Detail "waiting for the API... ($elapsed/$timeout s)" }
    }

    if (-not $token) {
        Fail "Could not obtain an OAuth token from $tokenUrl. Inspect with: docker compose logs webapi"
    }
    Write-Ok "OAuth token issued"

    try {
        $agencies = Invoke-RestMethod -Uri "$dataUrl/ed-fi/localEducationAgencies?limit=25" `
            -Headers @{ Authorization = "Bearer $token" } -TimeoutSec 30
    }
    catch {
        Fail "Token was issued but the data request failed: $($_.Exception.Message)"
    }

    $agencyCount = @($agencies).Count
    if ($agencyCount -eq 0) {
        Write-Warning "The API responded but returned no local education agencies. The '$DEFAULT_SCENARIO' ODS may be empty."
    }
    else {
        Write-Ok "Data request returned $agencyCount local education agenc$(if ($agencyCount -eq 1) { 'y' } else { 'ies' })"
    }
}

# ---------------------------------------------------------------------------
# Summary
# ---------------------------------------------------------------------------
Write-Host "`n=== Bootstrap Complete ===" -ForegroundColor Green

Write-Host "`nImage source:" -ForegroundColor Cyan
if ($usedRegistry) {
    Write-Host "  $env:REGISTRY (pulled)" -ForegroundColor Yellow
}
else {
    Write-Host "  built locally from the Ed-Fi NuGet packages" -ForegroundColor Yellow
}

Write-Host "`nScenarios:" -ForegroundColor Cyan
foreach ($scenario in $SCENARIOS) {
    Write-Host "  $API_BASE_URL/$($scenario.Name)/data/v3/" -ForegroundColor Yellow
}

Write-Host "`nAPI credentials:" -ForegroundColor Cyan
Write-Host "  Client ID:     $API_CLIENT_ID" -ForegroundColor Yellow
Write-Host "  Client Secret: $API_CLIENT_SECRET" -ForegroundColor Yellow

Write-Host "`nSQL Server:" -ForegroundColor Cyan
Write-Host "  Host:     localhost,$SqlPort" -ForegroundColor Yellow
Write-Host "  User:     sa" -ForegroundColor Yellow
Write-Host "  Password: $SA_PASSWORD" -ForegroundColor Yellow

# run-integration.ps1 derives the auth URL from the authority when -AuthUrl is omitted,
# which drops the scenario segment this API routes tokens under. Pass it explicitly.
Write-Host "`nRun the integration tests against this stack:" -ForegroundColor Cyan
Write-Host "  ../run-integration.ps1 ``" -ForegroundColor Gray
Write-Host "    -BaseUrl '$dataUrl' ``" -ForegroundColor Gray
Write-Host "    -AuthUrl '$tokenUrl' ``" -ForegroundColor Gray
Write-Host "    -ClientId '$API_CLIENT_ID' ``" -ForegroundColor Gray
Write-Host "    -ClientSecret '$API_CLIENT_SECRET'" -ForegroundColor Gray

if ($ApiPort -ne 5000 -or $SqlPort -ne 14333) {
    Write-Host "`nPorts saved to .env, so docker compose uses them too:" -ForegroundColor Cyan
    Write-Host "  API $ApiPort, SQL Server $SqlPort" -ForegroundColor Yellow
    Write-Host "  Update @baseUrl in edfi-api-test.http to match." -ForegroundColor DarkGray
}

Write-Host "`nOther useful commands:" -ForegroundColor Cyan
Write-Host "  docker compose logs -f webapi          Tail the API logs" -ForegroundColor Gray
Write-Host "  docker compose down                    Stop, keeping the databases" -ForegroundColor Gray
Write-Host "  docker compose down -v                 Stop and delete all data" -ForegroundColor Gray
Write-Host "  ./database/create-edfi-scenario.ps1 -ScenarioName 'MyScenario'" -ForegroundColor Gray
Write-Host ""

Pop-Location
