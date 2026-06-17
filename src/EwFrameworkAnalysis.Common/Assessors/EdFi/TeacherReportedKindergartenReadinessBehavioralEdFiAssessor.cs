using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class TeacherReportedKindergartenReadinessBehavioralEdFiAssessor : IEdFiAssessor
{
    private static readonly string[] _assessmentCategoryDescriptors =
    [
        "Developmental observation",
        "Early Learning - Approaches toward learning",
        "Prekindergarten Readiness",
        "Kindergarten Readiness"
    ];

    private static readonly string[] _assessmentKeywords =
    [
        // Generic terms
        "kindergarten readiness",
        "kindergarten ready",
        "K readiness",
        "approaches to learning",
        "self-regulation",
        "self regulation",
        "behavioral skills",
        "behavioral readiness",
        "developmental assessment",
        "developmental observation",

        // Instruments named in the E-W Framework
        "DRDP",                              // Desired Results Developmental Profile
        "Desired Results Developmental",
        "Desired Results Developmental Profile",
        "TS GOLD",                           // Teaching Strategies GOLD
        "Teaching Strategies GOLD",
        "Teaching Strategies",
        "GOLD",

        // Other widely used Pre-K/K behavioral readiness instruments
        "KREADY",
        "KRA",                               // Kindergarten Readiness Assessment
        "Kindergarten Readiness Assessment",
        "Work Sampling",                     // Work Sampling System
        "WSS",
        "COR Advantage",                     // HighScope COR Advantage
        "COR",
        "HighScope",
        "Brigance",                          // Brigance Early Childhood Screens
        "DIAL",                              // Developmental Indicators for the Assessment of Learning
        "Early Learning Scale"
    ];

    public string DataElementName => "Teacher-reported kindergarten readiness (behavioral skills)";

    public string AssessmentDescription =>
        "Identifies student assessments linked to teacher- or parent-observed developmental assessments " +
        "of kindergarten readiness in behavioral skills / approaches to learning (e.g., Desired Results " +
        "Developmental Profile (DRDP) Approaches to Learning – Self-Regulation domain and Teaching " +
        "Strategies (TS) GOLD Cognitive subscale named in the E-W Framework, plus KRA, Work Sampling " +
        "System, HighScope COR Advantage, Brigance) by matching assessmentCategoryDescriptor values, " +
        "well-known instrument names, and kindergarten readiness keywords in the Ed-Fi assessments " +
        "catalog. Ed-Fi has no standard descriptor for kindergarten readiness behavioral assessments, " +
        "so title-based and category-based matching is used as a proxy.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for kindergarten readiness behavioral assessments...");

        var matchingAssessments = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (IsKindergartenReadinessBehavioralAssessment(assessment))
                    matchingAssessments.Add((assessment.AssessmentIdentifier, assessment.Namespace));
            },
            context);

        context.Log($"Found {matchingAssessments.Count} kindergarten readiness behavioral assessment(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no kindergarten readiness behavioral assessments found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized kindergarten readiness behavioral instruments were found " +
                    "in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Counting student assessment records for kindergarten readiness behavioral instruments...");

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

        context.Log($"Found {totalCount:N0} kindergarten readiness behavioral assessment records");
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

    private static bool IsKindergartenReadinessBehavioralAssessment(EdFiAssessment assessment)
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
