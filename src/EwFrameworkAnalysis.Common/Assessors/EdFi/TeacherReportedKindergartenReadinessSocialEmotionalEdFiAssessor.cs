using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class TeacherReportedKindergartenReadinessSocialEmotionalEdFiAssessor : IEdFiAssessor
{
    private static readonly string[] _assessmentCategoryDescriptors =
    [
        "Developmental observation",
        "Early Learning - Social and emotional development",
        "Prekindergarten Readiness",
        "Kindergarten Readiness"
    ];

    private static readonly string[] _assessmentKeywords =
    [
        // Generic terms
        "kindergarten readiness",
        "kindergarten ready",
        "K readiness",
        "social-emotional",
        "social emotional",
        "social development",
        "emotional development",
        "social foundations",
        "developmental assessment",
        "developmental observation",

        // Instruments named in the E-W Framework
        "DRDP",                              // Desired Results Developmental Profile
        "Desired Results Developmental",
        "Desired Results Developmental Profile",
        "R4K",                               // Ready 4 Kindergarten Early Learning Assessment
        "R4K ELA",
        "Ready 4 Kindergarten",
        "TS GOLD",                           // Teaching Strategies GOLD
        "Teaching Strategies GOLD",
        "Teaching Strategies",
        "GOLD",

        // Other widely used Pre-K/K social-emotional readiness instruments
        "KRA",                               // Kindergarten Readiness Assessment
        "Kindergarten Readiness Assessment",
        "Work Sampling",                     // Work Sampling System
        "WSS",
        "COR Advantage",                     // HighScope COR Advantage
        "COR",
        "HighScope",
        "Brigance",                          // Brigance Early Childhood Screens
        "Early Learning Scale"
    ];

    public string DataElementName => "Teacher-reported kindergarten readiness (social-emotional skills)";

    public string AssessmentDescription =>
        "Identifies student assessments linked to teacher- or parent-observed developmental assessments " +
        "of kindergarten readiness in social-emotional skills (e.g., Desired Results Developmental " +
        "Profile (DRDP) Social and Emotional Development domain, Ready 4 Kindergarten (R4K) ELA " +
        "Social Foundations domain, and Teaching Strategies (TS) GOLD Social-Emotional subscale named " +
        "in the E-W Framework, plus KRA, Work Sampling System, HighScope COR Advantage, Brigance) by " +
        "matching assessmentCategoryDescriptor values, well-known instrument names, and kindergarten " +
        "readiness keywords in the Ed-Fi assessments catalog. Ed-Fi has no standard descriptor for " +
        "kindergarten readiness social-emotional assessments, so title-based and category-based " +
        "matching is used as a proxy.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for kindergarten readiness social-emotional assessments...");

        var matchingAssessments = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (IsKindergartenReadinessSocialEmotionalAssessment(assessment))
                    matchingAssessments.Add((assessment.AssessmentIdentifier, assessment.Namespace));
            },
            context);

        context.Log($"Found {matchingAssessments.Count} kindergarten readiness social-emotional assessment(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no kindergarten readiness social-emotional assessments found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized kindergarten readiness social-emotional instruments " +
                    "were found in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Counting student assessment records for kindergarten readiness social-emotional instruments...");

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

        context.Log($"Found {totalCount:N0} kindergarten readiness social-emotional assessment records");
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

    private static bool IsKindergartenReadinessSocialEmotionalAssessment(EdFiAssessment assessment)
    {
        if (!string.IsNullOrWhiteSpace(assessment.AssessmentCategoryDescriptor))
        {
            var category = EdFiDescriptorHelper.ParseDescriptorValue(assessment.AssessmentCategoryDescriptor);
            foreach (var descriptor in _assessmentCategoryDescriptors)
            {
                if (category.Equals(descriptor, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
        }

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
