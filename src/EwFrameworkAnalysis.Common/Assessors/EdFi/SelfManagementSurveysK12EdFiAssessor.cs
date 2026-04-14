using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class SelfManagementSurveysK12EdFiAssessor : IEdFiAssessor
{
    // Keywords for K-12 self-management survey instruments. Framework names the Shift
    // and Persist scale for children; widely used K-12 SEL survey suites that include a
    // self-management / self-regulation scale are also matched.
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

        // Other widely used K-12 survey suites with a self-management scale
        "Panorama",                          // Panorama SEL / Student Survey
        "CORE Districts",
        "CORE SEL",
        "Tripod",                            // Tripod Student Survey
        "PERTS",                             // Project for Education Research That Scales
        "DESSA",                             // Devereux Student Strengths Assessment
        "SAEBRS",                            // Social, Academic, and Emotional Behavior Risk Screener
        "SSIS",                              // Social Skills Improvement System
        "BRIEF",                             // Behavior Rating Inventory of Executive Function
        "ERICA",                             // Emotion Regulation Index for Children and Adolescents
        "ERQ"                                // Emotion Regulation Questionnaire
    ];

    public string DataElementName => "Self-management surveys (K-12)";

    public string AssessmentDescription =>
        "Identifies student assessments linked to K-12 self-management survey instruments (e.g., Shift and " +
        "Persist scale for children named in the E-W Framework, plus Panorama / CORE Districts SEL surveys, " +
        "Tripod, PERTS, DESSA, SAEBRS, SSIS, BRIEF, ERQ that include a self-management / self-regulation " +
        "scale) by matching well-known instrument names and self-management keywords in the Ed-Fi assessments " +
        "catalog. Ed-Fi has no standard descriptor for self-management surveys, so title-based matching is " +
        "used as a proxy.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for K-12 self-management surveys...");

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

        context.Log($"Found {matchingAssessments.Count} K-12 self-management survey(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no K-12 self-management surveys found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized K-12 self-management survey instruments were found " +
                    "in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Loading student assessment results for K-12 self-management surveys...");

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
            $"Found {result.RecordsWithScores:N0} of {result.TotalRecords:N0} K-12 self-management survey results with score results");
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
