using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class CommunicationSkillsAssessmentsEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Communication skills performance assessments (K-12)";

    public string AssessmentDescription =>
        "Count of studentAssessments as a baseline proxy for communication skills assessments. " +
        "Ed-Fi has no standard descriptor for communication skills assessment categories.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.Log($"Starting assessment: {DataElementName}");
        context.ReportProgress(0, "Initializing...");

        context.Log("Fetching student assessment count from Ed-Fi API");
        context.ReportProgress(25, "Querying API for total count...");

        var count = await EdFiApiPatterns.CountFromHeaderAsync(
            httpClient,
            "ed-fi/studentAssessments"
        );

        context.Log($"Found {count:N0} student assessments");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics = [new RecordCount(count)],
            Remarks = AssessmentDescription
        };
    }
}
