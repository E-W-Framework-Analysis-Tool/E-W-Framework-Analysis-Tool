# Contributing to E-W Framework Analysis Tool

Thank you for your interest in contributing! There are several ways to get involved, and we appreciate all of them.

## Ways to Contribute

- **Report a bug** – [Open an issue](https://github.com/E-W-Framework-Analysis-Tool/E-W-Framework-Analysis-Tool/issues)
  describing what you found and how to reproduce it.
- **Request a feature** –
  [Open an issue](https://github.com/E-W-Framework-Analysis-Tool/E-W-Framework-Analysis-Tool/issues) with the
  `enhancement` label describing the use case.
- **Submit a pull request** – Fix a bug, improve documentation, or implement an approved feature. See the workflow
  below.
- **Ask a question** –
  [Open a GitHub Discussion](https://github.com/E-W-Framework-Analysis-Tool/E-W-Framework-Analysis-Tool/discussions) if
  you're unsure about something or want to talk through an idea before opening a PR.

For significant changes, please open an issue first so we can discuss the approach before you invest time in
implementation.

---

## Development Workflow

### Prerequisites

- .NET 10 SDK
- PowerShell (for build scripts)
- Git with proper line ending configuration

### Local Development

1. **Fork and clone the repository**

   ```bash
   git clone https://github.com/<your-username>/E-W-Framework-Analysis-Tool.git
   cd E-W-Framework-Analysis-Tool
   ```

2. **Standard development workflow**

   Inner loop development can be done in the IDE of your choice (Visual Studio, Rider, VS Code, etc.).

   Before committing, run the build and test scripts to catch formatting issues before CI does:

   ```powershell
   # Build, format, test, and publish
   ./eng/build-solution.ps1 -Publish

   # Run E2E tests (Chrome only, serious A11y level by default)
   ./eng/run-e2e.ps1
   ```

3. **Additional testing options**

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

### Build Scripts

**`./eng/build-solution.ps1`** - Main build script

- Default: Auto-formats code, builds, runs unit tests
- `-Publish`: Creates deployable artifacts in `./publish/` (recommended for local dev)
- `-Check`: Enforces code formatting without auto-fixing (CI/CD use only)
- `-Configuration`: Build configuration (default: Release)

**`./eng/run-e2e.ps1`** - End-to-end testing

- Default: Chrome only, serious accessibility level
- `-AllBrowsers`: Test in Chrome, Firefox, and WebKit
- `-A11yFailLevel`: Accessibility fail threshold (`critical`|`serious`|`moderate`|`minor`)
- `-BaseUrl`: Test against a running application URL
- `-PublishPath`: Test against published application files

### Testing Strategy

**Unit Tests**: Run automatically during build

- Located in `*.Tests` projects
- Use xUnit with FluentAssertions
- Mock external dependencies

**E2E Tests**: Include accessibility validation

- **Development**: Run critical A11y tests frequently
- **Pre-commit**: Run comprehensive tests before pushing
- **CI/CD**: Automated based on branch (see CI section below)

**Accessibility Testing**:

- `critical`: Only blocks severe accessibility issues (quick feedback)
- `serious`: Catches most accessibility problems (default for local dev)
- `moderate`: Comprehensive accessibility validation
- `minor`: All accessibility issues including minor ones

---

## Branching Strategy

We use **trunk-based development** with the following practices:

- All work is done on short-lived branches off `main`
- Pull Requests (PRs) are required to merge into `main`
- PRs must use **squash commits**
- Branches are deleted after merge

### Branch Naming

Use one of the following formats based on the type of work:

- `feature/[TICKET]-short-description`
- `bug/[TICKET]-short-description`
- `chore/short-description`
- `hotfix/[TICKET]-short-description`

Examples:

- `feature/EW-101-add-sector-selector`
- `bug/EW-202-fix-crash-on-load`
- `chore/update-dependencies`

> Use uppercase ticket IDs consistently (e.g., `EW-123`).

---

## Pull Request Process

All contributions must be made via a PR from a fork.

### PR Requirements

- Must be up to date with `main` (merge or rebase)
- Must build and pass all CI checks
- Must include relevant unit tests and documentation updates
- Must have a clear title and short description of changes
- Must link to a related issue or ticket, if applicable

### Before Submitting a PR

1. **Run the standard development workflow**

   ```powershell
   # Auto-format, build, test, and publish
   ./eng/build-solution.ps1 -Publish

   # Run E2E tests with default settings (Chrome, serious A11y)
   ./eng/run-e2e.ps1
   ```

2. **Optional: Run comprehensive tests**

   ```powershell
   ./eng/run-e2e.ps1 -AllBrowsers -A11yFailLevel "moderate"
   ```

3. **Update documentation** if your changes affect behavior or setup

### PR Review

- At least one approving review from a maintainer is required before merging
- PRs should receive an initial response within a few business days
- Maintainers will verify code quality, test coverage, CI status, and documentation

---

## CI/CD Pipeline

### Pull Requests

- **Build & Unit Tests**: Standard .NET build with formatting verification
- **E2E Smoke Tests**: Chrome only, critical accessibility issues only
- **Feedback time**: ~5–6 minutes total

### Main Branch

- **Comprehensive Build**: Format, build, unit tests, publish
- **Full E2E Suite**: All browsers (Chrome, Firefox, WebKit), serious accessibility level
- **Deployment**: Automatic deployment to dev environment
- **Feedback time**: ~10–12 minutes total

### Accessibility Testing in CI

- **PRs**: Only critical accessibility issues block merges
- **Main**: Serious accessibility issues block deployments
- **Reports**: Accessibility reports are preserved as artifacts for analysis

---

## Development Practices

- Keep branches short-lived (preferably under 3 days)
- Break large changes into smaller, reviewable PRs
- Use feature flags for incomplete features when merging early
- Push branches to your fork early and often
- Run E2E tests locally before pushing significant UI changes

---

## Getting Help

- **Code standards**: See [docs/code-standards.md](docs/code-standards.md)
- **Questions**:
  [Open a GitHub Discussion](https://github.com/E-W-Framework-Analysis-Tool/E-W-Framework-Analysis-Tool/discussions)
- **Bug reports**:
  [Open a GitHub Issue](https://github.com/E-W-Framework-Analysis-Tool/E-W-Framework-Analysis-Tool/issues)
- **Feature requests**:
  [Open a GitHub Issue](https://github.com/E-W-Framework-Analysis-Tool/E-W-Framework-Analysis-Tool/issues) with the
  `enhancement` label

---

By contributing, you agree that your contributions will be licensed under the [Apache License 2.0](LICENSE).
