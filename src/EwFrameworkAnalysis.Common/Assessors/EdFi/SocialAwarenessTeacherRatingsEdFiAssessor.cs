using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class SocialAwarenessTeacherRatingsEdFiAssessor : IEdFiAssessor
{
    // Keywords for K-12 teacher-rated social awareness / social skills instruments.
    // Framework names the Social Skills Improvement System, Social-Emotional Learning
    // Edition (SSIS SEL); widely used teacher-rating SEL instruments are also matched.
    private static readonly string[] _assessmentKeywords =
    [
        // Generic terms
        "social awareness",
        "social skills",
        "perspective taking",
        "social perspective",
        "teacher rating",
        "teacher report",
        "teacher observation",

        // Instrument named in the E-W Framework
        "SSIS SEL",                          // Social Skills Improvement System, SEL Edition
        "SSIS",                              // Social Skills Improvement System
        "Social Skills Improvement",
        "SEHS-S",                            // Social Emotional Health Survey-Secondary (student self-report variant often referenced)
        "Social Emotional Health Survey",
        "Social Perspective Taking",

        // Other widely used teacher-rating SEL instruments with a social awareness domain
        "DESSA",                             // Devereux Student Strengths Assessment
        "SAEBRS",                            // Social, Academic, and Emotional Behavior Risk Screener
        "BASC",                              // Behavior Assessment System for Children
        "BESS",                              // Behavioral and Emotional Screening System
        "Panorama",
        "CORE SEL",
        "CORE Districts",
        "Second Step",
        "Work Sampling"
    ];

    public string DataElementName => "Social awareness teacher ratings";

    public string AssessmentDescription =>
        "Identifies student assessments linked to teacher ratings of social awareness / social skills (e.g., " +
        "Social Skills Improvement System, Social-Emotional Learning Edition (SSIS SEL) named in the E-W " +
        "Framework, plus SEHS-S, Social Perspective Taking Scale, DESSA, SAEBRS, BASC/BESS, Panorama, CORE " +
        "SEL, Second Step, Work Sampling) by matching well-known instrument names and social-awareness " +
        "keywords in the Ed-Fi assessments catalog. Ed-Fi has no standard descriptor for social awareness " +
        "teacher ratings, so title-based matching is used as a proxy.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for social awareness teacher rating instruments...");

        var matchingAssessments = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (IsSocialAwarenessAssessment(assessment))
                    matchingAssessments.Add((assessment.AssessmentIdentifier, assessment.Namespace));
            },
            context);

        context.Log($"Found {matchingAssessments.Count} social awareness teacher rating assessment(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no social awareness teacher rating assessments found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized social awareness teacher rating instruments were found " +
                    "in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Loading student assessment results for social awareness teacher rating instruments...");

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
            $"Found {result.RecordsWithScores:N0} of {result.TotalRecords:N0} social awareness teacher rating results with score results");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(result.TotalRecords),
                new Distribution(instrumentDistribution, "Assessment Instrument"),
                new Distribution(result.GradeLevelDistribution, "Grade Level Assessed"),
                new Distribution(result.PerformanceLevelDistribution, "Performance Level / Social Awareness Level"),
                new Completeness(result.TotalRecords, result.RecordsWithScores, "ScoreResults")
            ],
            Remarks = AssessmentDescription
        };
    }

    private static bool IsSocialAwarenessAssessment(EdFiAssessment assessment)
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
