using EwFrameworkAnalysis.Common.Models.Framework;

namespace EwFrameworkAnalysis.Common.FrameworkReferenceData;

public class DataElement
{
    public required string Name { get; init; }
    public required string DataElementCategory { get; init; }
    public required List<Sector> RelatedSectors { get; init; }
    public string? AdditionalNotes { get; init; }
    public string? ScoringRuleName { get; init; }

    private IEnumerable<Indicator> UsedByIndicators =>
        EwFrameworkIndicators.Indicators.Values.Where(i => i.DataElementNames.Contains(Name));

    /// <summary>
    /// The distinct E-W Clusters of every indicator this element is used by.
    /// Transitive rather than hand-tagged, since an element used by indicators
    /// in different clusters legitimately belongs to more than one (or none, if
    /// its indicators haven't been assigned a cluster yet).
    /// </summary>
    public IReadOnlyList<IndicatorCluster> RelatedIndicatorClusters =>
        UsedByIndicators
            .SelectMany(i => i.Cluster)
            .Distinct()
            .OrderBy(c => c)
            .ToList();

    /// <summary>The distinct IndicatorTypes of every indicator this element is used by.</summary>
    public IReadOnlyList<IndicatorType> RelatedIndicatorTypes =>
        UsedByIndicators
            .Select(i => i.Type)
            .Distinct()
            .OrderBy(t => t)
            .ToList();

    /// <summary>The distinct IndicatorDomains of every indicator this element is used by.</summary>
    public IReadOnlyList<IndicatorDomain> RelatedIndicatorDomains =>
        UsedByIndicators
            .Select(i => i.Domain)
            .Distinct()
            .OrderBy(d => d)
            .ToList();
}
