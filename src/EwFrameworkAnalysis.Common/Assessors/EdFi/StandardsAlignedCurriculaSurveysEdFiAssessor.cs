using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class StandardsAlignedCurriculaSurveysEdFiAssessor : IEdFiAssessor
{
    private static readonly string[] _surveyKeywords =
    [
        "standards-aligned",
        "standards aligned",
        "curricula alignment",
        "curriculum alignment",
        "culturally responsive",
        "curriculum implementation",
        "curriculum fidelity",
        "instructional materials",
        "curriculum survey",
        "standards-based curricula"
    ];

    public string DataElementName =>
        "Percentage of teachers regularly using standards-aligned; culturally responsive curricula";

    public string AssessmentDescription =>
        "Identifies surveys linked to standards-aligned and culturally responsive curricula usage " +
        "by matching curriculum alignment and fidelity keywords in the Ed-Fi surveys catalog. " +
        "Ed-Fi has no standard descriptor for curricula alignment surveys, so title-based " +
        "matching is used as a proxy.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching survey catalog for standards-aligned curricula surveys...");

        var matchingSurveys = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiSurvey>(
            httpClient,
            "ed-fi/surveys",
            survey =>
            {
                if (IsStandardsAlignedCurriculaSurvey(survey))
                    matchingSurveys.Add((survey.SurveyIdentifier, survey.Namespace));
            },
            context);

        context.Log($"Found {matchingSurveys.Count} standards-aligned curricula survey(s) in catalog");

        if (matchingSurveys.Count == 0)
        {
            context.ReportProgress(100, "Complete — no standards-aligned curricula surveys found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No surveys matching recognized standards-aligned curricula instruments " +
                    "were found in the survey catalog."
            };
        }

        context.ReportProgress(50, "Loading survey responses...");

        var surveyResponses = new List<EdFiSurveyResponse>();
        var surveyDistribution = new Dictionary<string, int>();

        foreach (var (identifier, ns) in matchingSurveys)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["surveyIdentifier"] = identifier,
                ["namespace"] = ns
            };

            var countBefore = surveyResponses.Count;

            await EdFiApiPatterns.PageAndProcessAsync<EdFiSurveyResponse>(
                httpClient,
                "ed-fi/surveyResponses",
                item => surveyResponses.Add(item),
                context,
                queryParams);

            var added = surveyResponses.Count - countBefore;
            if (added > 0)
                surveyDistribution[identifier] = added;
        }

        var totalResponses = surveyResponses.Count;
        var surveyLevelDistribution = new Dictionary<string, int>();

        foreach (var response in surveyResponses)
        {
            if (response.SurveyLevels == null)
                continue;

            foreach (var level in response.SurveyLevels)
            {
                var levelValue = EdFiDescriptorHelper.ParseDescriptorValue(level.SurveyLevelDescriptor);
                surveyLevelDistribution[levelValue] = surveyLevelDistribution.GetValueOrDefault(levelValue) + 1;
            }
        }

        context.Log($"Found {totalResponses:N0} standards-aligned curricula survey responses");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalResponses),
                new Distribution(surveyDistribution, "Survey Instrument"),
                new Distribution(surveyLevelDistribution, "Survey Level")
            ],
            Remarks = AssessmentDescription
        };
    }

    private static bool IsStandardsAlignedCurriculaSurvey(EdFiSurvey survey)
    {
        var title = survey.SurveyTitle ?? string.Empty;
        var identifier = survey.SurveyIdentifier ?? string.Empty;

        foreach (var keyword in _surveyKeywords)
        {
            if (title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                identifier.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
