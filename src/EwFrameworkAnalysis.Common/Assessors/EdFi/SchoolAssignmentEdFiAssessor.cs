using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class SchoolAssignmentEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "School assignment (prior and current year)";

    public string AssessmentDescription =>
        "Record count of staff school assignments from staffSchoolAssociations. " +
        "Ed-Fi ODS databases are typically partitioned by school year, so this reflects " +
        "the presence of any assignment data rather than longitudinal prior/current year records.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Querying API for total count...");

        var count = await EdFiApiPatterns.CountFromHeaderAsync(
            httpClient,
            "ed-fi/staffSchoolAssociations");

        context.Log($"Found {count:N0} staff school assignments");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics = [new RecordCount(count)],
            Remarks = AssessmentDescription
        };
    }
}
