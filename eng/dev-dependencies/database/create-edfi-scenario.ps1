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
    [string]$ApiClientSecret = "E1676E88-4D3B-4E4E-B7B7-7C3F8E5D2A9C",
    
    [Parameter(Mandatory=$false)]
    [string]$VendorName = "Test Vendor",
    
    [Parameter(Mandatory=$false)]
    [string]$ApplicationName = "Test Application",

    [Parameter(Mandatory=$false)]
    [string]$ContainerName = "ewftool-dev-sqlserver"
)

$server = "localhost,14333"
$username = "sa"
$password = "P@ssw0rd123"

$cleanScenarioName = $ScenarioName -replace '\s+', ''
$newOdsDbName = "EdFi_Ods_$cleanScenarioName"
$templateDbName = if ($Template -eq "minimal") { "EdFi_Ods_Minimal_Template" } else { "EdFi_Ods_Populated_Template" }

Write-Host "Creating scenario: $ScenarioName"
Write-Host "Template: $Template ($templateDbName)"
Write-Host "New ODS DB: $newOdsDbName"

# 1. Find or create template backup
Write-Host "Step 1: Locating template backup..."

# Determine which backup file to use based on template (using underscore naming)
$backupFileName = if ($Template -eq "minimal") { 
    "EdFi_Ods_Minimal_Template.bak" 
} else { 
    "EdFi_Ods_Populated_Template.bak" 
}

# Check if backup exists in ./backups folder
$backupFilePath = Join-Path "./backups" $backupFileName

if (Test-Path $backupFilePath) {
    Write-Host "Found existing backup: $backupFilePath" -ForegroundColor Green
} else {
    Write-Host "Backup not found in ./backups/, attempting to create from running database..." -ForegroundColor Yellow
    
    # Check if template database exists in container
    $checkDbQuery = "SELECT COUNT(*) FROM sys.databases WHERE name = '$templateDbName'"
    $dbExists = docker exec $ContainerName /opt/mssql-tools18/bin/sqlcmd -S localhost -U $username -P $password -C -Q $checkDbQuery -h -1
    
    if ($dbExists.Trim() -eq "0") {
        Write-Error "Template database '$templateDbName' not found in container. Please ensure the SQL Server image has been built with template databases."
        exit 1
    }
    
    Write-Host "Creating backup of $templateDbName..." -ForegroundColor Cyan
    
    # Use backup-database.ps1 to create the backup
    & "./backup-database.ps1" -DatabaseName $templateDbName -BackupFileName $backupFileName -ContainerName $ContainerName
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to create backup of template database"
        exit 1
    }
    
    Write-Host "✓ Template backup created" -ForegroundColor Green
}

# 2. Restore template to new scenario database
Write-Host "Step 2: Creating scenario database from template..."

# Use restore-database.ps1 script to create the new database
& "./restore-database.ps1" -BackupFilePath $backupFilePath -NewDatabaseName $newOdsDbName -LogicalDataName "EdFi_Ods_Populated_Template_Test" -LogicalLogName "EdFi_Ods_Populated_Template_Test_log" -ContainerName $ContainerName

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to restore template database to new scenario"
    exit 1
}

# 3. Bootstrap Admin database if needed (one-time setup)
Write-Host "Step 3: Checking if bootstrap is needed..."
$checkBootstrapQuery = "SELECT COUNT(*) FROM EdFi_Admin.dbo.Vendors WHERE VendorName = '$VendorName'"
$vendorExists = docker exec $ContainerName /opt/mssql-tools18/bin/sqlcmd -S localhost -U $username -P $password -C -Q $checkBootstrapQuery -h -1

