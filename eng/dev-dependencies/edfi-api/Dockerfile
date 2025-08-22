# Use the official ASP.NET Core runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the SDK image to download and prepare the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Step 1: Install dependencies
RUN apt-get update && apt-get install -y unzip

# Step 2: Add the NuGet package
RUN dotnet new console -n TempProject
WORKDIR /src/TempProject
RUN dotnet nuget add source "https://pkgs.dev.azure.com/ed-fi-alliance/Ed-Fi-Alliance-OSS/_packaging/EdFi%40Release/nuget/v3/index.json" --name "EdFi-Release"
RUN dotnet add package EdFi.Suite3.Ods.WebApi.Standard.5.2.0

# Step 3: Create directory and unzip the package
RUN mkdir -p /app/webapi
RUN find ~/.nuget/packages -name "edfi.suite3.ods.webapi.standard.5.2.0" -type d -exec find {} -name "*.nupkg" \; | head -1 | xargs -I {} unzip {} -d /app/webapi

# Step 4: Confirm the DLL exists (fail if it doesn't)
RUN find /app/webapi -name "EdFi.Ods.WebApi.dll" -type f | grep -q . || (echo "ERROR: EdFi.Ods.WebApi.dll not found!" && exit 1)

# Switch to runtime image
FROM base AS final
WORKDIR /app

# Step 5: Copy ALL contents from the extracted package
COPY --from=build /app/webapi ./

EXPOSE 80
ENTRYPOINT ["dotnet", "EdFi.Ods.WebApi.dll"]