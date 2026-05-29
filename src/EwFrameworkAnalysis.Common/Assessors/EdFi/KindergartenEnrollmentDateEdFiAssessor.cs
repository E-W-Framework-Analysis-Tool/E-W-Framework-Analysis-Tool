using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class KindergartenEnrollmentDateEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Kindergarten enrollment date";

    public string AssessmentDescription =>
        "Count of studentSchoolAssociations where gradeLevelDescriptor is Kindergarten";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Querying API for total count...");

        var count = await EdFiApiPatterns.CountFromHeaderAsync(
            httpClient,
            "ed-fi/studentSchoolAssociations",
            new Dictionary<string, string>
            {
                ["entryGradeLevelDescriptor"] = "uri://ed-fi.org/GradeLevelDescriptor#Kindergarten"
            });

        context.Log($"Found {count:N0} Kindergarten enrollment records");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics = [new RecordCount(count)],
            Remarks = AssessmentDescription
        };
    }
}
