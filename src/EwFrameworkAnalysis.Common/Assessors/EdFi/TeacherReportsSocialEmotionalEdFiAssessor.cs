using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class TeacherReportsSocialEmotionalEdFiAssessor : IEdFiAssessor
{
    // Keywords for teacher-observed developmental assessments that report on social-emotional
    // development. Framework names DRDP Social and Emotional Development, R4K ELA Social
    // Foundations, and TS GOLD Social-Emotional subscale as example instruments.
    private static readonly string[] _assessmentKeywords =
    [
        // Generic terms
        "social-emotional",
        "social emotional",
        "social and emotional",
        "social foundations",
        "teacher report",
        "teacher rating",
        "teacher observation",
        "observational assessment",

        // Instruments named in the E-W Framework
        "DRDP",                              // Desired Results Developmental Profile (Social and Emotional domain)
        "Desired Results Developmental Profile",
        "R4K",                               // Ready 4 Kindergarten (ELA Social Foundations domain)
        "Ready 4 Kindergarten",
        "Ready for Kindergarten",
        "TS GOLD",                           // Teaching Strategies GOLD (Social-Emotional subscale)
        "Teaching Strategies GOLD",

        // Other widely used teacher-observed SEL / developmental instruments
        "DECA",                              // Devereux Early Childhood Assessment
        "Devereux Early Childhood",
        "DESSA",                             // Devereux Student Strengths Assessment
        "SAEBRS",                            // Social, Academic, and Emotional Behavior Risk Screener
        "SSIS",                              // Social Skills Improvement System
        "Social Skills Improvement",
        "Second Step",
        "Panorama SEL",
        "CASEL",
        "Work Sampling",                     // Work Sampling System - Personal and Social Development
        "HighScope COR",
        "COR Advantage"
    ];

    public string DataElementName => "Teacher reports of social-emotional development";

    public string AssessmentDescription =>
        "Identifies student assessments linked to teacher-observed developmental assessments that report on " +
        "social-emotional development (e.g., DRDP Social and Emotional Development domain, R4K ELA Social " +
        "Foundations domain, TS GOLD Social-Emotional subscale named in the E-W Framework, plus DECA / DESSA, " +
        "SAEBRS, SSIS, Second Step, Panorama SEL, Work Sampling, HighScope COR) by matching well-known " +
        "instrument names and SEL teacher-report keywords in the Ed-Fi assessments catalog. Ed-Fi has no " +
        "standard descriptor for teacher-reported social-emotional development, so title-based matching is " +
        "used as a proxy.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for teacher-reported social-emotional instruments...");

        var matchingAssessments = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (IsTeacherReportSelAssessment(assessment))
                    matchingAssessments.Add((assessment.AssessmentIdentifier, assessment.Namespace));
            },
            context);

        context.Log($"Found {matchingAssessments.Count} teacher-reported social-emotional assessment(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no teacher-reported social-emotional assessments found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized teacher-observed developmental / SEL instruments were " +
                    "found in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Loading student assessment results for teacher-reported social-emotional instruments...");

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
            $"Found {result.RecordsWithScores:N0} of {result.TotalRecords:N0} teacher-reported social-emotional results with score results");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(result.TotalRecords),
                new Distribution(instrumentDistribution, "Assessment Instrument"),
                new Distribution(result.GradeLevelDistribution, "Grade Level Assessed"),
                new Distribution(result.PerformanceLevelDistribution, "Performance Level / SEL Level"),
                new Completeness(result.TotalRecords, result.RecordsWithScores, "ScoreResults")
            ],
            Remarks = AssessmentDescription
        };
    }

    private static bool IsTeacherReportSelAssessment(EdFiAssessment assessment)
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
