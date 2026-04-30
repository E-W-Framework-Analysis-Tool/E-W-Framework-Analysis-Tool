using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class CourseOutcomeEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Course outcome";

    public string AssessmentDescription =>
        "Distribution of course passage and completion results from courseTranscripts";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Querying API...");

        var distribution = new Dictionary<string, int>();
        var totalRecords = 0;

        await EdFiApiPatterns.PageAndProcessAsync<EdFiCourseTranscript>(
            httpClient,
            "ed-fi/courseTranscripts",
            transcript =>
            {
                totalRecords++;

                var result = transcript.CourseAttemptResultDescriptor;
                var label = !string.IsNullOrWhiteSpace(result)
                    ? EdFiDescriptorHelper.ParseDescriptorValue(result)
                    : "Not Specified";

                distribution[label] = distribution.GetValueOrDefault(label) + 1;
            },
            context
        );

        context.Log($"Found {totalRecords:N0} course transcripts across {distribution.Count} outcome types");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Distribution(distribution, "Course Outcome")
            ]
        };
    }
}
