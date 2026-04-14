using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class TeacherReportsExecutiveFunctionEdFiAssessor : IEdFiAssessor
{
    // Keywords for teacher- or parent-rating instruments that measure a child's
    // executive function through adult observation (rather than direct child performance).
    private static readonly string[] _assessmentKeywords =
    [
        // Generic terms
        "executive function",
        "executive functioning",
        "teacher report",
        "teacher rating",
        "teacher-rated",
        "behavior rating",
        "behavioral rating",

        // Instrument named in the E-W Framework
        "CBRS",                              // Child Behavior Rating Scale
        "Child Behavior Rating Scale",

        // Other common teacher-/parent-report EF instruments
        "BRIEF",                             // Behavior Rating Inventory of Executive Function
        "Behavior Rating Inventory of Executive Function",
        "CHEXI",                             // Childhood Executive Functioning Inventory
        "Childhood Executive Functioning Inventory",
        "TRS",                               // Teacher Rating Scale
        "Conners"
    ];

    public string DataElementName => "Teacher reports of executive function";

    public string AssessmentDescription =>
        "Identifies student assessments linked to teacher (or parent) rating instruments of children's " +
        "executive function. Matches the Child Behavior Rating Scale (CBRS) named in the E-W Framework " +
        "along with other widely used adult-report executive function instruments (BRIEF, CHEXI, Conners). " +
        "Ed-Fi has no standard descriptor for executive function teacher reports, so title-based matching " +
        "against the assessments catalog is used.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for teacher reports of executive function...");

        var matchingAssessments = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (IsTeacherReportExecutiveFunction(assessment))
                    matchingAssessments.Add((assessment.AssessmentIdentifier, assessment.Namespace));
            },
            context);

        context.Log($"Found {matchingAssessments.Count} teacher-report executive function assessment(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no teacher-report executive function assessments found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized teacher- or parent-report executive function " +
                    "instruments were found in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Loading student assessment results for teacher-report instruments...");

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
            $"Found {result.RecordsWithScores:N0} of {result.TotalRecords:N0} teacher-report executive function results with score results");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(result.TotalRecords),
                new Distribution(instrumentDistribution, "Assessment Instrument"),
                new Distribution(result.GradeLevelDistribution, "Grade Level Assessed"),
                new Distribution(result.PerformanceLevelDistribution, "Performance Level / Rating"),
                new Completeness(result.TotalRecords, result.RecordsWithScores, "ScoreResults")
            ],
            Remarks = AssessmentDescription
        };
    }

    private static bool IsTeacherReportExecutiveFunction(EdFiAssessment assessment)
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
