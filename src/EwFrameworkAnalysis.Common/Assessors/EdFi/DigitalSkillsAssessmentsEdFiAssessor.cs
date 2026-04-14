using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class DigitalSkillsAssessmentsEdFiAssessor : IEdFiAssessor
{
    // The E-W Framework notes no recommended K-12 instrument is currently available.
    // Legacy K-12 tools (iDCA, ST2L) are no longer distributed. Postsecondary / adult
    // instruments from the "Digital Resilience in the American Workforce" compendium
    // may appear if locally adopted, so keywords include those plus generic digital-
    // skill and technology-literacy terms.
    private static readonly string[] _assessmentKeywords =
    [
        // Generic terms
        "digital skills",
        "digital literacy",
        "digital competency",
        "digital competence",
        "technology literacy",
        "technology competency",
        "ICT literacy",
        "information literacy",
        "computer literacy",
        "digital resilience",

        // Legacy K-12 instruments named in the framework
        "iDCA",                       // Instant Digital Competence Assessment
        "Instant Digital Competence",
        "ST2L",                       // Student Tool for Technology Literacy
        "Student Tool for Technology Literacy",

        // Postsecondary / adult instruments referenced by the framework
        "Northstar",                  // Northstar Digital Literacy Assessment
        "IC3",                        // IC3 Digital Literacy Certification
        "TOSA",                       // Test of Digital Skills
        "PIAAC",                      // Programme for the International Assessment of Adult Competencies
        "ICDL"                        // International Computer Driving Licence
    ];

    public string DataElementName => "Digital skills assessments (K-12)";

    public string AssessmentDescription =>
        "Identifies student assessments linked to digital skills / technology literacy instruments. " +
        "The E-W Framework notes that no validated K-12 instrument is currently recommended (legacy " +
        "tools such as the Instant Digital Competence Assessment and Student Tool for Technology " +
        "Literacy are no longer available); postsecondary / adult instruments from the Digital " +
        "Resilience in the American Workforce compendium are matched as proxies along with generic " +
        "digital-skill and technology-literacy keywords. Ed-Fi has no standard descriptor for digital " +
        "skills assessments, so title-based matching against the assessments catalog is used.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for digital skills assessments...");

        var matchingAssessments = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (IsDigitalSkillsAssessment(assessment))
                    matchingAssessments.Add((assessment.AssessmentIdentifier, assessment.Namespace));
            },
            context);

        context.Log($"Found {matchingAssessments.Count} digital skills assessment(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no digital skills assessments found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized digital skills / technology literacy instruments " +
                    "were found in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Loading student assessment results for digital skills assessments...");

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
            $"Found {result.RecordsWithScores:N0} of {result.TotalRecords:N0} digital skills assessment results with score results");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(result.TotalRecords),
                new Distribution(instrumentDistribution, "Assessment Instrument"),
                new Distribution(result.GradeLevelDistribution, "Grade Level Assessed"),
                new Distribution(result.PerformanceLevelDistribution, "Performance Level / Proficiency"),
                new Completeness(result.TotalRecords, result.RecordsWithScores, "ScoreResults")
            ],
            Remarks = AssessmentDescription
        };
    }

    private static bool IsDigitalSkillsAssessment(EdFiAssessment assessment)
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
