using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class HealthRelatedQualityOfLifeEdFiAssessor : IEdFiAssessor
{
    // Keywords for Health-Related Quality of Life (HRQoL) instruments commonly used
    // with postsecondary / adult populations.
    private static readonly string[] _assessmentKeywords =
    [
        // Generic terms
        "health-related quality of life",
        "health related quality of life",
        "HRQoL",
        "HRQOL",
        "quality of life scale",
        "self-rated health",
        "self rated health",

        // Named HRQoL instruments
        "SF-36",                             // 36-Item Short Form Health Survey
        "SF-12",                             // 12-Item Short Form Health Survey
        "SF-8",
        "PROMIS",                            // Patient-Reported Outcomes Measurement Information System
        "EQ-5D",                             // EuroQol 5-Dimension
        "EuroQol",
        "CDC HRQOL",                         // CDC Healthy Days Core Module
        "Healthy Days",
        "PedsQL"                             // Pediatric Quality of Life Inventory
    ];

    public string DataElementName => "Health-Related Quality of Life Scale scores";

    public string AssessmentDescription =>
        "Identifies student / individual assessments linked to Health-Related Quality of Life (HRQoL) " +
        "instruments named in the E-W Framework and widely used in postsecondary / adult populations " +
        "(e.g., SF-36 / SF-12 Short Form Health Survey, PROMIS, EQ-5D, CDC Healthy Days, PedsQL, " +
        "Self-Rated Health Scale). Ed-Fi has no standard descriptor for HRQoL assessments, so " +
        "title-based matching against the assessments catalog is used as a proxy.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for Health-Related Quality of Life instruments...");

        var matchingAssessments = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (IsHrqolAssessment(assessment))
                    matchingAssessments.Add((assessment.AssessmentIdentifier, assessment.Namespace));
            },
            context);

        context.Log($"Found {matchingAssessments.Count} HRQoL assessment(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no HRQoL assessments found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized Health-Related Quality of Life instruments " +
                    "were found in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Loading student assessment results for HRQoL instruments...");

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
            $"Found {result.RecordsWithScores:N0} of {result.TotalRecords:N0} HRQoL assessment results with score results");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(result.TotalRecords),
                new Distribution(instrumentDistribution, "Assessment Instrument"),
                new Distribution(result.GradeLevelDistribution, "Grade Level Assessed"),
                new Distribution(result.PerformanceLevelDistribution, "Performance Level / Score"),
                new Completeness(result.TotalRecords, result.RecordsWithScores, "ScoreResults")
            ],
            Remarks = AssessmentDescription
        };
    }

    private static bool IsHrqolAssessment(EdFiAssessment assessment)
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
