using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class CulturalCompetencyAssessmentsK12EdFiAssessor : IEdFiAssessor
{
    // The E-W Framework notes that no K-12-specific tool exists; adapted postsecondary
    // instruments (HEIghten, IDI) may be used as proxies. Keywords cover those instruments
    // and generic cultural competency / intercultural terms that might appear on titles.
    private static readonly string[] _assessmentKeywords =
    [
        "cultural competency",
        "cultural competence",
        "intercultural competency",
        "intercultural competence",
        "cultural awareness",
        "cultural responsiveness",
        "cultural sensitivity",
        "diversity assessment",
        "intercultural development",
        "HEIghten",                       // HEIghten Outcomes Assessment for Intercultural Competency & Diversity
        "Intercultural Development Inventory",
        "IDI"                             // Intercultural Development Inventory
    ];

    public string DataElementName => "Cultural competency assessments (K-12)";

    public string AssessmentDescription =>
        "Identifies student assessments linked to cultural / intercultural competency instruments. " +
        "The E-W Framework notes no K-12-specific tool exists, so adapted adult / postsecondary " +
        "instruments (HEIghten Outcomes Assessment for Intercultural Competency & Diversity, " +
        "The Intercultural Development Inventory) and generic cultural-competency keywords are " +
        "matched against the Ed-Fi assessments catalog as a proxy. Ed-Fi has no standard descriptor " +
        "for cultural competency assessments.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for cultural competency assessments...");

        var matchingAssessments = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (IsCulturalCompetencyAssessment(assessment))
                    matchingAssessments.Add((assessment.AssessmentIdentifier, assessment.Namespace));
            },
            context);

        context.Log($"Found {matchingAssessments.Count} cultural competency assessment(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no cultural competency assessments found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized cultural / intercultural competency instruments " +
                    "were found in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Loading student assessment results for cultural competency assessments...");

        var totalRecords = 0;
        var instrumentDistribution = new Dictionary<string, int>();

        foreach (var (identifier, ns) in matchingAssessments)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["assessmentIdentifier"] = identifier,
                ["namespace"] = ns
            };

            var countBefore = totalRecords;

            await EdFiApiPatterns.PageAndProcessAsync<EdFiStudentAssessment>(
                httpClient,
                "ed-fi/studentAssessments",
                _ => totalRecords++,
                context,
                queryParams);

            var added = totalRecords - countBefore;
            if (added > 0)
                instrumentDistribution[identifier] = added;
        }

        context.Log($"Found {totalRecords:N0} cultural competency student assessment records");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Distribution(instrumentDistribution, "Assessment Instrument")
            ],
            Remarks = AssessmentDescription
        };
    }

    private static bool IsCulturalCompetencyAssessment(EdFiAssessment assessment)
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
