using EwFrameworkAnalysis.Common.Mapping;

namespace EwFrameworkAnalysis.Common.Models.Mapping;

/// <summary>
/// Top-level report produced by <see cref="AssessorMappingService"/>.
/// </summary>
public class AssessorMappingReport
{
    public IReadOnlyList<DataElementMappingRow> Rows { get; init; } = [];
    public int TotalElements => Rows.Count;

    public int EdFiMappedCount => Rows.Count(r => r.IsMappedToEdFi);
    public int CedsMappedCount => Rows.Count(r => r.IsMappedToCeds);

    public decimal EdFiCoveragePercent =>
        TotalElements == 0 ? 0 : Math.Round((decimal)EdFiMappedCount / TotalElements * 100, 1);
    public decimal CedsCoveragePercent =>
        TotalElements == 0 ? 0 : Math.Round((decimal)CedsMappedCount / TotalElements * 100, 1);

    // Per-category breakdowns (keyed by ClusterCategory)
    public IReadOnlyDictionary<string, (int Total, int EdFi, int Ceds)> ClusterBreakdown { get; init; }
        = new Dictionary<string, (int, int, int)>();
}


