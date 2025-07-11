# Deployment and CI/CD

This document outlines the continuous integration and deployment pipeline for the solution.

## Pipeline Overview

The CI/CD pipeline uses GitHub Actions to automatically build, test, and deploy the application across multiple
environments. The pipeline is triggered on pull requests and pushes to main, as well as version tags.

## Deployment Strategy

### Environment Promotion

- **Development**: Automatically deploys from `main` branch pushes or from version tags
- **Test**: Deploys only from version tags (format: `vX.X.X`) - _Currently disabled_
- **Production**: Deploys only from version tags after successful test deployment - _Currently disabled_

### Version Management

The pipeline automatically determines the deployment version:

- **Tagged releases**: Uses the tag version (e.g., `v1.2.3` → `1.2.3`)
- **Main branch**: Uses commit SHA (e.g., `main-abc1234`)

## Build Process

### Automated Steps

1. **Code Quality**: Runs formatting, linting, and static analysis
2. **Build**: Compiles the solution using `./eng/build-solution.ps1`
3. **Test**: Executes unit tests with the `-Check` flag
4. **Publish**: Creates deployment artifacts for the Blazor app

### Configuration

- Uses .NET 9.0.x runtime
- Enforces strict quality checks (`TreatWarningsAsErrors: true`)
- Runs on Ubuntu latest for consistency

## Deployment Configuration

### Environment Variables

Each environment receives specific configuration through environment variable transformation:

```yaml
EWFTOOLSETTING__DeploymentInfo__Version: # Build version
EWFTOOLSETTING__DeploymentInfo__DeployDateTime: # Deployment timestamp
EWFTOOLSETTING__DeploymentInfo__EnvironmentLabel: # "DEV", "TEST", "PROD"
EWFTOOLSETTING__DeploymentInfo__ShowDetails: # "true" for dev/test, "false" for prod
EWFTOOLSETTING__DeploymentInfo__GitCommit: # Full commit SHA
EWFTOOLSETTING__DeploymentInfo__BuildNumber: # GitHub run number
```

### Azure Static Web Apps

The application deploys to Azure Static Web Apps using:

- **Deploy Token**: Stored as `AZURE_SWA_DEPLOY_TOKEN` in GitHub Secrets
- **App Location**: `publish/wwwroot` (the built Blazor output)
- **Deployment Script**: `./eng/deploy-to-azure-swa.ps1`

## Release Process

### Creating a Release

1. **Tag the release**: Create a tag following semantic versioning (`vX.X.X`)

   ```bash
   git tag v1.2.3
   git push origin v1.2.3
   ```

2. **Automatic deployment**: The pipeline will automatically deploy to dev environment immediately

3. **Manual promotion**: Test and production deployments are currently disabled and require manual intervention

### Rollback Strategy

- **Development**: Redeploy from main branch or previous tag
- **Test/Production**: Deploy previous version tag when environments are re-enabled

## Monitoring and Artifacts

### Build Artifacts

- **Retention**: 30 days
- **Naming**: `blazor-app-{version}` (e.g., `blazor-app-1.2.3` or `blazor-app-main-abc1234`)
- **Contents**: Complete published Blazor application

## Security Considerations

- Deploy tokens are stored as GitHub repository secrets
- Environment-specific configurations are applied during deployment
- Production environment hides deployment details (`ShowDetails: false`)

## Future Enhancements

- Re-enable test and production environments with required reviewer gates (available when repository becomes public)
- Add automated testing for deployed environments
- Implement blue-green deployment strategy for zero-downtime releases
