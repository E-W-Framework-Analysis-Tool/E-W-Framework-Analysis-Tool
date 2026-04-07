using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class ReadingProficiencyGrades1And2EdFiAssessor : IEdFiAssessor
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

    private static readonly HashSet<string> _earlyGrades =
    [
        "uri://ed-fi.org/GradeLevelDescriptor#First grade",
        "uri://ed-fi.org/GradeLevelDescriptor#Second grade"
    ];

    private static readonly HashSet<string> _readingSubjects =
    [
        "uri://ed-fi.org/AcademicSubjectDescriptor#Reading",
        "uri://ed-fi.org/AcademicSubjectDescriptor#English Language Arts"
    ];

    public string DataElementName => "Reading proficiency (Grades 1 and 2)";

    public string AssessmentDescription =>
        "Count of grade 1-2 students with state standardized test results in reading or English Language Arts";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Finding reading assessments for grades 1-2...");

        var assessmentIdentifiers = new HashSet<string>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                var isStateTest = _stateTestCategories.Contains(assessment.AssessmentCategoryDescriptor ?? "");

                var isEarlyGrade = assessment.AssessedGradeLevels?.Any(gl =>
                    _earlyGrades.Contains(gl.GradeLevelDescriptor ?? "")) ?? false;

                var isReading = assessment.AcademicSubjects?.Any(subj =>
                    _readingSubjects.Contains(subj.AcademicSubjectDescriptor ?? "")) ?? false;

                if (isStateTest && isEarlyGrade && isReading)
                {
                    assessmentIdentifiers.Add(assessment.AssessmentIdentifier);
                    context.Log($"Found reading assessment: {assessment.AssessmentIdentifier}");
                }
            },
            context
        );

        context.Log($"Found {assessmentIdentifiers.Count} grades 1-2 reading assessments");

        if (assessmentIdentifiers.Count == 0)
        {
            context.Log("No matching reading assessments found.");
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

        context.Log($"Found {totalCount:N0} grades 1-2 reading assessment records");

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
