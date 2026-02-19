param(
    [Parameter(Mandatory)]
    [string]$ExcelPath,

    [string[]]$States,

    [string]$OutputDir = ""
)

$repoRoot = Resolve-Path "$PSScriptRoot/.."

if (-not $OutputDir) {
    $OutputDir = Join-Path $repoRoot "src/EwFrameworkAnalysis.Common/FrameworkReferenceData/EcsStateData"
}

function Fail($message) {
    Write-Error $message
    exit 1
}

# Resolve the Excel path
$ExcelPath = Resolve-Path $ExcelPath -ErrorAction SilentlyContinue
if (-not $ExcelPath) {
    Fail "Excel file not found: $ExcelPath"
}

$importerProject = Join-Path $repoRoot "eng/dev-dependencies/ecs-state-data-import-tool/import.csproj"

# Restore dependencies
Write-Host "Restoring dependencies..."
dotnet restore "$importerProject" --verbosity quiet
if ($LASTEXITCODE -ne 0) { Fail "Failed to restore dependencies." }

# Build arguments for the extraction program
$runArgs = @("run", "--project", $importerProject, "--", $ExcelPath, $OutputDir)
if ($States) {
    $runArgs += $States
}

Write-Host "Reading Excel file: $ExcelPath"
Write-Host "Output directory:   $OutputDir"
if ($States) {
    Write-Host "Filtering states:   $($States -join ', ')"
} else {
    Write-Host "Exporting:          All states"
}
Write-Host ""

& dotnet @runArgs
if ($LASTEXITCODE -ne 0) { Fail "Extraction failed." }

Write-Host ""
Write-Host "Done. JSON files are ready in $OutputDir"
