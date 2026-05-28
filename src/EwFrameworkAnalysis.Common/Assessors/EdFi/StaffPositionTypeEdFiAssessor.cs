using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class StaffPositionTypeEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Staff position type";

    public string AssessmentDescription =>
        "Distribution of staff by classification from staffEducationOrganizationAssignmentAssociations";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Querying API...");

        var distribution = new Dictionary<string, int>();
        var totalRecords = 0;

        await EdFiApiPatterns.PageAndProcessAsync<EdFiStaffEducationOrganizationAssignmentAssociation>(
            httpClient,
            "ed-fi/staffEducationOrganizationAssignmentAssociations",
            association =>
            {
                totalRecords++;

                var classification = association.StaffClassificationDescriptor;
                var label = !string.IsNullOrWhiteSpace(classification)
                    ? EdFiDescriptorHelper.ParseDescriptorValue(classification)
                    : "Not Specified";

                distribution[label] = distribution.GetValueOrDefault(label) + 1;
            },
            context
        );

        context.Log($"Found {totalRecords:N0} assignments across {distribution.Count} classifications");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Distribution(distribution, "Staff Classification")
            ]
        };
    }
}
