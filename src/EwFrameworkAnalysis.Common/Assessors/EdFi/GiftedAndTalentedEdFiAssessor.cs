using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class GiftedAndTalentedEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Gifted and talented participation";

    public string AssessmentDescription =>
        "Count of studentProgramAssociations with Gifted and Talented program type";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Querying API for total count...");

        var count = await EdFiApiPatterns.CountFromHeaderAsync(
            httpClient,
            "ed-fi/studentProgramAssociations",
            new Dictionary<string, string>
            {
                ["programTypeDescriptor"] = "uri://ed-fi.org/ProgramTypeDescriptor#Gifted and Talented"
            });

        context.Log($"Found {count:N0} Gifted and Talented program association records");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics = [new RecordCount(count)],
            Remarks = AssessmentDescription
        };
    }
}
