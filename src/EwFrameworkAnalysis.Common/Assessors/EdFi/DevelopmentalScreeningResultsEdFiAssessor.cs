using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class DevelopmentalScreeningResultsEdFiAssessor : IEdFiAssessor
{
    private static readonly string[] _screeningCategoryDescriptors =
    [
        "Developmental observation",
        "Early Learning - Approaches toward learning",
        "Early Learning - Cognition and general knowledge",
        "Early Learning - Language and literacy development",
        "Early Learning - Physical well-being and motor dev",
        "Early Learning - Social and emotional development",
        "Prekindergarten Readiness"
    ];

    private static readonly string[] _screeningKeywords =
    [
        // Generic terms
        "developmental screening",
        "developmental screen",
        "early childhood screening",
        "early childhood screen",
        "Birth to 5",
        "Watch Me Thrive",

        // Instruments from the Birth to 5: Watch Me Thrive! compendium
        "ASQ",                               // Ages and Stages Questionnaires
        "ASQ-3",
        "ASQ:SE",                            // ASQ: Social-Emotional
        "ASQ-SE",
        "Ages and Stages",
        "PEDS",                              // Parents' Evaluation of Developmental Status
        "PEDS:DM",                           // PEDS: Developmental Milestones
        "Parents' Evaluation of Developmental Status",
        "Brigance",                          // Brigance Early Childhood Screens
        "Denver II",
        "Denver Developmental",
        "Battelle",                          // Battelle Developmental Inventory Screening Test
        "ESI-R",                             // Early Screening Inventory-Revised
        "Early Screening Inventory",
        "DIAL",                              // Developmental Indicators for the Assessment of Learning
        "Developmental Indicators for the Assessment of Learning",
        "M-CHAT",                            // Modified Checklist for Autism in Toddlers
        "Modified Checklist for Autism",
        "BDI-2",                             // Battelle Developmental Inventory, 2nd ed.
        "FirstSTEp",                         // Screening Test for Evaluating Preschoolers
        "Bayley"                             // Bayley Scales of Infant and Toddler Development
    ];

    public string DataElementName => "Developmental screening results (PK)";

    public string AssessmentDescription =>
        "Identifies student assessments linked to Pre-K developmental screening tools for children under age 5 " +
        "(e.g., ASQ / ASQ:SE, PEDS, Brigance Early Childhood Screens, Denver II, Battelle Developmental Inventory, " +
        "ESI-R, DIAL, M-CHAT) by matching assessmentCategoryDescriptor values (Developmental observation, " +
        "Early Learning domains, Prekindergarten Readiness), well-known instrument names, and " +
        "developmental-screening keywords in the Ed-Fi assessments catalog. Instruments listed align with " +
        "the Birth to 5: Watch Me Thrive! compendium referenced in the E-W Framework.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for Pre-K developmental screening tools...");

        var matchingAssessments = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (IsDevelopmentalScreeningAssessment(assessment))
                    matchingAssessments.Add((assessment.AssessmentIdentifier, assessment.Namespace));
            },
            context);

        context.Log($"Found {matchingAssessments.Count} developmental screening assessment(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no developmental screening assessments found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized Pre-K developmental screening instruments were found " +
                    "in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Loading student assessment results for developmental screening instruments...");

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
            $"Found {result.RecordsWithScores:N0} of {result.TotalRecords:N0} developmental screening results with score results");
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

    private static bool IsDevelopmentalScreeningAssessment(EdFiAssessment assessment)
    {
        if (!string.IsNullOrWhiteSpace(assessment.AssessmentCategoryDescriptor))
        {
            var category = EdFiDescriptorHelper.ParseDescriptorValue(assessment.AssessmentCategoryDescriptor);
            foreach (var descriptor in _screeningCategoryDescriptors)
            {
                if (category.Equals(descriptor, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
        }

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
