#!/bin/bash

# Start SQL Server in background
/opt/mssql/bin/sqlservr &

# Run the restore script
/opt/scripts/restore-all-backups.sh

# Keep SQL Server running in foreground
wait
