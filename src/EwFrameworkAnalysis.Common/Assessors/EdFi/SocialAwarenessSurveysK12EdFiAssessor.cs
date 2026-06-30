using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class SocialAwarenessSurveysK12EdFiAssessor : IEdFiAssessor
{
    private static readonly string[] _surveyKeywords =
    [
        // Generic terms
        "social awareness",
        "social skills",
        "empathy",
        "perspective taking",
        "perspective-taking",
        "social perspective",

        // Instrument named in the E-W Framework
        "SSIS SEL",                          // Social Skills Improvement System, SEL Edition
        "SSIS",                              // Social Skills Improvement System
        "Social Skills Improvement",

        // Other widely used K-12 SEL survey suites with a social awareness scale
        "Panorama",                          // Panorama SEL / Student Survey
        "CORE Districts",
        "CORE SEL",
        "DESSA",                             // Devereux Student Strengths Assessment
        "SAEBRS",                            // Social, Academic, and Emotional Behavior Risk Screener
        "BASC",                              // Behavior Assessment System for Children
        "BESS",                              // Behavioral and Emotional Screening System
        "Second Step"
    ];

    public string DataElementName => "Social awareness surveys (K-12)";

    public string AssessmentDescription =>
        "Identifies surveys linked to K-12 social awareness instruments (e.g., the Social Skills " +
        "Improvement System, SEL Edition (SSIS SEL) named in the E-W Framework, plus Panorama / CORE " +
        "Districts SEL surveys, DESSA, SAEBRS, BASC/BESS, Second Step that include a social awareness " +
        "scale) by matching well-known instrument names and social-awareness keywords in the Ed-Fi " +
        "surveys catalog. Ed-Fi has no standard descriptor for social awareness surveys, so title-based " +
        "matching is used as a proxy. This is the survey-instrument variant; the teacher-rating " +
        "(assessment) variant is covered by Social awareness teacher ratings.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching survey catalog for K-12 social awareness surveys...");

        var matchingSurveys = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiSurvey>(
            httpClient,
            "ed-fi/surveys",
            survey =>
            {
                if (IsSocialAwarenessSurvey(survey))
                    matchingSurveys.Add((survey.SurveyIdentifier, survey.Namespace));
            },
            context);

        context.Log($"Found {matchingSurveys.Count} K-12 social awareness survey(s) in catalog");

        if (matchingSurveys.Count == 0)
        {
            context.ReportProgress(100, "Complete — no K-12 social awareness surveys found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No surveys matching recognized K-12 social awareness instruments were found " +
                    "in the survey catalog."
            };
        }

        context.ReportProgress(50, "Loading survey responses for K-12 social awareness surveys...");

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

        context.Log($"Found {totalResponses:N0} K-12 social awareness survey responses");
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

    private static bool IsSocialAwarenessSurvey(EdFiSurvey survey)
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
