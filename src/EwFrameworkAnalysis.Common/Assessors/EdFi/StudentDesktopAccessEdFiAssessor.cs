using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class StudentDesktopAccessEdFiAssessor : IEdFiAssessor
{
    private readonly EdFiStudentDemographicsProvider _demographicsProvider;

    public StudentDesktopAccessEdFiAssessor(EdFiStudentDemographicsProvider demographicsProvider)
    {
        _demographicsProvider = demographicsProvider;
    }

    public string DataElementName => "Indicator of access to desktop or laptop at home";

    public string AssessmentDescription =>
        "Distribution of students by primary learning device away from school from studentEducationOrganizationAssociations";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading student demographics...");

        var data = await _demographicsProvider.GetDataAsync(httpClient, context);

        var distribution = new Dictionary<string, int>();
        var totalRecords = data.Count;
        var reportedCount = 0;

        foreach (var association in data)
        {
            var descriptor = association.PrimaryLearningDeviceAwayFromSchoolDescriptor;

            if (string.IsNullOrWhiteSpace(descriptor))
            {
                var key = "Not Reported";
                distribution[key] = distribution.GetValueOrDefault(key) + 1;
            }
            else
            {
                reportedCount++;
                var value = EdFiDescriptorHelper.ParseDescriptorValue(descriptor);
                distribution[value] = distribution.GetValueOrDefault(value) + 1;
            }
        }

        context.Log($"Found {reportedCount:N0} students with device access reported out of {totalRecords:N0}");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Completeness(totalRecords, reportedCount, "PrimaryLearningDeviceAwayFromSchool"),
                new Distribution(distribution, "Primary Learning Device Away From School")
            ]
        };
    }
}
