using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class StudentGradeLevelEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Student grade level (K-12)";

    public string AssessmentDescription =>
        "Distribution of students by grade level from studentSchoolAssociations";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Querying API...");

        var gradeDistribution = new Dictionary<string, int>();
        var totalRecords = 0;
        var reportedCount = 0;

        await EdFiApiPatterns.PageAndProcessAsync<EdFiStudentSchoolAssociation>(
            httpClient,
            "ed-fi/studentSchoolAssociations",
            association =>
            {
                totalRecords++;

                var descriptor = association.EntryGradeLevelDescriptor;

                if (!string.IsNullOrWhiteSpace(descriptor))
                {
                    reportedCount++;
                    var value = EdFiDescriptorHelper.ParseDescriptorValue(descriptor);
                    gradeDistribution[value] = gradeDistribution.GetValueOrDefault(value) + 1;
                }
            },
            context
        );

        var grades = string.Join(", ", gradeDistribution.Keys.OrderBy(s => s));
        context.Log($"Found {totalRecords:N0} enrollments across grades: {grades}");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Completeness(totalRecords, reportedCount, "EntryGradeLevel"),
                new Distribution(gradeDistribution, "Grade Level")
            ]
        };
    }
}
