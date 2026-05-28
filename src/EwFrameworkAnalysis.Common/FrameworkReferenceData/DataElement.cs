using EwFrameworkAnalysis.Common.Models.Framework;

namespace EwFrameworkAnalysis.Common.FrameworkReferenceData;

public class DataElement
{
    public required string Name { get; init; }
    public required string ClusterOnlyCategory { get; init; }
    public required string DataElementCategory { get; init; }
    public required List<Sector> RelatedSectors { get; init; }
    public string? AdditionalNotes { get; init; }
    public string? ScoringRuleName { get; init; }
}
