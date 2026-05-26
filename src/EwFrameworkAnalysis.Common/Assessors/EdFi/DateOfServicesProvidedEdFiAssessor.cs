using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class DateOfServicesProvidedEdFiAssessor : IEdFiAssessor
{
    private readonly EdFiStudentInterventionProvider _interventionProvider;

    public DateOfServicesProvidedEdFiAssessor(EdFiStudentInterventionProvider interventionProvider)
    {
        _interventionProvider = interventionProvider;
    }

    public string DataElementName => "Date of services provided";

    public string AssessmentDescription =>
        "Evaluates the availability and distribution of service dates across student " +
        "intervention associations by examining linked intervention begin dates.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading student intervention associations...");

        var associations = await _interventionProvider.GetAssociationsAsync(httpClient, context);
        var interventionLookup = await _interventionProvider.GetInterventionLookupAsync(httpClient, context);

        context.ReportProgress(50, "Analyzing service dates...");

        var totalAssociations = associations.Count;
        var withDates = 0;
        var yearDistribution = new Dictionary<string, int>();

        foreach (var association in associations)
        {
            var reference = association.InterventionReference;
            var key = EdFiStudentInterventionProvider.BuildKey(
                reference?.EducationOrganizationId, reference?.InterventionIdentificationCode);
            if (!interventionLookup.TryGetValue(key, out var intervention))
                continue;

            // BeginDate is non-nullable; filter out uninitialized 0001-01-01 sentinel values.
            if (intervention.BeginDate.Year > 1)
            {
                withDates++;
                var year = intervention.BeginDate.Year.ToString();
                yearDistribution[year] = yearDistribution.GetValueOrDefault(year) + 1;
            }
        }

        context.Log($"Found {withDates:N0} intervention records with dates out of {totalAssociations:N0} total");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalAssociations),
                new Completeness(totalAssociations, withDates, "InterventionBeginDate"),
                new Distribution(yearDistribution, "Service Year")
            ]
        };
    }
}
