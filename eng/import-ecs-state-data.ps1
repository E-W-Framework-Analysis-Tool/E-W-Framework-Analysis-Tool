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

# Ensure ClosedXML is available via a temporary project
$tempDir = Join-Path ([System.IO.Path]::GetTempPath()) "ecs-import-$([guid]::NewGuid().ToString('N').Substring(0,8))"
New-Item -ItemType Directory -Path $tempDir -Force | Out-Null

try {
    # Create a minimal console project that reads the Excel and outputs JSON
    $csproj = @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="ClosedXML" Version="0.104.2" />
  </ItemGroup>
</Project>
"@

    $programCs = @"
using ClosedXML.Excel;
using System.Text.Json;

var excelPath = args[0];
var outputDir = args[1];
// Remaining args are state filters (empty = all states)
var stateFilters = args.Skip(2).ToHashSet(StringComparer.OrdinalIgnoreCase);
var filterStates = stateFilters.Count > 0;

using var workbook = new XLWorkbook(excelPath);
var worksheet = workbook.Worksheets.First();
var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

// Keyed by state, then by composite key (indicator|sector|elementName)
var stateRecords = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>(StringComparer.OrdinalIgnoreCase);
var duplicateCount = 0;

string NormalizeSector(string sector)
{
    return sector.Trim().ToLowerInvariant() switch
    {
        "pre-k" or "prek" or "pk" => "PK",
        "k-12" or "k12" => "K12",
        "postsecondary" or "ps" => "PS",
        "workforce" or "wf" => "WF",
        _ => sector.Trim()
    };
}

// Returns the best status: Found > Partial > Not Found > empty
string BestStatus(string existing, string incoming)
{
    int Rank(string s) => s.Trim().ToLowerInvariant() switch
    {
        "found" => 3,
        "partial" => 2,
        "not found" => 1,
        _ => 0
    };
    return Rank(incoming) > Rank(existing) ? incoming : existing;
}

for (var row = 2; row <= lastRow; row++)
{
    var state = worksheet.Cell(row, 2).GetString()?.Trim();
    if (string.IsNullOrWhiteSpace(state))
        continue;

    if (filterStates && !stateFilters.Contains(state))
        continue;

    var metricType = worksheet.Cell(row, 9).GetString()?.Trim();
    if (!string.Equals(metricType, "Data element", StringComparison.OrdinalIgnoreCase))
        continue;

    var elementName = worksheet.Cell(row, 10).GetString()?.Trim();
    if (string.IsNullOrWhiteSpace(elementName))
        continue;

    var rawSector = worksheet.Cell(row, 7).GetString()?.Trim() ?? "";
    var sector = NormalizeSector(rawSector);
    var indicator = worksheet.Cell(row, 8).GetString()?.Trim() ?? "";
    var collected = worksheet.Cell(row, 11).GetString()?.Trim() ?? "";
    var reported = worksheet.Cell(row, 21).GetString()?.Trim() ?? "";

    if (!stateRecords.ContainsKey(state))
        stateRecords[state] = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

    var key = $"{indicator}|{sector}|{elementName}";

    if (stateRecords[state].TryGetValue(key, out var existing))
    {
        // Duplicate — keep the best status for each column
        existing["collected"] = BestStatus(existing["collected"], collected);
        existing["reported"] = BestStatus(existing["reported"], reported);
        duplicateCount++;
    }
    else
    {
        stateRecords[state][key] = new Dictionary<string, string>
        {
            ["sector"] = sector,
            ["indicator"] = indicator,
            ["elementName"] = elementName,
            ["collected"] = collected,
            ["reported"] = reported
        };
    }
}

Directory.CreateDirectory(outputDir);
var options = new JsonSerializerOptions { WriteIndented = true };

foreach (var (state, recordMap) in stateRecords.OrderBy(kvp => kvp.Key))
{
    var records = recordMap.Values.ToList();
    var fileName = Path.Combine(outputDir, state + ".json");
    var json = JsonSerializer.Serialize(records, options);
    File.WriteAllText(fileName, json);
    Console.WriteLine($"  {state}: {records.Count} records -> {Path.GetFileName(fileName)}");
}

// Generate a distinct list of all indicators across all states
var allIndicators = stateRecords.Values
    .SelectMany(recordMap => recordMap.Values)
    .Select(r => r["indicator"])
    .Where(i => !string.IsNullOrWhiteSpace(i))
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .OrderBy(i => i, StringComparer.OrdinalIgnoreCase)
    .ToList();

var indicatorsFileName = Path.Combine(outputDir, "_indicators.json");
var indicatorsJson = JsonSerializer.Serialize(allIndicators, options);
File.WriteAllText(indicatorsFileName, indicatorsJson);
Console.WriteLine($"  Indicators: {allIndicators.Count} distinct -> {Path.GetFileName(indicatorsFileName)}");

if (duplicateCount > 0)
    Console.WriteLine($"Deduplicated: {duplicateCount} duplicate row(s) merged (best status kept).");
Console.WriteLine($"Total: {stateRecords.Count} state(s) exported.");
"@

    Set-Content -Path (Join-Path $tempDir "import.csproj") -Value $csproj
    Set-Content -Path (Join-Path $tempDir "Program.cs") -Value $programCs

    # Restore dependencies
    Write-Host "Restoring dependencies..."
    dotnet restore "$tempDir/import.csproj" --verbosity quiet
    if ($LASTEXITCODE -ne 0) { Fail "Failed to restore dependencies." }

    # Build arguments for the extraction program
    $runArgs = @("run", "--project", "$tempDir/import.csproj", "--", $ExcelPath, $OutputDir)
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
}
finally {
    # Clean up temp project
    Remove-Item -Path $tempDir -Recurse -Force -ErrorAction SilentlyContinue
}
