using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class DirectChildAssessmentsCognitionEdFiAssessor : IEdFiAssessor
{
    private static readonly string[] _assessmentKeywords =
    [
        "cognitive development",
        "cognitive assessment",
        "cognitive scale",
        "cognitive composite",
        "cognitive domain",
        "cognitive ability",

        "Bayley",
        "Bayley Scales",
        "BSID",
        "Mullen Scales",
        "Mullen",
        "WPPSI",
        "Wechsler Preschool"
    ];

    public string DataElementName => "Direct child assessments (cognition)";

    public string AssessmentDescription =>
        "Identifies student assessments linked to direct child assessments of cognition " +
        "(e.g., Bayley Scales of Infant Development (BSID), Mullen Scales of Early Learning, " +
        "WPPSI — Wechsler Preschool and Primary Scale of Intelligence) by matching well-known " +
        "instrument names and cognitive development keywords in the Ed-Fi assessments catalog.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for direct child cognition assessments...");

        var matchingAssessments = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (IsCognitionAssessment(assessment))
                    matchingAssessments.Add((assessment.AssessmentIdentifier, assessment.Namespace));
            },
            context);

        context.Log($"Found {matchingAssessments.Count} direct child cognition assessment(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no direct child cognition assessments found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized direct child cognition instruments " +
                    "were found in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Counting student assessment records for cognition instruments...");

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

        context.Log($"Found {totalCount:N0} direct child cognition assessment records");
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

    private static bool IsCognitionAssessment(EdFiAssessment assessment)
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
