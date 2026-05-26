using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class HealthServicesOfferedEdFiAssessor : IEdFiAssessor
{
    private static readonly string[] _matchKeywords =
    [
        "Health",
        "Medical",
        "Nursing",
        "Physical Health",
        "Wellness"
    ];

    private static readonly string[] _excludeKeywords =
    [
        "Mental Health",
        "Behavioral Health"
    ];

    private readonly EdFiStudentInterventionProvider _interventionProvider;

    public HealthServicesOfferedEdFiAssessor(EdFiStudentInterventionProvider interventionProvider)
    {
        _interventionProvider = interventionProvider;
    }

    public string DataElementName => "Health services offered";

    public string AssessmentDescription =>
        "Count and distribution of student intervention associations linked to physical " +
        "health services by matching intervention class descriptors and identification codes " +
        "against health/medical keywords (excluding mental health services).";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading student intervention associations...");

        var associations = await _interventionProvider.GetAssociationsAsync(httpClient, context);
        var interventionLookup = await _interventionProvider.GetInterventionLookupAsync(httpClient, context);

        context.ReportProgress(50, "Filtering for health services interventions...");

        var matchCount = 0;
        var distribution = new Dictionary<string, int>();

        foreach (var association in associations)
        {
            var reference = association.InterventionReference;
            var key = EdFiStudentInterventionProvider.BuildKey(
                reference?.EducationOrganizationId, reference?.InterventionIdentificationCode);
            if (!interventionLookup.TryGetValue(key, out var intervention))
                continue;

            if (!IsHealthServicesIntervention(intervention))
                continue;

            matchCount++;
            var label = intervention.InterventionIdentificationCode ?? "Unknown";
            distribution[label] = distribution.GetValueOrDefault(label) + 1;
        }

        context.Log($"Found {matchCount:N0} health services records out of {associations.Count:N0} total");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(matchCount),
                new Distribution(distribution, "Intervention")
            ]
        };
    }

    private static bool IsHealthServicesIntervention(EdFiIntervention intervention)
    {
        var classDescriptor = EdFiDescriptorHelper.ParseDescriptorValue(intervention.InterventionClassDescriptor);
        var identificationCode = intervention.InterventionIdentificationCode ?? string.Empty;
        var combined = $"{classDescriptor} {identificationCode}";

        foreach (var exclude in _excludeKeywords)
        {
            if (combined.Contains(exclude, StringComparison.OrdinalIgnoreCase))
                return false;
        }

        foreach (var keyword in _matchKeywords)
        {
            if (classDescriptor.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                identificationCode.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
