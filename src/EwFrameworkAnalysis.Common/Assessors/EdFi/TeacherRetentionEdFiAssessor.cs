using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class TeacherRetentionEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Employment date";

    public string AssessmentDescription =>
        "Count of staff where endDate is defined";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.Log($"Starting assessment: {DataElementName}");
        context.ReportProgress(0, "Initializing...");

        context.Log("Fetching Teacher employment dates count from Ed-Fi API");
        context.ReportProgress(25, "Querying API for total count...");

        var count = await EdFiApiPatterns.PageAndCountMatchesAsync<EdFiStaffEducationOrganizationEmploymentAssociation>(
            httpClient,
            "ed-fi/staffEducationOrganizationEmploymentAssociations",
            resp => resp.EndDate != null,
            context
        );

        context.Log($"Found {count:N0} teachers with employment dates");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics = [new RecordCount(count)],
            Remarks = AssessmentDescription
        };
    }
}

