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
}

public enum DataSourceType
{
    EdFiApi,
    CedsDw,
    Custom
}
