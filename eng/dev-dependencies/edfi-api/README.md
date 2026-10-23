# Ed-Fi API

Ed-Fi ODS API for development scenarios.

## Overview

This API rarely needs to be rebuilt. The base Ed-Fi NuGet packages are used with custom configuration for scenario-based
routing.

## Design

Uses context-based routing to allow the same API client to access different ODS databases with different test data:

- **Context Key:** `Scenario`
- **Route Template:** `{scenario}`
- **Naming:** Scenario names must be alphanumeric with no spaces (prefer `PascalCasing`)
- **Mode:** Single Tenant

Example URL: `http://localhost:5000/GrandBend/data/v3/`

## Database Requirements

When creating a new scenario, the following records are added to `EdFi_Admin`:

- `OdsInstance` record
- `OdsInstanceContext` record with `ContextKey = "Scenario"` and `ContextValue = "{ScenarioName}"`
- `ApiClientOdsInstances` record linking existing API client to new ODS instance

## Build Information

**Pinned version:** `EdFi.Suite3.Ods.WebApi.Standard.5.2.0` version `7.3.20297`, which targets **net10.0** and so runs
on the `mcr.microsoft.com/dotnet/aspnet:10.0` base image. The version is pinned via the `EDFI_WEBAPI_VERSION` build arg
in the Dockerfile — Ed-Fi republishes under the same package id, and an unpinned restore will eventually pull a build
whose target framework no longer matches the runtime base. The Dockerfile asserts the target framework at build time so
that mismatch fails the build rather than crash-looping the container.

**NuGet Package Source:**

[Official Ed-Fi Alliance OSS NuGet feed in Azure DevOps](https://pkgs.dev.azure.com/ed-fi-alliance/Ed-Fi-Alliance-OSS/_packaging/EdFi%40Release/nuget/v3/index.json)
