using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class PreKEnrollmentsEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Pre-K Enrollments";

    public string AssessmentDescription => "Count of studentSchoolAssociations where gradeLevelDescriptor is Prekindergarten";

    public async Task<DataElementAssessment> AssessAsync(HttpClient httpClient, DataSource dataSource)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["entryGradeLevelDescriptor"] = "uri://ed-fi.org/GradeLevelDescriptor#Prekindergarten"
        };

        var count = await EdFiApiPatterns.CountFromHeaderAsync(
            httpClient,
            "ed-fi/studentSchoolAssociations",
            queryParams
        );

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            DataSourceId = dataSource.Id,
            Characteristics = [new RecordCount(count)],
            Remarks = AssessmentDescription
        };
    }
}
