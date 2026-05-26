using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class DateOfServicesProvidedEdFiAssessor : IEdFiAssessor
{
    private static readonly string[] _targetDescriptors =
    [
        "uri://ed-fi.org/InterventionClassDescriptor#Curriculum",
        "uri://ed-fi.org/InterventionClassDescriptor#Other",
        "uri://ed-fi.org/InterventionClassDescriptor#Practice",
        "uri://ed-fi.org/InterventionClassDescriptor#Supplement"
    ];

    public string DataElementName => "Date of services provided";

    public string AssessmentDescription =>
        "Evaluates the availability and distribution of service dates by paging interventions " +
        "from the Ed-Fi interventions endpoint filtered by InterventionClassDescriptor " +
        "(Curriculum, Other, Practice, Supplement) and examining each intervention's BeginDate.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading interventions by class descriptor...");

        var totalInterventions = 0;
        var withDates = 0;
        var yearDistribution = new Dictionary<string, int>();

        for (var i = 0; i < _targetDescriptors.Length; i++)
        {
            var descriptor = _targetDescriptors[i];
            var label = EdFiDescriptorHelper.ParseDescriptorValue(descriptor);

            await EdFiApiPatterns.PageAndProcessAsync<EdFiIntervention>(
                httpClient,
                "ed-fi/interventions",
                intervention =>
                {
                    totalInterventions++;

                    // BeginDate is non-nullable; filter out uninitialized 0001-01-01 sentinel values.
                    if (intervention.BeginDate.Year > 1)
                    {
                        withDates++;
                        var year = intervention.BeginDate.Year.ToString();
                        yearDistribution[year] = yearDistribution.GetValueOrDefault(year) + 1;
                    }
                },
                context,
                queryParams: new Dictionary<string, string>
                {
                    ["interventionClassDescriptor"] = descriptor
                });

            var progress = (int)(((i + 1) * 100.0) / _targetDescriptors.Length);
            context.ReportProgress(progress, $"Processed {label}");
        }

        context.Log($"Found {withDates:N0} interventions with dates out of {totalInterventions:N0} total");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalInterventions),
                new Completeness(totalInterventions, withDates, "InterventionBeginDate"),
                new Distribution(yearDistribution, "Service Year")
            ]
        };
    }
}
