# Deployment

The E-W Framework Analysis Tool is a Blazor WebAssembly application that compiles to a set of static files — HTML, CSS,
JavaScript, and WebAssembly binaries. There is no server-side runtime component. Any host capable of serving static
files can run it.

The project maintainers publish a hosted instance as a convenience, but agencies and developers are encouraged to build
and host the application themselves, including on internal networks.

## Self-Hosting

### Building from Source

Prerequisites: [.NET 10 SDK](https://dotnet.microsoft.com/download)

```bash
# Build and publish the Blazor app
./eng/build-solution.ps1 -Publish
```

The published output will be in `publish/wwwroot`. Deploy its contents to any static file host.

### Hosting Requirements

The only hosting requirement beyond serving static files is that your host must return `index.html` for any path that
does not correspond to a static file. This is required for Blazor's client-side routing — without it, deep links and
page refreshes will return 404. The published output includes a `web.config` with the necessary IIS rewrite rules; for
other hosts, consult your server's documentation for configuring a single-page application fallback.

---

## CI/CD Pipeline

The project uses GitHub Actions for continuous integration and deployment. The workflow file is at
`.github/workflows/dotnet-cicd.yml`.

### Pipeline Steps

Every push and pull request runs the full quality gate:

1. **Format & lint** — enforces code style via `.editorconfig`
2. **Build** — compiles the solution (`./eng/build-solution.ps1`)
3. **Test** — runs unit tests with the `-Check` flag
4. **Publish** — produces the static Blazor output as a build artifact

Build artifacts are retained for 30 days and named `blazor-app-{version}` (e.g., `blazor-app-1.2.3` or
`blazor-app-main-abc1234`). These artifacts can be downloaded and deployed to any static host without rebuilding from
source.

### Deployment Targets

The pipeline supports three environments, each gated by trigger type:

| Environment | Trigger                           | Status             |
| ----------- | --------------------------------- | ------------------ |
| Development | Push to `main` or any version tag | Active             |
| Test        | Version tags (`vX.X.X`) only      | Currently disabled |
| Production  | Version tags, after test succeeds | Currently disabled |

Test and production deployments will be re-enabled with required reviewer gates once the repository is public.

### Versioning

| Trigger                     | Version format                           |
| --------------------------- | ---------------------------------------- |
| Version tag (e.g. `v1.2.3`) | `1.2.3`                                  |
| Push to `main`              | `main-{short SHA}` (e.g. `main-abc1234`) |

### Deploying to Azure Static Web Apps

The project maintainers use Azure Static Web Apps for their hosted instance. If you want to use the same pipeline for
your own Azure deployment:

1. Create an Azure Static Web App and copy the deployment token
2. Add the token as a repository secret named `AZURE_SWA_DEPLOY_TOKEN`
3. The deployment script (`./eng/deploy-to-azure-swa.ps1`) will be invoked automatically by the workflow

The following environment-specific values are injected into `appsettings.json` at deploy time via environment variable
substitution:

| Variable                           | Description                         |
| ---------------------------------- | ----------------------------------- |
| `DeploymentInfo__Version`          | Build version string                |
| `DeploymentInfo__DeployDateTime`   | Deployment timestamp                |
| `DeploymentInfo__EnvironmentLabel` | `DEV`, `TEST`, or `PROD`            |
| `DeploymentInfo__ShowDetails`      | `true` in dev/test; `false` in prod |
| `DeploymentInfo__GitCommit`        | Full commit SHA                     |
| `DeploymentInfo__BuildNumber`      | GitHub Actions run number           |

These values are informational and displayed in the application's about/version panel. They are not required for
self-hosted deployments.

### Rollback

Redeploy any previous build artifact or re-run the pipeline against a prior tag. Because the application is stateless on
the server side, rollback has no database or migration concerns.
