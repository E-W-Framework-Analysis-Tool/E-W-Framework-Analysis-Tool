using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class EndOfCourseExamParticipationEdFiAssessor : IEdFiAssessor
{
    private static readonly HashSet<string> _endOfCourseCategories =
    [
        "uri://ed-fi.org/AssessmentCategoryDescriptor#State high school course assessment",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#State high school subject assessment",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#Advanced Placement",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#International Baccalaureate"
    ];

    private static readonly string[] _titleKeywords =
    [
        "end-of-course",
        "end of course",
        "EOC",
        "Advanced Placement",
        "International Baccalaureate"
    ];

    public string DataElementName => "End-of-course exam participation";

    public string AssessmentDescription =>
        "Counts students who participated in an end-of-course exam — such as a state high school " +
        "course/subject assessment, or an AP or IB qualifying exam — by matching the assessment catalog " +
        "on assessmentCategoryDescriptor (State high school course assessment, State high school subject " +
        "assessment, Advanced Placement, International Baccalaureate) and end-of-course title keywords, " +
        "then counting student assessment records for the matched assessments.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Finding end-of-course exams...");

        var matchingAssessments = new List<(string Identifier, string Namespace, string Category)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (IsEndOfCourseExam(assessment))
                {
                    matchingAssessments.Add((
                        assessment.AssessmentIdentifier,
                        assessment.Namespace,
                        EdFiDescriptorHelper.ParseDescriptorValue(assessment.AssessmentCategoryDescriptor)));
                }
            },
            context);

        context.Log($"Found {matchingAssessments.Count} end-of-course exam(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no end-of-course exams found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching end-of-course exam categories or keywords were found " +
                    "in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Counting student end-of-course exam records...");

        var totalCount = 0;
        var categoryDistribution = new Dictionary<string, int>();

        foreach (var (identifier, ns, category) in matchingAssessments)
        {
            var count = await EdFiApiPatterns.CountFromHeaderAsync(
                httpClient,
                "ed-fi/studentAssessments",
                new Dictionary<string, string>
                {
                    ["assessmentIdentifier"] = identifier,
                    ["namespace"] = ns
                });

            totalCount += count;
            if (count > 0)
            {
                var label = string.IsNullOrWhiteSpace(category) ? "Unknown" : category;
                categoryDistribution[label] = categoryDistribution.GetValueOrDefault(label) + count;
            }
        }

        context.Log($"Found {totalCount:N0} student end-of-course exam records");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalCount),
                new Distribution(categoryDistribution, "Assessment Category")
            ],
            Remarks = AssessmentDescription
        };
    }

    private static bool IsEndOfCourseExam(EdFiAssessment assessment)
    {
        if (_endOfCourseCategories.Contains(assessment.AssessmentCategoryDescriptor ?? ""))
            return true;

        var title = assessment.AssessmentTitle ?? string.Empty;
        var identifier = assessment.AssessmentIdentifier ?? string.Empty;

        foreach (var keyword in _titleKeywords)
        {
            if (title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                identifier.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
