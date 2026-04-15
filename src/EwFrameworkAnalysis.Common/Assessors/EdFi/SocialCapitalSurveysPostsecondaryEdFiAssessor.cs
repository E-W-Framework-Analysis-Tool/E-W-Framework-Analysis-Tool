using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class SocialCapitalSurveysPostsecondaryEdFiAssessor : IEdFiAssessor
{
    // Keywords for postsecondary / adult social capital survey instruments. Framework
    // names the Social Capital Assessment + Learning for Equity (SCALE) Social Capital,
    // Network Diversity, and Network Strength scales; widely used postsecondary /
    // workforce network / mentoring / belonging instruments are also matched.
    private static readonly string[] _surveyKeywords =
    [
        // Generic terms
        "social capital",
        "network diversity",
        "network strength",
        "relationship network",
        "professional network",
        "career network",
        "mentorship",
        "mentoring",
        "sense of belonging",
        "belonging",
        "campus connectedness",
        "supportive relationships",

        // Instrument named in the E-W Framework
        "SCALE",                             // Social Capital Assessment + Learning for Equity
        "Social Capital Assessment",
        "Learning for Equity",

        // Other widely used postsecondary / adult surveys with a social capital / network scale
        "Gallup-Purdue Index",
        "NSSE",                              // National Survey of Student Engagement
        "CCSSE",                             // Community College Survey of Student Engagement
        "BCSSE",                             // Beginning College Survey of Student Engagement
        "Strada",
        "Hello Insight",
        "Search Institute",
        "Developmental Relationships",
        "Resource Generator"
    ];

    public string DataElementName => "Social capital surveys (Postsecondary)";

    public string AssessmentDescription =>
        "Identifies student / individual assessments linked to postsecondary / adult social capital survey " +
        "instruments (e.g., Social Capital Assessment + Learning for Equity (SCALE) Social Capital, Network " +
        "Diversity, and Network Strength scales named in the E-W Framework, plus Gallup-Purdue Index, NSSE / " +
        "CCSSE / BCSSE engagement surveys, Strada, Hello Insight, Search Institute Developmental " +
        "Relationships, Resource Generator) by matching well-known instrument names and social-capital " +
        "keywords in the Ed-Fi assessments catalog. Ed-Fi has no standard descriptor for social capital " +
        "surveys, so title-based matching is used as a proxy.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for postsecondary social capital surveys...");

        var matchingAssessments = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (IsSocialCapitalSurvey(assessment))
                    matchingAssessments.Add((assessment.AssessmentIdentifier, assessment.Namespace));
            },
            context);

        context.Log($"Found {matchingAssessments.Count} postsecondary social capital survey(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no postsecondary social capital surveys found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized postsecondary / adult social capital survey instruments " +
                    "were found in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Loading student assessment results for postsecondary social capital surveys...");

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
            $"Found {result.RecordsWithScores:N0} of {result.TotalRecords:N0} postsecondary social capital survey results with score results");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(result.TotalRecords),
                new Distribution(surveyDistribution, "Survey Instrument"),
                new Distribution(result.GradeLevelDistribution, "Grade Level Assessed"),
                new Distribution(result.PerformanceLevelDistribution, "Performance Level / Social Capital Level"),
                new Completeness(result.TotalRecords, result.RecordsWithScores, "ScoreResults")
            ],
            Remarks = AssessmentDescription
        };
    }

    private static bool IsSocialCapitalSurvey(EdFiAssessment assessment)
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
