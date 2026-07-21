using EwFrameworkAnalysis.Common.Models.Framework;

namespace EwFrameworkAnalysis.Common.FrameworkReferenceData;

public class Indicator
{
    public required string Name { get; set; }
    public required IndicatorType Type { get; set; }
    public required IndicatorDomain Domain { get; set; }

    /// <summary>
    /// The E-W Framework cluster(s) this indicator belongs to. An indicator commonly
    /// belongs to more than one cluster (e.g. "Communication skills" spans several).
    /// </summary>
    public List<IndicatorCluster> Cluster { get; set; } = [];

    /// <summary>
    /// Readiness for adoption per the E-W Framework's "Where do I start?" guidance,
    /// ascribed per sector — the same indicator can be well-established for one sector
    /// and emerging for another.
    /// </summary>
    public List<IndicatorSectorReadiness> SectorReadiness { get; set; } = [];

    public required string Definition { get; set; }
    public required string RecommendedMetrics { get; set; }
    public List<DataCategory> DataNeeded { get; set; } = [];

    /// <summary>The sectors this indicator applies to — transitive from SectorReadiness rather than hand-authored, since the two must always agree.</summary>
    public List<Sector> Sectors => SectorReadiness.Select(sr => sr.Sector).Distinct().OrderBy(s => s).ToList();

    public List<string> DataElementNames { get; set; } = [];
}
