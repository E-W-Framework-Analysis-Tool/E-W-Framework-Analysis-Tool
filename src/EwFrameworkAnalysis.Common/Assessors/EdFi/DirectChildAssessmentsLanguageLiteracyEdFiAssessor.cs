using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class DirectChildAssessmentsLanguageLiteracyEdFiAssessor : IEdFiAssessor
{
    private static readonly string[] _assessmentKeywords =
    [
        "language development",
        "language assessment",
        "phonological awareness",
        "expressive vocabulary",
        "receptive vocabulary",

        "PPVT",
        "Peabody",
        "Peabody Picture Vocabulary",
        "TOPEL",
        "Test of Preschool Early Literacy",
        "PALS-PreK",
        "PALS PreK",
        "WJ Language",
        "Woodcock-Johnson Language",

        "ECAD",
        "Woodcock-Johnson IV Tests of ECAD",
        "Letter-Word",
        "Writing subtest",
        "IGDIs Early Literacy",
        "IGDI"
    ];

    public string DataElementName => "Direct child assessments (language and literacy)";

    public string AssessmentDescription =>
        "Identifies student assessments linked to direct child assessments of language and " +
        "literacy (e.g., PPVT — Peabody Picture Vocabulary Test, TOPEL — Test of Preschool " +
        "Early Literacy, PALS-PreK, Woodcock-Johnson Language) by matching well-known " +
        "instrument names and language development keywords in the Ed-Fi assessments catalog.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for direct child language/literacy assessments...");

        var matchingAssessments = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (IsLanguageLiteracyAssessment(assessment))
                    matchingAssessments.Add((assessment.AssessmentIdentifier, assessment.Namespace));
            },
            context);

        context.Log($"Found {matchingAssessments.Count} direct child language/literacy assessment(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no direct child language/literacy assessments found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized direct child language and literacy " +
                    "instruments were found in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Counting student assessment records for language/literacy instruments...");

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

        context.Log($"Found {totalCount:N0} direct child language/literacy assessment records");
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

    private static bool IsLanguageLiteracyAssessment(EdFiAssessment assessment)
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
