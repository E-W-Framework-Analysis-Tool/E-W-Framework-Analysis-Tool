#!/bin/bash

# Get the target server name from environment variable, default to 'localhost'
TARGET_SERVER="${EDFI_DB_SERVER:-localhost}"

echo "Updating connection strings to use server: $TARGET_SERVER"

# Update all OdsInstances connection strings to use the correct server name
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -Q "
USE EdFi_Admin;
GO

-- Update existing OdsInstances to use the correct server name
UPDATE dbo.OdsInstances
SET ConnectionString = REPLACE(
    ConnectionString,
    'Server=' + SUBSTRING(
        ConnectionString,
        CHARINDEX('Server=', ConnectionString) + 7,
        CHARINDEX(';', ConnectionString, CHARINDEX('Server=', ConnectionString)) - CHARINDEX('Server=', ConnectionString) - 7
    ),
    'Server=$TARGET_SERVER'
)
WHERE ConnectionString LIKE 'Server=%';

PRINT 'Updated ' + CAST(@@ROWCOUNT AS VARCHAR) + ' connection string(s) to use Server=$TARGET_SERVER';
GO
" 2>/dev/null

if [ $? -eq 0 ]; then
    echo "Connection strings updated successfully"
else
    echo "Failed to update connection strings"
    exit 1
fi
