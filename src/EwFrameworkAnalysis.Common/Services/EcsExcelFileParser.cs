using ClosedXML.Excel;
using EwFrameworkAnalysis.Common.FrameworkReferenceData;
using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Services;

public enum EcsDataColumn
{
    Collected,
    Reported
}

public class EcsParsingStats
{
    public int TotalRowsRead { get; set; }
    public int DataElementsProcessed { get; set; }
    public int DataElementsSkipped { get; set; }
    public int DataElementsMapped { get; set; }
    public int DataElementsUnmapped { get; set; }
    public List<string> UnmappedElements { get; set; } = [];
    public List<string> SkippedReasons { get; set; } = [];
    public bool HasWarnings => DataElementsUnmapped > 0 || DataElementsSkipped > 0;
}

public class EcsExcelFileParser
{
    private const int STATE_COL = 2;       // B
    private const int SECTOR_COL = 7;      // G
    private const int METRIC_TYPE_COL = 9; // I
    private const int ELEMENT_NAME_COL = 10; // J
    private const int COLLECTED_COL = 11;  // K
    private const int REPORTED_COL = 21;   // U

    public Task<List<string>> GetAvailableStatesAsync(Stream stream)
    {
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheets.First();
        var states = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        for (var row = 2; row <= lastRow; row++)
        {
            var state = worksheet.Cell(row, STATE_COL).GetString()?.Trim();
            if (!string.IsNullOrWhiteSpace(state))
                states.Add(state);
        }

        return Task.FromResult(states.OrderBy(s => s).ToList());
    }

    public Task<(DataSourceAssessment Assessment, EcsParsingStats Stats)> ParseStateDataAsync(
        Stream stream,
        string stateName,
        EcsDataColumn dataColumn)
    {
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheets.First();

        var stats = new EcsParsingStats();
        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        // Track mapped elements with best-status consolidation
        var mappedElements = new Dictionary<string, AvailabilityJudgment>(StringComparer.OrdinalIgnoreCase);
        var unmappedSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var row = 2; row <= lastRow; row++)
        {
            var state = worksheet.Cell(row, STATE_COL).GetString()?.Trim();
            if (!string.Equals(state, stateName, StringComparison.OrdinalIgnoreCase))
                continue;

            var metricType = worksheet.Cell(row, METRIC_TYPE_COL).GetString()?.Trim();
            if (!string.Equals(metricType, "Data element", StringComparison.OrdinalIgnoreCase))
                continue;

            stats.TotalRowsRead++;

            var ecsElementName = worksheet.Cell(row, ELEMENT_NAME_COL).GetString()?.Trim();
            if (string.IsNullOrWhiteSpace(ecsElementName))
            {
                stats.DataElementsSkipped++;
                stats.SkippedReasons.Add($"Row {row}: Empty element name");
                continue;
            }

            var statusCol = dataColumn == EcsDataColumn.Collected ? COLLECTED_COL : REPORTED_COL;
            var statusValue = worksheet.Cell(row, statusCol).GetString()?.Trim();

            if (string.IsNullOrWhiteSpace(statusValue))
            {
                stats.DataElementsSkipped++;
                stats.SkippedReasons.Add($"Row {row}: No status value for '{ecsElementName}'");
                continue;
            }

            var judgment = MapStatus(statusValue);
            if (judgment == null)
            {
                stats.DataElementsSkipped++;
                stats.SkippedReasons.Add($"Row {row}: Unrecognized status '{statusValue}' for '{ecsElementName}'");
                continue;
            }

            stats.DataElementsProcessed++;

            var sector = worksheet.Cell(row, SECTOR_COL).GetString()?.Trim();
            var frameworkName = EcsElementMapping.MapToFrameworkElement(ecsElementName, sector);

            if (frameworkName == null)
            {
                if (unmappedSet.Add(ecsElementName))
                {
                    stats.DataElementsUnmapped++;
                    stats.UnmappedElements.Add(ecsElementName);
                }
                continue;
            }

            stats.DataElementsMapped++;

            // Consolidate: keep the best status (Available > PartiallyAvailable > NotAvailable)
            if (mappedElements.TryGetValue(frameworkName, out var existing))
            {
                if (judgment.Value < existing) // Lower enum value = better
                    mappedElements[frameworkName] = judgment.Value;
            }
            else
            {
                mappedElements[frameworkName] = judgment.Value;
            }
        }

        var assessment = new DataSourceAssessment
        {
            ConductedAt = DateTimeOffset.Now,
            DataElementAssessments = [.. mappedElements.Select(kvp => new DataElementAssessment
            {
                DataElementName = kvp.Key,
                Characteristics = [new ReportedAvailability(kvp.Value)]
            })]
        };

        return Task.FromResult((assessment, stats));
    }

    public (DataSourceAssessment Assessment, EcsParsingStats Stats) ProcessStateData(
        List<EcsStateDataRecord> records,
        EcsDataColumn dataColumn)
    {
        var stats = new EcsParsingStats();
        var mappedElements = new Dictionary<string, AvailabilityJudgment>(StringComparer.OrdinalIgnoreCase);
        var unmappedSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var record in records)
        {
            stats.TotalRowsRead++;

            if (string.IsNullOrWhiteSpace(record.ElementName))
            {
                stats.DataElementsSkipped++;
                stats.SkippedReasons.Add("Empty element name");
                continue;
            }

            var statusValue = dataColumn == EcsDataColumn.Collected ? record.Collected : record.Reported;

            if (string.IsNullOrWhiteSpace(statusValue))
            {
                stats.DataElementsSkipped++;
                stats.SkippedReasons.Add($"No status value for '{record.ElementName}'");
                continue;
            }

            var judgment = MapStatus(statusValue);
            if (judgment == null)
            {
                stats.DataElementsSkipped++;
                stats.SkippedReasons.Add($"Unrecognized status '{statusValue}' for '{record.ElementName}'");
                continue;
            }

            stats.DataElementsProcessed++;

            var frameworkName = EcsElementMapping.MapToFrameworkElement(record.ElementName, record.Sector);

            if (frameworkName == null)
            {
                if (unmappedSet.Add(record.ElementName))
                {
                    stats.DataElementsUnmapped++;
                    stats.UnmappedElements.Add(record.ElementName);
                }
                continue;
            }

            stats.DataElementsMapped++;

            if (mappedElements.TryGetValue(frameworkName, out var existing))
            {
                if (judgment.Value < existing)
                    mappedElements[frameworkName] = judgment.Value;
            }
            else
            {
                mappedElements[frameworkName] = judgment.Value;
            }
        }

        var assessment = new DataSourceAssessment
        {
            ConductedAt = DateTimeOffset.Now,
            DataElementAssessments = [.. mappedElements.Select(kvp => new DataElementAssessment
            {
                DataElementName = kvp.Key,
                Characteristics = [new ReportedAvailability(kvp.Value)]
            })]
        };

        return (assessment, stats);
    }

    private static AvailabilityJudgment? MapStatus(string status)
    {
        return status.Trim().ToLowerInvariant() switch
        {
            "found" => AvailabilityJudgment.Available,
            "partial" => AvailabilityJudgment.PartiallyAvailable,
            "not found" => AvailabilityJudgment.NotAvailable,
            _ => null
        };
    }
}
