# Database Scripts

Scripts for managing the SQL Server development environment.

## Normal Usage

For typical development tasks, see the [parent README](../README.md) which covers:

- Starting the environment
- Creating scenarios
- Updating images

## Package Versions

The Ed-Fi NuGet package versions in `Dockerfile` are pinned. Ed-Fi republishes new builds under the same package ids, so
an unpinned restore makes the image non-reproducible and can pull in a build targeting a newer .NET runtime than the
base image provides. Bump them deliberately:

| Package                                             | Version     |
| --------------------------------------------------- | ----------- |
| `EdFi.Database.Admin.Standard.5.2.0`                | `7.3.20021` |
| `EdFi.Database.Security.Standard.5.2.0`             | `7.3.20023` |
| `EdFi.Suite3.Ods.Populated.Template.Standard.5.2.0` | `7.3.20054` |
| `EdFi.Suite3.Ods.Minimal.Template.Standard.5.2.0`   | `7.3.20051` |

## Versions

- **SQL Server:** 2022 (`mcr.microsoft.com/mssql/server:2022-latest`, linux/amd64 only)
- **Ed-Fi Data Standard:** v5.2
- **CEDS Data Warehouse:** v11.0.0.0
  - Source:
    [CEDS-Data-Warehouse-V11.0.0.0.sql](https://github.com/CEDStandards/CEDS-Data-Warehouse/blob/V11.0.0.0/src/ddl/CEDS-Data-Warehouse-V11.0.0.0.sql)

## Building New Images

1. Make your changes (create scenarios, modify data, etc.)
2. Run `backup-database.ps1` to capture all databases
3. Run `update-image.ps1` to build and push new image

## Troubleshooting Scripts

### View all ODS Instances

```sql
SELECT * FROM EdFi_Admin.dbo.ApiClientOdsInstances
SELECT * FROM EdFi_Admin.dbo.OdsInstanceContexts
SELECT * FROM EdFi_Admin.dbo.OdsInstances
```

### Clean up a specific ODS Instance

```sql
DELETE aco
FROM EdFi_Admin.dbo.ApiClientOdsInstances aco
INNER JOIN EdFi_Admin.dbo.OdsInstances oi ON aco.OdsInstance_OdsInstanceId = oi.OdsInstanceId
WHERE oi.Name = 'TestScenarioName';

DELETE oic
FROM EdFi_Admin.dbo.OdsInstanceContexts oic
INNER JOIN EdFi_Admin.dbo.OdsInstances oi ON oic.OdsInstance_OdsInstanceId = oi.OdsInstanceId
WHERE oi.Name = 'TestScenarioName';

DELETE FROM EdFi_Admin.dbo.OdsInstances
WHERE Name = 'TestScenarioName';
```

## Available Scripts

- `backup-database.ps1` - Backup databases to .bak files
- `create-edfi-scenario.ps1` - Create new Ed-Fi scenarios
- `restore-database.ps1` - Restore database from backup
- `update-image.ps1` - Build and push new SQL Server image
