#!/bin/bash

# Wait for SQL Server to be ready
until /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -Q "SELECT 1" &> /dev/null
do
  echo "Waiting for SQL Server to start..."
  sleep 5
done

echo "SQL Server is ready. Restoring databases..."

# Find and restore all .bak files.
#
# Databases that already exist are left alone. The data directory is a persistent
# volume, so restoring unconditionally would reset EdFi_Admin to the image baseline on
# every container start - silently discarding registered scenarios and API clients and
# leaving the API unable to route to ODS databases that are still sitting in the volume.
# Set FORCE_RESTORE=true to restore over existing databases (for example after baking
# new backups into the image while keeping an old volume).
for bakfile in /opt/backups/*.bak; do
  if [ -f "$bakfile" ]; then
    filename=$(basename "$bakfile" .bak)

    if [ "${FORCE_RESTORE:-false}" != "true" ]; then
      dbexists=$(/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C \
        -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM sys.databases WHERE name = '$filename'" \
        -h -1 2>/dev/null | tr -d '[:space:]')

      if [ "$dbexists" = "1" ]; then
        echo "Skipping $filename - already exists (set FORCE_RESTORE=true to overwrite)"
        continue
      fi
    fi

    echo "Restoring $filename from $bakfile"
    
    # Get the logical file names - use a simpler approach
    filelistoutput=$(/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -Q "RESTORE FILELISTONLY FROM DISK = '$bakfile'" -h -1 2>/dev/null)
    
    # Extract logical names from the first two non-empty lines
    datafile=$(echo "$filelistoutput" | grep -E "^\S" | head -1 | awk '{print $1}')
    logfile=$(echo "$filelistoutput" | grep -E "^\S" | head -2 | tail -1 | awk '{print $1}')
    
    if [ -n "$datafile" ] && [ -n "$logfile" ]; then
      echo "Found logical files: Data='$datafile', Log='$logfile'"
      
      # Restore with the actual logical file names
      /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -Q "
      RESTORE DATABASE [$filename] 
      FROM DISK = '$bakfile' 
      WITH REPLACE, 
      MOVE '$datafile' TO '/var/opt/mssql/data/${filename}.mdf',
      MOVE '$logfile' TO '/var/opt/mssql/data/${filename}_log.ldf'
      " 2>/dev/null
      
      if [ $? -eq 0 ]; then
        echo "Successfully restored $filename"
      else
        echo "Failed to restore $filename"
      fi
    else
      echo "Could not determine logical file names for $filename"
      echo "Debug - datafile: '$datafile', logfile: '$logfile'"
    fi
  fi
done

echo "Database restoration complete."
