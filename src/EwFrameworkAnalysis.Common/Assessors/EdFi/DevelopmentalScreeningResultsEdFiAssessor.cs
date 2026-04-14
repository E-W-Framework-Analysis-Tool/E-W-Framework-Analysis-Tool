using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class DevelopmentalScreeningResultsEdFiAssessor : IEdFiAssessor
{
    // Keywords cover both Pre-K developmental screening tools (Birth to 5: Watch Me Thrive!
    // compendium) and K-12 mental/emotional health screening tools commonly used in schools.
    private static readonly string[] _screeningKeywords =
    [
        // Generic terms
        "developmental screening",
        "developmental screen",
        "universal screening",
        "mental health",
        "emotional health",
        "social emotional",
        "social-emotional",
        "behavioral screen",
        "behavior screen",
        "wellbeing",
        "well-being",
        "early childhood screening",

        // Pre-K / Birth to 5 instruments
        "ASQ",        // Ages and Stages Questionnaires
        "ASQ:SE",
        "ASQ-SE",
        "ASQ-3",
        "PEDS",       // Parents' Evaluation of Developmental Status
        "PEDS:DM",    // PEDS: Developmental Milestones
        "Brigance",   // Brigance Early Childhood Screens
        "Denver II",
        "Battelle",   // Battelle Developmental Inventory Screening Test
        "ESI-R",      // Early Screening Inventory-Revised
        "DIAL",       // Developmental Indicators for the Assessment of Learning
        "M-CHAT",     // Modified Checklist for Autism in Toddlers
        "Watch Me Thrive",

        // K-12 mental / emotional health screeners
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
        "Conners"
    ];

    public string DataElementName => "Developmental screening results";

    public string AssessmentDescription =>
        "Identifies student assessments linked to developmental or mental/emotional health screening tools. " +
        "For Pre-K, matches instruments from the Birth to 5: Watch Me Thrive! compendium (ASQ, PEDS, Brigance, " +
        "Denver II, Battelle, M-CHAT, etc.). For K-12, matches universal mental health screeners " +
        "(SDQ, BASC/BESS, SAEBRS, DESSA, PHQ-9, GAD-7, SSBD, PSC). Ed-Fi has no standard descriptor for " +
        "developmental or mental health screening, so title-based matching against the assessments catalog " +
        "is used as a proxy.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for developmental / mental health screening tools...");

        var matchingAssessments = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (IsScreeningAssessment(assessment))
                    matchingAssessments.Add((assessment.AssessmentIdentifier, assessment.Namespace));
            },
            context);

        context.Log($"Found {matchingAssessments.Count} developmental / screening assessment(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no developmental or screening assessments found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized developmental (Pre-K) or mental/emotional health (K-12) " +
                    "screening instruments were found in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Loading student assessment results for screening instruments...");

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
            $"Found {result.RecordsWithScores:N0} of {result.TotalRecords:N0} screening results with score results");
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

    private static bool IsScreeningAssessment(EdFiAssessment assessment)
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
