
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
            DataElementScores = dataElementScores
        };        

        // Simple readiness metric: % of data elements available
        if (indicatorScore.DataElementScores.Count > 0)
        {
            var availableCount = indicatorScore.DataElementScores.Count(x => x.IsAvailable);
            indicatorScore.ReadinessScore =
                Math.Round((decimal)availableCount / indicatorScore.DataElementScores.Count, 2);
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
}
