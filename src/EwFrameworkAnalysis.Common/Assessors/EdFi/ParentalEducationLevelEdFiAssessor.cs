using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class ParentalEducationLevelEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Parental education level";

    public string AssessmentDescription =>
        "Distribution of parent/contact highest completed education levels from the " +
        "Ed-Fi contacts endpoint (HighestCompletedLevelOfEducationDescriptor).";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading contacts (parents/guardians)...");

        var totalContacts = 0;
        var withEducationLevel = 0;
        var distribution = new Dictionary<string, int>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiContact>(
            httpClient,
            "ed-fi/contacts",
            contact =>
            {
                totalContacts++;

                var descriptor = contact.HighestCompletedLevelOfEducationDescriptor;
                if (string.IsNullOrWhiteSpace(descriptor))
                    return;

                withEducationLevel++;
                var level = EdFiDescriptorHelper.ParseDescriptorValue(descriptor);
                distribution[level] = distribution.GetValueOrDefault(level) + 1;
            },
            context);

        context.Log($"Found {withEducationLevel:N0} contacts with education level out of {totalContacts:N0} total");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalContacts),
                new Completeness(totalContacts, withEducationLevel, "HighestCompletedLevelOfEducation"),
                new Distribution(distribution, "Education Level")
            ]
        };
    }
}
