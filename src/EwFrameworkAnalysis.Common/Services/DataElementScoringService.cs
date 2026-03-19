
using EwFrameworkAnalysis.Common.FrameworkReferenceData;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Models.Scoring;
using EwFrameworkAnalysis.Common.Scoring;

namespace EwFrameworkAnalysis.Common.Services;

public class DataElementScoringService
{
    private DataElementScoringRuleRegistry _ruleRegistry;

    public DataElementScoringService(DataElementScoringRuleRegistry ruleRegistry)
    {
        _ruleRegistry = ruleRegistry;
    }

    public List<QuestionScore> CalculateScoresForActiveData(List<DataSourceAssessmentWithSource> assessments)
    {
        var questions = EwFrameworkEssentialQuestions.Questions;

        var questionScores = new List<QuestionScore>();
        foreach (var question in questions)
        {

            var questionScore = GetQuestionScores(question, assessments);

            questionScores.Add(questionScore);
        }

        return questionScores;
    }

    private QuestionScore GetQuestionScores(EssentialQuestion question, List<DataSourceAssessmentWithSource> assessments)
    {
        var indicatorScores = question.RelatedIndicatorNames
            .Select(indicatorName => GetIndicatorScores(indicatorName, assessments))
            .Where(score => score.IndicatorCode != string.Empty)
            .ToList();

        return new QuestionScore
        {
            QuestionNumber = question.QuestionNumber,
            IndicatorScores = indicatorScores,
            ReadinessScore = indicatorScores.Count == 0
                ? 0
                : Math.Round(indicatorScores.Average(i => i.ReadinessScore), 2)
        };
    }

    private IndicatorScore GetIndicatorScores(string indicatorName, List<DataSourceAssessmentWithSource> assessments)
    {
        if (!EwFrameworkIndicators.Indicators.TryGetValue(indicatorName, out var indicator))
            return new IndicatorScore { };

        var dataElementScores = indicator.DataElementNames
            .Select(dataElementName => GetDataElementScore(dataElementName, indicatorName, assessments))
            .Where(score => score.DataElementName != string.Empty)
            .ToList();

        var indicatorScore = new IndicatorScore
        {
            IndicatorCode = indicatorName,
            Sectors = indicator.Sectors,
            DataElementScores = dataElementScores
        };

        // Weighted readiness: Available=1.0, PartiallyAvailable=0.5, NotAvailable=0.0
        if (indicatorScore.DataElementScores.Count > 0)
        {
            indicatorScore.ReadinessScore =
                Math.Round(indicatorScore.DataElementScores.Average(x => x.QualityScore), 2);
        }

        return indicatorScore;
    }

    private DataElementScore GetDataElementScore(string dataElementName, string indicatorName, List<DataSourceAssessmentWithSource> assessments)
    {

        EwFrameworkDataElements.Elements.TryGetValue(dataElementName, out var dataElement);

        // Flatten assessed data elements with their parent assessment for tracking
        var assessedDataElementsWithSource = assessments
            .Where(x => x.Active ?? false)
            .SelectMany(assessment => assessment.DataElementAssessments.Select(dataElem => new { Assessment = assessment, DataElement = dataElem }))
            .ToList();

        var matches = assessedDataElementsWithSource
            .Where(x => x.DataElement.DataElementName.Equals(dataElementName, StringComparison.OrdinalIgnoreCase))
            .Select(x => new DataElementAssessmentContext
            {
                AssessmentId = x.Assessment.Id,
                AssessmentName = x.Assessment.Name,
                DataSourceType = x.Assessment.DataSourceType,
                DataSourceName = x.Assessment.DataSourceName,
                Assessment = x.DataElement
            })
            .ToList();

        var scoringRequest = new DataElementScoringRequest
        {
            DataElementName = dataElementName,
            IndicatorName = indicatorName,
            ScoringRuleName = dataElement?.ScoringRuleName ?? "ReportedAndCount",
            Matches = matches
        };

        var rule = _ruleRegistry.Resolve(scoringRequest.ScoringRuleName);
        return rule.Score(scoringRequest);
    }

    public List<SectorReadinessResult> CalculateSectorReadiness(IEnumerable<QuestionScore> questionScores)
    {
        var uniqueIndicators = questionScores
            .SelectMany(q => q.IndicatorScores)
            .GroupBy(i => i.IndicatorCode)
            .Select(g => g.First())
            .ToList();

        return [..uniqueIndicators
            .SelectMany(i => i.Sectors.Select(s => new
            {
                Sector = s,
                i.ReadinessScore
            }))
            .GroupBy(x => x.Sector)
            .Select(g => new SectorReadinessResult
            {
                Sector = g.Key,
                ReadinessScore = Math.Round(g.Average(x => x.ReadinessScore), 2),
                IndicatorCount = g.Count()
            })
            .OrderBy(r => r.Sector)];
    }

    public decimal CalculateReadinessForAssessments(List<DataSourceAssessmentWithSource> assessments)
    {
        if (assessments.Count == 0) return 0;

        var scores = CalculateScoresForActiveData(assessments)
            .SelectMany(q => q.IndicatorScores)
            .Select(i => i.ReadinessScore)
            .ToList();

        return scores.Count == 0 ? 0 : Math.Round(scores.Average(), 2);
    }

    public OverallReadinessResults CalculateOverallReadinessScores(List<DataSourceAssessmentWithSource> assessments)
    {
        var active = assessments.Where(a => a.Active == true).ToList();

        return new OverallReadinessResults
        {
            CustomDataSourceReadiness = CalculateReadinessForAssessments([.. active.Where(a => a.DataSourceType == DataSourceType.Custom)]),
            AutomatedDataSourceReadiness = CalculateReadinessForAssessments([.. active.Where(a => a.DataSourceType == DataSourceType.EdFiApi || a.DataSourceType == DataSourceType.CedsDw)]),
            EcsReadiness = CalculateReadinessForAssessments([.. active.Where(a => a.DataSourceType == DataSourceType.EcsState)]),
            CombinedReadiness = CalculateReadinessForAssessments(active),
        };
    }
}
