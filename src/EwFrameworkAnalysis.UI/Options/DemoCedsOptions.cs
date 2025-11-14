namespace EwFrameworkAnalysis.UI.Options;

/// <summary>
/// Configuration options for demo CEDS Data Warehouse sample data
/// </summary>
public class DemoCedsOptions
{
    public const string SectionName = "DemoCeds";

    /// <summary>
    /// Whether demo sample results are enabled
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Whether the demo CEDS options are properly configured
    /// </summary>
    public bool IsConfigured => Enabled;
}
