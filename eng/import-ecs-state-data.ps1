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

// Map ECS indicator names to their exact EW Framework equivalents
var indicatorNameMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
{
    ["Access to full day pre-K"] = "Access to full-day pre-K",
    ["Grade point average (MS+HS)"] = "Grade point average",
    ["Postsecondary enrollment after high school graduation"] = "Postsecondary enrollment directly after high school graduation",
    ["Successful completion of Algebra 1 by 9th grade"] = "Successful completion of Algebra I by 9th grade"
};

string NormalizeIndicator(string indicator)
{
    var trimmed = indicator.Trim();
    return indicatorNameMap.TryGetValue(trimmed, out var mapped) ? mapped : trimmed;
}

// Load data element name mapping from _DataElementMappings.json (ECS raw name -> normalized name)
var elementMapPath = Path.Combine(outputDir, "_DataElementMappings.json");
var elementNameMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
if (File.Exists(elementMapPath))
{
    var mapJson = File.ReadAllText(elementMapPath);
    var parsed = JsonSerializer.Deserialize<Dictionary<string, string>>(mapJson);
    if (parsed != null)
    {
        foreach (var kvp in parsed)
            elementNameMap[kvp.Key] = kvp.Value;
    }
}

string NormalizeElementName(string name)
{
    var trimmed = name.Trim();
    return elementNameMap.TryGetValue(trimmed, out var mapped) ? mapped : trimmed;
}

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

    var type = worksheet.Cell(row, 5).GetString()?.Trim();
    if (string.Equals(type, "Disaggregate", StringComparison.OrdinalIgnoreCase))
        continue;

    var metricType = worksheet.Cell(row, 9).GetString()?.Trim();
    if (!string.Equals(metricType, "Data element", StringComparison.OrdinalIgnoreCase))
        continue;

    var rawElementName = worksheet.Cell(row, 10).GetString()?.Trim();
    if (string.IsNullOrWhiteSpace(rawElementName))
        continue;

    var elementName = NormalizeElementName(rawElementName);
    var rawSector = worksheet.Cell(row, 7).GetString()?.Trim() ?? "";
    var sector = NormalizeSector(rawSector);
    var indicator = NormalizeIndicator(worksheet.Cell(row, 8).GetString()?.Trim() ?? "");
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

// Collect all raw element names encountered during import and merge into the mapping
var rawElementNames = stateRecords.Values
    .SelectMany(recordMap => recordMap.Values)
    .Select(r => r["elementName"])
    .Where(n => !string.IsNullOrWhiteSpace(n))
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToList();

foreach (var name in rawElementNames)
{
    if (!elementNameMap.ContainsKey(name))
        elementNameMap[name] = name;
}

// Write the data element mapping as an ordered key-value object
/*var orderedElementMap = elementNameMap
    .OrderBy(kvp => kvp.Key, StringComparer.OrdinalIgnoreCase)
    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

var elementMapFileName = Path.Combine(outputDir, "_DataElementMappings.json");
var elementMapJson = JsonSerializer.Serialize(orderedElementMap, options);
File.WriteAllText(elementMapFileName, elementMapJson);
Console.WriteLine($"  Data elements: {orderedElementMap.Count} entries -> {Path.GetFileName(elementMapFileName)}");
*/

// Build indicator-to-data-elements mapping across all states
var indicatorDataElements = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
foreach (var recordMap in stateRecords.Values)
{
    foreach (var record in recordMap.Values)
    {
        var indicator = record["indicator"];
        var element = record["elementName"];
        if (string.IsNullOrWhiteSpace(indicator) || string.IsNullOrWhiteSpace(element))
            continue;

        if (!indicatorDataElements.ContainsKey(indicator))
            indicatorDataElements[indicator] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        indicatorDataElements[indicator].Add(element);
    }
}

// Write indicators with their associated data elements
var indicatorsList = indicatorDataElements
    .OrderBy(kvp => kvp.Key, StringComparer.OrdinalIgnoreCase)
    .Select(kvp => new Dictionary<string, object>
    {
        ["indicator"] = kvp.Key,
        ["dataElements"] = kvp.Value.OrderBy(e => e, StringComparer.OrdinalIgnoreCase).ToList()
    })
    .ToList();

var indicatorsFileName = Path.Combine(outputDir, "_indicators.json");
var indicatorsJson = JsonSerializer.Serialize(indicatorsList, options);
File.WriteAllText(indicatorsFileName, indicatorsJson);
Console.WriteLine($"  Indicators: {indicatorsList.Count} entries -> {Path.GetFileName(indicatorsFileName)}");

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
