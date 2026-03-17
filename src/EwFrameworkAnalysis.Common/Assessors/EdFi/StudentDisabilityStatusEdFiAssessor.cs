using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class StudentDisabilityStatusEdFiAssessor : IEdFiAssessor
{
    private readonly EdFiStudentDemographicsProvider _demographicsProvider;

    public StudentDisabilityStatusEdFiAssessor(EdFiStudentDemographicsProvider demographicsProvider)
    {
        _demographicsProvider = demographicsProvider;
    }

    public string DataElementName => "Disability status";

    public string AssessmentDescription =>
        "Distribution of students by disability status from studentEducationOrganizationAssociations";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading student demographics...");

        var data = await _demographicsProvider.GetDataAsync(httpClient, context);

        var distribution = new Dictionary<string, int>
        {
            ["Has Disability"] = 0,
            ["No Disability"] = 0
        };
        var disabilityTypeDistribution = new Dictionary<string, int>();
        var totalRecords = data.Count;

        foreach (var association in data)
        {
            if (association.Disabilities != null && association.Disabilities.Count > 0)
            {
                distribution["Has Disability"]++;
                foreach (var disability in association.Disabilities)
                {
                    var disabilityValue = EdFiDescriptorHelper.ParseDescriptorValue(disability.DisabilityDescriptor);
                    if (!disabilityTypeDistribution.ContainsKey(disabilityValue))
                        disabilityTypeDistribution[disabilityValue] = 0;
                    disabilityTypeDistribution[disabilityValue]++;
                }
            }
            else
            {
                distribution["No Disability"]++;
            }
        }

        context.Log($"Found {distribution["Has Disability"]:N0} of {totalRecords:N0} records with disability data");
        context.ReportProgress(100, "Complete");

        var characteristics = new List<DataCharacteristicBase>
        {
            new RecordCount(totalRecords),
            new Distribution(distribution, "Disability Status")
        };

        if (disabilityTypeDistribution.Count > 0)
            characteristics.Add(new Distribution(disabilityTypeDistribution, "Disability Type"));

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics = characteristics
        };
    }
}
