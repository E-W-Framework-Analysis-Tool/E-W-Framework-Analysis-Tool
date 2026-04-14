using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class PhysicalHealthSurveysPostsecondaryEdFiAssessor : IEdFiAssessor
{
    // Keywords for postsecondary / adult self-rated physical health survey instruments.
    private static readonly string[] _surveyKeywords =
    [
        // Generic terms
        "physical health",
        "physical well-being",
        "physical wellbeing",
        "self-rated health",
        "self rated health",
        "adult health survey",

        // Instruments named in the E-W Framework
        "Self-Rated Health Scale",
        "Health-Related Quality of Life",
        "health related quality of life",
        "HRQoL",
        "HRQOL",

        // Other widely used adult / postsecondary physical health surveys
        "BRFSS",                             // Behavioral Risk Factor Surveillance System
        "Behavioral Risk Factor",
        "NHIS",                              // National Health Interview Survey
        "SF-36",                             // Short Form Health Survey
        "SF-12",
        "PROMIS",                            // Patient-Reported Outcomes Measurement Information System
        "EQ-5D",                             // EuroQol 5-Dimension
        "EuroQol",
        "CDC HRQOL",
        "Healthy Days",
        "NCHA",                              // National College Health Assessment
        "National College Health Assessment"
    ];

    public string DataElementName => "Physical health surveys (Postsecondary)";

    public string AssessmentDescription =>
        "Identifies student / individual assessments linked to postsecondary / adult self-rated " +
        "physical health surveys (e.g., Self-Rated Health Scale and Health-Related Quality of Life " +
        "Scale named in the E-W Framework, plus BRFSS, NHIS, SF-36 / SF-12, PROMIS, EQ-5D, CDC " +
        "Healthy Days, and the National College Health Assessment) by matching well-known " +
        "instrument names and physical-health keywords in the Ed-Fi assessments catalog. Ed-Fi has " +
        "no standard descriptor for physical health surveys, so title-based matching is used as a proxy.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for postsecondary physical health surveys...");

        var matchingAssessments = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (IsPhysicalHealthSurvey(assessment))
                    matchingAssessments.Add((assessment.AssessmentIdentifier, assessment.Namespace));
            },
            context);

        context.Log($"Found {matchingAssessments.Count} postsecondary physical health survey(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no postsecondary physical health surveys found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized postsecondary / adult physical health survey " +
                    "instruments were found in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Loading student assessment results for postsecondary physical health surveys...");

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
            $"Found {result.RecordsWithScores:N0} of {result.TotalRecords:N0} postsecondary physical health survey results with score results");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(result.TotalRecords),
                new Distribution(surveyDistribution, "Survey Instrument"),
                new Distribution(result.GradeLevelDistribution, "Grade Level Assessed"),
                new Distribution(result.PerformanceLevelDistribution, "Performance Level / Health Rating"),
                new Completeness(result.TotalRecords, result.RecordsWithScores, "ScoreResults")
            ],
            Remarks = AssessmentDescription
        };
    }

    private static bool IsPhysicalHealthSurvey(EdFiAssessment assessment)
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
