using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class CTECourseCompletionEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "CTE course completion";

    public string AssessmentDescription =>
        "Count of studentCTEProgramAssociations representing CTE course completions";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.Log($"Starting assessment: {DataElementName}");
        context.ReportProgress(0, "Initializing...");

        context.Log("Fetching CTE course completion count from Ed-Fi API");
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
