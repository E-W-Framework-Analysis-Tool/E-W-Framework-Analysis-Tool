namespace EwFrameworkAnalysis.UI.Options;

/// <summary>
/// Configuration options for site-wide contact information.
/// </summary>
public class ContactInfoOptions
{
    public const string SectionName = "ContactInfo";

    /// <summary>
    /// Support email address displayed in the footer.
    /// When null or empty, the support link is hidden.
    /// </summary>
    public string? SupportEmail { get; set; } = "support@ewfanalysistool.com";
}
