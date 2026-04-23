namespace EwFrameworkAnalysis.UI.Pages.Survey;

public class SurveyResponse
{
    public InterestLevel? Interest { get; set; }
    public SurveyAvailability? Availability { get; set; }
    public bool? CanQueryAndShare { get; set; }
}

public enum InterestLevel
{
    NoInterest,
    LowPriority,
    HighPriority
}

public enum SurveyAvailability
{
    NotAvailable,
    PartiallyAvailable,
    Available
}

public static class SurveyEnumExtensions
{
    public static string GetDisplayName(this InterestLevel level) => level switch
    {
        InterestLevel.NoInterest => "No Interest",
        InterestLevel.LowPriority => "Low Priority",
        InterestLevel.HighPriority => "High Priority",
        _ => level.ToString()
    };

    public static string GetShortName(this InterestLevel level) => level switch
    {
        InterestLevel.NoInterest => "None",
        InterestLevel.LowPriority => "Low",
        InterestLevel.HighPriority => "High",
        _ => level.ToString()
    };

    public static string GetDisplayName(this SurveyAvailability avail) => avail switch
    {
        SurveyAvailability.NotAvailable => "Not Available",
        SurveyAvailability.PartiallyAvailable => "Partially Available",
        SurveyAvailability.Available => "Available",
        _ => avail.ToString()
    };

    public static string GetShortName(this SurveyAvailability avail) => avail switch
    {
        SurveyAvailability.NotAvailable => "None",
        SurveyAvailability.PartiallyAvailable => "Partial",
        SurveyAvailability.Available => "Avail",
        _ => avail.ToString()
    };
}
