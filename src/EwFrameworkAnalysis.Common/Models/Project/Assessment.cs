namespace EwFrameworkAnalysis.Common.Models.Project;

/// <summary>
/// A complete assessment run across data sources at a point in time
/// </summary>
public class DataSourceAssessment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; }
    public DateTime ConductedAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }

    // All assessments from this session
    public List<DataElementAssessment> DataElementAssessments { get; set; } = [];
}

/// <summary>
/// Assessment result for a specific data element from a specific data source
/// </summary>
public class DataElementAssessment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AssessmentSessionId { get; set; }
    public string DataElementName { get; set; } = string.Empty;
    public Guid DataSourceId { get; set; }
    public DateTime AssessedAt { get; set; } = DateTime.UtcNow;

    // Assessment results
    public List<DataCharacteristicBase> Characteristics { get; set; } = [];
    public string? Remarks { get; set; }
    public bool? AvailabilityUserOverride { get; set; }
}
