using EwFrameworkAnalysis.Common.Assessors.Ceds;
using EwFrameworkAnalysis.Common.Assessors.EdFi;
using EwFrameworkAnalysis.Common.FrameworkReferenceData;
using EwFrameworkAnalysis.Common.Models.Mapping;
using EwFrameworkAnalysis.Common.Models.Project;
using Microsoft.Extensions.DependencyInjection;

namespace EwFrameworkAnalysis.Common.Mapping;

// ---------------------------------------------------------------------------
// Service
// ---------------------------------------------------------------------------

/// <summary>
/// Builds an <see cref="AssessorMappingReport"/> by inspecting all registered
/// assessors. The assessors are the authoritative source of truth — if an
/// assessor exists for a data element, that element is mapped to that
/// data source type. No spreadsheet sync required.
/// </summary>
public class AssessorMappingService
{
    private readonly IServiceProvider _serviceProvider;

    // Lazy-built, shared across calls within the application lifetime.
    // Safe to cache permanently — assessor metadata is structural, not runtime data.
    private AssessorMappingReport? _cachedReport;

    public AssessorMappingService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public AssessorMappingReport GetReport()
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

    private AssessorMappingReport BuildReport(
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

        // Walk every data element and build a mapping row
        var rows = EwFrameworkDataElements.Elements.Values
            .Select(de =>
            {
                var edFiMapping = edFiIndex.TryGetValue(de.Name, out var edFiAssessor)
                    ? new DataElementMappingInfo
                    {
                        DataElementName = de.Name,
                        DataSource = DataSourceType.EdFiApi,
                        AssessmentDescription = edFiAssessor.AssessmentDescription,
                        MappedEndpoints = (edFiAssessor as IAssessorEndpointMetadata)?.MappedEndpoints ?? []
                    }
                    : null;

                var cedsMapping = cedsIndex.TryGetValue(de.Name, out var cedsAssessor)
                    ? new DataElementMappingInfo
                    {
                        DataElementName = de.Name,
                        DataSource = DataSourceType.CedsDw,
                        AssessmentDescription = cedsAssessor.AssessmentDescription,
                        MappedEndpoints = (cedsAssessor as IAssessorEndpointMetadata)?.MappedEndpoints ?? [],
                        Query = cedsAssessor.Query
                    }
                    : null;

                return new DataElementMappingRow
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

        return new AssessorMappingReport
        {
            Rows = rows,
            ClusterBreakdown = clusterBreakdown
        };
    }
}

/// <summary>
/// Assessors may optionally implement this to expose which API endpoints
/// or database objects they consult. Surfaced in the mapping UI as "why mapped".
/// </summary>
public interface IAssessorEndpointMetadata
{
    /// <summary>e.g. ["ed-fi/assessments", "ed-fi/studentAssessments"]</summary>
    IReadOnlyList<string> MappedEndpoints { get; }
}
