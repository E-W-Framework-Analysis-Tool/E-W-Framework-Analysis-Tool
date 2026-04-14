using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class SelfEfficacySurveysK12EdFiAssessor : IEdFiAssessor
{
    // Keywords for K-12 self-efficacy survey instruments. Framework names the Creative
    // Self-Efficacy scale; widely used K-12 SEL survey suites that include a self-efficacy
    // scale are also matched.
    private static readonly string[] _surveyKeywords =
    [
        // Generic terms
        "self-efficacy",
        "self efficacy",
        "academic self-efficacy",
        "academic efficacy",
        "self-confidence",

        // Instrument named in the E-W Framework
        "Creative Self-Efficacy",

        // Other widely used K-12 survey suites with a self-efficacy scale
        "Panorama",                          // Panorama SEL / Student Survey
        "CORE Districts",
        "CORE SEL",
        "Tripod",                            // Tripod Student Survey
        "PERTS",                             // Project for Education Research That Scales
        "MDR3C",                             // Motivation and Engagement Scale
        "PALS",                              // Patterns of Adaptive Learning Scales
        "Patterns of Adaptive Learning",
        "MSLQ",                              // Motivated Strategies for Learning Questionnaire
        "Motivated Strategies for Learning"
    ];

    public string DataElementName => "Self-efficacy surveys (K-12)";

    public string AssessmentDescription =>
        "Identifies student assessments linked to K-12 self-efficacy survey instruments (e.g., Creative " +
        "Self-Efficacy scale named in the E-W Framework, plus Panorama / CORE Districts SEL surveys, " +
        "Tripod, PERTS, PALS, MSLQ that include a self-efficacy scale) by matching well-known instrument " +
        "names and self-efficacy keywords in the Ed-Fi assessments catalog. Ed-Fi has no standard descriptor " +
        "for self-efficacy surveys, so title-based matching is used as a proxy.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for K-12 self-efficacy surveys...");

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

        context.Log($"Found {matchingAssessments.Count} K-12 self-efficacy survey(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no K-12 self-efficacy surveys found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized K-12 self-efficacy survey instruments were found " +
                    "in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Loading student assessment results for K-12 self-efficacy surveys...");

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
            $"Found {result.RecordsWithScores:N0} of {result.TotalRecords:N0} K-12 self-efficacy survey results with score results");
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
