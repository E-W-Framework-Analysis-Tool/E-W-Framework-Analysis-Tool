using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class HealthServicesOfferedEdFiAssessor : IEdFiAssessor
{
    private static readonly string[] _targetDescriptors =
    [
        "uri://ed-fi.org/InterventionClassDescriptor#Health",
        "uri://ed-fi.org/InterventionClassDescriptor#Medical",
        "uri://ed-fi.org/InterventionClassDescriptor#Nursing",
        "uri://ed-fi.org/InterventionClassDescriptor#Physical Health",
        "uri://ed-fi.org/InterventionClassDescriptor#Wellness"
    ];

    public string DataElementName => "Health services offered";

    public string AssessmentDescription =>
        "Count and distribution of interventions classified under physical health service " +
        "descriptors (Health, Medical, Nursing, Physical Health, Wellness).";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Counting interventions by health service class...");

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

        context.Log($"Found {totalMatching:N0} interventions across {_targetDescriptors.Length} health service classes");
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
