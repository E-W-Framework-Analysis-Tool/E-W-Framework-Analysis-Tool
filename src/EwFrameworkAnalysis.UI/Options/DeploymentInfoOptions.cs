namespace EwFrameworkAnalysis.UI.Options;

public class DeploymentInfoOptions
{
    public string Version { get; set; } = "Unknown";
    public string DeployDateTime { get; set; } = "Unknown";
    public string EnvironmentLabel { get; set; } = "Unknown";
    public bool ShowDetails { get; set; }
}
