using System.ComponentModel.DataAnnotations;

namespace EwFrameworkAnalysis.Common.Models.Project;

/// <summary>
/// A data source that can be assessed (API, SQL, Manual)
/// </summary>
public class DataSource
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty; // User-provided name like "District Ed-Fi API"
    public string? Description { get; set; }
    public bool Enabled { get; set; } = true;
    public DataSourceType Type { get; set; } // Which type of system this is

    public List<DataSourceAssessment> Assessments { get; set; } = [];
}

public enum DataSourceType
{
    [Display(Name = "Ed-Fi ODS API", Description = "Ed-Fi ODS API endpoint for automated data discovery")]
    EdFiApi,

    [Display(Name = "CEDS Data Warehouse", Description = "Upload SQL result sets from CEDS DW")]
    CedsDw,

    [Display(Name = "Manual Entry", Description = "Flexible checklist for manual review")]
    Custom,

    [Display(Name = "ECS State Reference Profile", Description = "State-specific inventory of data as collected by ECS or reported by the state")]
    EcsState
}
