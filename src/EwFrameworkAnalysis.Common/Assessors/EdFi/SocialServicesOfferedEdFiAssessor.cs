using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class SocialServicesOfferedEdFiAssessor : IEdFiAssessor
{
    private static readonly string[] _matchKeywords =
    [
        "Social",
        "Social Services",
        "Family Services",
        "Social Work",
        "Case Management"
    ];

    private readonly EdFiStudentInterventionProvider _interventionProvider;

    public SocialServicesOfferedEdFiAssessor(EdFiStudentInterventionProvider interventionProvider)
    {
        _interventionProvider = interventionProvider;
    }

    public string DataElementName => "Social services offered";

    public string AssessmentDescription =>
        "Count and distribution of student intervention associations linked to social services " +
        "by matching intervention class descriptors and identification codes against social " +
        "services keywords.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading student intervention associations...");

        var associations = await _interventionProvider.GetAssociationsAsync(httpClient, context);
        var interventionLookup = await _interventionProvider.GetInterventionLookupAsync(httpClient, context);

        context.ReportProgress(50, "Filtering for social services interventions...");

        var matchCount = 0;
        var distribution = new Dictionary<string, int>();

        foreach (var association in associations)
        {
            var reference = association.InterventionReference;
            var key = EdFiStudentInterventionProvider.BuildKey(
                reference?.EducationOrganizationId, reference?.InterventionIdentificationCode);
            if (!interventionLookup.TryGetValue(key, out var intervention))
                continue;

            if (!IsSocialServicesIntervention(intervention))
                continue;

            matchCount++;
            var label = intervention.InterventionIdentificationCode ?? "Unknown";
            distribution[label] = distribution.GetValueOrDefault(label) + 1;
        }

        context.Log($"Found {matchCount:N0} social services records out of {associations.Count:N0} total");
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

    private static bool IsSocialServicesIntervention(EdFiIntervention intervention)
    {
        var classDescriptor = EdFiDescriptorHelper.ParseDescriptorValue(intervention.InterventionClassDescriptor);
        var identificationCode = intervention.InterventionIdentificationCode ?? string.Empty;

        foreach (var keyword in _matchKeywords)
        {
            if (classDescriptor.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                identificationCode.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
