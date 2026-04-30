using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class TeacherQualificationEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Teacher qualification or certification type";

    public string AssessmentDescription =>
        "Distribution of credentials by type from credentials";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Querying API...");

        var distribution = new Dictionary<string, int>();
        var totalRecords = 0;

        await EdFiApiPatterns.PageAndProcessAsync<EdFiCredential>(
            httpClient,
            "ed-fi/credentials",
            credential =>
            {
                totalRecords++;

                var credentialType = credential.CredentialTypeDescriptor;
                var label = !string.IsNullOrWhiteSpace(credentialType)
                    ? EdFiDescriptorHelper.ParseDescriptorValue(credentialType)
                    : "Not Specified";

                distribution[label] = distribution.GetValueOrDefault(label) + 1;
            },
            context
        );

        context.Log($"Found {totalRecords:N0} credentials across {distribution.Count} types");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Distribution(distribution, "Credential Type")
            ]
        };
    }
}
