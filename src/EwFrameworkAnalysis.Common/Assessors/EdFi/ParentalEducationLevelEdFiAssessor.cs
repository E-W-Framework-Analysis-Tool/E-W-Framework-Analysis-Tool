using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class ParentalEducationLevelEdFiAssessor : IEdFiAssessor
{
    private static readonly string[] _targetDescriptors =
    [
        "uri://ed-fi.org/LevelOfEducationDescriptor#Associate's Degree (two years or more)",
        "uri://ed-fi.org/LevelOfEducationDescriptor#Bachelor's",
        "uri://ed-fi.org/LevelOfEducationDescriptor#Did Not Graduate High School",
        "uri://ed-fi.org/LevelOfEducationDescriptor#Doctorate",
        "uri://ed-fi.org/LevelOfEducationDescriptor#High School Diploma",
        "uri://ed-fi.org/LevelOfEducationDescriptor#Master's",
        "uri://ed-fi.org/LevelOfEducationDescriptor#Some College No Degree"
    ];

    public string DataElementName => "Parental education level";

    public string AssessmentDescription =>
        "Count of contacts (parents/guardians) whose HighestCompletedLevelOfEducationDescriptor " +
        "matches one of the recognized E-W Framework education level values.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Counting contacts by education level...");

        var totalMatching = 0;
        var distribution = new Dictionary<string, int>();

        for (var i = 0; i < _targetDescriptors.Length; i++)
        {
            var descriptor = _targetDescriptors[i];
            var label = EdFiDescriptorHelper.ParseDescriptorValue(descriptor);

            var count = await EdFiApiPatterns.CountFromHeaderAsync(
                httpClient,
                "ed-fi/contacts",
                new Dictionary<string, string>
                {
                    ["highestCompletedLevelOfEducationDescriptor"] = descriptor
                });

            distribution[label] = count;
            totalMatching += count;

            var progress = (int)(((i + 1) * 100.0) / _targetDescriptors.Length);
            context.ReportProgress(progress, $"Counted {label}: {count:N0}");
        }

        context.Log($"Found {totalMatching:N0} contacts across {_targetDescriptors.Length} recognized education levels");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalMatching),
                new Distribution(distribution, "Education Level")
            ]
        };
    }
}
