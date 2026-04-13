using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class StudentMigrantStatusEdFiAssessor : IEdFiAssessor
{
    private static readonly string _migrantDescriptor = "Migrant";

    private readonly EdFiStudentDemographicsProvider _demographicsProvider;

    public StudentMigrantStatusEdFiAssessor(EdFiStudentDemographicsProvider demographicsProvider)
    {
        _demographicsProvider = demographicsProvider;
    }

    public string DataElementName => "Student from migrant family household";

    public string AssessmentDescription =>
        "Distribution of students by migrant family status from studentEducationOrganizationAssociations";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading student demographics...");

        var data = await _demographicsProvider.GetDataAsync(httpClient, context);

        var distribution = new Dictionary<string, int>
        {
            ["Migrant"] = 0,
            ["Not Migrant"] = 0,
            ["Not Reported"] = 0
        };
        var totalRecords = data.Count;

        foreach (var association in data)
        {
            var isMigrant = association.StudentIndicators?
                .Any(i => EdFiDescriptorHelper.ParseDescriptorValue(i.IndicatorName)
                    .Contains(_migrantDescriptor, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(i.Indicator, "true", StringComparison.OrdinalIgnoreCase)) ?? false;

            if (association.StudentIndicators == null || association.StudentIndicators.Count == 0)
            {
                distribution["Not Reported"]++;
            }
            else if (isMigrant)
            {
                distribution["Migrant"]++;
            }
            else
            {
                distribution["Not Migrant"]++;
            }
        }

        context.Log($"Found {distribution["Migrant"]:N0} migrant students out of {totalRecords:N0}");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Distribution(distribution, "Migrant Status")
            ]
        };
    }
}
