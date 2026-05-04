using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class TeacherReportsSocialEmotionalEdFiAssessor : IEdFiAssessor
{
    private static readonly string[] _surveyKeywords =
    [
        // Generic terms
        "social-emotional",
        "social emotional",
        "social development",
        "emotional development",
        "sel",
        "social skills",
        "emotional functioning",
        "identity and belonging",
        "positive relationships",

        // Instruments named in the E-W Framework
        "CBRS",                              // Child Behavior Rating Scale
        "Child Behavior Rating",
        "Child Behavior Rating Scale",
        "DECA",                              // Devereux Early Childhood Assessment
        "DECA-P2",                           // DECA Preschool Program, 2nd Edition
        "Devereux",
        "Devereux Early Childhood",

        // Other widely used teacher report SEL instruments
        "SAEBRS",                            // Social, Academic, and Emotional Behavior Risk Screener
        "Panorama",
        "Second Step",
        "CASEL",
        "BASC",                              // Behavior Assessment System for Children
        "BESS",                              // Behavioral and Emotional Screening System
        "SSIS",                              // Social Skills Improvement System
        "Social Skills Improvement",
        "DESSA",                             // Devereux Student Strengths Assessment
        "LearnPad"
    ];

    public string DataElementName => "Teacher reports of social-emotional development";

    public string AssessmentDescription =>
        "Identifies surveys linked to teacher reports of social-emotional development (e.g., " +
        "Child Behavior Rating Scale (CBRS) and Devereux Early Childhood Assessment Preschool " +
        "Program (DECA-P2) named in the E-W Framework, plus SAEBRS, Panorama, Second Step, " +
        "BASC/BESS, SSIS, DESSA) by matching well-known instrument names and social-emotional " +
        "keywords in the Ed-Fi surveys catalog. Ed-Fi has no standard descriptor for social-emotional " +
        "teacher reports, so title-based matching is used as a proxy.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching survey catalog for teacher reports of social-emotional development...");

        var matchingSurveys = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiSurvey>(
            httpClient,
            "ed-fi/surveys",
            survey =>
            {
                if (IsSocialEmotionalSurvey(survey))
                    matchingSurveys.Add((survey.SurveyIdentifier, survey.Namespace));
            },
            context);

        context.Log($"Found {matchingSurveys.Count} social-emotional teacher report survey(s) in catalog");

        if (matchingSurveys.Count == 0)
        {
            context.ReportProgress(100, "Complete — no social-emotional teacher report surveys found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No surveys matching recognized social-emotional teacher report instruments were found " +
                    "in the survey catalog."
            };
        }

        context.ReportProgress(50, "Counting survey responses for social-emotional teacher reports...");

        var totalResponses = 0;
        var surveyDistribution = new Dictionary<string, int>();

        foreach (var (identifier, ns) in matchingSurveys)
        {
            var count = await EdFiApiPatterns.CountFromHeaderAsync(
                httpClient,
                "ed-fi/surveyResponses",
                new Dictionary<string, string>
                {
                    ["surveyIdentifier"] = identifier,
                    ["namespace"] = ns
                });

            totalResponses += count;
            if (count > 0)
                surveyDistribution[identifier] = count;
        }

        context.Log($"Found {totalResponses:N0} social-emotional teacher report survey responses");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalResponses),
                new Distribution(surveyDistribution, "Survey Instrument")
            ],
            Remarks = AssessmentDescription
        };
    }

    private static bool IsSocialEmotionalSurvey(EdFiSurvey survey)
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
