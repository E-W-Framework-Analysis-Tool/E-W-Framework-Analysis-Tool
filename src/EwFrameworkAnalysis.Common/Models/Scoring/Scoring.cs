namespace EwFrameworkAnalysis.Common.Models.Scoring;

/// <summary>
/// Represents the readiness score for an essential question
/// </summary>
public class QuestionScore
{
    public int QuestionNumber { get; set; }
    public decimal ReadinessScore { get; set; }
    public List<IndicatorScore> IndicatorScores { get; set; } = [];
    public string? Notes { get; set; }
}

/// <summary>
/// Represents the readiness score for an indicator within a question
/// </summary>
public class IndicatorScore
{
    public string IndicatorCode { get; set; } = string.Empty;
    public decimal ReadinessScore { get; set; }
    public List<DataElementScore> DataElementScores { get; set; } = [];
    public string? Notes { get; set; }
}

/// <summary>
/// Represents the availability/quality score for a data element
/// </summary>
public class DataElementScore
{
    public string DataElementName { get; set; } = string.Empty;
    public decimal AvailabilityScore { get; set; }
    public decimal? QualityScore { get; set; }
    public bool IsAvailable { get; set; }
    public string? Source { get; set; }
    public string? Notes { get; set; }
}
