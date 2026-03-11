using EwFrameworkAnalysis.Common.Assessors.Ceds;
using EwFrameworkAnalysis.Common.Assessors.EdFi;
using EwFrameworkAnalysis.Common.FrameworkReferenceData;
using Microsoft.Extensions.DependencyInjection;

namespace EwFrameworkAnalysis.Common.Mapping;

// ---------------------------------------------------------------------------
// Models
// ---------------------------------------------------------------------------

public enum CoverageStandard
{
    EdFi,
    Ceds
}

/// <summary>
/// Mapping metadata for a single data element within one standard.
/// </summary>
public class StandardElementMapping
{
    public string DataElementName { get; init; } = string.Empty;
    public CoverageStandard Standard { get; init; }

    /// <summary>The version string for the standard this assessor targets.</summary>
    // TODO: Make this a proper property on IEdFiAssessor / ICedsDWAssessor rather than hard-coding here.
    public string StandardVersion => Standard switch
    {
        CoverageStandard.EdFi => "Ed-Fi ODS API v7.2",
        CoverageStandard.Ceds => "CEDS Data Warehouse v13",
        _ => "Unknown"
    };

    /// <summary>Human-readable description of what the assessor measures.</summary>
    public string AssessmentDescription { get; init; } = string.Empty;

    /// <summary>
    /// API endpoints or DB objects consulted by this assessor.
    /// Populated from <see cref="IAssessorEndpointMetadata"/> when available.
    /// </summary>
    public IReadOnlyList<string> MappedEndpoints { get; init; } = [];
}

/// <summary>
/// Coverage view for a single EWF data element across all standards.
/// </summary>
public class DataElementCoverageRow
{
    public string DataElementName { get; init; } = string.Empty;
    public string ClusterCategory { get; init; } = string.Empty;
    public string DataElementCategory { get; init; } = string.Empty;

    public StandardElementMapping? EdFiMapping { get; init; }
    public StandardElementMapping? CedsMapping { get; init; }

    public bool IsMappedToEdFi => EdFiMapping is not null;
    public bool IsMappedToCeds => CedsMapping is not null;
    public bool IsMappedToAny => IsMappedToEdFi || IsMappedToCeds;
    public bool IsMappedToAll => IsMappedToEdFi && IsMappedToCeds;
}

/// <summary>
/// Top-level report produced by <see cref="StandardCoverageService"/>.
/// </summary>
public class StandardCoverageReport
{
    public IReadOnlyList<DataElementCoverageRow> Rows { get; init; } = [];
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

// ---------------------------------------------------------------------------
// Optional metadata interface — implement on assessors to surface endpoints
// ---------------------------------------------------------------------------

/// <summary>
/// Assessors may optionally implement this to expose which API endpoints
/// or database objects they consult. Surfaced in the coverage UI as "why mapped".
/// </summary>
public interface IAssessorEndpointMetadata
{
    /// <summary>e.g. ["ed-fi/assessments", "ed-fi/studentAssessments"]</summary>
    IReadOnlyList<string> MappedEndpoints { get; }
}

// ---------------------------------------------------------------------------
// Service
// ---------------------------------------------------------------------------

/// <summary>
/// Builds a <see cref="StandardCoverageReport"/> by inspecting all registered
/// assessors. The assessors are the authoritative source of truth — if an
/// assessor exists for a data element, that element is mapped to that standard.
/// No spreadsheet sync required.
/// </summary>
public class StandardCoverageService
{
    private readonly IServiceProvider _serviceProvider;

    // Lazy-built, shared across calls within the application lifetime.
    // Safe to cache permanently — assessor metadata is structural, not runtime data.
    private StandardCoverageReport? _cachedReport;

    public StandardCoverageService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public StandardCoverageReport GetReport()
    {
        if (_cachedReport is not null)
            return _cachedReport;

        // Open a short-lived scope just to resolve the scoped assessors.
        // We only need their metadata (DataElementName, AssessmentDescription),
        // not to run them, so the scope can be disposed immediately after.
        using var scope = _serviceProvider.CreateScope();
        var edFiAssessors = scope.ServiceProvider.GetRequiredService<IEnumerable<IEdFiAssessor>>();
        var cedsAssessors = scope.ServiceProvider.GetRequiredService<IEnumerable<ICedsDWAssessor>>();

        _cachedReport = BuildReport(edFiAssessors, cedsAssessors);
        return _cachedReport;
    }

    private StandardCoverageReport BuildReport(
        IEnumerable<IEdFiAssessor> edFiAssessors,
        IEnumerable<ICedsDWAssessor> cedsAssessors)
    {
        // Index assessors by DataElementName (case-insensitive)
        var edFiIndex = edFiAssessors
            .GroupBy(a => a.DataElementName, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                g => g.Key,
                g => g.First(),    // take first if multiple; edge case
                StringComparer.OrdinalIgnoreCase);

        var cedsIndex = cedsAssessors
            .GroupBy(a => a.DataElementName, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                g => g.Key,
                g => g.First(),
                StringComparer.OrdinalIgnoreCase);

        // Walk every EWF data element and build a coverage row
        var rows = EwFrameworkDataElements.Elements.Values
            .Select(de =>
            {
                var edFiMapping = edFiIndex.TryGetValue(de.Name, out var edFiAssessor)
                    ? new StandardElementMapping
                    {
                        DataElementName = de.Name,
                        Standard = CoverageStandard.EdFi,
                        AssessmentDescription = edFiAssessor.AssessmentDescription,
                        MappedEndpoints = (edFiAssessor as IAssessorEndpointMetadata)?.MappedEndpoints ?? []
                    }
                    : null;

                var cedsMapping = cedsIndex.TryGetValue(de.Name, out var cedsAssessor)
                    ? new StandardElementMapping
                    {
                        DataElementName = de.Name,
                        Standard = CoverageStandard.Ceds,
                        AssessmentDescription = cedsAssessor.AssessmentDescription,
                        MappedEndpoints = (cedsAssessor as IAssessorEndpointMetadata)?.MappedEndpoints ?? []
                    }
                    : null;

                return new DataElementCoverageRow
                {
                    DataElementName = de.Name,
                    ClusterCategory = de.ClusterOnlyCategory,
                    DataElementCategory = de.DataElementCategory,
                    EdFiMapping = edFiMapping,
                    CedsMapping = cedsMapping
                };
            })
            .OrderBy(r => r.ClusterCategory)
            .ThenBy(r => r.DataElementName)
            .ToList();

        // Per-cluster breakdown
        var clusterBreakdown = rows
            .GroupBy(r => r.ClusterCategory)
            .ToDictionary(
                g => g.Key,
                g => (Total: g.Count(), EdFi: g.Count(r => r.IsMappedToEdFi), Ceds: g.Count(r => r.IsMappedToCeds)));

        return new StandardCoverageReport
        {
            Rows = rows,
            ClusterBreakdown = clusterBreakdown
        };
    }
}
