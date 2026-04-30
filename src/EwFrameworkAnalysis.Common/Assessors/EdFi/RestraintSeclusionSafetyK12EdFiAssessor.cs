using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class RestraintSeclusionSafetyK12EdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Restraint and seclusion for safety (K-12)";

    public string AssessmentDescription =>
        "Count and distribution of restraint events from the dedicated ed-fi/restraintEvents endpoint, " +
        "with breakdown by restraintEventReasonDescriptor";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Querying restraint events...");

        var reasonDistribution = new Dictionary<string, int>();
        var totalRecords = 0;

        await EdFiApiPatterns.PageAndProcessAsync<EdFiRestraintEvent>(
            httpClient,
            "ed-fi/restraintEvents",
            restraintEvent =>
            {
                totalRecords++;

                if (restraintEvent.Reasons is { Count: > 0 })
                {
                    foreach (var reason in restraintEvent.Reasons)
                    {
                        var value = EdFiDescriptorHelper.ParseDescriptorValue(
                            reason.RestraintEventReasonDescriptor ?? "Unknown");
                        reasonDistribution[value] = reasonDistribution.GetValueOrDefault(value) + 1;
                    }
                }
                else
                {
                    reasonDistribution["Not Reported"] = reasonDistribution.GetValueOrDefault("Not Reported") + 1;
                }
            },
            context
        );

        context.Log($"Found {totalRecords:N0} restraint events");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Distribution(reasonDistribution, "Restraint Event Reason")
            ]
        };
    }
}
