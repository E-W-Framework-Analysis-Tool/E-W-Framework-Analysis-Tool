using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class StudentInternetAccessEdFiAssessor : IEdFiAssessor
{
    private readonly EdFiStudentDemographicsProvider _demographicsProvider;

    public StudentInternetAccessEdFiAssessor(EdFiStudentDemographicsProvider demographicsProvider)
    {
        _demographicsProvider = demographicsProvider;
    }

    public string DataElementName => "Indicator of access to reliable broadband internet";

    public string AssessmentDescription =>
        "Distribution of students by internet access in residence from studentEducationOrganizationAssociations";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading student demographics...");

        var data = await _demographicsProvider.GetDataAsync(httpClient, context);

        var distribution = new Dictionary<string, int>
        {
            ["Has Internet Access"] = 0,
            ["No Internet Access"] = 0,
            ["Not Reported"] = 0
        };
        var totalRecords = data.Count;

        foreach (var association in data)
        {
            if (association.InternetAccessInResidence == null)
            {
                distribution["Not Reported"]++;
            }
            else if (association.InternetAccessInResidence == true)
            {
                distribution["Has Internet Access"]++;
            }
            else
            {
                distribution["No Internet Access"]++;
            }
        }

        var reported = distribution["Has Internet Access"] + distribution["No Internet Access"];
        context.Log($"Found {distribution["Has Internet Access"]:N0} students with internet access out of {reported:N0} reported ({totalRecords:N0} total)");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Completeness(totalRecords, reported, "InternetAccessInResidence"),
                new Distribution(distribution, "Internet Access in Residence")
            ]
        };
    }
}
