using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class SocialServicesOfferedEdFiAssessor : IEdFiAssessor
{
    private static readonly string[] _targetDescriptors =
    [
        "uri://ed-fi.org/InterventionClassDescriptor#Social",
        "uri://ed-fi.org/InterventionClassDescriptor#Social Services",
        "uri://ed-fi.org/InterventionClassDescriptor#Family Services",
        "uri://ed-fi.org/InterventionClassDescriptor#Social Work",
        "uri://ed-fi.org/InterventionClassDescriptor#Case Management"
    ];

    public string DataElementName => "Social services offered";

    public string AssessmentDescription =>
        "Count and distribution of interventions classified under social service descriptors " +
        "(Social, Social Services, Family Services, Social Work, Case Management).";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Counting interventions by social service class...");

        var totalMatching = 0;
        var distribution = new Dictionary<string, int>();

        for (var i = 0; i < _targetDescriptors.Length; i++)
        {
            var descriptor = _targetDescriptors[i];
            var label = EdFiDescriptorHelper.ParseDescriptorValue(descriptor);

            var count = await EdFiApiPatterns.CountFromHeaderAsync(
                httpClient,
                "ed-fi/interventions",
                new Dictionary<string, string>
                {
                    ["interventionClassDescriptor"] = descriptor
                });

            distribution[label] = count;
            totalMatching += count;

            var progress = (int)(((i + 1) * 100.0) / _targetDescriptors.Length);
            context.ReportProgress(progress, $"Counted {label}: {count:N0}");
        }

        context.Log($"Found {totalMatching:N0} interventions across {_targetDescriptors.Length} social service classes");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalMatching),
                new Distribution(distribution, "Intervention Class")
            ]
        };
    }
}
