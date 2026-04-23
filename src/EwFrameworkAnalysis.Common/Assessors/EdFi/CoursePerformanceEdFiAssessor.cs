using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class CoursePerformanceEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Course performance (English and Math)";

    public string AssessmentDescription =>
        "Distribution of course attempt results from courseTranscripts";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Querying API...");

        var resultDistribution = new Dictionary<string, int>();
        var totalRecords = 0;
        var withGradeCount = 0;

        await EdFiApiPatterns.PageAndProcessAsync<EdFiCourseTranscript>(
            httpClient,
            "ed-fi/courseTranscripts",
            transcript =>
            {
                totalRecords++;

                var result = transcript.CourseAttemptResultDescriptor;
                if (!string.IsNullOrWhiteSpace(result))
                {
                    var label = EdFiDescriptorHelper.ParseDescriptorValue(result);
                    resultDistribution[label] = resultDistribution.GetValueOrDefault(label) + 1;
                }

                if (!string.IsNullOrWhiteSpace(transcript.FinalLetterGradeEarned) ||
                    transcript.FinalNumericGradeEarned != null)
                {
                    withGradeCount++;
                }
            },
            context
        );

        context.Log($"Found {totalRecords:N0} course transcripts, {withGradeCount:N0} with grade data");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Completeness(totalRecords, withGradeCount, "FinalGrade"),
                new Distribution(resultDistribution, "Course Attempt Result")
            ]
        };
    }
}
