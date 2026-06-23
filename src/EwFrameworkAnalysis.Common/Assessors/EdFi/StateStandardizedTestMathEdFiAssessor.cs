using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

// Grade-agnostic state standardized math assessment (all grades). Grade-level breakdown is reported as
// a "Grade Level Assessed" distribution derived from each assessment's AssessedGradeLevels.
public class StateStandardizedTestMathEdFiAssessor : IEdFiAssessor
{
    private static readonly HashSet<string> _stateTestCategories =
    [
        "uri://ed-fi.org/AssessmentCategoryDescriptor#State assessment",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#Benchmark test",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#State summative assessment 3-8 general",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#State alternative assessment/grade-level standards",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#State alternative assessment/modified standards",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#State alternate assessment/ELL"
    ];

    public string DataElementName => "State standardized test (Math proficiency)";

    public string AssessmentDescription =>
        "Count of students with state standardized test results in mathematics across all grade levels, " +
        "broken down by the grade level(s) each assessment is administered for.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Finding state standardized mathematics assessments...");

        // assessmentIdentifier -> grade-level descriptors (from AssessedGradeLevels) for that assessment
        var assessmentGradeLevels = new Dictionary<string, List<string>>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                var isStateTest = _stateTestCategories.Contains(assessment.AssessmentCategoryDescriptor ?? "");

                var isMath = assessment.AcademicSubjects?.Any(subj =>
                    subj.AcademicSubjectDescriptor == "uri://ed-fi.org/AcademicSubjectDescriptor#Mathematics") ?? false;

                if (isStateTest && isMath)
                {
                    var gradeLevels = assessment.AssessedGradeLevels?
                        .Select(gl => gl.GradeLevelDescriptor)
                        .Where(descriptor => !string.IsNullOrEmpty(descriptor))
                        .ToList() ?? [];

                    assessmentGradeLevels[assessment.AssessmentIdentifier] = gradeLevels!;
                    context.Log($"Found math assessment: {assessment.AssessmentIdentifier}");
                }
            },
            context
        );

        context.Log($"Found {assessmentGradeLevels.Count} state standardized mathematics assessments");

        if (assessmentGradeLevels.Count == 0)
        {
            context.Log("No matching mathematics assessments found.");
            context.ReportProgress(100, "Complete");

            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)]
            };
        }

        context.ReportProgress(50, "Counting student assessment records by grade level...");

        var totalCount = 0;
        var gradeDistribution = new Dictionary<string, int>();

        // Attribute each assessment's student record count to the grade level(s) it is administered for.
        // A single-grade assessment (the norm for state tests) maps to exactly one grade.
        foreach (var (assessmentId, gradeLevels) in assessmentGradeLevels)
        {
            var count = await EdFiApiPatterns.CountFromHeaderAsync(
                httpClient,
                "ed-fi/studentAssessments",
                new Dictionary<string, string>
                {
                    ["assessmentIdentifier"] = assessmentId
                }
            );

            totalCount += count;

            if (gradeLevels.Count == 0)
            {
                AddToDistribution(gradeDistribution, "Unspecified", count);
            }
            else
            {
                foreach (var descriptor in gradeLevels)
                {
                    AddToDistribution(gradeDistribution, EdFiDescriptorHelper.ParseDescriptorValue(descriptor), count);
                }
            }
        }

        context.Log($"Found {totalCount:N0} state standardized mathematics assessment records");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalCount),
                new Distribution(gradeDistribution, "Grade Level Assessed")
            ]
        };
    }

    private static void AddToDistribution(Dictionary<string, int> distribution, string key, int count)
    {
        if (!distribution.ContainsKey(key))
        {
            distribution[key] = 0;
        }
        distribution[key] += count;
    }
}
