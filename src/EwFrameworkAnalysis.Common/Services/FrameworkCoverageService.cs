using EwFrameworkAnalysis.Common.FrameworkReferenceData;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Models.Scoring;
using EwFrameworkAnalysis.Common.Scoring;

namespace EwFrameworkAnalysis.Common.Services;

public class FrameworkCoverageService
{
    private readonly DataElementScoringRuleRegistry _ruleRegistry;

    public FrameworkCoverageService(DataElementScoringRuleRegistry ruleRegistry)
    {
        _ruleRegistry = ruleRegistry;
    }

    /// <summary>
    /// The single entry point. Pass in active assessments, get back a fully populated
    /// FrameworkCoverage result. All derived views (BySector, BySourceType, etc.)
    /// are either computed here or derived from the result's own properties.
    /// </summary>
    public FrameworkCoverage CalculateFrameworkCoverage(IEnumerable<DataSourceAssessmentWithSource> assessments)
    {
        var assessmentList = assessments.ToList();
        var questionScores = ScoreQuestions(assessmentList);

        return new FrameworkCoverage
        {
            QuestionScores = questionScores,
            DisaggregateScores = ScoreDisaggregates(assessmentList),
            BySourceType = new SourceTypeCoverageBreakdown
            {
                Automated = AverageCoverage(ScoreQuestions([.. assessmentList.Where(a => a.DataSourceType is DataSourceType.EdFiApi or DataSourceType.CedsDw)])),
                Manual = AverageCoverage(ScoreQuestions([.. assessmentList.Where(a => a.DataSourceType == DataSourceType.Custom)])),
                Ecs = AverageCoverage(ScoreQuestions([.. assessmentList.Where(a => a.DataSourceType == DataSourceType.EcsState)])),
            }
        };
    }

    // --- Private implementation ---

    private List<QuestionCoverageScore> ScoreQuestions(List<DataSourceAssessmentWithSource> assessments)
    {
        return [.. EwFrameworkEssentialQuestions.Questions.Select(q => ScoreQuestion(q, assessments))];
    }

    private QuestionCoverageScore ScoreQuestion(EssentialQuestion question, List<DataSourceAssessmentWithSource> assessments)
    {
        var indicatorScores = question.RelatedIndicatorNames
            .Select(name => ScoreIndicator(name, assessments))
            .Where(s => s.IndicatorCode != string.Empty)
            .ToList();

        return new QuestionCoverageScore
        {
            QuestionNumber = question.QuestionNumber,
            IndicatorScores = indicatorScores,
            CoverageScore = indicatorScores.Count == 0
                ? 0
                : Math.Round(indicatorScores.Average(i => i.CoverageScore), 2)
        };
    }

    private IndicatorCoverageScore ScoreIndicator(string indicatorName, List<DataSourceAssessmentWithSource> assessments)
    {
        if (!EwFrameworkIndicators.Indicators.TryGetValue(indicatorName, out var indicator))
            return new IndicatorCoverageScore();

        var dataElementScores = indicator.DataElementNames
            .Select(name => ScoreDataElement(name, indicatorName, assessments))
            .Where(s => s.DataElementName != string.Empty)
            .ToList();

        return new IndicatorCoverageScore
        {
            IndicatorCode = indicatorName,
            Sectors = indicator.Sectors,
            DataElementScores = dataElementScores,
            CoverageScore = dataElementScores.Count == 0
                ? 0
                : Math.Round(dataElementScores.Average(x => x.QualityScore), 2)
        };
    }

    private DataElementScore ScoreDataElement(string dataElementName, string indicatorName, List<DataSourceAssessmentWithSource> assessments)
    {
        EwFrameworkDataElements.Elements.TryGetValue(dataElementName, out var dataElement);

        var matches = assessments
            .Where(a => a.Active ?? false)
            .SelectMany(a => a.DataElementAssessments
                .Where(d => d.DataElementName.Equals(dataElementName, StringComparison.OrdinalIgnoreCase))
                .Select(d => new DataElementAssessmentContext
                {
                    AssessmentId = a.Id,
                    AssessmentName = a.Name,
                    DataSourceType = a.DataSourceType,
                    DataSourceName = a.DataSourceName,
                    Assessment = d
                }))
            .ToList();

        var request = new DataElementScoringRequest
        {
            DataElementName = dataElementName,
            IndicatorName = indicatorName,
            ScoringRuleName = dataElement?.ScoringRuleName ?? "ReportedAndCount",
            Matches = matches
        };

        return _ruleRegistry.Resolve(request.ScoringRuleName).Score(request);
    }

    private List<DisaggregateCoverageScore> ScoreDisaggregates(List<DataSourceAssessmentWithSource> assessments) =>
    [.. EwFrameworkDisaggregates.Disaggregates
        .Select(d => new DisaggregateCoverageScore
        {
            Name = d.Name,
            DataElementScores = [.. d.DataElementNames.Select(name => ScoreDataElement(name, d.Name, assessments))]
        })];

    private static decimal AverageCoverage(List<QuestionCoverageScore> questionScores)
    {
        var indicatorScores = questionScores
            .SelectMany(q => q.IndicatorScores)
            .Select(i => i.CoverageScore)
            .ToList();

        return indicatorScores.Count == 0
            ? 0
            : Math.Round(indicatorScores.Average(), 2);
    }
}
