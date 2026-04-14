using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class DirectChildAssessmentsExecutiveFunctionEdFiAssessor : IEdFiAssessor
{
    // Keywords for instruments that directly assess a child's executive function,
    // administered to the child rather than completed by an adult observer.
    private static readonly string[] _assessmentKeywords =
    [
        // Generic terms
        "executive function",
        "executive functioning",
        "cognitive self-regulation",
        "self-regulation",
        "self regulation",
        "direct child assessment",

        // Instruments named in the E-W Framework
        "HTKS",                              // Heads Toes Knees Shoulders
        "Heads Toes Knees Shoulders",
        "Head Toes Knees Shoulders",
        "MEFS",                              // Minnesota Executive Function Scale
        "Minnesota Executive Function",

        // Other direct child EF assessments commonly used
        "Dimensional Change Card Sort",
        "DCCS",
        "NIH Toolbox Cognition",
        "Flanker",
        "Peg Tapping",
        "Day Night",
        "Bear Dragon"
    ];

    public string DataElementName => "Direct child assessments of executive function";

    public string AssessmentDescription =>
        "Identifies student assessments linked to direct child assessments of executive function " +
        "(administered to the child). Matches instruments named in the E-W Framework such as the " +
        "Heads Toes Knees Shoulders (HTKS) task and the Minnesota Executive Function Scale (MEFS), " +
        "along with other direct-assessment executive function tools (e.g., Dimensional Change Card " +
        "Sort, NIH Toolbox Cognition, Flanker). Ed-Fi has no standard descriptor for executive " +
        "function assessments, so title-based matching against the assessments catalog is used.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for direct executive function assessments...");

        var matchingAssessments = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (IsDirectExecutiveFunctionAssessment(assessment))
                    matchingAssessments.Add((assessment.AssessmentIdentifier, assessment.Namespace));
            },
            context);

        context.Log($"Found {matchingAssessments.Count} direct executive function assessment(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no direct executive function assessments found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized direct child executive function instruments " +
                    "were found in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Loading student assessment results for executive function assessments...");

        var studentAssessments = new List<EdFiStudentAssessment>();
        var instrumentDistribution = new Dictionary<string, int>();

        foreach (var (identifier, ns) in matchingAssessments)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["assessmentIdentifier"] = identifier,
                ["namespace"] = ns
            };

            var countBefore = studentAssessments.Count;

            await EdFiApiPatterns.PageAndProcessAsync<EdFiStudentAssessment>(
                httpClient,
                "ed-fi/studentAssessments",
                item => studentAssessments.Add(item),
                context,
                queryParams);

            var added = studentAssessments.Count - countBefore;
            if (added > 0)
                instrumentDistribution[identifier] = added;
        }

        var result = StudentAssessmentAnalyzer.Analyze(studentAssessments);

        context.Log(
            $"Found {result.RecordsWithScores:N0} of {result.TotalRecords:N0} executive function assessment results with score results");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(result.TotalRecords),
                new Distribution(instrumentDistribution, "Assessment Instrument"),
                new Distribution(result.GradeLevelDistribution, "Grade Level Assessed"),
                new Distribution(result.PerformanceLevelDistribution, "Performance Level / Proficiency"),
                new Completeness(result.TotalRecords, result.RecordsWithScores, "ScoreResults")
            ],
            Remarks = AssessmentDescription
        };
    }

    private static bool IsDirectExecutiveFunctionAssessment(EdFiAssessment assessment)
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
