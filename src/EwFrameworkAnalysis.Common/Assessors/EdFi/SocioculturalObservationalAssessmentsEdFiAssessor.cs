using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class SocioculturalObservationalAssessmentsEdFiAssessor : IEdFiAssessor
{
    private static readonly string[] _assessmentKeywords =
    [
        "sociocultural",
        "cultural observation",
        "intercultural",
        "multicultural assessment",
        "cross-cultural",
        "culturally responsive",
        "cultural competence observation",
        "cultural assessment",

        "ECERS",
        "Early Childhood Environment Rating Scale"
    ];

    public string DataElementName => "Sociocultural observational assessments";

    public string AssessmentDescription =>
        "Identifies student assessments linked to sociocultural observational instruments " +
        "(e.g., ECERS — Early Childhood Environment Rating Scale, culturally responsive " +
        "observation tools) by matching sociocultural and cross-cultural keywords in the " +
        "Ed-Fi assessments catalog.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for sociocultural observational assessments...");

        var matchingAssessments = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (IsSocioculturalAssessment(assessment))
                    matchingAssessments.Add((assessment.AssessmentIdentifier, assessment.Namespace));
            },
            context);

        context.Log($"Found {matchingAssessments.Count} sociocultural observational assessment(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no sociocultural observational assessments found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized sociocultural observational instruments " +
                    "were found in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Counting student assessment records...");

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

        context.Log($"Found {totalCount:N0} sociocultural observational assessment records");
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

    private static bool IsSocioculturalAssessment(EdFiAssessment assessment)
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
