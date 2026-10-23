#!/usr/bin/env pwsh
# Create Ed-Fi Scenario
param(
    [Parameter(Mandatory=$true)]
    [string]$ScenarioName,
    
    [Parameter(Mandatory=$false)]
    [ValidateSet("minimal", "populated")]
    [string]$Template = "populated",
    
    [Parameter(Mandatory=$false)]
    [string]$ApiClientKey = "RvcohKz9zHI4",
    
    [Parameter(Mandatory=$false)]
    [string]$ContainerName = "ewftool-dev-sqlserver",

    # Only affects the URLs printed below; the scenario itself is port-independent.
    [Parameter(Mandatory=$false)]
    [int]$ApiPort = 5000
)

$username = "sa"
$password = "P@ssw0rd123"

$cleanScenarioName = $ScenarioName -replace '\s+', ''
$newOdsDbName = "EdFi_Ods_$cleanScenarioName"
$templateDbName = if ($Template -eq "minimal") { "EdFi_Ods_Minimal_Template" } else { "EdFi_Ods_Populated_Template" }

Write-Host ""
Write-Host "Creating scenario: $ScenarioName" -ForegroundColor Green
Write-Host "Template: $templateDbName" -ForegroundColor Yellow
Write-Host "New database: $newOdsDbName" -ForegroundColor Yellow
Write-Host ""

# Step 1: Check if scenario already exists
Write-Host "Step 1: Checking if scenario exists..." -ForegroundColor Cyan
$checkQuery = "SET NOCOUNT ON; SELECT COUNT(*) FROM EdFi_Admin.dbo.OdsInstances WHERE [Name] = '$ScenarioName'"
$exists = docker exec $ContainerName /opt/mssql-tools18/bin/sqlcmd -S localhost -U $username -P $password -C -Q $checkQuery -h -1 2>$null

if ($exists.Trim() -ne "0") {
    Write-Error "Scenario '$ScenarioName' already exists. Delete it first or choose a different name."
    exit 1
}
Write-Host "✓ Scenario name is available" -ForegroundColor Green

# Step 2: Copy template database using BACKUP/RESTORE
Write-Host ""
Write-Host "Step 2: Copying template database..." -ForegroundColor Cyan

$copyDbScript = @"
USE master;
GO

-- Backup template
BACKUP DATABASE [$templateDbName] 
TO DISK = '/var/opt/mssql/data/temp_scenario.bak' 
WITH INIT, FORMAT;

-- Get logical file names
DECLARE @DataFile NVARCHAR(255);
DECLARE @LogFile NVARCHAR(255);

SELECT @DataFile = name FROM sys.master_files 
WHERE database_id = DB_ID('$templateDbName') AND type = 0;

SELECT @LogFile = name FROM sys.master_files 
WHERE database_id = DB_ID('$templateDbName') AND type = 1;

-- Restore to new database
DECLARE @RestoreCmd NVARCHAR(MAX) = 
    'RESTORE DATABASE [$newOdsDbName] FROM DISK = ''/var/opt/mssql/data/temp_scenario.bak'' ' +
    'WITH REPLACE, ' +
    'MOVE ''' + @DataFile + ''' TO ''/var/opt/mssql/data/${newOdsDbName}.mdf'', ' +
    'MOVE ''' + @LogFile + ''' TO ''/var/opt/mssql/data/${newOdsDbName}_log.ldf''';

EXEC sp_executesql @RestoreCmd;

PRINT 'Database copied successfully';
GO
"@

docker exec $ContainerName /opt/mssql-tools18/bin/sqlcmd -S localhost -U $username -P $password -C -Q $copyDbScript 2>$null

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to copy template database"
    exit 1
}
Write-Host "✓ Template database copied to $newOdsDbName" -ForegroundColor Green

# Step 3: Create OdsInstance record
Write-Host ""
Write-Host "Step 3: Creating OdsInstance record..." -ForegroundColor Cyan

$odsConnectionString = "Server=$ContainerName;Database=$newOdsDbName;User ID=sa;Password=$password;TrustServerCertificate=true;"

$createOdsInstanceQuery = @"
USE EdFi_Admin;
GO

DECLARE @OdsInstanceId INT;

