using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class GrowthMindsetSurveysPostsecondaryEdFiAssessor : IEdFiAssessor
{
    // Keywords for postsecondary growth mindset survey instruments and common
    // student-success / engagement surveys that include a growth mindset scale.
    private static readonly string[] _surveyKeywords =
    [
        // Generic terms
        "growth mindset",
        "fixed mindset",
        "mindset scale",
        "mindset survey",
        "implicit theories of intelligence",
        "theories of intelligence",

        // Named instruments / survey suites that include a growth mindset scale
        "Dweck Mindset",
        "PERTS",                             // Project for Education Research That Scales
        "Mindset Meter",
        "College Transition Collaborative",
        "CTC Mindset",
        "SSIPP",                             // Student Success / Persistence surveys
        "Panorama"
    ];

    public string DataElementName => "Growth mindset surveys (Postsecondary)";

    public string AssessmentDescription =>
        "Identifies student assessments linked to postsecondary growth mindset survey instruments " +
        "(e.g., Dweck Mindset Scale / Implicit Theories of Intelligence Scale, PERTS Mindset Meter, " +
        "College Transition Collaborative mindset surveys) by matching well-known instrument names " +
        "and growth-mindset keywords in the Ed-Fi assessments catalog. Ed-Fi has no standard descriptor " +
        "for growth mindset surveys, so title-based matching is used as a proxy.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for postsecondary growth mindset surveys...");

        var matchingAssessments = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (IsGrowthMindsetSurvey(assessment))
                    matchingAssessments.Add((assessment.AssessmentIdentifier, assessment.Namespace));
            },
            context);

        context.Log($"Found {matchingAssessments.Count} postsecondary growth mindset survey(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no postsecondary growth mindset surveys found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized postsecondary growth mindset survey instruments " +
                    "were found in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Loading student assessment results for postsecondary growth mindset surveys...");

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
            $"Found {result.RecordsWithScores:N0} of {result.TotalRecords:N0} postsecondary growth mindset survey results with score results");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(result.TotalRecords),
                new Distribution(surveyDistribution, "Survey Instrument"),
                new Distribution(result.GradeLevelDistribution, "Grade Level Assessed"),
                new Distribution(result.PerformanceLevelDistribution, "Performance Level / Mindset Level"),
                new Completeness(result.TotalRecords, result.RecordsWithScores, "ScoreResults")
            ],
            Remarks = AssessmentDescription
        };
    }

    private static bool IsGrowthMindsetSurvey(EdFiAssessment assessment)
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
