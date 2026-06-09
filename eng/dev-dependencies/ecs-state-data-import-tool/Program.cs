using ClosedXML.Excel;
using System.Text.Json;

var excelPath = args[0];
var outputDir = args[1];
var remainingArgs = args.Skip(2).ToList();
var includeCollected = remainingArgs.Remove("--include-collected");
var stateFilters = remainingArgs.ToHashSet(StringComparer.OrdinalIgnoreCase);
var filterStates = stateFilters.Count > 0;

using var workbook = new XLWorkbook(excelPath);
var worksheet = workbook.Worksheets.First();
var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

// Keyed by state, then by composite key (indicator|sector|elementName)
var stateRecords = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>(StringComparer.OrdinalIgnoreCase);

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
var elementMapPath = Path.Combine(AppContext.BaseDirectory, "DataElementNormalizationMap.json");
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
else
{
    Console.Error.WriteLine($"Data Element mapping file not found: {elementMapPath}");
    return 1;
}

string NormalizeElementName(string name, string normalizedSector)
{
    var trimmed = name.Trim();
    var sectorKey = $"{trimmed} ({normalizedSector})";
    if (elementNameMap.TryGetValue(sectorKey, out var sectorMapped))
        return sectorMapped;
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

string? MapStatus(string status) => status.Trim().ToLowerInvariant() switch
{
    "found" => "Available",
    "partial" => "PartiallyAvailable",
    "not found" => "NotAvailable",
    _ => null
};

// First pass: build lookup of parent Metric reported status by (state, uniqueOrderKey).
// ECS did not always fill out Data Reported for Data Elements — blank entries inherit from their parent Metric.
var metricReportedByStateAndKey = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

for (var row = 2; row <= lastRow; row++)
{
    var state = worksheet.Cell(row, 2).GetString()?.Trim();
    if (string.IsNullOrWhiteSpace(state))
        continue;

    if (filterStates && !stateFilters.Contains(state))
        continue;

    var metricType = worksheet.Cell(row, 9).GetString()?.Trim();
    if (!string.Equals(metricType, "Metric", StringComparison.OrdinalIgnoreCase))
        continue;

    var uniqueOrderKey = worksheet.Cell(row, 37).GetString()?.Trim() ?? "";
    if (string.IsNullOrWhiteSpace(uniqueOrderKey))
        continue;

    var reported = worksheet.Cell(row, 21).GetString()?.Trim() ?? "";
    if (string.IsNullOrWhiteSpace(reported))
        continue;

    var lookupKey = $"{state}|{uniqueOrderKey}";
    if (metricReportedByStateAndKey.TryGetValue(lookupKey, out var existingStatus))
        metricReportedByStateAndKey[lookupKey] = BestStatus(existingStatus, reported);
    else
        metricReportedByStateAndKey[lookupKey] = reported;
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

    var rawSector = worksheet.Cell(row, 7).GetString()?.Trim() ?? "";
    var sector = NormalizeSector(rawSector);
    var elementName = NormalizeElementName(rawElementName, sector);
    var indicator = NormalizeIndicator(worksheet.Cell(row, 8).GetString()?.Trim() ?? "");
    var collected = worksheet.Cell(row, 11).GetString()?.Trim() ?? "";
    var reported = worksheet.Cell(row, 21).GetString()?.Trim() ?? "";

    // Inherit reported status from parent Metric when blank
    if (string.IsNullOrWhiteSpace(reported))
    {
        var parentMetricKey = worksheet.Cell(row, 39).GetString()?.Trim() ?? "";
        if (!string.IsNullOrWhiteSpace(parentMetricKey))
        {
            var lookupKey = $"{state}|{parentMetricKey}";
            if (metricReportedByStateAndKey.TryGetValue(lookupKey, out var parentReported))
                reported = parentReported;
        }
    }

    if (!stateRecords.ContainsKey(state))
        stateRecords[state] = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

    var key = $"{indicator}|{sector}|{elementName}";

    if (stateRecords[state].TryGetValue(key, out var existing))
    {
        // Duplicate — keep the best status for each column
        existing["collected"] = BestStatus(existing["collected"], collected);
        existing["reported"] = BestStatus(existing["reported"], reported);
    }
    else
    {
        stateRecords[state][key] = new Dictionary<string, string>
        {
            ["sector"] = sector,
            ["indicator"] = indicator,
            ["elementName"] = elementName,
            ["rawElementName"] = rawElementName!,
            ["collected"] = collected,
            ["reported"] = reported
        };
    }
}

Directory.CreateDirectory(outputDir);
var options = new JsonSerializerOptions { WriteIndented = true };

foreach (var (state, recordMap) in stateRecords.OrderBy(kvp => kvp.Key))
{
    var records = recordMap.Values.Select(r => new Dictionary<string, string?>
    {
        ["sector"]      = r["sector"],
        ["indicator"]   = r["indicator"],
        ["elementName"] = r["elementName"],
        ["reported"]    = MapStatus(r["reported"])
    }).ToList();

    var fileName = Path.Combine(outputDir, state + ".json");
    File.WriteAllText(fileName, JsonSerializer.Serialize(records, options));
    Console.WriteLine($"  {state}: {records.Count} records -> {Path.GetFileName(fileName)}");

    if (includeCollected)
    {
        var now = DateTimeOffset.Now;
        var assessmentId = Guid.NewGuid().ToString();
        var dataSourceId = Guid.NewGuid().ToString();

        int? MapAvailabilityValue(string status) => status.Trim().ToLowerInvariant() switch
        {
            "found"     => 0, // Available
            "partial"   => 1, // PartiallyAvailable
            "not found" => 2, // NotAvailable
            _           => null
        };

        var dataElementAssessments = recordMap.Values
            .Where(r => MapAvailabilityValue(r["collected"]) is not null)
            .Select(r => new
            {
                id           = Guid.NewGuid().ToString(),
                dataElementName = r["elementName"],
                assessedAt   = now,
                characteristics = new[]
                {
                    new Dictionary<string, object?>
                    {
                        ["$type"]     = "ReportedAvailability",
                        ["value"]     = MapAvailabilityValue(r["collected"])!.Value,
                        ["id"]        = Guid.NewGuid().ToString(),
                        ["remarks"]   = null,
                        ["measuredAt"] = now
                    }
                },
                remarks                  = (string?)null,
                availabilityUserOverride = (string?)null
            }).ToList();

        var project = new
        {
            id             = Guid.NewGuid().ToString(),
            schemaVersion  = 2,
            title          = $"ECS Collected Data — {state}",
            createdAt      = now,
            lastModifiedAt = now,
            dataSources    = new[]
            {
                new
                {
                    id          = dataSourceId,
                    name        = $"ECS - {state} Collected Data",
                    description = $"Collected data indicators for {state} as surveyed by the Education Commission of the States (ECS). " +
                                  "These elements were noted as collected by the state but may be in varying states of readiness for use. " +
                                  "Data collected does not necessarily imply it is reportable as-is or publicly available. " +
                                  "This profile is intended for authorized internal reference only.",
                    enabled     = true,
                    type        = 3,
                    assessments = new[]
                    {
                        new
                        {
                            id         = assessmentId,
                            name       = $"{state} - Collected",
                            conductedAt = now,
                            notes      = (string?)null,
                            active     = true,
                            dataElementAssessments
                        }
                    }
                }
            }
        };

        var collectedFileName = Path.Combine(outputDir, state + ".collected.json");
        File.WriteAllText(collectedFileName, JsonSerializer.Serialize(project, options));
        Console.WriteLine($"  {state}: {dataElementAssessments.Count} collected elements -> {Path.GetFileName(collectedFileName)}");
    }
}

Console.WriteLine($"Total: {stateRecords.Count} state(s) exported.");

return 0;
