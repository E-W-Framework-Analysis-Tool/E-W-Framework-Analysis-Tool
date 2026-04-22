using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class ReportedKindergartenReadinessBehavioralEdFiAssessor : IEdFiAssessor
{
    // Keywords for teacher- or parent-observed developmental assessments measuring
    // kindergarten readiness in the behavioral / approaches-to-learning / self-regulation domain.
    private static readonly string[] _assessmentKeywords =
    [
        // Generic terms
        "kindergarten readiness",
        "kindergarten entry",
        "KEA",                               // Kindergarten Entry Assessment
        "approaches to learning",
        "self-regulation",
        "self regulation",
        "initiative and curiosity",
        "executive function",

        // Instruments named in the E-W Framework
        "DRDP",                              // Desired Results Developmental Profile
        "Desired Results Developmental Profile",
        "TS GOLD",                           // Teaching Strategies GOLD
        "Teaching Strategies GOLD",
        "Teaching Strategies Gold",

        // Other widely used kindergarten readiness / ATL instruments
        "PELI",                              // Preschool Early Literacy Indicators
        "Brigance",                          // Brigance Early Childhood / Kindergarten Screens
        "KRA",                               // Kindergarten Readiness Assessment
        "WaKIDS",                            // Washington Kindergarten Inventory of Developing Skills
        "PRA",                               // Pennsylvania Kindergarten Entry Inventory / similar
        "WSO",                               // Work Sampling Online
        "Work Sampling"
    ];

    public string DataElementName => "Reported kindergarten readiness (behavioral skills)";

    public string AssessmentDescription =>
        "Identifies student assessments linked to teacher- or parent-observed developmental assessments " +
        "of kindergarten readiness in the behavioral skills / approaches-to-learning / self-regulation " +
        "domain. Matches instruments named in the E-W Framework such as the Desired Results Developmental " +
        "Profile (DRDP) Approaches to Learning – Self-Regulation domain and Teaching Strategies GOLD " +
        "Cognitive subscale, along with other common kindergarten entry assessments (KRA, WaKIDS, Brigance, " +
        "Work Sampling). NOTE: Early learning / Pre-K assessment data is rarely available in Ed-Fi ODS " +
        "instances — most districts do not load kindergarten readiness instruments into Ed-Fi, so this " +
        "data element is unlikely to return results.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for kindergarten readiness assessments...");

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

        context.Log($"Found {matchingAssessments.Count} kindergarten readiness assessment(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no kindergarten readiness assessments found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized kindergarten readiness / approaches-to-learning " +
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
            $"Found {result.RecordsWithScores:N0} of {result.TotalRecords:N0} kindergarten readiness assessment results with score results");
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
