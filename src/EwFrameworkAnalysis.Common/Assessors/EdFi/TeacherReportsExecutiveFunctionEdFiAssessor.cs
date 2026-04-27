using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class TeacherReportsExecutiveFunctionEdFiAssessor : IEdFiAssessor
{
    private static readonly string[] _surveyKeywords =
    [
        // Generic terms
        "executive function",
        "cognitive self-regulation",
        "behavioral self-regulation",
        "self-regulation",
        "self regulation",
        "inhibitory control",
        "attention regulation",
        "approaches to learning",

        // Instrument named in the E-W Framework
        "CBRS",                              // Child Behavior Rating Scale
        "Child Behavior Rating",
        "Child Behavior Rating Scale",

        // Other widely used teacher report instruments for EF
        "BRIEF",                             // Behavior Rating Inventory of Executive Function
        "Behavior Rating Inventory of Executive Function",
        "BRIEF-P",                           // BRIEF - Preschool Version
        "BRIEF-2",                           // BRIEF, Second Edition
        "CHEXI",                             // Childhood Executive Functioning Inventory
        "Childhood Executive Functioning",
        "DECA",                              // Devereux Early Childhood Assessment
        "Devereux",
        "LearnPad"
    ];

    public string DataElementName => "Teacher reports of executive function";

    public string AssessmentDescription =>
        "Identifies surveys linked to teacher reports of children's executive function (e.g., " +
        "Child Behavior Rating Scale (CBRS) named in the E-W Framework, plus BRIEF / BRIEF-P / BRIEF-2, " +
        "CHEXI, DECA) by matching well-known instrument names and executive function keywords in the " +
        "Ed-Fi surveys catalog. Ed-Fi has no standard descriptor for executive function teacher " +
        "reports, so title-based matching is used as a proxy.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching survey catalog for teacher reports of executive function...");

        var matchingSurveys = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiSurvey>(
            httpClient,
            "ed-fi/surveys",
            survey =>
            {
                if (IsExecutiveFunctionSurvey(survey))
                    matchingSurveys.Add((survey.SurveyIdentifier, survey.Namespace));
            },
            context);

        context.Log($"Found {matchingSurveys.Count} executive function teacher report survey(s) in catalog");

        if (matchingSurveys.Count == 0)
        {
            context.ReportProgress(100, "Complete — no executive function teacher report surveys found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No surveys matching recognized executive function teacher report instruments were found " +
                    "in the survey catalog."
            };
        }

        context.ReportProgress(50, "Counting survey responses for executive function teacher reports...");

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

        context.Log($"Found {totalResponses:N0} executive function teacher report survey responses");
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

    private static bool IsExecutiveFunctionSurvey(EdFiSurvey survey)
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
