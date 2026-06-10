using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class CourseOutcomeEdFiAssessor : IEdFiAssessor
{
    // Course attempt results, counted via the API's totalCount header.
    private static readonly string[] _resultDescriptors =
    [
        "uri://ed-fi.org/CourseAttemptResultDescriptor#Pass",
        "uri://ed-fi.org/CourseAttemptResultDescriptor#Fail",
        "uri://ed-fi.org/CourseAttemptResultDescriptor#Incomplete",
        "uri://ed-fi.org/CourseAttemptResultDescriptor#Withdrawn"
    ];

    private const string NOT_SPECIFIED_LABEL = "Not Specified";

    public string DataElementName => "Course outcome";

    public string AssessmentDescription =>
        "Distribution of course passage and completion results from courseTranscripts";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Querying API...");

        // Total transcripts 
        var totalRecords = await EdFiApiPatterns.CountFromHeaderAsync(
            httpClient, "ed-fi/courseTranscripts");

        var distribution = _resultDescriptors.ToDictionary(
            EdFiDescriptorHelper.ParseDescriptorValue, _ => 0);
        var bucketTotal = 0;

        var processed = 0;
        foreach (var descriptor in _resultDescriptors)
        {
            var count = await EdFiApiPatterns.CountFromHeaderAsync(
                httpClient,
                "ed-fi/courseTranscripts",
                new Dictionary<string, string> { ["courseAttemptResultDescriptor"] = descriptor });

            distribution[EdFiDescriptorHelper.ParseDescriptorValue(descriptor)] = count;
            bucketTotal += count;

            processed++;
            context.ReportProgress(processed * 100 / _resultDescriptors.Length, "Counting outcomes...");
        }

        // Transcripts whose result is absent or outside the known set.
        var notSpecified = totalRecords - bucketTotal;
        if (notSpecified > 0)
            distribution[NOT_SPECIFIED_LABEL] = notSpecified;

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
