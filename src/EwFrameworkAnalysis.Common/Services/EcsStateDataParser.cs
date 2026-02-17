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

public class EcsStateDataParser
{
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

            AvailabilityJudgment judgment;
            if (string.IsNullOrWhiteSpace(statusValue))
            {
                judgment = AvailabilityJudgment.NotAvailable;
            }
            else
            {
                var mapped = MapStatus(statusValue);
                if (mapped == null)
                {
                    stats.DataElementsSkipped++;
                    stats.SkippedReasons.Add($"Unrecognized status '{statusValue}' for '{record.ElementName}'");
                    continue;
                }
                judgment = mapped.Value;
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
                if (judgment < existing)
                    mappedElements[frameworkName] = judgment;
            }
            else
            {
                mappedElements[frameworkName] = judgment;
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
