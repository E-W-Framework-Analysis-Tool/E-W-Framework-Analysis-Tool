# Contributing to E-W Framework Analysis Tool

Thank you for contributing! Please follow the guidelines below when making changes.

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

> Use uppercase ticket IDs consistently if used (e.g., `EW-123`).

## Pull Requests

All contributions must be made via a PR.

### PR Requirements

- Must be up to date with `main` (merge or rebase)
- Must build and pass all CI checks
- Must include relevant unit tests and documentation updates
- Must have a clear title and short description of changes
- Must link to related issue or ticket, if available

### PR Review

- At least one approving review is required
- PRs should be reviewed within 1 business day
- Reviewers should verify:
  - Code quality and consistency
  - Test coverage is sufficient
  - CI has passed
  - Documentation is updated as needed

## Development Practices

- Keep branches short-lived (preferably <3 days)
- Break large changes into smaller, reviewable PRs
- Use feature flags for incomplete features when merging early
- Push branches to the remote early and often

## Releases

- Every merge to `main` creates a deployable artifact
- Releases are tagged and promoted through environments
- Configuration should control feature availability, not separate branches

---

For questions, please open a discussion or contact a maintainer via GitHub Issues.
