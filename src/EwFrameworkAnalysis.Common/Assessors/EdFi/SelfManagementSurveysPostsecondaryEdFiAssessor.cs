using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class SelfManagementSurveysPostsecondaryEdFiAssessor : IEdFiAssessor
{
    // Keywords for postsecondary / adult self-management survey instruments. Framework
    // names the Shift and Persist scale for teens and adults; widely used adult
    // self-regulation / emotion-regulation instruments are also matched.
    private static readonly string[] _surveyKeywords =
    [
        // Generic terms
        "self-management",
        "self management",
        "self-regulation",
        "self regulation",
        "emotion regulation",
        "behavior regulation",
        "impulse control",
        "executive function",

        // Instrument named in the E-W Framework
        "Shift and Persist",

        // Other widely used postsecondary / adult self-management instruments
        "ERQ",                               // Emotion Regulation Questionnaire (Gross & John)
        "DERS",                              // Difficulties in Emotion Regulation Scale
        "SSRQ",                              // Short Self-Regulation Questionnaire
        "Short Self-Regulation",
        "SRQ",                               // Self-Regulation Questionnaire
        "MSLQ",                              // Motivated Strategies for Learning Questionnaire (self-regulation subscale)
        "Motivated Strategies for Learning",
        "BRIEF-A",                           // Behavior Rating Inventory of Executive Function - Adult
        "Grit Scale",                        // Duckworth Grit Scale (persistence)
        "Panorama"
    ];

    public string DataElementName => "Self-management surveys (Postsecondary)";

    public string AssessmentDescription =>
        "Identifies student / individual assessments linked to postsecondary / adult self-management survey " +
        "instruments (e.g., Shift and Persist scale for teens and adults named in the E-W Framework, plus " +
        "Emotion Regulation Questionnaire (ERQ), Difficulties in Emotion Regulation Scale (DERS), Short " +
        "Self-Regulation Questionnaire, MSLQ self-regulation subscale, BRIEF-A, Grit Scale) by matching " +
        "well-known instrument names and self-management keywords in the Ed-Fi assessments catalog. Ed-Fi has " +
        "no standard descriptor for self-management surveys, so title-based matching is used as a proxy.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for postsecondary self-management surveys...");

        var matchingAssessments = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (IsSelfManagementSurvey(assessment))
                    matchingAssessments.Add((assessment.AssessmentIdentifier, assessment.Namespace));
            },
            context);

        context.Log($"Found {matchingAssessments.Count} postsecondary self-management survey(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no postsecondary self-management surveys found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized postsecondary / adult self-management survey instruments " +
                    "were found in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Loading student assessment results for postsecondary self-management surveys...");

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
            $"Found {result.RecordsWithScores:N0} of {result.TotalRecords:N0} postsecondary self-management survey results with score results");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(result.TotalRecords),
                new Distribution(surveyDistribution, "Survey Instrument"),
                new Distribution(result.GradeLevelDistribution, "Grade Level Assessed"),
                new Distribution(result.PerformanceLevelDistribution, "Performance Level / Self-Management Level"),
                new Completeness(result.TotalRecords, result.RecordsWithScores, "ScoreResults")
            ],
            Remarks = AssessmentDescription
        };
    }

    private static bool IsSelfManagementSurvey(EdFiAssessment assessment)
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
