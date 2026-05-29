using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class ReportedIntentToEnrollEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Reported intent to enroll in postsecondary education";

    public string AssessmentDescription =>
        "Count of postSecondaryEvents with College Acceptance category";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Querying API for total count...");

        var count = await EdFiApiPatterns.CountFromHeaderAsync(
            httpClient,
            "ed-fi/postSecondaryEvents",
            new Dictionary<string, string>
            {
                ["postSecondaryEventCategoryDescriptor"] =
                    "uri://ed-fi.org/PostSecondaryEventCategoryDescriptor#College Acceptance"
            });

        context.Log($"Found {count:N0} postsecondary intent-to-enroll event records");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics = [new RecordCount(count)],
            Remarks = AssessmentDescription
        };
    }
}
