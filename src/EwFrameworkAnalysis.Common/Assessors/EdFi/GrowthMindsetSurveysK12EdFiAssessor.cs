using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class GrowthMindsetSurveysK12EdFiAssessor : IEdFiAssessor
{
    private static readonly string[] _surveyKeywords =
    [
        // Generic terms
        "growth mindset",
        "fixed mindset",
        "mindset scale",
        "mindset survey",
        "implicit theories of intelligence",
        "theories of intelligence",

        // Named instruments / survey suites that include a growth mindset scale
        "Dweck Mindset",
        "PERTS",                             // Project for Education Research That Scales
        "Mindset Meter",
        "Panorama",                          // Panorama SEL / Student Survey
        "CORE Districts",
        "CORE SEL",
        "Tripod"                             // Tripod Student Survey
    ];

    public string DataElementName => "Growth mindset surveys (K-12)";

    public string AssessmentDescription =>
        "Identifies surveys linked to K-12 growth mindset instruments " +
        "(e.g., Dweck Mindset Scale / Implicit Theories of Intelligence Scale, PERTS Mindset Meter, " +
        "Panorama / CORE Districts SEL surveys with a growth-mindset scale) by matching well-known " +
        "instrument names and growth-mindset keywords in the Ed-Fi surveys catalog. Ed-Fi has no " +
        "standard descriptor for growth mindset surveys, so title-based matching is used as a proxy.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching survey catalog for growth mindset surveys...");

        var matchingSurveys = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiSurvey>(
            httpClient,
            "ed-fi/surveys",
            survey =>
            {
                if (IsGrowthMindsetSurvey(survey))
                    matchingSurveys.Add((survey.SurveyIdentifier, survey.Namespace));
            },
            context);

        context.Log($"Found {matchingSurveys.Count} growth mindset survey(s) in catalog");

        if (matchingSurveys.Count == 0)
        {
            context.ReportProgress(100, "Complete — no growth mindset surveys found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No surveys matching recognized growth mindset instruments were found " +
                    "in the survey catalog."
            };
        }

        context.ReportProgress(50, "Loading survey responses for growth mindset surveys...");

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

        context.Log($"Found {totalResponses:N0} growth mindset survey responses");
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

    private static bool IsGrowthMindsetSurvey(EdFiSurvey survey)
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
