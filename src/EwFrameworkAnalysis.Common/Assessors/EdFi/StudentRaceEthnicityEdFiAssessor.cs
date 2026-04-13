using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class StudentRaceEthnicityEdFiAssessor : IEdFiAssessor
{
    private readonly EdFiStudentDemographicsProvider _demographicsProvider;

    public StudentRaceEthnicityEdFiAssessor(EdFiStudentDemographicsProvider demographicsProvider)
    {
        _demographicsProvider = demographicsProvider;
    }

    public string DataElementName => "Student race/ethnicity";

    public string AssessmentDescription =>
        "Distribution of students by race/ethnicity from studentEducationOrganizationAssociations";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading student demographics...");

        var data = await _demographicsProvider.GetDataAsync(httpClient, context);

        var distribution = new Dictionary<string, int>();
        var totalRecords = data.Count;
        var recordsWithRace = 0;

        foreach (var association in data)
        {
            if (association.Races != null && association.Races.Count > 0)
            {
                recordsWithRace++;
                foreach (var race in association.Races)
                {
                    var raceValue = EdFiDescriptorHelper.ParseDescriptorValue(race.RaceDescriptor);
                    if (!distribution.ContainsKey(raceValue))
                        distribution[raceValue] = 0;
                    distribution[raceValue]++;
                }
            }
        }

        context.Log($"Found {recordsWithRace:N0} of {totalRecords:N0} records with race/ethnicity data");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Distribution(distribution, "Race/Ethnicity"),
                new Completeness(totalRecords, recordsWithRace, "Race")
            ]
        };
    }
}
