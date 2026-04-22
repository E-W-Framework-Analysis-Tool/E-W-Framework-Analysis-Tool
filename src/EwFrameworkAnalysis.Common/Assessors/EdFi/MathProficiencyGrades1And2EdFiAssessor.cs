using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class MathProficiencyGrades1And2EdFiAssessor : IEdFiAssessor
{
    private static readonly string[] _stateTestCategories =
    [
        "State assessment",
        "Benchmark test",
        "State alternative assessment/grade-level standards",
        "State alternative assessment/modified standards",
        "State alternate assessment/ELL",
        "Alternate assessment/ELL",
        "Alternate assessment/grade-level standards",
        "Alternative assessment/modified standards"
    ];

    private static readonly string[] _earlyGrades =
    [
        "First grade",
        "Second grade"
    ];

    public string DataElementName => "Math proficiency (Grades 1 and 2)";

    public string AssessmentDescription =>
        "Count of grade 1-2 students with state standardized test results in mathematics";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Finding mathematics assessments for grades 1-2...");

        var assessmentIdentifiers = new HashSet<string>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                var category = EdFiDescriptorHelper.ParseDescriptorValue(assessment.AssessmentCategoryDescriptor ?? "");
                var isStateTest = _stateTestCategories.Any(c => category.Equals(c, StringComparison.OrdinalIgnoreCase));

                var isEarlyGrade = assessment.AssessedGradeLevels?.Any(gl =>
                {
                    var grade = EdFiDescriptorHelper.ParseDescriptorValue(gl.GradeLevelDescriptor ?? "");
                    return _earlyGrades.Any(g => grade.Equals(g, StringComparison.OrdinalIgnoreCase));
                }) ?? false;

                var isMath = assessment.AcademicSubjects?.Any(subj =>
                {
                    var subject = EdFiDescriptorHelper.ParseDescriptorValue(subj.AcademicSubjectDescriptor ?? "");
                    return subject.Equals("Mathematics", StringComparison.OrdinalIgnoreCase);
                }) ?? false;

                if (isStateTest && isEarlyGrade && isMath)
                {
                    assessmentIdentifiers.Add(assessment.AssessmentIdentifier);
                    context.Log($"Found math assessment: {assessment.AssessmentIdentifier}");
                }
            },
            context
        );

        context.Log($"Found {assessmentIdentifiers.Count} grades 1-2 mathematics assessments");

        if (assessmentIdentifiers.Count == 0)
        {
            context.Log("No matching mathematics assessments found.");
            context.ReportProgress(100, "Complete");

            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)]
            };
        }

        context.ReportProgress(50, "Counting student assessment records...");

        var totalCount = 0;
        foreach (var assessmentId in assessmentIdentifiers)
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
        }

        context.Log($"Found {totalCount:N0} grades 1-2 mathematics assessment records");

        context.ReportProgress(75, "Collecting distribution by assessment category...");
        var categoryDistribution = new Dictionary<string, int>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (assessmentIdentifiers.Contains(assessment.AssessmentIdentifier))
                {
                    var category = assessment.AssessmentCategoryDescriptor ?? "Unknown";
                    var categoryName = EdFiDescriptorHelper.ParseDescriptorValue(category);

                    categoryDistribution[categoryName] = categoryDistribution.GetValueOrDefault(categoryName) + 1;
                }
            },
            context
        );

        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalCount),
                new Distribution(categoryDistribution, "Assessment Category")
            ]
        };
    }
}
