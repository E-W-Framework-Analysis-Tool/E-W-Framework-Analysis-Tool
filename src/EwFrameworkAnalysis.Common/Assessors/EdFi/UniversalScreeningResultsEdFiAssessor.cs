using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class UniversalScreeningResultsEdFiAssessor : IEdFiAssessor
{
    // Keywords covering common K-12 mental/emotional health universal screening tools
    // and generic terms used in assessment titles for these instruments.
    private static readonly string[] _screeningKeywords =
    [
        "universal screening",
        "mental health",
        "emotional health",
        "social emotional",
        "social-emotional",
        "behavioral screen",
        "behavior screen",
        "wellbeing",
        "well-being",
        "SDQ",        // Strengths and Difficulties Questionnaire
        "BASC",       // Behavior Assessment System for Children
        "BESS",       // Behavioral and Emotional Screening System
        "BIMAS",      // Behavior Intervention Monitoring Assessment System
        "SAEBRS",     // Social, Academic, and Emotional Behavior Risk Screener
        "DESSA",      // Devereux Student Strengths Assessment
        "PSC",        // Pediatric Symptom Checklist
        "PHQ-9",      // Patient Health Questionnaire
        "GAD-7",      // Generalized Anxiety Disorder
        "SSBD",       // Systematic Screening for Behavior Disorders
        "CBCL",       // Child Behavior Checklist
        "Conners",
        "ASQ:SE",     // Ages and Stages Questionnaire: Social-Emotional
        "ASQ-SE"
    ];

    public string DataElementName => "Universal screening results";

    public string AssessmentDescription =>
        "Identifies student assessments linked to K-12 mental or emotional health universal screening tools " +
        "(e.g., SDQ, BASC/BESS, SAEBRS, DESSA, PHQ-9, GAD-7, SSBD, PSC) by matching well-known instrument " +
        "names and screening-related keywords in the Ed-Fi assessments catalog. Ed-Fi has no standard descriptor " +
        "for universal screening, so title-based matching is used as a proxy.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for mental/emotional health screening tools...");

        var matchingAssessments = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (IsMentalHealthScreeningAssessment(assessment))
                    matchingAssessments.Add((assessment.AssessmentIdentifier, assessment.Namespace));
            },
            context);

        context.Log($"Found {matchingAssessments.Count} mental/emotional health screening assessment(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no mental/emotional health screening assessments found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized universal mental/emotional health screening tools " +
                    "were found in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Loading student assessment results for screening instruments...");

        var studentAssessments = new List<EdFiStudentAssessment>();
        var assessmentTitleDistribution = new Dictionary<string, int>();

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
                assessmentTitleDistribution[identifier] = added;
        }

        var result = StudentAssessmentAnalyzer.Analyze(studentAssessments);

        context.Log(
            $"Found {result.RecordsWithScores:N0} of {result.TotalRecords:N0} screening results with score results");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(result.TotalRecords),
                new Distribution(assessmentTitleDistribution, "Screening Instrument"),
                new Distribution(result.GradeLevelDistribution, "Grade Level Assessed"),
                new Distribution(result.PerformanceLevelDistribution, "Performance Level / Risk Category"),
                new Completeness(result.TotalRecords, result.RecordsWithScores, "ScoreResults")
            ],
            Remarks = AssessmentDescription
        };
    }

    private static bool IsMentalHealthScreeningAssessment(EdFiAssessment assessment)
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
