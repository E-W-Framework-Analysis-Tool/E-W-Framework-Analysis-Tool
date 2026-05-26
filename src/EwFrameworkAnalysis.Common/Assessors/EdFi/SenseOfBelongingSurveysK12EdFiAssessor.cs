using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class SenseOfBelongingSurveysK12EdFiAssessor : IEdFiAssessor
{
    private static readonly string[] _surveyKeywords =
    [
        "sense of belonging",
        "belonging",
        "school connectedness",
        "school attachment",
        "school bonding",
        "school membership",
        "student connectedness",

        "Panorama belonging",
        "PERTS belonging",
        "CORE belonging"
    ];

    public string DataElementName => "Sense of belonging surveys (K-12)";

    public string AssessmentDescription =>
        "Identifies surveys linked to K-12 sense of belonging instruments (e.g., Panorama " +
        "belonging scale, PERTS belonging survey) by matching well-known instrument names and " +
        "belonging/connectedness keywords in the Ed-Fi surveys catalog.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching survey catalog for K-12 sense of belonging surveys...");

        var matchingSurveys = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiSurvey>(
            httpClient,
            "ed-fi/surveys",
            survey =>
            {
                if (IsSenseOfBelongingSurvey(survey))
                    matchingSurveys.Add((survey.SurveyIdentifier, survey.Namespace));
            },
            context);

        context.Log($"Found {matchingSurveys.Count} K-12 sense of belonging survey(s) in catalog");

        if (matchingSurveys.Count == 0)
        {
            context.ReportProgress(100, "Complete — no K-12 sense of belonging surveys found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No surveys matching recognized K-12 sense of belonging instruments were found " +
                    "in the survey catalog."
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

        context.Log($"Found {totalResponses:N0} sense of belonging survey responses");
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

    private static bool IsSenseOfBelongingSurvey(EdFiSurvey survey)
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
