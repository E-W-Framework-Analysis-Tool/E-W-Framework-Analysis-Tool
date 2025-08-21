# Dev Dependencies

Development environment dependencies for the E-W Framework Analysis Tool (ewftool). This is intended to set up required
services for some of the functionality of the application.

## Quick Start

```bash
docker compose up -d
```

**Note:** SQL Server restores databases from backup on first run - databases may be temporarily unavailable during
initial startup.

## Services

### SQL Server

- **Host:** `localhost,14333`
- **Username:** `sa`
- **Password:** `P@ssw0rd123`
- **Image:** `ewftool.azurecr.io/ewftool-dev-sqlserver:latest`

### Ed-Fi API

- **URL:** `http://localhost:5000/{ScenarioName}`
- **Client ID:** `RvcohKz9zHI4`
- **Client Secret:** `E1676E88-4D3B-4E4E-B7B7-7C3F8E5D2A9C`
- **Image:** `ewftool.azurecr.io/ewftool-dev-edfi-api:latest`

Use `edfi-api-test.http` in VS Code to verify the API is running correctly.

## Test Scenarios

The tooling is configured to support having multiple test scenarios embedded in the SQL Server image.

The Ed-Fi API is configured to use the same ApiClient for all "test scenarios". The test scenario "ScenarioName" is used
to select a different ODS database to serve a request. This is intended to support easily switching between sample
datasets for integration tests or demoing certain configurations.

### Ed-Fi Test Scenarios

| Scenario    | Description                     |
| ----------- | ------------------------------- |
| `GrandBend` | Grand Bend sample dataset       |
| `Empty`     | Empty ODS with descriptors only |

Use the scenario creation script to add new data environments to **your local environment only**:

```powershell
./database/create-edfi-scenario.ps1 -ScenarioName "MyScenario" -Template populated
```

Access at: `http://localhost:5000/MyScenario/data/v3/`

### CEDS Test Scenarios

The empty CEDS Data Warehouse is present in the SQL Server image. Test scenarios should just copy this database and run
scripts to insert data as needed.

## Updating Images

**Recommended workflow for creating new shared scenarios:**

1. **Login to Azure Container Registry:**

   ```bash
   az acr login --name ewftool
   ```

2. **Start with clean environment:**

   ```bash
   docker compose pull
   docker compose down -v
   docker compose up -d
   ```

3. **Run your scenario creation steps** (SQL scripts, API requests, etc.)
   - Script your modifications and check them into the repository (e.g. in `dev-dependencies/`)

4. **Backup all databases:**

   ```powershell
   ./database/backup-database.ps1
   ```

5. **Update the SQL Server image:**

   ```powershell
   ./database/update-image.ps1
   ```

This creates a tight loop to avoid accidentally overwriting other changes.

## Complete Reset

To completely reset all databases and start fresh:

```bash
docker compose down -v
docker compose up -d
```

The `-v` flag removes the persistent volume containing all database data.
