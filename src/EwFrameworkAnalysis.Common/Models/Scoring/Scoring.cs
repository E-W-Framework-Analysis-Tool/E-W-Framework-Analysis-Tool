using EwFrameworkAnalysis.Common.Models.Framework;
using EwFrameworkAnalysis.Common.Models.Project;

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
    public List<Sector> Sectors { get; set; } = [];
}

/// <summary>
/// Represents the availability/quality score for a data element
/// </summary>
public class DataElementScore
{
    public string DataElementName { get; set; } = string.Empty;
    public AvailabilityJudgment AvailabilityScore { get; set; }
    public decimal QualityScore { get; set; } = 0;
    public bool IsAvailable { get; set; }
    public string? Source { get; set; }
    public string? Notes { get; set; }

    // Metadata
    public string ScoringRuleName { get; set; } = string.Empty;
    public string SelectedSource { get; set; } = string.Empty;

    // AUDIT
    public List<DataElementSourceScore> SourceScores { get; set; } = [];
}

public class DataElementSourceScore
{
    public Guid AssessmentId { get; set; }
    public string SourceType { get; set; } = string.Empty;
    public string DataSourceName { get; set; } = string.Empty;

    public AvailabilityJudgment AvailabilityScore { get; set; }
    public decimal QualityScore { get; set; }
    public bool IsAvailable { get; set; }

    public string Notes { get; set; } = string.Empty;
}


public class DataElementScoringRequest
{
    public string DataElementName { get; init; } = string.Empty;
    public string IndicatorName { get; init; } = string.Empty;
    public string ScoringRuleName { get; init; } = string.Empty;

    // All matches across sources (manual, Ed-Fi, CEDS, etc.)
    public List<DataElementAssessmentContext> Matches { get; init; } = [];
}

public class DataElementAssessmentContext
{
    public Guid AssessmentId { get; init; }
    public string? AssessmentName { get; init; }
    public DataSourceType DataSourceType { get; init; }
    public string DataSourceName { get; init; } = string.Empty;
    public DataElementAssessment Assessment { get; init; } = null!;
}


public interface IDataElementScoringRule
{
    string RuleName { get; }

    DataElementScore Score(DataElementScoringRequest request);
}

public class SectorReadinessResult
{
    public Sector Sector { get; init; }
    public decimal ReadinessScore { get; init; } // 0–1
    public int IndicatorCount { get; init; }
}

public class OverallReadinessResults
{
    public decimal CustomDataSourceReadiness { get; set; }
    public decimal AutomatedDataSourceReadiness { get; set; }
    public decimal EcsReadiness { get; set; }
    public decimal CombinedReadiness { get; set; }
}

