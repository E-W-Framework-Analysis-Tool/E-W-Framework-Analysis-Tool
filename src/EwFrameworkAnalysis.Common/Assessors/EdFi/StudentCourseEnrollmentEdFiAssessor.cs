using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class StudentCourseEnrollmentEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Student course enrollment record";

    public string AssessmentDescription =>
        "Count of studentSectionAssociations representing student course enrollments";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.Log($"Starting assessment: {DataElementName}");
        context.ReportProgress(0, "Initializing...");

        context.Log("Fetching student course enrollment count from Ed-Fi API");
        context.ReportProgress(25, "Querying API for total count...");

        var count = await EdFiApiPatterns.CountFromHeaderAsync(
            httpClient,
            "ed-fi/studentSectionAssociations"
        );

        context.Log($"Found {count:N0} student course enrollments");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics = [new RecordCount(count)],
            Remarks = AssessmentDescription
        };
    }
}
