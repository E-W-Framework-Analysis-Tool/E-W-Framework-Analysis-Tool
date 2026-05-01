using EwFrameworkAnalysis.Common.FrameworkReferenceData;
using EwFrameworkAnalysis.Common.Models.Framework;
using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Models.Scoring;

/// <summary>
/// The single output of CalculateFrameworkCoverage().
/// Everything else is a derived view of this.
/// </summary>
public class FrameworkCoverage
{
    public List<QuestionCoverageScore> QuestionScores { get; init; } = [];

    // Flat projections — no recalculation, just different views of QuestionScores
    public IReadOnlyList<IndicatorCoverageScore> IndicatorScores =>
        QuestionScores.SelectMany(q => q.IndicatorScores).ToList();

    public IReadOnlyList<DataElementScore> DataElementScores =>
        IndicatorScores.SelectMany(i => i.DataElementScores).ToList();

    public decimal OverallCoverage => QuestionScores.Count == 0 ? 0
        : Math.Round(QuestionScores.Average(q => q.CoverageScore), 2);

    public IReadOnlyList<SectorCoverageScore> BySector =>
        IndicatorScores
            .SelectMany(i => i.Sectors.Select(s => new { Sector = s, i.CoverageScore }))
            .GroupBy(x => x.Sector)
            .Select(g => new SectorCoverageScore
            {
                Sector = g.Key,
                CoverageScore = Math.Round(g.Average(x => x.CoverageScore), 2),
                IndicatorCount = g.Count()
            })
            .OrderBy(r => r.Sector)
            .ToList();

    public IReadOnlyList<DataElementScore> DisaggregateScores =>
        EwFrameworkDisaggregates.Disaggregates
            .SelectMany(d => d.DataElementNames.Count > 0
                ? d.DataElementNames.Select(name =>
                    DataElementScores.FirstOrDefault(s =>
                        s.DataElementName.Equals(name, StringComparison.OrdinalIgnoreCase))
                    ?? new DataElementScore { DataElementName = name })
                : [new DataElementScore { DataElementName = d.Name }])
            .ToList();

    public SourceTypeCoverageBreakdown BySourceType { get; init; } = new();
}

/// <summary>
/// Coverage score for a single essential question, containing its indicator breakdown.
/// </summary>
public class QuestionCoverageScore
{
    public int QuestionNumber { get; set; }
    public decimal CoverageScore { get; set; }
    public List<IndicatorCoverageScore> IndicatorScores { get; set; } = [];
    public string? Notes { get; set; }
}

/// <summary>
/// Coverage score for a single indicator within a question.
/// </summary>
public class IndicatorCoverageScore
{
    public string IndicatorCode { get; set; } = string.Empty;
    public decimal CoverageScore { get; set; }
    public List<DataElementScore> DataElementScores { get; set; } = [];
    public List<Sector> Sectors { get; set; } = [];
    public string? Notes { get; set; }
}

/// <summary>
/// Availability/quality score for a single data element.
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
    // Audit trail
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

/// <summary>
/// Pre-computed per-source-type coverage breakdowns.
/// Each slice requires a separate scoring pass so computed by the service, not derived.
/// </summary>
public class SourceTypeCoverageBreakdown
{
    public decimal Combined { get; init; }   // Total combined
    public decimal Automated { get; init; }  // EdFi + CEDS
    public decimal Manual { get; init; }      // Custom
    public decimal Ecs { get; init; }         // EcsState
}

/// <summary>
/// Coverage score for a sector, aggregated across all indicators in that sector.
/// </summary>
public class SectorCoverageScore
{
    public Sector Sector { get; init; }
    public decimal CoverageScore { get; init; }
    public int IndicatorCount { get; init; }
}

public class DataElementScoringRequest
{
    public string DataElementName { get; init; } = string.Empty;
    public string IndicatorName { get; init; } = string.Empty;
    public string ScoringRuleName { get; init; } = string.Empty;
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
