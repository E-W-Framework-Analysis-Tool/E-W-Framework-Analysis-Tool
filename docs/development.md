# Development Guide

This guide is for team members with write access to the repository. For the external contributor workflow, see
[CONTRIBUTING.md](../CONTRIBUTING.md).

---

## Prerequisites

- .NET 10 SDK
- PowerShell (for build scripts)
- Git with proper line ending configuration

---

## Local Development

### Standard workflow

Inner loop development can be done in the IDE of your choice (Visual Studio, Rider, VS Code, etc.).

Before committing, run the build and test scripts to catch formatting issues before CI does:

```powershell
# Build, format, test, and publish
./eng/build-solution.ps1 -Publish

# Run E2E tests (Chrome only, serious A11y level by default)
./eng/run-e2e.ps1
```

### Additional testing options

```powershell
# Always publish first when you have code changes
./eng/build-solution.ps1 -Publish

# Comprehensive tests (all browsers)
./eng/run-e2e.ps1 -AllBrowsers

# Only critical accessibility issues
./eng/run-e2e.ps1 -A11yFailLevel "critical"

# Test against a running instance (no publish needed)
./eng/run-e2e.ps1 -BaseUrl "https://localhost:5001"

# Full comprehensive suite
./eng/run-e2e.ps1 -AllBrowsers -A11yFailLevel "moderate"
```

### Build scripts

**`./eng/build-solution.ps1`** — Main build script

- Default: Auto-formats code, builds, runs unit tests
- `-Publish`: Creates deployable artifacts in `./publish/` (recommended for local dev)
- `-Check`: Enforces code formatting without auto-fixing (CI/CD use only)
- `-Configuration`: Build configuration (default: Release)

**`./eng/run-e2e.ps1`** — End-to-end testing

- Default: Chrome only, serious accessibility level
- `-AllBrowsers`: Test in Chrome, Firefox, and WebKit
- `-A11yFailLevel`: Accessibility fail threshold (`critical`|`serious`|`moderate`|`minor`)
- `-BaseUrl`: Test against a running application URL
- `-PublishPath`: Test against published application files

---

## Testing Strategy

**Unit tests** run automatically during build:

- Located in `*.Tests` projects
- Use xUnit with FluentAssertions
- Mock external dependencies

**E2E tests** include accessibility validation:

- **Development**: Run critical A11y tests frequently
- **Pre-commit**: Run comprehensive tests before pushing
- **CI/CD**: Automated based on branch (see CI section below)

**Accessibility levels:**

- `critical`: Only blocks severe issues (quick feedback)
- `serious`: Catches most problems (default for local dev)
- `moderate`: Comprehensive validation
- `minor`: All issues including minor ones

---

## Branching Strategy

We use trunk-based development:

- All work is done on short-lived branches off `main` — no forking required
- Pull Requests are required to merge into `main`
- PRs use **squash commits**
- Branches are deleted after merge
- Keep branches short-lived (preferably under 3 days)
- Push early and often
- Break large changes into smaller, reviewable PRs
- Use feature flags for incomplete work when merging early

### Branch naming

- `feature/[TICKET]-short-description`
- `bug/[TICKET]-short-description`
- `chore/short-description`
- `hotfix/[TICKET]-short-description`

Examples: `feature/EW-101-add-sector-selector`, `bug/EW-202-fix-crash-on-load`

Use uppercase ticket IDs consistently (e.g., `EW-123`).

---

## Pull Request Process

### Requirements

- Must be up to date with `main` (merge or rebase)
- Must build and pass all CI checks
- Must include relevant unit tests and documentation updates
- Must have a clear title and description
- Must link to a related issue or ticket if applicable

### Before submitting

```powershell
# Auto-format, build, test, and publish
./eng/build-solution.ps1 -Publish

# Run E2E tests (Chrome, serious A11y)
./eng/run-e2e.ps1

# Optional: full suite
./eng/run-e2e.ps1 -AllBrowsers -A11yFailLevel "moderate"
```

### Review

- At least one approving review is required
- Reviewers verify code quality, test coverage, CI status, and documentation

---

## CI/CD Pipeline

### Pull requests

- Build and unit tests with formatting verification
- E2E smoke tests: Chrome only, critical accessibility issues only
- Feedback time: ~5–6 minutes

### Main branch

- Full build: format, build, unit tests, publish
- Full E2E suite: all browsers, serious accessibility level
- Automatic deployment to dev environment
- Feedback time: ~10–12 minutes

### Accessibility in CI

- **PRs**: Critical issues block merges
- **Main**: Serious issues block deployments
- Accessibility reports are preserved as artifacts

---

## Releases

- Every merge to `main` creates a deployable artifact
- Releases are tagged and promoted through environments
- Configuration controls feature availability — not separate branches

---

## Additional References

- [Code Standards](code-standards.md)
- [Data Assessor Design](data-assessor-design.md)
- [Deployment and CI/CD](deployment.md)
- [Architecture and Security Brief](technical-overview.md)
