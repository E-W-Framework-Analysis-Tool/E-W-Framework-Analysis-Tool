using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Assessors.EdFi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

// Early grades on track
public class EarlyLearningAssessmentsEdFiAssessor : IEdFiAssessor
{
    private static readonly HashSet<string> _earlyLearningCategories =
    [
        "uri://ed-fi.org/AssessmentCategoryDescriptor#Early Learning - Approaches toward learning",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#Early Learning - Cognition and general knowledge",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#Early Learning - Language and literacy development",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#Early Learning - Physical well-being and motor dev",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#Early Learning - Social and emotional development"
    ];
    public string DataElementName => "Early Learning";

    public string AssessmentDescription =>
        "Count of students with early learning results";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Finding Early Learning assessments...");

        var assessmentIdentifiers = new HashSet<string>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                var isEarlyLearning = _earlyLearningCategories.Contains(assessment.AssessmentCategoryDescriptor ?? "");

                if (isEarlyLearning)
                {
                    assessmentIdentifiers.Add(assessment.AssessmentIdentifier);
                    context.Log($"Found Early Learning assessment: {assessment.AssessmentIdentifier}");
                }
            },
            context
        );

        context.Log($"Found {assessmentIdentifiers.Count} Early Learning assessments");

        if (assessmentIdentifiers.Count == 0)
        {
            context.Log("No matching Early Learning assessments found.");
            context.ReportProgress(100, "Complete");

            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)]
            };
        }

        context.ReportProgress(50, "Counting student assessment records...");

        var totalCount = 0;
        foreach (var assessmentId in assessmentIdentifiers)
        {
            var count = await EdFiApiPatterns.CountFromHeaderAsync(
                httpClient,
                "ed-fi/studentAssessments",
                new Dictionary<string, string>
                {
                    ["assessmentIdentifier"] = assessmentId
                }
            );
            totalCount += count;
        }

        context.Log($"Found {totalCount:N0} student Early Learning assessment records");

        // Step 3: Collect distribution by assessment category
        context.ReportProgress(75, "Collecting distribution by assessment category...");
        var categoryDistribution = new Dictionary<string, int>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (assessmentIdentifiers.Contains(assessment.AssessmentIdentifier))
                {
                    var category = assessment.AssessmentCategoryDescriptor ?? "Unknown";
                    var categoryName = category.Split('#').LastOrDefault() ?? category;

                    if (!categoryDistribution.ContainsKey(categoryName))
                    {
                        categoryDistribution[categoryName] = 0;
                    }
                    categoryDistribution[categoryName]++;
                }
            },
            context
        );

        context.Log("Assessment category distribution:");
        foreach (var kvp in categoryDistribution.OrderByDescending(x => x.Value))
        {
            context.Log($"  {kvp.Key}: {kvp.Value} assessment(s)");
        }

        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics = [
                new RecordCount(totalCount),
                new Distribution(categoryDistribution, "Assessment Category")
            ]
        };
    }
}
