using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class EnrollmentStatusEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Enrollment status (current and prior years)";

    public string AssessmentDescription =>
        "Count of studentSchoolAssociations. " +
        "Ed-Fi ODS is typically partitioned by school year; this counts enrollment records in the current ODS.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Querying API for total count...");

        var count = await EdFiApiPatterns.CountFromHeaderAsync(
            httpClient,
            "ed-fi/studentSchoolAssociations");

        context.Log($"Found {count:N0} enrollment records");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics = [new RecordCount(count)],
            Remarks = AssessmentDescription
        };
    }
}
