using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class DirectChildAssessmentsExecutiveFunctionEdFiAssessor : IEdFiAssessor
{
    private static readonly string[] _assessmentKeywords =
    [
        // Generic terms
        "executive function",
        "cognitive self-regulation",
        "inhibitory control",
        "working memory",
        "cognitive flexibility",
        "attention shifting",
        "approaches to learning",

        // Instruments named in the E-W Framework
        "HTKS",                              // Heads Toes Knees Shoulders task
        "Heads Toes Knees Shoulders",
        "Heads-Toes-Knees-Shoulders",
        "MEFS",                              // Minnesota Executive Function Scale
        "Minnesota Executive Function",

        // Other widely used direct child EF assessment instruments
        "Dimensional Change Card Sort",
        "DCCS",                              // Dimensional Change Card Sort
        "Flanker",                           // NIH Toolbox Flanker Inhibitory Control
        "NIH Toolbox",
        "Day-Night",                         // Day-Night Stroop task
        "Shape School",
        "Tower of Hanoi",
        "BRIEF-P"                            // Behavior Rating Inventory of Executive Function - Preschool
    ];

    public string DataElementName => "Direct child assessments of executive function";

    public string AssessmentDescription =>
        "Identifies student assessments linked to direct child assessments of executive function (e.g., " +
        "Heads Toes Knees Shoulders (HTKS) task and Minnesota Executive Function Scale (MEFS) named in " +
        "the E-W Framework, plus Dimensional Change Card Sort (DCCS), NIH Toolbox Flanker, Day-Night " +
        "Stroop, BRIEF-P) by matching well-known instrument names and executive function keywords in " +
        "the Ed-Fi assessments catalog. Ed-Fi has no standard descriptor for executive function " +
        "assessments, so title-based matching is used as a proxy.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for direct child EF assessments...");

        var matchingAssessments = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (IsExecutiveFunctionAssessment(assessment))
                    matchingAssessments.Add((assessment.AssessmentIdentifier, assessment.Namespace));
            },
            context);

        context.Log($"Found {matchingAssessments.Count} executive function assessment(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no direct child EF assessments found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized direct child executive function instruments were found " +
                    "in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Counting student assessment records for EF instruments...");

        var totalCount = 0;
        var instrumentDistribution = new Dictionary<string, int>();

        foreach (var (identifier, ns) in matchingAssessments)
        {
            var count = await EdFiApiPatterns.CountFromHeaderAsync(
                httpClient,
                "ed-fi/studentAssessments",
                new Dictionary<string, string>
                {
                    ["assessmentIdentifier"] = identifier,
                    ["namespace"] = ns
                });

            totalCount += count;
            if (count > 0)
                instrumentDistribution[identifier] = count;
        }

        context.Log($"Found {totalCount:N0} executive function assessment records");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalCount),
                new Distribution(instrumentDistribution, "Assessment Instrument")
            ],
            Remarks = AssessmentDescription
        };
    }

    private static bool IsExecutiveFunctionAssessment(EdFiAssessment assessment)
    {
        var title = assessment.AssessmentTitle ?? string.Empty;
        var identifier = assessment.AssessmentIdentifier ?? string.Empty;

        foreach (var keyword in _assessmentKeywords)
        {
            if (title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                identifier.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
