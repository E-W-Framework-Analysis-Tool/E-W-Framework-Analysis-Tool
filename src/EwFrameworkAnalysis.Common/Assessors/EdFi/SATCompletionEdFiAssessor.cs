using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class SATCompletionEdFiAssessor : IEdFiAssessor
{
    private static readonly HashSet<string> _satCategories =
    [
        "uri://ed-fi.org/AssessmentCategoryDescriptor#College entrance exam"
    ];

    public string DataElementName => "SAT completion";

    public string AssessmentDescription =>
        "Count of students with SAT assessment records";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Finding SAT assessments...");

        var assessmentIdentifiers = new HashSet<string>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                var isSat = _satCategories.Contains(assessment.AssessmentCategoryDescriptor ?? "")
                    || (assessment.AssessmentTitle ?? "").Contains("SAT", StringComparison.OrdinalIgnoreCase);

                if (isSat)
                {
                    assessmentIdentifiers.Add(assessment.AssessmentIdentifier);
                    context.Log($"Found SAT assessment: {assessment.AssessmentIdentifier}");
                }
            },
            context
        );

        if (assessmentIdentifiers.Count == 0)
        {
            context.Log("No SAT assessments found.");
            context.ReportProgress(100, "Complete");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)]
            };
        }

        context.ReportProgress(50, "Counting student SAT records...");

        var totalCount = 0;
        foreach (var assessmentId in assessmentIdentifiers)
        {
            var count = await EdFiApiPatterns.CountFromHeaderAsync(
                httpClient,
                "ed-fi/studentAssessments",
                new Dictionary<string, string>
                {
                    ["assessmentIdentifier"] = assessmentId
                });
            totalCount += count;
        }

        context.Log($"Found {totalCount:N0} student SAT assessment records");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics = [new RecordCount(totalCount)]
        };
    }
}
