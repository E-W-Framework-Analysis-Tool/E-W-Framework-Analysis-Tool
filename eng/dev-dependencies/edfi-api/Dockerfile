# Ed-Fi ODS API image, assembled from the official Ed-Fi NuGet package.
#
# The package version is pinned deliberately. Ed-Fi publishes new builds under the same
# package id, and an unpinned `dotnet add package` silently picks up whatever is newest
# - which is how this image ended up shipping a net10.0 build on an aspnet:8.0 runtime.
# To move to a newer Ed-Fi build, bump EDFI_WEBAPI_VERSION and confirm the target
# framework in EdFi.Ods.WebApi.runtimeconfig.json still matches the runtime base below.
ARG EDFI_WEBAPI_VERSION=7.3.20297

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG EDFI_WEBAPI_VERSION
WORKDIR /src

RUN apt-get update && apt-get install -y --no-install-recommends unzip \
    && rm -rf /var/lib/apt/lists/*

# Download the package from the Ed-Fi Alliance feed
RUN dotnet new console -n TempProject
WORKDIR /src/TempProject
RUN dotnet nuget add source "https://pkgs.dev.azure.com/ed-fi-alliance/Ed-Fi-Alliance-OSS/_packaging/EdFi%40Release/nuget/v3/index.json" --name "EdFi-Release"
RUN dotnet add package EdFi.Suite3.Ods.WebApi.Standard.5.2.0 --version ${EDFI_WEBAPI_VERSION}

# Extract the package - it ships the published application, not a library to build
RUN mkdir -p /app/webapi
RUN find ~/.nuget/packages -name "edfi.suite3.ods.webapi.standard.5.2.0" -type d \
        -exec find {} -name "*.nupkg" \; \
    | head -1 | xargs -I {} unzip -q {} -d /app/webapi

RUN find /app/webapi -name "EdFi.Ods.WebApi.dll" -type f | grep -q . \
    || (echo "ERROR: EdFi.Ods.WebApi.dll not found in the package!" && exit 1)

# Fail the build rather than the container if the package's target framework moves
# ahead of the runtime image again.
RUN CONFIG=$(find /app/webapi -name "EdFi.Ods.WebApi.runtimeconfig.json" -type f | head -1); \
    grep -q '"net10.0"' "$CONFIG" \
    || (echo "ERROR: package $EDFI_WEBAPI_VERSION does not target net10.0:" \
        && cat "$CONFIG" && exit 1)

FROM base AS final
WORKDIR /app
COPY --from=build /app/webapi ./

EXPOSE 80
ENTRYPOINT ["dotnet", "EdFi.Ods.WebApi.dll"]
