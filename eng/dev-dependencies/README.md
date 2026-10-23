# Dev Dependencies

Development environment dependencies for the E-W Framework Analysis Tool (ewftool). This stands up a local **Ed-Fi ODS
API** backed by **SQL Server** in Docker, so the Ed-Fi profiler and integration tests can be exercised against a real
API without touching an agency's system.

> The application itself remains client-side only. This stack is a development dependency, not part of the deployed app.

## Quick Start

```powershell
./bootstrap.ps1
```

That is the whole first-time setup. The script:

1. Checks prerequisites (Docker running, architecture, Azure CLI)
2. Pulls images from the team's Azure Container Registry when it is reachable, and builds them locally from the Ed-Fi
   NuGet packages when it is not
3. Starts the containers and waits for SQL Server to finish restoring the Ed-Fi databases
4. Creates the `GrandBend` and `Empty` scenarios
5. Verifies the result by requesting an OAuth token and reading data back

It is safe to re-run, and it leaves existing scenarios alone.

| Flag                 | Effect                                                             |
| -------------------- | ------------------------------------------------------------------ |
| `-Build`             | Always build locally; never contact the registry                   |
| `-UseACR`            | Require the registry; fail rather than falling back to a build     |
| `-SkipScenarios`     | Start the containers without creating scenario databases           |
| `-SkipVerification`  | Do not exercise the API afterwards                                 |
| `-RegistryName`      | Registry short name (default `ewftool`)                            |
| `-ApiPort`           | Host port for the Ed-Fi API (default `5000`)                       |
| `-SqlPort`           | Host port for SQL Server (default `14333`)                         |
| `-RemoveConflicting` | Remove containers holding these names from another compose project |

Once it is running, `docker compose up -d` is enough on subsequent days.

### What survives a restart

The SQL Server data directory is a persistent Docker volume, and startup restores only databases that are not already
there. So scenarios, API clients, and any data you load all survive `docker compose restart` and `docker compose up -d`.

Two things override that:

- `docker compose down -v` deletes the volume, and the next start restores the image baseline. Run `./bootstrap.ps1`
  afterwards to recreate the scenarios.
- `FORCE_RESTORE=true docker compose up -d` restores over existing databases from the backups baked into the image,
  discarding local changes. Use it after pulling an image with new backups while keeping an old volume. It can also be
  set in `.env`.

### Prerequisites

- **Docker Desktop**, running
- **PowerShell 7+** (`pwsh`) — available on Windows, macOS, and Linux
- **Azure CLI**, only for the registry fast path. Without it the script builds locally instead.

The first local build takes 10–15 minutes; afterwards Docker's layer cache makes it quick.

### Port 5000 on macOS

macOS runs the **AirPlay Receiver** on port 5000, so the API cannot bind it. `bootstrap.ps1` checks both ports before
starting anything and tells you what is holding them. Either turn AirPlay Receiver off in **System Settings → General →
AirDrop & Handoff**, or pick another port — once:

```powershell
./bootstrap.ps1 -ApiPort 5010
```

The choice is saved to `.env`, which Docker Compose loads automatically. From then on both `./bootstrap.ps1` and plain
`docker compose up -d` use that port, with no flag to remember. `.env` is gitignored, so it stays your machine's
setting; see `.env.example` for the keys.

Avoid `5001` — the Blazor app itself uses `https://localhost:5001` during local development. After overriding the port,
update `@baseUrl` in `edfi-api-test.http` to match.

### On Apple Silicon

Microsoft publishes SQL Server for `linux/amd64` only, so `docker-compose.yml` pins that platform explicitly and the
container runs under emulation. Enable **Docker Desktop → Settings → General → "Use Rosetta for x86_64/amd64
emulation"** — without it, startup is slow and SQL Server may fail to launch. Local builds of the SQL Server image are
also emulated and noticeably slower than on an x86 machine; the registry path avoids that.

## Services

### Ed-Fi API

- **URL:** `http://localhost:5000/{ScenarioName}/data/v3/`
- **Token URL:** `http://localhost:5000/{ScenarioName}/oauth/token`
- **Client ID:** `RvcohKz9zHI4`
- **Client Secret:** `E1676E88-4D3B-4E4E-B7B7-7C3F8E5D2A9C`
- **Image:** `ewftool-dev-edfi-api` (Ed-Fi ODS/API 5.2.0, Data Standard 5.2)

Use `edfi-api-test.http` in VS Code to verify the API is running correctly.

### SQL Server

- **Host:** `localhost,14333`
- **Username:** `sa`
- **Password:** `P@ssw0rd123`
- **Image:** `ewftool-dev-sqlserver` (SQL Server 2022)

These credentials are development-only and intentionally checked in — this stack contains sample data only.

## Test Scenarios

The tooling supports multiple test scenarios embedded in the SQL Server image. The Ed-Fi API uses the same ApiClient for
all of them; the scenario name in the URL selects which ODS database serves the request. This makes it easy to switch
between sample datasets for integration tests or for demoing particular configurations.

### Ed-Fi Test Scenarios

| Scenario    | Description                     |
| ----------- | ------------------------------- |
| `GrandBend` | Grand Bend sample dataset       |
| `Empty`     | Empty ODS with descriptors only |

Add more to **your local environment only** with:

```powershell
./database/create-edfi-scenario.ps1 -ScenarioName "MyScenario" -Template populated
```

Access it at `http://localhost:5000/MyScenario/data/v3/`. Restart the API afterwards (`docker compose restart webapi`)
so it picks up the new ODS instance.

### CEDS Test Scenarios

