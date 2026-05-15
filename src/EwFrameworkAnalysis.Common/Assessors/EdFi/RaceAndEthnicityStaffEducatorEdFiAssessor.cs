using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class RaceAndEthnicityStaffEducatorEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Race and ethnicity (staff/educator)";

    public string AssessmentDescription =>
        "Distribution of staff by race/ethnicity from staffs";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Querying API...");

        var distribution = new Dictionary<string, int>();
        var totalRecords = 0;
        var recordsWithRace = 0;

        await EdFiApiPatterns.PageAndProcessAsync<EdFiStaff>(
            httpClient,
            "ed-fi/staffs",
            staff =>
            {
                totalRecords++;

                if (staff.Races != null && staff.Races.Count > 0)
                {
                    recordsWithRace++;
                    foreach (var race in staff.Races)
                    {
                        var raceValue = EdFiDescriptorHelper.ParseDescriptorValue(race.RaceDescriptor);
                        distribution[raceValue] = distribution.GetValueOrDefault(raceValue) + 1;
                    }
                }
            },
            context
        );

        context.Log($"Found {recordsWithRace:N0} of {totalRecords:N0} staff with race/ethnicity data");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Completeness(totalRecords, recordsWithRace, "Race"),
                new Distribution(distribution, "Race/Ethnicity")
            ]
        };
    }
}
