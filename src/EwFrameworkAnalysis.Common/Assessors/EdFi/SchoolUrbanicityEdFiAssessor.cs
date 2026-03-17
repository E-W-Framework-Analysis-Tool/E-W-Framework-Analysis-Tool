using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class SchoolUrbanicityEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Urbanicity";

    public string AssessmentDescription =>
        "Distribution of schools by locale/urbanicity from school addresses";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Querying schools...");

        var distribution = new Dictionary<string, int>();
        var totalSchools = 0;
        var schoolsWithLocale = 0;

        await EdFiApiPatterns.PageAndProcessAsync<EdFiSchool>(
            httpClient,
            "ed-fi/schools",
            school =>
            {
                totalSchools++;

                if (school.Addresses != null && school.Addresses.Count > 0)
                {
                    var address = school.Addresses.First();
                    var locale = EdFiDescriptorHelper.ParseDescriptorValue(address.LocaleDescriptor);

                    if (locale != "Unknown")
                    {
                        schoolsWithLocale++;
                        if (!distribution.ContainsKey(locale))
                            distribution[locale] = 0;
                        distribution[locale]++;
                    }
                }
            },
            context
        );

        context.Log($"Found {totalSchools:N0} schools, {schoolsWithLocale:N0} with locale/urbanicity data");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalSchools),
                new Distribution(distribution, "Urbanicity/Locale"),
                new Completeness(totalSchools, schoolsWithLocale, "LocaleDescriptor")
            ]
        };
    }
}
