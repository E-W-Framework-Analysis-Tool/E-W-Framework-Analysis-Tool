using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class StudentMilitaryStatusEdFiAssessor : IEdFiAssessor
{
    private static readonly string _militaryDescriptor = "Military";

    private readonly EdFiStudentDemographicsProvider _demographicsProvider;

    public StudentMilitaryStatusEdFiAssessor(EdFiStudentDemographicsProvider demographicsProvider)
    {
        _demographicsProvider = demographicsProvider;
    }

    public string DataElementName => "Individual or family military status";

    public string AssessmentDescription =>
        "Distribution of students by military connected status from studentEducationOrganizationAssociations";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading student demographics...");

        var data = await _demographicsProvider.GetDataAsync(httpClient, context);

        var distribution = new Dictionary<string, int>
        {
            ["Military Connected"] = 0,
            ["Not Military Connected"] = 0,
            ["Not Reported"] = 0
        };
        var totalRecords = data.Count;

        foreach (var association in data)
        {
            var isMilitaryConnected = association.StudentCharacteristics?
                .Any(c => EdFiDescriptorHelper.ParseDescriptorValue(c.StudentCharacteristicDescriptor)
                    .Contains(_militaryDescriptor, StringComparison.OrdinalIgnoreCase)) ?? false;

            if (association.StudentCharacteristics == null || association.StudentCharacteristics.Count == 0)
            {
                distribution["Not Reported"]++;
            }
            else if (isMilitaryConnected)
            {
                distribution["Military Connected"]++;
            }
            else
            {
                distribution["Not Military Connected"]++;
            }
        }

        context.Log($"Found {distribution["Military Connected"]:N0} military connected students out of {totalRecords:N0}");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Distribution(distribution, "Military Status")
            ]
        };
    }
}
