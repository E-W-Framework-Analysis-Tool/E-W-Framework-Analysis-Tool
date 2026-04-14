using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class DirectChildAssessmentsPhysicalDevelopmentEdFiAssessor : IEdFiAssessor
{
    // Keywords for direct child assessments of physical development — gross/fine motor skills,
    // administered by teachers, healthcare professionals, or other qualified adults.
    private static readonly string[] _assessmentKeywords =
    [
        // Generic terms
        "physical development",
        "motor development",
        "motor skills",
        "gross motor",
        "fine motor",
        "motor proficiency",
        "psychomotor",

        // Instrument named in the E-W Framework
        "Peabody Developmental Motor",       // Peabody Developmental Motor Scales (PDMS / PDMS-2)
        "PDMS",

        // Other widely used direct motor / physical development assessments
        "BOT-2",                             // Bruininks-Oseretsky Test of Motor Proficiency
        "Bruininks-Oseretsky",
        "Movement ABC",                      // Movement Assessment Battery for Children
        "MABC",
        "TGMD",                              // Test of Gross Motor Development
        "Test of Gross Motor Development"
    ];

    public string DataElementName => "Direct child assessments of physical development";

    public string AssessmentDescription =>
        "Identifies student assessments linked to direct child assessments of physical development " +
        "(gross and fine motor skills) administered by teachers, healthcare professionals, or other " +
        "qualified adults. Matches the Peabody Developmental Motor Scale named in the E-W Framework " +
        "along with other widely used direct motor instruments (Bruininks-Oseretsky BOT-2, Movement " +
        "ABC, Test of Gross Motor Development). Ed-Fi has no standard descriptor for physical " +
        "development assessments, so title-based matching against the assessments catalog is used.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for direct physical development assessments...");

        var matchingAssessments = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (IsPhysicalDevelopmentAssessment(assessment))
                    matchingAssessments.Add((assessment.AssessmentIdentifier, assessment.Namespace));
            },
            context);

        context.Log($"Found {matchingAssessments.Count} direct physical development assessment(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no direct physical development assessments found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized direct child physical development instruments " +
                    "were found in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Loading student assessment results for physical development assessments...");

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
            $"Found {result.RecordsWithScores:N0} of {result.TotalRecords:N0} physical development assessment results with score results");
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

    private static bool IsPhysicalDevelopmentAssessment(EdFiAssessment assessment)
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
