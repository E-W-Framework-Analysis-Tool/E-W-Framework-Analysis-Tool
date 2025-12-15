# .NET CI/CD Workflow

This directory contains the GitHub Actions workflow for building, testing, and deploying the E-W Framework Analysis
Tool.

## Workflow: `dotnet-ci-cd.yml`

### Triggers

- **Pull Requests**: Runs build and tests on source code changes
- **Push to `main`**: Runs full pipeline and deploys to Dev
- **Version Tags** (`v*`): Runs full pipeline and deploys to Test and Production (when enabled)

### Jobs

1. **Build & Unit Tests** - Compiles solution and runs unit tests
2. **E2E & Accessibility Tests** - Runs end-to-end and accessibility tests using Playwright
3. **Integration Tests** - Runs integration tests against Ed-Fi API
4. **Prepare Deployment** - Packages artifacts for deployment
5. **Deploy to Dev** - Deploys to Dev environment (on `main` or tags)
6. **Deploy to Test** - Deploys to Test environment (on tags, currently disabled)
7. **Deploy to Prod** - Deploys to Production environment (on tags, currently disabled)

## Required Configuration

### 1. Create Azure Service Principals

If you do not already have service principals created that can be used to deploy to the target Azure environment, you will need to configure them. The Terraform code associated with the environment setup creates this and provides these secrets as an output. Use the following commands if you have not run that or if the SPs do not otherwise exist already. 

Run these commands for each environment, replacing `<subscription-id>` with your Azure subscription ID:

```bash
# Dev Environment
az ad sp create-for-rbac \
  --name "E-W Framework Analysis Tool - Dev Deploy" \
  --role "Storage Blob Data Contributor" \
  --scopes /subscriptions/<subscription-id>/resourceGroups/<dev-resource-group-name> \
  --sdk-auth

# Test Environment
az ad sp create-for-rbac \
  --name "E-W Framework Analysis Tool - Test Deploy" \
  --role "Storage Blob Data Contributor" \
  --scopes /subscriptions/<subscription-id>/resourceGroups/<test-resource-group-name> \
  --sdk-auth

# Production Environment
az ad sp create-for-rbac \
  --name "E-W Framework Analysis Tool - Prod Deploy" \
  --role "Storage Blob Data Contributor" \
  --scopes /subscriptions/<subscription-id>/resourceGroups/<prod-resource-group-name> \
  --sdk-auth
```

Each command outputs JSON credentials. **Save the entire JSON output** for the next step.

> ASSIGN READER ROLE TO SERVICE PRINCIPAL FOR RESOURCE GROUP OR FOR STORAGE ACCOUNT.

### 2. Configure GitHub Secrets

Navigate to **Settings** → **Secrets and variables** → **Actions** in your GitHub repository.

Add the following **Secrets** (paste the entire JSON from the service principal creation):

| Secret Name              | Value                                   |
| ------------------------ | --------------------------------------- |
| `AZURE_CREDENTIALS_DEV`  | JSON output from Dev service principal  |
| `AZURE_CREDENTIALS_TEST` | JSON output from Test service principal |
| `AZURE_CREDENTIALS_PROD` | JSON output from Prod service principal |

### 3. Configure GitHub Variables

Add the following **Variables** in the same location:

#### Dev Environment

- `AZURE_STORAGE_ACCOUNT_NAME_DEV` - Name of the dev storage account
- `AZURE_RESOURCE_GROUP_DEV` - Name of the dev resource group

#### Test Environment

- `AZURE_STORAGE_ACCOUNT_NAME_TEST` - Name of the test storage account
- `AZURE_RESOURCE_GROUP_TEST` - Name of the test resource group

#### Production Environment

- `AZURE_STORAGE_ACCOUNT_NAME_PROD` - Name of the prod storage account
- `AZURE_RESOURCE_GROUP_PROD` - Name of the prod resource group

#### Application Configuration (per environment)

Each environment also needs these variables configured:

- `EWFTOOLSETTING__DeploymentInfo__EnvironmentLabel` - e.g., "Development", "Test", "Production"
- `EWFTOOLSETTING__DeploymentInfo__ShowDetails` - `true` or `false`
- `EWFTOOLSETTING__DemoEdFiApi__BaseUrl` - Ed-Fi API base URL
- `EWFTOOLSETTING__DemoEdFiApi__ClientId` - Ed-Fi API client ID
- `EWFTOOLSETTING__DemoEdFiApi__ClientSecret` - Ed-Fi API client secret (use secret instead of variable)

#### Integration Test Variables

- `EWTEST_EDFI_BASE_URL` - Ed-Fi API URL for integration tests
- `EWETEST_EDFI_CLIENT_ID` - Ed-Fi API client ID for integration tests
- `EWTEST_EDFI_CLIENT_SECRET` - Ed-Fi API client secret for integration tests (use secret)
- `EWTEST_EDFI_AUTH_URL` - Ed-Fi API auth URL for integration tests

### 4. Optional: CDN Cache Purging

If you have Azure CDN configured, grant additional permissions to each service principal:

```bash
# Get the clientId from the service principal JSON output
az role assignment create \
  --assignee <clientId> \
  --role "CDN Endpoint Contributor" \
  --scope /subscriptions/<subscription-id>/resourceGroups/<resource-group-name>
```

The deployment script will automatically detect and purge CDN endpoints.

## Deployment Flow

### Development (`main` branch)

1. Push to `main` → Deploys to **Dev** environment automatically

### Release (version tags)

1. Create and push a tag: `git tag v1.0.0 && git push origin v1.0.0`
2. Deploys to **Dev** environment
3. Deploys to **Test** environment (when enabled)
4. After Test approval, deploys to **Production** (when enabled)

### Enabling Test and Production Deployments

The Test and Production deployment jobs are currently disabled. To enable them:

1. Remove `&& false` from the `if` condition in `deploy-test` and `deploy-prod` jobs
2. Ensure all secrets and variables are configured for those environments
3. Configure GitHub Environment protection rules (Settings → Environments) for approval workflows

## Local Development

To test the deployment script locally:

```bash
# Login to Azure
az login

# Run deployment script
./eng/deploy-to-azure-storage.ps1 `
  -StorageAccountName "yourstorageaccount" `
  -ResourceGroupName "your-resource-group" `
  -AppLocation "publish/wwwroot"
```

## Troubleshooting

### Permission Errors

- Verify service principal has "Storage Blob Data Contributor" role
- Check that role assignment is on the correct resource group
- Ensure you're using the latest Azure CLI version

### Deployment Failures

- Check Azure CLI is installed on the runner
- Verify storage account exists and has static website enabled
- Review workflow logs for specific error messages

### CDN Not Purging

- Verify service principal has "CDN Endpoint Contributor" role
- Check CDN endpoint's origin is set to the storage account's static website URL
- CDN purges are non-blocking; failures will show warnings but won't fail the deployment
