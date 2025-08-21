#!/usr/bin/env pwsh
param(
    [Parameter(Mandatory=$true)]
    [string]$BackupFilePath,
    
    [Parameter(Mandatory=$true)]
    [string]$NewDatabaseName,
    
    [Parameter(Mandatory=$true)]
    [string]$LogicalDataName,
    
    [Parameter(Mandatory=$true)]
    [string]$LogicalLogName,
    
    [Parameter(Mandatory=$false)]
    [string]$Username = "sa",
    
    [Parameter(Mandatory=$false)]
    [string]$Password = "P@ssw0rd123",

    [Parameter(Mandatory=$false)]
    [string]$ContainerName = "ewftool-dev-sqlserver"
)

# Check if container is running
$containerStatus = docker ps --filter "name=$ContainerName" --format "{{.Status}}"
if (!$containerStatus) {
    Write-Error "Container '$ContainerName' is not running. Please start with 'docker compose up -d'"
    exit 1
}

# Copy .bak file to container if it's not already there
if ($BackupFilePath -notlike "/opt/backups/*") {
    $backupFileName = Split-Path $BackupFilePath -Leaf
    
    # Convert to absolute path and normalize for Docker
    $absolutePath = Resolve-Path $BackupFilePath
    Write-Host "Copying backup file from: $absolutePath"
    
    # Copy directly to /opt/backups/
    docker cp "$absolutePath" "${ContainerName}:/opt/backups/$backupFileName"
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to copy backup file to container"
        exit 1
    }
    
    $containerBackupPath = "/opt/backups/$backupFileName"
} else {
    $containerBackupPath = $BackupFilePath
}

# Restore database with specific file mappings
$restoreQuery = @"
RESTORE DATABASE [$NewDatabaseName] 
FROM DISK = '$containerBackupPath' 
WITH REPLACE,
MOVE '$LogicalDataName' TO '/var/opt/mssql/data/$NewDatabaseName.mdf',
MOVE '$LogicalLogName' TO '/var/opt/mssql/data/$($NewDatabaseName)_log.ldf'
"@

Write-Host "Restoring $NewDatabaseName from $containerBackupPath..."
docker exec $ContainerName /opt/mssql-tools18/bin/sqlcmd -S localhost -U $Username -P $Password -C -Q $restoreQuery

if ($LASTEXITCODE -eq 0) {
    Write-Host "Database $NewDatabaseName restored successfully!" -ForegroundColor Green
} else {
    Write-Error "Failed to restore database $NewDatabaseName"
    exit 1
}
