using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class IncomeLevelIndividualFamilyEdFiAssessor : IEdFiAssessor
{
    private static readonly string _economicDisadvantagedDescriptor = "Economic Disadvantaged";

    private readonly EdFiStudentDemographicsProvider _demographicsProvider;

    public IncomeLevelIndividualFamilyEdFiAssessor(EdFiStudentDemographicsProvider demographicsProvider)
    {
        _demographicsProvider = demographicsProvider;
    }

    public string DataElementName => "Income level (individual/family)";

    public string AssessmentDescription =>
        "Distribution of students by economic disadvantaged status from studentEducationOrganizationAssociations";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading student demographics...");

        var data = await _demographicsProvider.GetDataAsync(httpClient, context);

        var distribution = new Dictionary<string, int>
        {
            ["Economic Disadvantaged"] = 0,
            ["Not Economic Disadvantaged"] = 0,
            ["Not Reported"] = 0
        };
        var totalRecords = data.Count;

        foreach (var association in data)
        {
            var isEconDisadvantaged = association.StudentCharacteristics?
                .Any(c => EdFiDescriptorHelper.ParseDescriptorValue(c.StudentCharacteristicDescriptor)
                    .Contains(_economicDisadvantagedDescriptor, StringComparison.OrdinalIgnoreCase)) ?? false;

            if (association.StudentCharacteristics == null || association.StudentCharacteristics.Count == 0)
            {
                distribution["Not Reported"]++;
            }
            else if (isEconDisadvantaged)
            {
                distribution["Economic Disadvantaged"]++;
            }
            else
            {
                distribution["Not Economic Disadvantaged"]++;
            }
        }

        context.Log($"Found {distribution["Economic Disadvantaged"]:N0} economically disadvantaged students out of {totalRecords:N0}");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Distribution(distribution, "Economic Status")
            ]
        };
    }
}
