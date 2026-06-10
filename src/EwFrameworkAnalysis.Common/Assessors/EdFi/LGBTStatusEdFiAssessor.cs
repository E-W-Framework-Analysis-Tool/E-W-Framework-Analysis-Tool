using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class LGBTStatusEdFiAssessor : IEdFiAssessor
{
    private static readonly string[] _lgbtDescriptors =
    [
        "LGBT",
        "LGBTQ",
        "Sexual Orientation",
        "Non-Binary",
        "Transgender"
    ];

    private readonly EdFiStudentDemographicsProvider _demographicsProvider;

    public LGBTStatusEdFiAssessor(EdFiStudentDemographicsProvider demographicsProvider)
    {
        _demographicsProvider = demographicsProvider;
    }

    public string DataElementName => "LGBT status";

    public string AssessmentDescription =>
        "Distribution of students by LGBT-related characteristics from " +
        "studentEducationOrganizationAssociations. Ed-Fi does not define a standard " +
        "descriptor for this; districts using custom descriptors for sexual orientation " +
        "or gender identity will be detected.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading student demographics...");

        var data = await _demographicsProvider.GetDataAsync(httpClient, context);

        var distribution = new Dictionary<string, int>
        {
            ["LGBT Identified"] = 0,
            ["Not LGBT Identified"] = 0,
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

            var isLgbt = association.StudentCharacteristics
                .Any(c =>
                {
                    var value = EdFiDescriptorHelper.ParseDescriptorValue(c.StudentCharacteristicDescriptor);
                    return _lgbtDescriptors.Any(d => value.Contains(d, StringComparison.OrdinalIgnoreCase));
                });

            if (isLgbt)
                distribution["LGBT Identified"]++;
            else
                distribution["Not LGBT Identified"]++;
        }

        context.Log($"Found {distribution["LGBT Identified"]:N0} LGBT-identified students out of {totalRecords:N0}");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Distribution(distribution, "LGBT Status")
            ]
        };
    }
}
