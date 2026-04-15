using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class SocialProficiencyAssessmentsPostsecondaryEdFiAssessor : IEdFiAssessor
{
    // Keywords for postsecondary / workforce social proficiency performance assessments.
    // Framework names The National Work Readiness Credential Essential Soft Skills
    // assessment; widely used adult soft-skills / workplace readiness instruments are
    // also matched.
    private static readonly string[] _assessmentKeywords =
    [
        // Generic terms
        "social proficiency",
        "soft skills",
        "interpersonal skills",
        "workplace readiness",
        "work readiness",
        "employability skills",
        "professional skills",

        // Instrument named in the E-W Framework
        "National Work Readiness Credential",
        "NWRC",
        "Essential Soft Skills",

        // Other widely used postsecondary / workforce soft-skills / social-proficiency assessments
        "WorkKeys",                          // ACT WorkKeys (Workplace Documents, Applied Math, Graphic Literacy, + Talent/Fit soft-skill profile)
        "Workplace Essential Skills",
        "CASAS",                             // Comprehensive Adult Student Assessment Systems (workforce readiness)
        "TABE",                              // Tests of Adult Basic Education (WorkForce)
        "TABE WorkForce",
        "CSEI",                              // College Self-Efficacy Inventory (social subscale)
        "SSIPP",                             // Student Success / Persistence surveys with social items
        "Panorama",
        "Career Readiness",
        "Employability",
        "EQ-i",                              // Emotional Quotient Inventory (social awareness / interpersonal)
        "Emotional Quotient",
        "MSCEIT"                             // Mayer-Salovey-Caruso Emotional Intelligence Test
    ];

    public string DataElementName => "Social proficiency performance assessments (Postsecondary)";

    public string AssessmentDescription =>
        "Identifies student / individual assessments linked to postsecondary / workforce social proficiency " +
        "performance assessments (e.g., The National Work Readiness Credential Essential Soft Skills " +
        "assessment named in the E-W Framework, plus ACT WorkKeys, CASAS workforce readiness, TABE " +
        "WorkForce, EQ-i, MSCEIT, and other soft-skills / employability assessments) by matching well-known " +
        "instrument names and social-proficiency keywords in the Ed-Fi assessments catalog. Ed-Fi has no " +
        "standard descriptor for social proficiency performance assessments, so title-based matching is used " +
        "as a proxy.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for postsecondary social proficiency assessments...");

        var matchingAssessments = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (IsSocialProficiencyAssessment(assessment))
                    matchingAssessments.Add((assessment.AssessmentIdentifier, assessment.Namespace));
            },
            context);

        context.Log($"Found {matchingAssessments.Count} postsecondary social proficiency assessment(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no postsecondary social proficiency assessments found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized postsecondary / workforce social proficiency performance " +
                    "assessments were found in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Loading student assessment results for postsecondary social proficiency assessments...");

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
            $"Found {result.RecordsWithScores:N0} of {result.TotalRecords:N0} postsecondary social proficiency results with score results");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(result.TotalRecords),
                new Distribution(instrumentDistribution, "Assessment Instrument"),
                new Distribution(result.GradeLevelDistribution, "Grade Level Assessed"),
                new Distribution(result.PerformanceLevelDistribution, "Performance Level / Social Proficiency Level"),
                new Completeness(result.TotalRecords, result.RecordsWithScores, "ScoreResults")
            ],
            Remarks = AssessmentDescription
        };
    }

    private static bool IsSocialProficiencyAssessment(EdFiAssessment assessment)
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
