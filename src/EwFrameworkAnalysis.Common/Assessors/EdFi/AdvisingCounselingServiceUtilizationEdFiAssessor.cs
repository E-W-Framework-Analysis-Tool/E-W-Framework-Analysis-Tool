using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class AdvisingCounselingServiceUtilizationEdFiAssessor : IEdFiAssessor
{
    private static readonly string[] _targetDescriptors =
    [
        "uri://ed-fi.org/InterventionClassDescriptor#Curriculum",
        "uri://ed-fi.org/InterventionClassDescriptor#Other",
        "uri://ed-fi.org/InterventionClassDescriptor#Practice",
        "uri://ed-fi.org/InterventionClassDescriptor#Supplement"
    ];

    public string DataElementName => "Advising and counseling service utilization";

    public string AssessmentDescription =>
        "Count and distribution of interventions broken down by InterventionClassDescriptor " +
        "(Curriculum, Other, Practice, Supplement) used as a proxy for advising and counseling " +
        "service utilization.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Counting interventions by class descriptor...");

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

        context.Log($"Found {totalMatching:N0} interventions across {_targetDescriptors.Length} class descriptors");
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
