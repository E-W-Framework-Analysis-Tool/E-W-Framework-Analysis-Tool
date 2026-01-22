
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
        var indicators = EwFrameworkIndicators.Indicators;

        // Flatten assessed data elements with their parent assessment for tracking
        var assessedDataElementsWithSource = assessments.Where(x => x.Active ?? false)
            .SelectMany(a => a.DataElementAssessments.Select(de => new { Assessment = a, DataElement = de }))
            .ToList();

        var questionScores = new List<QuestionScore>();
        foreach (var question in questions)
        {
            var questionScore = new QuestionScore
            {
                QuestionNumber = question.QuestionNumber
            };

            foreach (var indicatorName in question.RelatedIndicatorNames)
            {
                if (!indicators.TryGetValue(indicatorName, out var indicator))
                    continue;

                var indicatorScore = new IndicatorScore
                {
                    IndicatorCode = indicatorName
                };

                foreach (var dataElementName in indicator.DataElementNames)
                {

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

                    EwFrameworkDataElements.Elements.TryGetValue(dataElementName, out var dataElement);

                    var scoringRequest = new DataElementScoringRequest
                    {
                        DataElementName = dataElementName,
                        IndicatorName = indicatorName,
                        ScoringRuleName = dataElement?.ScoringRuleName ?? "ReportedAndCount",
                        Matches = matches
                    };

                    var rule = _ruleRegistry.Resolve(scoringRequest.ScoringRuleName);
                    var dataElementScore = rule.Score(scoringRequest);

                    indicatorScore.DataElementScores.Add(dataElementScore);

                }

                // Simple readiness metric: % of data elements available
                if (indicatorScore.DataElementScores.Count > 0)
                {
                    var availableCount = indicatorScore.DataElementScores.Count(x => x.IsAvailable);
                    indicatorScore.ReadinessScore =
                        Math.Round((decimal)availableCount / indicatorScore.DataElementScores.Count, 2);
                }

                questionScore.IndicatorScores.Add(indicatorScore);
            }

            // Compute overall question readiness as the average of indicator scores
            if (questionScore.IndicatorScores.Count > 0)
            {
                questionScore.ReadinessScore =
                    Math.Round(questionScore.IndicatorScores.Average(i => i.ReadinessScore), 2);
            }

            questionScores.Add(questionScore);
        }

        return questionScores;
    }
}
