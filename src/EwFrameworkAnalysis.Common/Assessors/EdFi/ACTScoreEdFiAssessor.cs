using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class ACTScoreEdFiAssessor : IEdFiAssessor
{
    private static readonly HashSet<string> _actCategories =
    [
        "uri://ed-fi.org/AssessmentCategoryDescriptor#College entrance exam"
    ];

    public string DataElementName => "ACT score";

    public string AssessmentDescription =>
        "Count of students with ACT score results";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Finding ACT assessments...");

        var assessmentIdentifiers = new HashSet<string>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                var isAct = _actCategories.Contains(assessment.AssessmentCategoryDescriptor ?? "")
                    || (assessment.AssessmentTitle ?? "").Contains("ACT", StringComparison.OrdinalIgnoreCase);

                if (isAct)
                    assessmentIdentifiers.Add(assessment.AssessmentIdentifier);
            },
            context
        );

        if (assessmentIdentifiers.Count == 0)
        {
            context.Log("No ACT assessments found.");
            context.ReportProgress(100, "Complete");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)]
            };
        }

        context.ReportProgress(50, "Counting student ACT score records...");

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

        context.Log($"Found {totalCount:N0} student ACT score records");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics = [new RecordCount(totalCount)]
        };
    }
}