The empty CEDS Data Warehouse is present in the SQL Server image. Test scenarios should copy this database and run
scripts to insert data as needed.

## Running the Integration Tests

```powershell
../run-integration.ps1 `
  -BaseUrl 'http://localhost:5000/GrandBend/data/v3' `
  -AuthUrl 'http://localhost:5000/GrandBend/oauth/token' `
  -ClientId 'RvcohKz9zHI4' `
  -ClientSecret 'E1676E88-4D3B-4E4E-B7B7-7C3F8E5D2A9C'
```

`-AuthUrl` is required. Omitted, the test fixture derives the token endpoint from the host alone, which drops the
scenario segment this API routes tokens under, and every test is skipped. `bootstrap.ps1` prints this command with the
correct values when it finishes.

## How It Works

**SQL Server container startup** (`database/entrypoint.sh`):

1. SQL Server starts in the background
2. `restore-all-backups.sh` restores every `.bak` in `/opt/backups/` — the Ed-Fi core databases and templates baked in
   at image build, plus anything checked into `database/backups/`
3. `init-scripts/bootstrap-admin.sql` creates the default vendor, application, and API client (first run only)
4. `update-connection-strings.sh` rewrites the registered ODS connection strings to match the environment, using
   `EDFI_DB_SERVER` — `sqlserver` under Docker Compose, `localhost` for the Azure Container Apps sidecar. The same image
   works in both places.

**An API request** to `http://localhost:5000/GrandBend/data/v3/ed-fi/schools`:

1. The API extracts the scenario context `GrandBend` from the route
2. It matches that against `EdFi_Admin.dbo.OdsInstanceContexts`
3. It reads the connection string from `EdFi_Admin.dbo.OdsInstances`
4. It connects to `EdFi_Ods_GrandBend` and returns the data

### Directory Structure

```text
dev-dependencies/
├── bootstrap.ps1                       # Main setup script — start here
├── docker-compose.yml                  # Container orchestration
├── push-images-to-azure-registry.ps1   # Publish images for the team
├── edfi-api-test.http                  # Sample API requests
├── database/
│   ├── Dockerfile                      # SQL Server image (Ed-Fi .bak files from NuGet)
│   ├── entrypoint.sh                   # Startup orchestrator
│   ├── restore-all-backups.sh          # Restore .bak files
│   ├── update-connection-strings.sh    # Environment-specific connection strings
│   ├── init-scripts/
│   │   └── bootstrap-admin.sql         # Default API client
│   ├── backups/                        # Extra .bak files baked into the image
│   ├── seed-data/                      # Optional SQL data seeds
│   ├── create-edfi-scenario.ps1        # Create a scenario database
│   ├── backup-database.ps1             # Capture databases as .bak
│   ├── restore-database.ps1            # Restore a specific database
│   └── update-image.ps1                # Rebuild the SQL Server image
├── edfi-api/
│   └── Dockerfile                      # Ed-Fi ODS API image
└── ecs-state-data-import-tool/         # Unrelated to Docker; see its own README
```

## Updating Images

Shared images live in the team's Azure Container Registry. Because you are replacing what everyone sees, start from the
current registry state so existing scenarios are preserved.

1. **Start from the published image:**

   ```powershell
   ./bootstrap.ps1 -UseACR
   ```

2. **Make your changes** — create scenarios, run SQL scripts, POST data through the API. Script your modifications and
   check them in (for example under `database/seed-data/`) so they are reviewable and repeatable.

3. **Capture every database:**

   ```powershell
   ./database/backup-database.ps1
   ```

4. **Build the new image:**

   ```powershell
   ./database/update-image.ps1
   ```

5. **Test from scratch:**

   ```powershell
   docker compose down -v
   docker compose up -d
   ```

6. **Publish:**

   ```powershell
   ./push-images-to-azure-registry.ps1 -RegistryName "ewftool"
   ```

Anything you did not back up in step 3 is lost. This creates a tight loop that avoids accidentally overwriting other
people's changes.

> **Note for Apple Silicon:** a `.bak` produced by SQL Server 2022 cannot be restored on 2019. If teammates are still on
> an older image, coordinate before publishing backups captured locally.

## Troubleshooting

**View logs:**

```bash
docker compose logs -f sqlserver
docker compose logs -f webapi
```

**Container name conflicts:** these containers use fixed names. If an older copy of this tooling started a stack from a
different directory, its containers hold those names. Re-run with `-RemoveConflicting`, which removes the containers and
leaves their volumes alone.

**Complete reset** — removes the volume holding all database data:

```powershell
docker compose down -v
./bootstrap.ps1
```

**API returns 404 for a scenario:** the API caches ODS instance configuration at startup. Restart it with
`docker compose restart webapi`. If the scenario is missing from `EdFi_Admin.dbo.OdsInstances` entirely, re-run
`./bootstrap.ps1` — it recreates any scenario that is not registered and leaves the rest alone.

**API not responding:**

1. `docker compose ps` — are both containers healthy?
2. `docker compose logs webapi`
3. Confirm the scenario is registered: `SELECT * FROM EdFi_Admin.dbo.OdsInstances`

**Database connection issues:**

```bash
docker exec ewftool-dev-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "P@ssw0rd123" -C -Q "SELECT 1"
```

Connection strings in `EdFi_Admin.dbo.OdsInstances` should read `Server=sqlserver` for local development. If they do
not, `update-connection-strings.sh` did not run — check the SQL Server logs.

## Additional Resources

- [database/README.md](database/README.md) — database script details and Ed-Fi/CEDS versions
- [edfi-api/README.md](edfi-api/README.md) — scenario routing design and API build details
