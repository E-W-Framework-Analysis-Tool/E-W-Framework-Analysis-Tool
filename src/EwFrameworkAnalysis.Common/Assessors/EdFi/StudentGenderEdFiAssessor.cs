using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class StudentGenderEdFiAssessor : IEdFiAssessor
{
    private readonly EdFiStudentDemographicsProvider _demographicsProvider;

    public StudentGenderEdFiAssessor(EdFiStudentDemographicsProvider demographicsProvider)
    {
        _demographicsProvider = demographicsProvider;
    }

    public string DataElementName => "Gender";

    public string AssessmentDescription =>
        "Distribution of students by sex/gender from studentEducationOrganizationAssociations";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading student demographics...");

        var data = await _demographicsProvider.GetDataAsync(httpClient, context);

        var distribution = new Dictionary<string, int>();
        var totalRecords = data.Count;
        var recordsWithGender = 0;

        foreach (var association in data)
        {
            var sexValue = EdFiDescriptorHelper.ParseDescriptorValue(association.SexDescriptor);
            if (sexValue != "Unknown")
                recordsWithGender++;

            if (!distribution.ContainsKey(sexValue))
                distribution[sexValue] = 0;
            distribution[sexValue]++;
        }

        context.Log($"Found {recordsWithGender:N0} of {totalRecords:N0} records with gender data");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Distribution(distribution, "Gender"),
                new Completeness(totalRecords, recordsWithGender, "SexDescriptor")
            ]
        };
    }
}
