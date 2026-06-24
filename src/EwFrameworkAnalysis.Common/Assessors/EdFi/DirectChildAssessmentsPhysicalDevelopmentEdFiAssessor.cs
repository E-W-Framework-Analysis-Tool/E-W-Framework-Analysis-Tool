using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class DirectChildAssessmentsPhysicalDevelopmentEdFiAssessor : IEdFiAssessor
{
    private static readonly string[] _assessmentKeywords =
    [
        // Generic terms
        "physical development",
        "motor development",
        "gross motor",
        "fine motor",
        "motor skills",
        "perceptual motor",
        "physical well-being",
        "physical well being",

        // Instrument named in the E-W Framework
        "Peabody Developmental Motor",
        "PDMS",                              // Peabody Developmental Motor Scale
        "PDMS-2",
        "PDMS-3",

        // Other widely used direct child physical development instruments
        "BOT-2",                             // Bruininks-Oseretsky Test of Motor Proficiency
        "Bruininks-Oseretsky",
        "Bruininks Oseretsky",
        "MABC",                              // Movement Assessment Battery for Children
        "Movement ABC",
        "Movement Assessment Battery",
        "TGMD",                              // Test of Gross Motor Development
        "Test of Gross Motor",
        "AIM",                               // Alberta Infant Motor Scale
        "Alberta Infant Motor"
    ];

    public string DataElementName => "Direct child assessments (physical development)";

    public string AssessmentDescription =>
        "Identifies student assessments linked to direct child assessments of physical development / " +
        "motor skills (e.g., Peabody Developmental Motor Scale (PDMS) named in the E-W Framework, " +
        "plus Bruininks-Oseretsky Test (BOT-2), Movement Assessment Battery for Children (MABC), " +
        "Test of Gross Motor Development (TGMD)) by matching well-known instrument names and " +
        "physical development keywords in the Ed-Fi assessments catalog. Ed-Fi has no standard " +
        "descriptor for physical development assessments, so title-based matching is used as a proxy.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for direct child physical development assessments...");

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

        context.Log($"Found {matchingAssessments.Count} physical development assessment(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no direct child physical development assessments found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized direct child physical development instruments were found " +
                    "in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Counting student assessment records for physical development instruments...");

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

        context.Log($"Found {totalCount:N0} physical development assessment records");
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
