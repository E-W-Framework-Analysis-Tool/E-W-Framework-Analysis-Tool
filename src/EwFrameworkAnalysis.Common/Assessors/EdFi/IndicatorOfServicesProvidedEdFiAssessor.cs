using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class IndicatorOfServicesProvidedEdFiAssessor : IEdFiAssessor
{
    private static readonly string[] _targetDescriptors =
    [
        "uri://ed-fi.org/InterventionClassDescriptor#Curriculum",
        "uri://ed-fi.org/InterventionClassDescriptor#Other",
        "uri://ed-fi.org/InterventionClassDescriptor#Practice",
        "uri://ed-fi.org/InterventionClassDescriptor#Supplement"
    ];

    public string DataElementName => "Indicator of whether services were provided";

    public string AssessmentDescription =>
        "Indicates whether services were provided by checking each catalog intervention " +
        "(filtered by InterventionClassDescriptor — Curriculum, Other, Practice, Supplement) " +
        "for at least one student intervention association. An intervention with one or more " +
        "associations is considered as having services provided.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading interventions by class descriptor...");

        var totalInterventions = 0;
        var interventionsWithServices = 0;
        var distribution = new Dictionary<string, int>();

        for (var i = 0; i < _targetDescriptors.Length; i++)
        {
            var descriptor = _targetDescriptors[i];
            var label = EdFiDescriptorHelper.ParseDescriptorValue(descriptor);
            var classProvidedCount = 0;

            var interventionsInClass = new List<(long? EdOrgId, string? Code)>();

            await EdFiApiPatterns.PageAndProcessAsync<EdFiIntervention>(
                httpClient,
                "ed-fi/interventions",
                intervention =>
                {
                    totalInterventions++;
                    interventionsInClass.Add((
                        intervention.EducationOrganizationReference?.EducationOrganizationId,
                        intervention.InterventionIdentificationCode));
                },
                context,
                queryParams: new Dictionary<string, string>
                {
                    ["interventionClassDescriptor"] = descriptor
                });

            foreach (var (edOrgId, code) in interventionsInClass)
            {
                if (await HasAnyAssociationAsync(httpClient, edOrgId, code))
                {
                    interventionsWithServices++;
                    classProvidedCount++;
                }
            }

            distribution[label] = classProvidedCount;

            var progress = (int)(((i + 1) * 100.0) / _targetDescriptors.Length);
            context.ReportProgress(progress, $"{label}: {classProvidedCount:N0} interventions with services");
        }

        context.Log($"Services provided for {interventionsWithServices:N0} of {totalInterventions:N0} interventions");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(interventionsWithServices),
                new Distribution(distribution, "Intervention Class")
            ]
        };
    }

    private static async Task<bool> HasAnyAssociationAsync(
        HttpClient httpClient,
        long? educationOrganizationId,
        string? interventionIdentificationCode,
        CancellationToken ct = default)
    {
        if (educationOrganizationId is null || string.IsNullOrWhiteSpace(interventionIdentificationCode))
            return false;

        var count = await EdFiApiPatterns.CountFromHeaderAsync(
            httpClient,
            "ed-fi/studentInterventionAssociations",
            new Dictionary<string, string>
            {
                ["educationOrganizationId"] = educationOrganizationId.Value.ToString(),
                ["interventionIdentificationCode"] = interventionIdentificationCode
            },
            ct);

        return count > 0;
    }
}
