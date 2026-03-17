using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class StudentHomelessnessEdFiAssessor : IEdFiAssessor
{
    private static readonly string _homelessDescriptor = "Homeless";

    private readonly EdFiStudentDemographicsProvider _demographicsProvider;

    public StudentHomelessnessEdFiAssessor(EdFiStudentDemographicsProvider demographicsProvider)
    {
        _demographicsProvider = demographicsProvider;
    }

    public string DataElementName => "Individuals experiencing homelessness";

    public string AssessmentDescription =>
        "Distribution of students by homelessness status from studentEducationOrganizationAssociations";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading student demographics...");

        var data = await _demographicsProvider.GetDataAsync(httpClient, context);

        var distribution = new Dictionary<string, int>
        {
            ["Homeless"] = 0,
            ["Not Homeless"] = 0,
            ["Not Reported"] = 0
        };
        var totalRecords = data.Count;

        foreach (var association in data)
        {
            var isHomeless = association.StudentCharacteristics?
                .Any(c => EdFiDescriptorHelper.ParseDescriptorValue(c.StudentCharacteristicDescriptor)
                    .Contains(_homelessDescriptor, StringComparison.OrdinalIgnoreCase)) ?? false;

            if (association.StudentCharacteristics == null || association.StudentCharacteristics.Count == 0)
            {
                distribution["Not Reported"]++;
            }
            else if (isHomeless)
            {
                distribution["Homeless"]++;
            }
            else
            {
                distribution["Not Homeless"]++;
            }
        }

        context.Log($"Found {distribution["Homeless"]:N0} homeless students out of {totalRecords:N0}");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Distribution(distribution, "Homelessness Status")
            ]
        };
    }
}
