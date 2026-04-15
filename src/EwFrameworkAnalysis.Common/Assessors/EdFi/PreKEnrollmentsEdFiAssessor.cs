using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class PreKEnrollmentsEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Enrollment in quality public pre-K";

    public string AssessmentDescription =>
        "Count of studentSchoolAssociations where gradeLevelDescriptor is Prekindergarten";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.Log($"Starting assessment: {DataElementName}");
        context.ReportProgress(0, "Initializing...");

        var queryParams = new Dictionary<string, string>
        {
            ["entryGradeLevelDescriptor"] = "uri://ed-fi.org/GradeLevelDescriptor#Prekindergarten"
        };

        context.Log("Fetching Pre-K enrollment count from Ed-Fi API");
        context.ReportProgress(25, "Querying API for total count...");

        var count = await EdFiApiPatterns.CountFromHeaderAsync(
            httpClient,
            "ed-fi/studentSchoolAssociations",
            queryParams
        );

        context.Log($"Found {count:N0} Pre-K enrollments");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics = [new RecordCount(count)],
            Remarks = AssessmentDescription
        };
    }
}