if ($vendorExists.Trim() -eq "0") {
    Write-Host "Bootstrapping Admin database..." -ForegroundColor Cyan
    $bootstrapQuery = @"
USE EdFi_Admin;

DECLARE @VendorId INT;
DECLARE @ApplicationId INT;

-- Create Vendor
INSERT INTO dbo.Vendors (VendorName) VALUES ('$VendorName');
SELECT @VendorId = VendorId FROM dbo.Vendors WHERE VendorName = '$VendorName';

-- Create Vendor Namespace Prefix
INSERT INTO dbo.VendorNamespacePrefixes (NamespacePrefix, Vendor_VendorId)
VALUES ('uri://ed-fi.org', @VendorId);

-- Create Application
INSERT INTO dbo.Applications (ApplicationName, OperationalContextUri, Vendor_VendorId, ClaimSetName)
VALUES ('$ApplicationName', 'uri://ed-fi.org', @VendorId, 'Ed-Fi Sandbox');

SELECT @ApplicationId = ApplicationId FROM dbo.Applications WHERE ApplicationName = '$ApplicationName' AND Vendor_VendorId = @VendorId;

-- Create Education Organization
INSERT INTO dbo.ApplicationEducationOrganizations (EducationOrganizationId, Application_ApplicationId)
VALUES (255901001, @ApplicationId);

-- Create API Client
INSERT INTO dbo.ApiClients ([Key], [Secret], [Name], IsApproved, UseSandbox, SandboxType, SecretIsHashed, Application_ApplicationId)
VALUES ('$ApiClientKey', '$ApiClientSecret', 'Default API Client', 1, 0, 0, 0, @ApplicationId);

-- Link API Client to Education Organizations
INSERT INTO dbo.ApiClientApplicationEducationOrganizations (ApiClient_ApiClientId, ApplicationEducationOrganization_ApplicationEducationOrganizationId)
SELECT ac.ApiClientId, aeo.ApplicationEducationOrganizationId
FROM dbo.ApiClients ac
CROSS JOIN dbo.ApplicationEducationOrganizations aeo
INNER JOIN dbo.Applications a ON aeo.Application_ApplicationId = a.ApplicationId
WHERE ac.[Key] = '$ApiClientKey' AND a.ApplicationName = '$ApplicationName';

PRINT 'Bootstrap completed';
"@

    docker exec $ContainerName /opt/mssql-tools18/bin/sqlcmd -S localhost -U $username -P $password -C -Q $bootstrapQuery
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to bootstrap Admin database"
        exit 1
    }
    Write-Host "✓ Bootstrap completed" -ForegroundColor Green
}
else {
    Write-Host "Bootstrap already exists, skipping..." -ForegroundColor Yellow
}

# 4. Create OdsInstance and OdsInstanceContext records
Write-Host "Step 4: Creating OdsInstance and context..." -ForegroundColor Cyan
# NOTE: Using container name as hostname since this connection string is used by the webapi Docker service
$containerHostname = $ContainerName -replace "-sqlserver$", "-sqlserver" # Keep the hostname consistent
$odsConnectionString = "Server=$containerHostname;Database=$newOdsDbName;User ID=sa;Password=$password;TrustServerCertificate=true;"

$createOdsInstanceQuery = @"
USE EdFi_Admin;

DECLARE @OdsInstanceId INT;

-- Create ODS Instance
INSERT INTO dbo.OdsInstances ([Name], InstanceType, ConnectionString)
VALUES ('$ScenarioName', 'Sandbox', '$odsConnectionString');

SELECT @OdsInstanceId = OdsInstanceId FROM dbo.OdsInstances WHERE [Name] = '$ScenarioName';

-- Create ODS Instance Context
INSERT INTO dbo.OdsInstanceContexts (OdsInstance_OdsInstanceId, ContextKey, ContextValue)
VALUES (@OdsInstanceId, 'Scenario', '$cleanScenarioName');

PRINT 'OdsInstance created with ID: ' + CAST(@OdsInstanceId AS VARCHAR);
"@

docker exec $ContainerName /opt/mssql-tools18/bin/sqlcmd -S localhost -U $username -P $password -C -Q $createOdsInstanceQuery

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to create OdsInstance"
    exit 1
}

# 5. Create ApiClientOdsInstance record
Write-Host "Step 5: Linking API client to ODS instance..." -ForegroundColor Cyan
$linkApiClientQuery = @"
USE EdFi_Admin;

DECLARE @ApiClientId INT;
DECLARE @OdsInstanceId INT;

SELECT @ApiClientId = ApiClientId FROM dbo.ApiClients WHERE [Key] = '$ApiClientKey';
SELECT @OdsInstanceId = OdsInstanceId FROM dbo.OdsInstances WHERE [Name] = '$ScenarioName';

-- Create API Client ODS Instance link
INSERT INTO dbo.ApiClientOdsInstances (ApiClient_ApiClientId, OdsInstance_OdsInstanceId)
VALUES (@ApiClientId, @OdsInstanceId);

PRINT 'API Client linked to ODS Instance';
"@

docker exec $ContainerName /opt/mssql-tools18/bin/sqlcmd -S localhost -U $username -P $password -C -Q $linkApiClientQuery

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to link API client to ODS instance"
    exit 1
}

Write-Host ""
Write-Host "=== Scenario '$ScenarioName' created successfully! ===" -ForegroundColor Green
Write-Host "Database: $newOdsDbName" -ForegroundColor Yellow
Write-Host "API Client Key: $ApiClientKey" -ForegroundColor Yellow
Write-Host "API Client Secret: $ApiClientSecret" -ForegroundColor Yellow
Write-Host "Context: Scenario = $cleanScenarioName" -ForegroundColor Yellow
Write-Host ""
Write-Host "You can now:" -ForegroundColor Cyan
Write-Host "1. Access the API at: http://localhost:5000/$cleanScenarioName/data/v3/" -ForegroundColor Cyan
Write-Host "2. Modify data in the '$newOdsDbName' database" -ForegroundColor Cyan
Write-Host "3. Run './backup-database.ps1' to capture all changes" -ForegroundColor Cyan
Write-Host "4. Run './promote-sqlserver.ps1' to build new image with scenarios" -ForegroundColor Cyan
