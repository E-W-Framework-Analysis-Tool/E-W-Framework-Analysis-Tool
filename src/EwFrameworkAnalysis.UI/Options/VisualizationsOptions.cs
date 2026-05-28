namespace EwFrameworkAnalysis.UI.Options;

/// <summary>
/// Configuration options for the Visualizations / BI Dashboards page
/// </summary>
public class VisualizationsOptions
{
    public const string SectionName = "Visualizations";

    /// <summary>
    /// URL of the GitHub repository containing the BI dashboard assets.
    /// Defaults to the public E-W-Indicator-Analytics repo.
    /// </summary>
    public string GitHubRepoUrl { get; set; } =
        "https://github.com/E-W-Framework-Analysis-Tool/E-W-Indicator-Analytics";

    /// <summary>
    /// Per-EQ Power BI embed configuration. Key is the EQ identifier (e.g. "EQ12").
    /// </summary>
    public Dictionary<string, EqEmbedOptions> Embeds { get; set; } = [];
}

/// <summary>
/// Power BI embed options for a single Essential Question report
/// </summary>
public class EqEmbedOptions
{
    /// <summary>
    /// Power BI embed URL. When null or empty the embed section is hidden.
    /// </summary>
    public string? EmbedUrl { get; set; }

    /// <summary>
    /// Whether the embed is enabled and should be shown to users.
    /// </summary>
    public bool Enabled { get; set; }

    public bool IsConfigured => Enabled && !string.IsNullOrWhiteSpace(EmbedUrl);
}
