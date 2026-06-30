# CI Workflows

This directory contains the GitHub Actions workflows for building and testing the E-W Framework Analysis Tool.

## Workflows

### `dotnet-ci.yml`

Runs on pull requests, pushes to `main`, and version tags.

| Job | Description |
| --- | ----------- |
| **Build & Unit Tests** | Compiles the solution and runs unit tests |
| **E2E & Accessibility Tests** | Runs Playwright tests against the built app |
| **Integration Tests** | Runs integration tests against a live Ed-Fi API endpoint |
| **Prepare Deployment** | Packages a versioned build artifact (retained 30 days) |
| **Trigger Deployment** | Dispatches a deploy event to the Infrastructure repo (push to `main` and version tags only) |

### `lint-markdown.yml`

Runs markdownlint on all Markdown files. Triggers on pull requests that touch `.md` files.

---

## Artifact Naming

| Trigger | Version | Artifact name |
| ------- | ------- | ------------- |
| Version tag (e.g. `v1.2.3`) | `1.2.3` | `blazor-app-1.2.3` |
| Push to `main` | `main-{short SHA}` | `blazor-app-main-abc1234` |

---

## Required Configuration (this repo)

### Secrets

| Name | Description |
| ---- | ----------- |
| `INFRA_DISPATCH_TOKEN` | Fine-grained PAT with **Contents: Write** on the Infrastructure repo. Used to trigger deployments. |

### Variables

Integration test configuration — set at the repository level:

| Name | Description |
| ---- | ----------- |
| `EWTEST_EDFI_BASE_URL` | Ed-Fi API base URL for integration tests |
| `EWTEST_EDFI_CLIENT_ID` | Ed-Fi API client ID for integration tests |
| `EWTEST_EDFI_CLIENT_SECRET` | Ed-Fi API client secret for integration tests |
| `EWTEST_EDFI_AUTH_URL` | Ed-Fi API auth URL for integration tests |

---

Deployment configuration and operational runbooks live in the
[Infrastructure repository](https://github.com/E-W-Framework-Analysis-Tool/Infrastructure).
