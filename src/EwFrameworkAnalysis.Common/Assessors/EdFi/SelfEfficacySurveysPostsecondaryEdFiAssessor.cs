using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class SelfEfficacySurveysPostsecondaryEdFiAssessor : IEdFiAssessor
{
    // Keywords for postsecondary / adult self-efficacy survey instruments. Framework names
    // the New General Self-Efficacy Scale and the Ascend Survey (Self-Efficacy Scale);
    // widely used adult self-efficacy instruments are also matched.
    private static readonly string[] _surveyKeywords =
    [
        // Generic terms
        "self-efficacy",
        "self efficacy",
        "academic self-efficacy",
        "academic efficacy",
        "self-confidence",

        // Instruments named in the E-W Framework
        "New General Self-Efficacy",         // New General Self-Efficacy Scale (NGSE)
        "NGSE",
        "Ascend Survey",                     // Ascend Survey (Self-Efficacy Scale)

        // Other widely used postsecondary / adult self-efficacy instruments
        "General Self-Efficacy Scale",       // Schwarzer & Jerusalem GSE
        "GSE",
        "College Self-Efficacy",
        "CSEI",                              // College Self-Efficacy Inventory
        "Career Decision Self-Efficacy",
        "CDSE",
        "MSLQ",                              // Motivated Strategies for Learning Questionnaire
        "Motivated Strategies for Learning",
        "SSIPP",                             // Student Success / Persistence surveys
        "Panorama"
    ];

    public string DataElementName => "Self-efficacy surveys (Postsecondary)";

    public string AssessmentDescription =>
        "Identifies student / individual assessments linked to postsecondary / adult self-efficacy survey " +
        "instruments (e.g., New General Self-Efficacy Scale and Ascend Survey Self-Efficacy Scale named in " +
        "the E-W Framework, plus Schwarzer & Jerusalem General Self-Efficacy Scale, College Self-Efficacy " +
        "Inventory, Career Decision Self-Efficacy Scale, MSLQ) by matching well-known instrument names and " +
        "self-efficacy keywords in the Ed-Fi assessments catalog. Ed-Fi has no standard descriptor for " +
        "self-efficacy surveys, so title-based matching is used as a proxy.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for postsecondary self-efficacy surveys...");

        var matchingAssessments = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (IsSelfEfficacySurvey(assessment))
                    matchingAssessments.Add((assessment.AssessmentIdentifier, assessment.Namespace));
            },
            context);

        context.Log($"Found {matchingAssessments.Count} postsecondary self-efficacy survey(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no postsecondary self-efficacy surveys found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized postsecondary / adult self-efficacy survey instruments " +
                    "were found in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Loading student assessment results for postsecondary self-efficacy surveys...");

        var studentAssessments = new List<EdFiStudentAssessment>();
        var surveyDistribution = new Dictionary<string, int>();

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
                surveyDistribution[identifier] = added;
        }

        var result = StudentAssessmentAnalyzer.Analyze(studentAssessments);

        context.Log(
            $"Found {result.RecordsWithScores:N0} of {result.TotalRecords:N0} postsecondary self-efficacy survey results with score results");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(result.TotalRecords),
                new Distribution(surveyDistribution, "Survey Instrument"),
                new Distribution(result.GradeLevelDistribution, "Grade Level Assessed"),
                new Distribution(result.PerformanceLevelDistribution, "Performance Level / Self-Efficacy Level"),
                new Completeness(result.TotalRecords, result.RecordsWithScores, "ScoreResults")
            ],
            Remarks = AssessmentDescription
        };
    }

    private static bool IsSelfEfficacySurvey(EdFiAssessment assessment)
    {
        var title = assessment.AssessmentTitle ?? string.Empty;
        var identifier = assessment.AssessmentIdentifier ?? string.Empty;

        foreach (var keyword in _surveyKeywords)
        {
            if (title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                identifier.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
