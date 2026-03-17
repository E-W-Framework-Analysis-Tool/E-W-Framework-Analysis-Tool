using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class SchoolTypeEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "K-12 school type";

    public string AssessmentDescription =>
        "Distribution of schools by school type descriptor";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Querying schools...");

        var distribution = new Dictionary<string, int>();
        var totalSchools = 0;
        var schoolsWithType = 0;

        await EdFiApiPatterns.PageAndProcessAsync<EdFiSchool>(
            httpClient,
            "ed-fi/schools",
            school =>
            {
                totalSchools++;

                var schoolType = EdFiDescriptorHelper.ParseDescriptorValue(school.SchoolTypeDescriptor);
                if (schoolType != "Unknown")
                    schoolsWithType++;

                if (!distribution.ContainsKey(schoolType))
                    distribution[schoolType] = 0;
                distribution[schoolType]++;
            },
            context
        );

        context.Log($"Found {totalSchools:N0} schools, {schoolsWithType:N0} with school type data");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalSchools),
                new Distribution(distribution, "School Type"),
                new Completeness(totalSchools, schoolsWithType, "SchoolTypeDescriptor")
            ]
        };
    }
}
