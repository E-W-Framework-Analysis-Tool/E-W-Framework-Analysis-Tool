using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Services;

public class EcsParsingStats
{
    public int TotalRowsRead { get; set; }
    public int DataElementsProcessed { get; set; }
    public int DataElementsSkipped { get; set; }
    public List<string> SkippedReasons { get; set; } = [];
    public bool HasWarnings => DataElementsSkipped > 0;
}

public class EcsStateDataParser
{
    public (DataSourceAssessment Assessment, EcsParsingStats Stats) ProcessStateData(
        List<EcsStateDataRecord> records)
    {
        var stats = new EcsParsingStats();
        var elements = new Dictionary<string, AvailabilityJudgment>(StringComparer.OrdinalIgnoreCase);

        foreach (var record in records)
        {
            stats.TotalRowsRead++;

            if (string.IsNullOrWhiteSpace(record.ElementName))
            {
                stats.DataElementsSkipped++;
                stats.SkippedReasons.Add("Empty element name");
                continue;
            }

            if (record.Reported == null)
            {
                stats.DataElementsSkipped++;
                stats.SkippedReasons.Add($"No valid status for '{record.ElementName}'");
                continue;
            }

            stats.DataElementsProcessed++;

            if (elements.TryGetValue(record.ElementName, out var existing))
            {
                if (record.Reported.Value < existing)
                    elements[record.ElementName] = record.Reported.Value;
            }
            else
            {
                elements[record.ElementName] = record.Reported.Value;
            }
        }

        var assessment = new DataSourceAssessment
        {
            ConductedAt = DateTimeOffset.Now,
            DataElementAssessments = [.. elements.Select(kvp => new DataElementAssessment
            {
                DataElementName = kvp.Key,
                Characteristics = [new ReportedAvailability(kvp.Value)]
            })]
        };

        return (assessment, stats);
    }
}
