using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class StandardsAlignedCurriculaSurveysEdFiAssessor : IEdFiAssessor
{
    private const string TeacherCategoryDescriptor =
        "uri://ed-fi.org/SurveyCategoryDescriptor#Teacher";

    private static readonly string[] _surveyKeywords =
    [
        "standards-aligned",
        "standards aligned",
        "curricula alignment",
        "curriculum alignment",
        "culturally responsive",
        "curriculum implementation",
        "curriculum fidelity",
        "instructional materials",
        "curriculum survey",
        "standards-based curricula"
    ];

    public string DataElementName =>
        "Percentage of teachers regularly using standards-aligned; culturally responsive curricula";

    public string AssessmentDescription =>
        "Identifies teacher-category surveys linked to standards-aligned and culturally responsive " +
        "curricula usage by filtering on the Teacher survey category descriptor and matching " +
        "curriculum alignment and fidelity keywords in the Ed-Fi surveys catalog.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching teacher surveys for standards-aligned curricula instruments...");

        var totalAdministered = 0;
        var surveyDistribution = new Dictionary<string, int>();

        var queryParams = new Dictionary<string, string>
        {
            ["surveyCategoryDescriptor"] = TeacherCategoryDescriptor
        };

        await EdFiApiPatterns.PageAndProcessAsync<EdFiSurvey>(
            httpClient,
            "ed-fi/surveys",
            survey =>
            {
                if (!IsStandardsAlignedCurriculaSurvey(survey))
                    return;

                var administered = survey.NumberAdministered ?? 0;
                totalAdministered += administered;
                surveyDistribution[survey.SurveyIdentifier] = administered;
            },
            context,
            queryParams);

        context.Log($"Found {surveyDistribution.Count} standards-aligned curricula survey(s) " +
                     $"with {totalAdministered:N0} total administered");
        context.ReportProgress(100, "Complete");

        if (surveyDistribution.Count == 0)
        {
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No surveys matching recognized standards-aligned curricula instruments " +
                    "were found in the survey catalog."
            };
        }

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalAdministered),
                new Distribution(surveyDistribution, "Survey Instrument")
            ],
            Remarks = AssessmentDescription
        };
    }

    private static bool IsStandardsAlignedCurriculaSurvey(EdFiSurvey survey)
    {
        var title = survey.SurveyTitle ?? string.Empty;
        var identifier = survey.SurveyIdentifier ?? string.Empty;

        foreach (var keyword in _surveyKeywords)
        {
            if (title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                identifier.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