-- Create ODS Instance
INSERT INTO dbo.OdsInstances ([Name], InstanceType, ConnectionString)
VALUES ('$ScenarioName', 'Sandbox', '$odsConnectionString');

SELECT @OdsInstanceId = OdsInstanceId FROM dbo.OdsInstances WHERE [Name] = '$ScenarioName';

PRINT 'OdsInstance created with ID: ' + CAST(@OdsInstanceId AS VARCHAR);
GO
"@

docker exec $ContainerName /opt/mssql-tools18/bin/sqlcmd -S localhost -U $username -P $password -C -Q $createOdsInstanceQuery 2>$null

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to create OdsInstance"
    exit 1
}
Write-Host "✓ OdsInstance created" -ForegroundColor Green

# Step 4: Create OdsInstanceContext record
Write-Host ""
Write-Host "Step 4: Creating OdsInstanceContext record..." -ForegroundColor Cyan

$createContextQuery = @"
USE EdFi_Admin;
GO

DECLARE @OdsInstanceId INT;

SELECT @OdsInstanceId = OdsInstanceId FROM dbo.OdsInstances WHERE [Name] = '$ScenarioName';

-- Create ODS Instance Context
INSERT INTO dbo.OdsInstanceContexts (OdsInstance_OdsInstanceId, ContextKey, ContextValue)
VALUES (@OdsInstanceId, 'Scenario', '$cleanScenarioName');

PRINT 'OdsInstanceContext created';
GO
"@

docker exec $ContainerName /opt/mssql-tools18/bin/sqlcmd -S localhost -U $username -P $password -C -Q $createContextQuery 2>$null

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to create OdsInstanceContext"
    exit 1
}
Write-Host "✓ OdsInstanceContext created (Scenario = $cleanScenarioName)" -ForegroundColor Green

# Step 5: Link API client to ODS instance
Write-Host ""
Write-Host "Step 5: Linking API client to ODS instance..." -ForegroundColor Cyan

$linkApiClientQuery = @"
USE EdFi_Admin;
GO

DECLARE @ApiClientId INT;
DECLARE @OdsInstanceId INT;

SELECT @ApiClientId = ApiClientId FROM dbo.ApiClients WHERE [Key] = '$ApiClientKey';
SELECT @OdsInstanceId = OdsInstanceId FROM dbo.OdsInstances WHERE [Name] = '$ScenarioName';

-- Verify API client exists
IF @ApiClientId IS NULL
BEGIN
    RAISERROR('API Client not found. Run bootstrap first.', 16, 1);
    RETURN;
END

-- Create API Client ODS Instance link
INSERT INTO dbo.ApiClientOdsInstances (ApiClient_ApiClientId, OdsInstance_OdsInstanceId)
VALUES (@ApiClientId, @OdsInstanceId);

PRINT 'API Client linked to ODS Instance';
GO
"@

docker exec $ContainerName /opt/mssql-tools18/bin/sqlcmd -S localhost -U $username -P $password -C -Q $linkApiClientQuery 2>$null

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to link API client to ODS instance"
    exit 1
}
Write-Host "✓ API client linked to scenario" -ForegroundColor Green

# Clean up temp backup file
Write-Host ""
Write-Host "Cleaning up..." -ForegroundColor Cyan
docker exec $ContainerName rm /var/opt/mssql/data/temp_scenario.bak 2>$null

Write-Host ""
Write-Host "=== Scenario '$ScenarioName' Created Successfully! ===" -ForegroundColor Green
Write-Host ""
Write-Host "Database: $newOdsDbName" -ForegroundColor Yellow
Write-Host "API URL: http://localhost:$ApiPort/$cleanScenarioName/data/v3/" -ForegroundColor Yellow
Write-Host "Context: Scenario = $cleanScenarioName" -ForegroundColor Yellow
Write-Host ""
Write-Host "Use existing API credentials:" -ForegroundColor Cyan
Write-Host "  Client ID: $ApiClientKey" -ForegroundColor Yellow
Write-Host "  Client Secret: E1676E88-4D3B-4E4E-B7B7-7C3F8E5D2A9C" -ForegroundColor Yellow
Write-Host ""
