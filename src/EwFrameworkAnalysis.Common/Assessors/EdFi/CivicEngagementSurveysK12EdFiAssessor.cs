using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class CivicEngagementSurveysK12EdFiAssessor : IEdFiAssessor
{
    private static readonly string[] _surveyKeywords =
    [
        "civic engagement",
        "civic participation",
        "civic responsibility",
        "civic knowledge",
        "civic attitudes",
        "civic behavior",
        "civic identity",
        "civic character",
        "youth civic",
        "civic and character",
        "character measures",
        "civic indicators",
        "civic mindedness",
        "civic-mindedness",
        "community engagement",
        "YCCMT",   // Youth Civic and Character Measures Toolkit
        "YCEIP"    // Youth Civic Engagement Indicators Project
    ];

    public string DataElementName => "Civic engagement surveys (K-12)";

    public string AssessmentDescription =>
        "Identifies surveys linked to K-12 civic engagement instruments " +
        "(e.g., Youth Civic and Character Measures Toolkit, Youth Civic Engagement Indicators Project) " +
        "by matching well-known instrument names and civic-engagement keywords in the Ed-Fi surveys " +
        "catalog. Ed-Fi has no standard descriptor for civic engagement surveys, so title-based matching " +
        "is used as a proxy.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching survey catalog for civic engagement surveys...");

        var matchingSurveys = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiSurvey>(
            httpClient,
            "ed-fi/surveys",
            survey =>
            {
                if (IsCivicEngagementSurvey(survey))
                    matchingSurveys.Add((survey.SurveyIdentifier, survey.Namespace));
            },
            context);

        context.Log($"Found {matchingSurveys.Count} civic engagement survey(s) in catalog");

        if (matchingSurveys.Count == 0)
        {
            context.ReportProgress(100, "Complete — no civic engagement surveys found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No surveys matching recognized civic engagement instruments were found " +
                    "in the survey catalog."
            };
        }

        context.ReportProgress(50, "Loading survey responses for civic engagement surveys...");

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
                if (!surveyLevelDistribution.ContainsKey(levelValue))
                    surveyLevelDistribution[levelValue] = 0;
                surveyLevelDistribution[levelValue]++;
            }
        }

        context.Log($"Found {totalResponses:N0} civic engagement survey responses");
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

    private static bool IsCivicEngagementSurvey(EdFiSurvey survey)
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
