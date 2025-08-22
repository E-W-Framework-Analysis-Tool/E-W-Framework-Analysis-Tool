#!/usr/bin/env pwsh
# Backup a database or all databases to specified files
param(
    [Parameter(Mandatory=$false)]
    [string]$DatabaseName,
    
    [Parameter(Mandatory=$false)]
    [string]$BackupFileName,
    
    [Parameter(Mandatory=$false)]
    [string]$OutputDirectory = "./backups",
    
    [Parameter(Mandatory=$false)]
    [string]$Username = "sa",
    
    [Parameter(Mandatory=$false)]
    [string]$Password = "P@ssw0rd123",

    [Parameter(Mandatory=$false)]
    [string]$ContainerName = "ewftool-dev-sqlserver",

    [Parameter(Mandatory=$false)]
    [switch]$ExcludeEdFiDefaults
)

# Function to backup a single database
function Backup-SingleDatabase {
    param(
        [string]$DbName,
        [string]$BackupFile
    )
    
    # Container backup path
    $containerBackupPath = "/var/opt/mssql/backup/$BackupFile"
    
    # Create backup inside container
    $backupQuery = @"
BACKUP DATABASE [$DbName] 
TO DISK = '$containerBackupPath' 
WITH INIT, FORMAT, COMPRESSION;
"@

    Write-Host "Creating backup of $DbName..." -ForegroundColor Cyan
    docker exec $ContainerName /opt/mssql-tools18/bin/sqlcmd -S localhost -U $Username -P $Password -C -Q $backupQuery

    if ($LASTEXITCODE -eq 0) {
        # Copy backup file from container to host
        $outputPath = Join-Path $OutputDirectory $BackupFile
        docker cp "${ContainerName}:$containerBackupPath" $outputPath
        
        if ($LASTEXITCODE -eq 0) {
            # Clean up backup file from container
            docker exec $ContainerName rm $containerBackupPath
            
            Write-Host "Database $DbName backed up to: $outputPath" -ForegroundColor Green
            return $outputPath
        } else {
            Write-Error "Failed to copy backup file from container"
            return $null
        }
    } else {
        Write-Error "Failed to create backup of database $DbName"
        return $null
    }
}

# Function to check if database should be excluded
function Should-ExcludeDatabase {
    param([string]$DatabaseName)
    
    if (-not $ExcludeEdFiDefaults) {
        return $false
    }
    
    # Define the default Ed-Fi database names (with underscores as per your Docker setup)
    $edFiDefaults = @(
        "EdFi_Admin",
        "EdFi_Security", 
        "EdFi_Ods_Minimal_Template",
        "EdFi_Ods_Populated_Template"
    )
    
    return $DatabaseName -in $edFiDefaults
}

# Ensure output directory exists
if (-not (Test-Path $OutputDirectory)) {
    New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
}

# Check if container is running
$containerStatus = docker ps --filter "name=$ContainerName" --format "{{.Status}}"
if (!$containerStatus) {
    Write-Error "Container '$ContainerName' is not running. Please start with 'docker compose up -d'"
    exit 1
}

if ($DatabaseName) {
    # Backup single database
    if (-not $BackupFileName) {
        $BackupFileName = "$DatabaseName.bak"
    }
    
    if (Should-ExcludeDatabase -DatabaseName $DatabaseName) {
        Write-Warning "Skipping Ed-Fi default database '$DatabaseName' due to -ExcludeEdFiDefaults flag"
        exit 0
    }
    
    $result = Backup-SingleDatabase -DbName $DatabaseName -BackupFile $BackupFileName
    if ($result) {
        Write-Host "Backup completed successfully!" -ForegroundColor Green
    } else {
        exit 1
    }
} else {
    # Backup all databases
    Write-Host "=== Backing up ALL databases ===" -ForegroundColor Green
    if ($ExcludeEdFiDefaults) {
        Write-Host "(Excluding Ed-Fi default databases)" -ForegroundColor Yellow
    }
    
    # Get list of user databases (exclude system databases)
    $getDatabasesQuery = @"
SELECT name FROM sys.databases 
WHERE database_id > 4 
AND name NOT IN ('master', 'tempdb', 'model', 'msdb')
ORDER BY name;
"@

    Write-Host "Discovering databases..." -ForegroundColor Cyan
    $databasesOutput = docker exec $ContainerName /opt/mssql-tools18/bin/sqlcmd -S localhost -U $Username -P $Password -C -Q $getDatabasesQuery -h -1 -W
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to get database list"
        exit 1
    }
    
    # Parse database names from output
    $databases = $databasesOutput | Where-Object { 
        $_.Trim() -ne "" -and 
        $_.Trim() -ne "name" -and 
        $_ -notmatch "^-+$" -and
        $_ -notmatch "^\(\d+ rows affected\)$" -and
        $_ -notmatch "^Command.*completed successfully" -and
        $_.Trim().Length -gt 0 -and
        -not $_.Contains("affected")
    } | ForEach-Object { $_.Trim() }
    
    if ($databases.Count -eq 0) {
        Write-Warning "No user databases found to backup"
        exit 0
    }
    
    # Filter out Ed-Fi defaults if requested
    $filteredDatabases = $databases | Where-Object { -not (Should-ExcludeDatabase -DatabaseName $_) }
    
    if ($filteredDatabases.Count -eq 0) {
        Write-Warning "No databases to backup after filtering"
        exit 0
    }
    
    Write-Host "Found $($filteredDatabases.Count) databases to backup:" -ForegroundColor Yellow
    $filteredDatabases | ForEach-Object { Write-Host "  - $_" -ForegroundColor Yellow }
    
    if ($ExcludeEdFiDefaults -and ($databases.Count -ne $filteredDatabases.Count)) {
        $excludedCount = $databases.Count - $filteredDatabases.Count
        Write-Host "Excluded $excludedCount Ed-Fi default database(s)" -ForegroundColor Yellow
    }
    
    Write-Host ""
    
    $successCount = 0
    $failCount = 0
    
    foreach ($db in $filteredDatabases) {
        $backupFile = "$db.bak"
        $result = Backup-SingleDatabase -DbName $db -BackupFile $backupFile
        
        if ($result) {
            $successCount++
        } else {
            $failCount++
        }
    }
    
    Write-Host ""
    Write-Host "=== Backup Summary ===" -ForegroundColor Green
    Write-Host "✓ Successful: $successCount" -ForegroundColor Green
    if ($failCount -gt 0) {
        Write-Host "✗ Failed: $failCount" -ForegroundColor Red
        exit 1
    } else {
        Write-Host "All databases backed up successfully!" -ForegroundColor Green
    }
}
