namespace EwFrameworkAnalysis.Common.Models.Project;

/// <summary>
/// A complete assessment run across data sources at a point in time
/// </summary>
public class DataSourceAssessment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DataSourceId { get; set; }
    public string Name { get; set; } = "";
    public DateTimeOffset ConductedAt { get; set; }
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
    public DateTimeOffset AssessedAt { get; set; } = DateTimeOffset.Now;

    // Assessment results
    public List<DataCharacteristicBase> Characteristics { get; set; } = [];
    public string? Remarks { get; set; }
    public bool? AvailabilityUserOverride { get; set; }
}
