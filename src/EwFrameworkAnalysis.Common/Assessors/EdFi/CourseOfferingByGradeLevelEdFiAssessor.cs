using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class CourseOfferingByGradeLevelEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Course offering by grade level";

    public string AssessmentDescription =>
        "Count of course offerings from courseOfferings";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Querying API for total count...");

        var count = await EdFiApiPatterns.CountFromHeaderAsync(
            httpClient,
            "ed-fi/courseOfferings");

        context.Log($"Found {count:N0} course offerings");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics = [new RecordCount(count)],
            Remarks = AssessmentDescription
        };
    }
}
