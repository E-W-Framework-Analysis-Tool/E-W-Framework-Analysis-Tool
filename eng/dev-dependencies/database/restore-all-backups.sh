#!/bin/bash

# Wait for SQL Server to be ready
until /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -Q "SELECT 1" &> /dev/null
do
  echo "Waiting for SQL Server to start..."
  sleep 5
done

echo "SQL Server is ready. Restoring databases..."

# Find and restore all .bak files
for bakfile in /opt/backups/*.bak; do
  if [ -f "$bakfile" ]; then
    filename=$(basename "$bakfile" .bak)
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
