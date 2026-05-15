using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class UniversalMentalHealthScreeningResultsK12EdFiAssessor : IEdFiAssessor
{
    // Keywords for K-12 universal mental / emotional health screening tools suitable
    // for school-based use (per the E-W Framework reference to Mental Health Screening
    // Tools for Grades K-12).
    private static readonly string[] _screeningKeywords =
    [
        // Generic terms
        "universal screening",
        "universal screener",
        "mental health screen",
        "emotional health screen",
        "behavioral screen",
        "behavior screen",
        "social emotional screen",
        "social-emotional screen",
        "SEL screener",

        // Named K-12 universal screening instruments
        "SDQ",                               // Strengths and Difficulties Questionnaire
        "Strengths and Difficulties",
        "BASC",                              // Behavior Assessment System for Children
        "BESS",                              // Behavioral and Emotional Screening System
        "BIMAS",                             // Behavior Intervention Monitoring Assessment System
        "SAEBRS",                            // Social, Academic, and Emotional Behavior Risk Screener
        "mySAEBRS",
        "DESSA",                             // Devereux Student Strengths Assessment
        "PSC",                               // Pediatric Symptom Checklist
        "Pediatric Symptom Checklist",
        "PHQ-9",                             // Patient Health Questionnaire
        "PHQ-A",
        "GAD-7",                             // Generalized Anxiety Disorder
        "SSBD",                              // Systematic Screening for Behavior Disorders
        "CBCL",                              // Child Behavior Checklist
        "Conners",
        "SRSS",                              // Student Risk Screening Scale
        "Student Risk Screening Scale",
        "PSC-17",
        "Columbia DISC",
        "SWIFT"                              // Social and Emotional Health Survey (panorama / SWIFT variants)
    ];

    public string DataElementName => "Universal mental health screening results (K-12)";

    public string AssessmentDescription =>
        "Identifies student assessments linked to K-12 universal mental / emotional health screening tools " +
        "(e.g., SDQ, BASC / BESS, BIMAS, SAEBRS, DESSA, PHQ-9, GAD-7, SSBD, PSC, Conners, SRSS) by matching " +
        "well-known instrument names and screening-related keywords in the Ed-Fi assessments catalog. Instruments " +
        "listed align with the Mental Health Screening Tools for Grades K-12 resource referenced in the E-W " +
        "Framework. Ed-Fi has no standard descriptor for universal screening, so title-based matching is used " +
        "as a proxy.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for K-12 universal screening tools...");

        var matchingAssessments = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (IsUniversalScreeningAssessment(assessment))
                    matchingAssessments.Add((assessment.AssessmentIdentifier, assessment.Namespace));
            },
            context);

        context.Log($"Found {matchingAssessments.Count} universal screening assessment(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no universal screening assessments found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized K-12 universal mental / emotional health screening " +
                    "tools were found in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Loading student assessment results for universal screening instruments...");

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
            $"Found {result.RecordsWithScores:N0} of {result.TotalRecords:N0} universal screening results with score results");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(result.TotalRecords),
                new Distribution(instrumentDistribution, "Screening Instrument"),
                new Distribution(result.GradeLevelDistribution, "Grade Level Assessed"),
                new Distribution(result.PerformanceLevelDistribution, "Performance Level / Risk Category"),
                new Completeness(result.TotalRecords, result.RecordsWithScores, "ScoreResults")
            ],
            Remarks = AssessmentDescription
        };
    }

    private static bool IsUniversalScreeningAssessment(EdFiAssessment assessment)
    {
        var title = assessment.AssessmentTitle ?? string.Empty;
        var identifier = assessment.AssessmentIdentifier ?? string.Empty;

        foreach (var keyword in _screeningKeywords)
        {
            if (title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                identifier.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
