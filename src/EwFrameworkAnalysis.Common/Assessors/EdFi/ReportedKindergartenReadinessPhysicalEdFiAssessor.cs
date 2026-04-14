using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class ReportedKindergartenReadinessPhysicalEdFiAssessor : IEdFiAssessor
{
    // Keywords for teacher- or parent-observed developmental assessments measuring
    // kindergarten readiness in the physical development / health / motor-skills domain.
    private static readonly string[] _assessmentKeywords =
    [
        // Generic terms
        "kindergarten readiness",
        "kindergarten entry",
        "KEA",                               // Kindergarten Entry Assessment
        "physical development",
        "physical well-being",
        "physical wellbeing",
        "motor development",
        "motor skills",
        "gross motor",
        "fine motor",
        "health, safety",
        "health and safety",

        // Instruments named in the E-W Framework
        "DRDP",                              // Desired Results Developmental Profile — Physical Development/Health
        "Desired Results Developmental Profile",
        "R4K ELA",                           // Ready 4 Kindergarten Early Learning Assessment
        "Ready 4 Kindergarten",
        "Ready for Kindergarten",
        "TS GOLD",                           // Teaching Strategies GOLD — Physical subscale
        "Teaching Strategies GOLD",
        "Teaching Strategies Gold",

        // Other widely used kindergarten readiness instruments with physical domains
        "Brigance",                          // Brigance Early Childhood / Kindergarten Screens
        "KRA",                               // Kindergarten Readiness Assessment
        "WaKIDS",                            // Washington Kindergarten Inventory of Developing Skills
        "WSO",                               // Work Sampling Online
        "Work Sampling"
    ];

    public string DataElementName => "Reported kindergarten readiness (physical development)";

    public string AssessmentDescription =>
        "Identifies student assessments linked to teacher- or parent-observed developmental assessments " +
        "of kindergarten readiness in the physical development / health / motor-skills domain. Matches " +
        "instruments named in the E-W Framework such as the Desired Results Developmental Profile (DRDP) " +
        "Physical Development – Health domain, Ready 4 Kindergarten Early Learning Assessment (R4K ELA) " +
        "Physical Well-Being and Motor Development domain, and Teaching Strategies GOLD Physical subscale, " +
        "along with other common kindergarten entry assessments (KRA, WaKIDS, Brigance, Work Sampling). " +
        "Ed-Fi has no standard descriptor for kindergarten readiness assessments, so title-based matching " +
        "against the assessments catalog is used.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for kindergarten readiness (physical) assessments...");

        var matchingAssessments = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (IsKindergartenReadinessAssessment(assessment))
                    matchingAssessments.Add((assessment.AssessmentIdentifier, assessment.Namespace));
            },
            context);

        context.Log($"Found {matchingAssessments.Count} kindergarten readiness (physical) assessment(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no kindergarten readiness (physical) assessments found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized kindergarten readiness / physical development " +
                    "instruments were found in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Loading student assessment results for kindergarten readiness assessments...");

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
            $"Found {result.RecordsWithScores:N0} of {result.TotalRecords:N0} kindergarten readiness (physical) assessment results with score results");
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

    private static bool IsKindergartenReadinessAssessment(EdFiAssessment assessment)
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
