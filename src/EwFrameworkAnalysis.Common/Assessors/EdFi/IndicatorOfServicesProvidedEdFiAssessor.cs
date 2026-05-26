using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class IndicatorOfServicesProvidedEdFiAssessor : IEdFiAssessor
{
    private readonly EdFiStudentInterventionProvider _interventionProvider;

    public IndicatorOfServicesProvidedEdFiAssessor(EdFiStudentInterventionProvider interventionProvider)
    {
        _interventionProvider = interventionProvider;
    }

    public string DataElementName => "Indicator of whether services were provided";

    public string AssessmentDescription =>
        "Counts the total student intervention associations as an indicator of whether " +
        "services were provided. The presence of records indicates services have been delivered.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading student intervention associations...");

        var associations = await _interventionProvider.GetAssociationsAsync(httpClient, context);

        var totalAssociations = associations.Count;
        var uniqueStudents = associations
            .Where(a => a.StudentReference != null)
            .Select(a => a.StudentReference!.StudentUniqueId)
            .Distinct()
            .Count();

        context.Log($"Found {totalAssociations:N0} intervention records for {uniqueStudents:N0} unique students");
        context.ReportProgress(100, "Complete");

        var distribution = new Dictionary<string, int>
        {
            ["Students With Services"] = uniqueStudents
        };

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalAssociations),
                new Distribution(distribution, "Service Status")
            ]
        };
    }
}
