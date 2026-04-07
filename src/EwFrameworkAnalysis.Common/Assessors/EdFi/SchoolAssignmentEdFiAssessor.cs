using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class SchoolAssignmentEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "School assignment (prior and current year)";

    public string AssessmentDescription =>
        "Record count and distribution of staff school assignments from staffSchoolAssociations";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Querying API...");

        var schoolDistribution = new Dictionary<string, int>();
        var totalRecords = 0;

        await EdFiApiPatterns.PageAndProcessAsync<EdFiStaffSchoolAssociation>(
            httpClient,
            "ed-fi/staffSchoolAssociations",
            association =>
            {
                totalRecords++;

                var schoolId = association.SchoolReference?.SchoolId.ToString() ?? "Unknown";
                schoolDistribution[schoolId] = schoolDistribution.GetValueOrDefault(schoolId) + 1;
            },
            context
        );

        context.Log($"Found {totalRecords:N0} staff school assignments across {schoolDistribution.Count} schools");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Distribution(schoolDistribution, "School Assignment")
            ]
        };
    }
}
