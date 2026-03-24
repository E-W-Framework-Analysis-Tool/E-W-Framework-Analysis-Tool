using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class IndustryCredentialEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Industry-recognized credential attainment";

    public string AssessmentDescription =>
        "Analyzes credentials for type distribution and field completeness";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Loading credentials...");

        var totalRecords = 0;
        var recordsWithField = 0;
        var typeDistribution = new Dictionary<string, int>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiCredential>(
            httpClient,
            "ed-fi/credentials",
            credential =>
            {
                totalRecords++;

                var credType = EdFiDescriptorHelper.ParseDescriptorValue(credential.CredentialTypeDescriptor);
                if (!typeDistribution.ContainsKey(credType))
                    typeDistribution[credType] = 0;
                typeDistribution[credType]++;

                if (!string.IsNullOrWhiteSpace(credential.CredentialFieldDescriptor))
                    recordsWithField++;
            },
            context
        );

        context.Log($"Found {recordsWithField:N0} of {totalRecords:N0} credentials with CredentialFieldDescriptor");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Distribution(typeDistribution, "Credential Type"),
                new Completeness(totalRecords, recordsWithField, "CredentialFieldDescriptor")
            ],
            Remarks = AssessmentDescription
        };
    }
}
