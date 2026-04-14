using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class ReportedKindergartenReadinessSocialEmotionalEdFiAssessor : IEdFiAssessor
{
    // Keywords for teacher- / parent-observed kindergarten readiness assessments that report
    // on social-emotional development. Framework names DRDP Social and Emotional Development,
    // R4K ELA Social Foundations, and TS GOLD Social-Emotional subscale as example instruments.
    private static readonly string[] _assessmentKeywords =
    [
        // Generic terms
        "kindergarten readiness",
        "KEA",                               // Kindergarten Entry Assessment
        "social-emotional",
        "social emotional",
        "social and emotional",
        "social foundations",
        "relationships with adults",
        "relationships with peers",
        "sense of identity",
        "sense of belonging",

        // Instruments named in the E-W Framework
        "DRDP",                              // Desired Results Developmental Profile
        "Desired Results Developmental Profile",
        "R4K",                               // Ready 4 Kindergarten
        "Ready 4 Kindergarten",
        "Ready for Kindergarten",
        "TS GOLD",                           // Teaching Strategies GOLD
        "Teaching Strategies GOLD",

        // Other widely used teacher- / parent-observed readiness instruments with SEL domains
        "Brigance",
        "KRA",                               // Kindergarten Readiness Assessment (Maryland/Ohio)
        "WaKIDS",                            // Washington Kindergarten Inventory of Developing Skills
        "WSO",                               // Work Sampling Online
        "Work Sampling",
        "PELI",                              // Preschool Early Literacy Indicators (SEL in some suites)
        "HighScope COR",                     // Child Observation Record
        "COR Advantage"
    ];

    public string DataElementName => "Reported kindergarten readiness (social-emotional skills)";

    public string AssessmentDescription =>
        "Identifies student assessments linked to teacher- or parent-observed kindergarten readiness " +
        "assessments that report on social-emotional development (e.g., DRDP Social and Emotional " +
        "Development domain, R4K ELA Social Foundations domain, TS GOLD Social-Emotional subscale named " +
        "in the E-W Framework, plus Brigance, KRA, WaKIDS, Work Sampling, HighScope COR) by matching " +
        "well-known instrument names and SEL-readiness keywords in the Ed-Fi assessments catalog. Ed-Fi " +
        "has no standard descriptor for kindergarten readiness, so title-based matching is used as a proxy.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for kindergarten readiness (social-emotional) instruments...");

        var matchingAssessments = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (IsReadinessAssessment(assessment))
                    matchingAssessments.Add((assessment.AssessmentIdentifier, assessment.Namespace));
            },
            context);

        context.Log($"Found {matchingAssessments.Count} kindergarten readiness (social-emotional) assessment(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no kindergarten readiness (social-emotional) assessments found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized kindergarten readiness instruments with a social-emotional " +
                    "domain were found in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Loading student assessment results for kindergarten readiness (social-emotional) instruments...");

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
            $"Found {result.RecordsWithScores:N0} of {result.TotalRecords:N0} kindergarten readiness (social-emotional) results with score results");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(result.TotalRecords),
                new Distribution(instrumentDistribution, "Assessment Instrument"),
                new Distribution(result.GradeLevelDistribution, "Grade Level Assessed"),
                new Distribution(result.PerformanceLevelDistribution, "Performance Level / Readiness Level"),
                new Completeness(result.TotalRecords, result.RecordsWithScores, "ScoreResults")
            ],
            Remarks = AssessmentDescription
        };
    }

    private static bool IsReadinessAssessment(EdFiAssessment assessment)
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
