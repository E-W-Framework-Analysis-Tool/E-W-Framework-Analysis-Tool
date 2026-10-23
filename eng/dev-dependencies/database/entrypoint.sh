#!/bin/bash

# Start SQL Server in background
/opt/mssql/bin/sqlservr &

# Wait for SQL Server to be ready
echo "Waiting for SQL Server to start..."
until /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -Q "SELECT 1" &> /dev/null
do
  sleep 3
done

echo "SQL Server is ready!"

# Restore base databases from backups (EdFi_Admin, EdFi_Security, templates)
/opt/scripts/restore-all-backups.sh

# Bootstrap Admin database if needed (one-time setup)
BOOTSTRAP_CHECK=$(/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM EdFi_Admin.dbo.Vendors WHERE VendorName = 'Test Vendor'" -h -1 2>/dev/null | tr -d '[:space:]')

if [ "$BOOTSTRAP_CHECK" = "0" ]; then
    echo "Bootstrapping Admin database with default API client..."
    /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -i /opt/scripts/init-scripts/bootstrap-admin.sql
    
    if [ $? -eq 0 ]; then
        echo "Admin bootstrap complete"
    else
        echo "Admin bootstrap failed"
        exit 1
    fi
else
    echo "Admin already bootstrapped, skipping..."
fi

# Update connection strings to use correct server name
echo "Updating ODS connection strings..."
/opt/scripts/update-connection-strings.sh

echo "Database initialization complete!"

# Keep SQL Server running
wait