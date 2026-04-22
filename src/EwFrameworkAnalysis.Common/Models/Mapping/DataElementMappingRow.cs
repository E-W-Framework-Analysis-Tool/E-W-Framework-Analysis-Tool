using EwFrameworkAnalysis.Common.Mapping;
using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Models.Mapping;

/// <summary>
/// Mapping view for a single data element across all automated data source types.
/// </summary>
public class DataElementMappingRow
{
    public string DataElementName { get; init; } = string.Empty;
    public string ClusterCategory { get; init; } = string.Empty;
    public string DataElementCategory { get; init; } = string.Empty;

    public DataElementMappingInfo? EdFiMapping { get; init; }
    public DataElementMappingInfo? CedsMapping { get; init; }

    public bool IsMappedToEdFi => EdFiMapping is not null;
    public bool IsMappedToCeds => CedsMapping is not null;
    public bool IsMappedToAny => IsMappedToEdFi || IsMappedToCeds;
    public bool IsMappedToAll => IsMappedToEdFi && IsMappedToCeds;
}

/// <summary>
/// Mapping metadata for a single data element within one data source type.
/// </summary>
public class DataElementMappingInfo
{
    public string DataElementName { get; init; } = string.Empty;
    public DataSourceType DataSource { get; init; }

    /// <summary>The version string for the implementation this assessor targets.</summary>
    // TODO: Make this a proper property on IEdFiAssessor / ICedsDWAssessor rather than hard-coding here.
    public string DataSourceImplementation => DataSource switch
    {
        DataSourceType.EdFiApi => "Ed-Fi ODS API v7.2",
        DataSourceType.CedsDw => "CEDS Data Warehouse v13",
        _ => "Unknown"
    };

    /// <summary>Human-readable description of what the assessor measures.</summary>
    public string AssessmentDescription { get; init; } = string.Empty;

    /// <summary>
    /// API endpoints or DB objects consulted by this assessor.
    /// Populated from <see cref="IAssessorEndpointMetadata"/> when available.
    /// </summary>
    public IReadOnlyList<string> MappedEndpoints { get; init; } = [];

    public string? Query { get; set; }  // CEDS only — null for Ed-Fi
}
