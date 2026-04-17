using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class SelfManagementSurveysK12EdFiAssessor : IEdFiAssessor
{
    private static readonly string[] _surveyKeywords =
    [
        // Generic terms
        "self-management",
        "self management",
        "self-regulation",
        "self regulation",
        "emotion regulation",
        "behavior regulation",
        "impulse control",
        "executive function",

        // Instrument named in the E-W Framework
        "Shift and Persist",

        // Other widely used K-12 survey suites with a self-management scale
        "Panorama",                          // Panorama SEL / Student Survey
        "CORE Districts",
        "CORE SEL",
        "Tripod",                            // Tripod Student Survey
        "PERTS",                             // Project for Education Research That Scales
        "DESSA",                             // Devereux Student Strengths Assessment
        "SAEBRS",                            // Social, Academic, and Emotional Behavior Risk Screener
        "SSIS",                              // Social Skills Improvement System
        "BRIEF",                             // Behavior Rating Inventory of Executive Function
        "ERICA",                             // Emotion Regulation Index for Children and Adolescents
        "ERQ"                                // Emotion Regulation Questionnaire
    ];

    public string DataElementName => "Self-management surveys (K-12)";

    public string AssessmentDescription =>
        "Identifies surveys linked to K-12 self-management instruments (e.g., Shift and " +
        "Persist scale for children named in the E-W Framework, plus Panorama / CORE Districts SEL surveys, " +
        "Tripod, PERTS, DESSA, SAEBRS, SSIS, BRIEF, ERQ that include a self-management / self-regulation " +
        "scale) by matching well-known instrument names and self-management keywords in the Ed-Fi surveys " +
        "catalog. Ed-Fi has no standard descriptor for self-management surveys, so title-based matching is " +
        "used as a proxy.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching survey catalog for K-12 self-management surveys...");

        var matchingSurveys = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiSurvey>(
            httpClient,
            "ed-fi/surveys",
            survey =>
            {
                if (IsSelfManagementSurvey(survey))
                    matchingSurveys.Add((survey.SurveyIdentifier, survey.Namespace));
            },
            context);

        context.Log($"Found {matchingSurveys.Count} K-12 self-management survey(s) in catalog");

        if (matchingSurveys.Count == 0)
        {
            context.ReportProgress(100, "Complete — no K-12 self-management surveys found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No surveys matching recognized K-12 self-management instruments were found " +
                    "in the survey catalog."
            };
        }

        context.ReportProgress(50, "Loading survey responses for K-12 self-management surveys...");

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

        context.Log($"Found {totalResponses:N0} K-12 self-management survey responses");
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

    private static bool IsSelfManagementSurvey(EdFiSurvey survey)
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
