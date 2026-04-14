using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class CivicEngagementSurveysK12EdFiAssessor : IEdFiAssessor
{
    // Keywords covering common K-12 civic engagement survey instruments
    // and generic terms used in assessment titles for these surveys.
    private static readonly string[] _surveyKeywords =
    [
        "civic engagement",
        "civic participation",
        "civic responsibility",
        "civic knowledge",
        "civic attitudes",
        "civic behavior",
        "civic identity",
        "civic character",
        "youth civic",
        "civic and character",
        "character measures",
        "civic indicators",
        "civic mindedness",
        "civic-mindedness",
        "community engagement",
        "YCCMT",   // Youth Civic and Character Measures Toolkit
        "YCEIP"    // Youth Civic Engagement Indicators Project
    ];

    public string DataElementName => "Civic engagement surveys (K-12)";

    public string AssessmentDescription =>
        "Identifies student assessments linked to K-12 civic engagement survey instruments " +
        "(e.g., Youth Civic and Character Measures Toolkit, Youth Civic Engagement Indicators Project) " +
        "by matching well-known instrument names and civic-engagement keywords in the Ed-Fi assessments " +
        "catalog. Ed-Fi has no standard descriptor for civic engagement surveys, so title-based matching " +
        "is used as a proxy.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for civic engagement surveys...");

        var matchingAssessments = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (IsCivicEngagementSurvey(assessment))
                    matchingAssessments.Add((assessment.AssessmentIdentifier, assessment.Namespace));
            },
            context);

        context.Log($"Found {matchingAssessments.Count} civic engagement survey(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no civic engagement surveys found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized civic engagement survey instruments were found " +
                    "in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Loading student assessment results for civic engagement surveys...");

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
            $"Found {result.RecordsWithScores:N0} of {result.TotalRecords:N0} civic engagement survey results with score results");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(result.TotalRecords),
                new Distribution(surveyDistribution, "Survey Instrument"),
                new Distribution(result.GradeLevelDistribution, "Grade Level Assessed"),
                new Distribution(result.PerformanceLevelDistribution, "Performance Level / Engagement Level"),
                new Completeness(result.TotalRecords, result.RecordsWithScores, "ScoreResults")
            ],
            Remarks = AssessmentDescription
        };
    }

    private static bool IsCivicEngagementSurvey(EdFiAssessment assessment)
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
