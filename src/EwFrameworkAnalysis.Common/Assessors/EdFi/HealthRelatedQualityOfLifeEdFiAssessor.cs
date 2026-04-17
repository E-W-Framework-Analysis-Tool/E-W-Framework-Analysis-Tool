using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class HealthRelatedQualityOfLifeEdFiAssessor : IEdFiAssessor
{
    private static readonly string[] _surveyKeywords =
    [
        // Generic terms
        "health-related quality of life",
        "health related quality of life",
        "HRQoL",
        "HRQOL",
        "quality of life scale",
        "self-rated health",
        "self rated health",

        // Named HRQoL instruments
        "SF-36",                             // 36-Item Short Form Health Survey
        "SF-12",                             // 12-Item Short Form Health Survey
        "SF-8",
        "PROMIS",                            // Patient-Reported Outcomes Measurement Information System
        "EQ-5D",                             // EuroQol 5-Dimension
        "EuroQol",
        "CDC HRQOL",                         // CDC Healthy Days Core Module
        "Healthy Days",
        "PedsQL"                             // Pediatric Quality of Life Inventory
    ];

    public string DataElementName => "Health-Related Quality of Life Scale scores";

    public string AssessmentDescription =>
        "Identifies surveys linked to Health-Related Quality of Life (HRQoL) " +
        "instruments named in the E-W Framework and widely used in postsecondary / adult populations " +
        "(e.g., SF-36 / SF-12 Short Form Health Survey, PROMIS, EQ-5D, CDC Healthy Days, PedsQL, " +
        "Self-Rated Health Scale). Ed-Fi has no standard descriptor for HRQoL surveys, so " +
        "title-based matching against the surveys catalog is used as a proxy.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching survey catalog for Health-Related Quality of Life instruments...");

        var matchingSurveys = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiSurvey>(
            httpClient,
            "ed-fi/surveys",
            survey =>
            {
                if (IsHrqolSurvey(survey))
                    matchingSurveys.Add((survey.SurveyIdentifier, survey.Namespace));
            },
            context);

        context.Log($"Found {matchingSurveys.Count} HRQoL survey(s) in catalog");

        if (matchingSurveys.Count == 0)
        {
            context.ReportProgress(100, "Complete — no HRQoL surveys found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No surveys matching recognized Health-Related Quality of Life instruments " +
                    "were found in the survey catalog."
            };
        }

        context.ReportProgress(50, "Loading survey responses for HRQoL instruments...");

        var surveyResponses = new List<EdFiSurveyResponse>();
        var instrumentDistribution = new Dictionary<string, int>();

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
                instrumentDistribution[identifier] = added;
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

        context.Log($"Found {totalResponses:N0} HRQoL survey responses");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalResponses),
                new Distribution(instrumentDistribution, "Survey Instrument"),
                new Distribution(surveyLevelDistribution, "Survey Level")
            ],
            Remarks = AssessmentDescription
        };
    }

    private static bool IsHrqolSurvey(EdFiSurvey survey)
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
