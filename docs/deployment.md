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

### Injecting Deployment Metadata (Optional)

The application reads optional deployment metadata from `appsettings.json` at startup. These values are informational
and displayed in the application's about/version panel. They are not required for the application to function.

| Field                              | Description                         |
| ---------------------------------- | ----------------------------------- |
| `DeploymentInfo.Version`           | Build version string                |
| `DeploymentInfo.DeployDateTime`    | Deployment timestamp                |
| `DeploymentInfo.EnvironmentLabel`  | e.g. `DEV`, `TEST`, or `PROD`       |
| `DeploymentInfo.ShowDetails`       | `true` or `false`                   |
| `DeploymentInfo.GitCommit`         | Full commit SHA                     |
| `DeploymentInfo.BuildNumber`       | CI run number                       |

---

## CI Pipeline

The project uses GitHub Actions for continuous integration. The workflow file is at
`.github/workflows/dotnet-ci.yml`.

### Pipeline Steps

Every push and pull request runs the full quality gate:

1. **Build & Unit Tests** — compiles the solution and runs unit tests (`./eng/build-solution.ps1`)
2. **E2E & Accessibility Tests** — runs Playwright tests against the built app (`./eng/run-e2e.ps1`)
3. **Integration Tests** — runs live API integration tests against a test Ed-Fi endpoint
4. **Prepare Deployment** — produces a versioned build artifact retained for 30 days

Build artifacts are named `blazor-app-{version}` (e.g., `blazor-app-1.2.3` or `blazor-app-main-abc1234`) and can be
downloaded and deployed to any static host without rebuilding from source.

### Versioning

| Trigger                     | Version format                           |
| --------------------------- | ---------------------------------------- |
| Version tag (e.g. `v1.2.3`) | `1.2.3`                                  |
| Push to `main`              | `main-{short SHA}` (e.g. `main-abc1234`) |

### Deployment

Deployment of the maintainer-hosted instance is managed separately and is not part of this repository. Build artifacts
produced by this pipeline are consumed by the deployment pipeline after CI passes.

---

### Rollback

Redeploy any previous build artifact or re-run the pipeline against a prior tag. Because the application is stateless
on the server side, rollback has no database or migration concerns.
