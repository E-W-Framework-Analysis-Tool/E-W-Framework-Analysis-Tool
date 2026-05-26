using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class ChildWelfareInvolvementEdFiAssessor : IEdFiAssessor
{
    private static readonly string[] _welfareDescriptors =
    [
        "Foster Care",
        "Foster",
        "Child Welfare",
        "DCFS",
        "Child Protective Services",
        "Neglect"
    ];

    private readonly EdFiStudentDemographicsProvider _demographicsProvider;

    public ChildWelfareInvolvementEdFiAssessor(EdFiStudentDemographicsProvider demographicsProvider)
    {
        _demographicsProvider = demographicsProvider;
    }

    public string DataElementName => "Individual with current or past child welfare involvement";

    public string AssessmentDescription =>
        "Distribution of students by child welfare involvement status (foster care, CPS, " +
        "dependency) from studentEducationOrganizationAssociations characteristics.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading student demographics...");

        var data = await _demographicsProvider.GetDataAsync(httpClient, context);

        var distribution = new Dictionary<string, int>
        {
            ["Child Welfare Involved"] = 0,
            ["Not Child Welfare Involved"] = 0,
            ["Not Reported"] = 0
        };
        var totalRecords = data.Count;

        foreach (var association in data)
        {
            if (association.StudentCharacteristics == null || association.StudentCharacteristics.Count == 0)
            {
                distribution["Not Reported"]++;
                continue;
            }

            var isWelfareInvolved = association.StudentCharacteristics
                .Any(c =>
                {
                    var value = EdFiDescriptorHelper.ParseDescriptorValue(c.StudentCharacteristicDescriptor);
                    return _welfareDescriptors.Any(d => value.Contains(d, StringComparison.OrdinalIgnoreCase));
                });

            if (isWelfareInvolved)
                distribution["Child Welfare Involved"]++;
            else
                distribution["Not Child Welfare Involved"]++;
        }

        context.Log($"Found {distribution["Child Welfare Involved"]:N0} child welfare-involved students out of {totalRecords:N0}");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Distribution(distribution, "Child Welfare Status")
            ]
        };
    }
}
