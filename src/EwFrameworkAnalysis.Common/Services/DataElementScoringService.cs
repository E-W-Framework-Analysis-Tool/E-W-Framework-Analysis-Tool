
using EwFrameworkAnalysis.Common.FrameworkReferenceData;
using EwFrameworkAnalysis.Common.Models.Framework;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Models.Scoring;
using EwFrameworkAnalysis.Common.Scoring;

namespace EwFrameworkAnalysis.Common.Services;

public class DataElementScoringService
{
    private readonly DataElementScoringRuleRegistry _ruleRegistry;
    private readonly Dictionary<string, Indicator> _indicators;

    public DataElementScoringService(DataElementScoringRuleRegistry ruleRegistry, EcsStateDataProvider ecsStateDataProvider)
    {
        _ruleRegistry = ruleRegistry;
        _indicators = BuildEnrichedIndicators(ecsStateDataProvider);
    }

    private static Dictionary<string, Indicator> BuildEnrichedIndicators(EcsStateDataProvider ecsStateDataProvider)
    {
        // Deep copy indicators so we don't mutate the static reference data
        var indicators = EwFrameworkIndicators.Indicators.ToDictionary(
            kvp => kvp.Key,
            kvp => new Indicator
            {
                Name = kvp.Value.Name,
                Type = kvp.Value.Type,
                Domain = kvp.Value.Domain,
                Definition = kvp.Value.Definition,
                RecommendedMetrics = kvp.Value.RecommendedMetrics,
                DataNeeded = [.. kvp.Value.DataNeeded],
                Sectors = [.. kvp.Value.Sectors],
                DataElementNames = [.. kvp.Value.DataElementNames]
            });

        // Enrich each indicator with data element names from all ECS state files
        foreach (var state in ecsStateDataProvider.GetAvailableStates())
        {
            var records = ecsStateDataProvider.GetStateData(state);

            foreach (var group in records.GroupBy(r => r.Indicator, StringComparer.OrdinalIgnoreCase))
            {
                if (!indicators.TryGetValue(group.Key, out var indicator))
                    continue;

                var existing = new HashSet<string>(indicator.DataElementNames, StringComparer.OrdinalIgnoreCase);

                foreach (var elementName in group.Select(r => r.ElementName.Trim()).Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrWhiteSpace(elementName) && existing.Add(elementName))
                    {
                        indicator.DataElementNames.Add(elementName);
                    }
                }
            }
        }

        return indicators;
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
        if (!_indicators.TryGetValue(indicatorName, out var indicator))
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

    public OverallReadinessResults CalculateOverallReadinessScores(List<DataSourceAssessmentWithSource> assessments)
    {
        // Score All Active Assessments

        var assessmentScores = new List<AssessmentScore>();

        foreach (var assessment in assessments.Where(a => a.Active == true))
        {
            var score = GetAssessmentScore(assessment);
            assessmentScores.Add(score);
        }

        // Group IDs
        var manualIds = assessments
            .Where(a => a.DataSourceType == DataSourceType.Custom)
            .Select(a => a.Id)
            .ToHashSet();

        var publicIds = assessments
            .Where(a => a.DataSourceType == DataSourceType.EdFiApi ||
                        a.DataSourceType == DataSourceType.CedsDw ||
                        a.DataSourceType == DataSourceType.EcsExcel)
            .Select(a => a.Id)
            .ToHashSet();

        var allIds = assessments
            .Select(a => a.Id)
            .ToHashSet();

        // Helper to compute average indicator readiness across selected assessment scores
        decimal ComputeScore(HashSet<Guid> selectedIds)
        {
            var scores = assessmentScores
                .Where(a => selectedIds.Contains(a.DataSourceAssessmentId))
                .SelectMany(a => a.QuestionScores)
                .SelectMany(q => q.IndicatorScores)
                .Select(i => i.ReadinessScore)
                .ToList();

            if (scores.Count == 0)
                return 0;

            return Math.Round(scores.Average(), 2);
        }

        return new OverallReadinessResults
        {
            CurrentReadiness = ComputeScore(manualIds),
            WithPublicDataReadiness = ComputeScore(publicIds),
            CompleteReadiness = ComputeScore(allIds)
        };
    }

    private AssessmentScore GetAssessmentScore(DataSourceAssessmentWithSource assessment)
    {
        var questions = EwFrameworkEssentialQuestions.Questions;
        var indicators = _indicators;
        var dataElements = EwFrameworkDataElements.Elements;

        var assessmentScore = new AssessmentScore
        {
            DataSourceAssessmentId = assessment.Id,
            SourceType = assessment.DataSourceType
        };

        var assessedDataElementsWithSource = assessment
            .DataElementAssessments.Select(de => new { Assessment = assessment, DataElement = de })
            .ToList();

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

                    dataElements.TryGetValue(dataElementName, out var dataElement);

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

                indicatorScore.ReadinessScore =
                    indicatorScore.DataElementScores.Count == 0 ? 0 : indicatorScore.DataElementScores.Average(x => x.QualityScore);

                questionScore.IndicatorScores.Add(indicatorScore);
            }

            questionScore.ReadinessScore =
                questionScore.IndicatorScores.Count == 0
                ? 0
                : questionScore.IndicatorScores.Average(x => x.ReadinessScore);

            assessmentScore.QuestionScores.Add(questionScore);
        }

        return assessmentScore;
    }
}
