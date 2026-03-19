using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class WorkBasedLearningEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Participation in work-based learning";

    public string AssessmentDescription =>
        "Count of studentCTEProgramAssociations representing participation in work-based learning";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.Log($"Starting assessment: {DataElementName}");
        context.ReportProgress(0, "Initializing...");

        context.Log("Fetching work-based learning participation count from Ed-Fi API");
        context.ReportProgress(25, "Querying API for total count...");

        var count = await EdFiApiPatterns.CountFromHeaderAsync(
            httpClient,
            "ed-fi/studentCTEProgramAssociations"
        );

        context.Log($"Found {count:N0} CTE program associations");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics = [new RecordCount(count)],
            Remarks = AssessmentDescription
        };
    }
}
