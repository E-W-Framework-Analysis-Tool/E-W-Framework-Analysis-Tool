using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class FamilyEngagementSurveysK12EdFiAssessor : IEdFiAssessor
{
    private const string ParentCategoryDescriptor =
        "uri://ed-fi.org/SurveyCategoryDescriptor#Parent";

    private static readonly string[] _surveyKeywords =
    [
        "family engagement",
        "parent engagement",
        "family involvement",
        "parent involvement",
        "family-school partnership",
        "family school partnership",
        "home-school",
        "caregiver survey",
        "family survey",
        "parent survey",
        "parent satisfaction"
    ];

    public string DataElementName => "Family engagement surveys (K-12)";

    public string AssessmentDescription =>
        "Identifies parent-category surveys linked to K-12 family engagement instruments by " +
        "filtering on the Parent survey category descriptor and matching family engagement, " +
        "parent involvement, and family-school partnership keywords in the Ed-Fi surveys catalog.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching parent surveys for K-12 family engagement instruments...");

        var totalAdministered = 0;
        var surveyDistribution = new Dictionary<string, int>();

        var queryParams = new Dictionary<string, string>
        {
            ["surveyCategoryDescriptor"] = ParentCategoryDescriptor
        };

        await EdFiApiPatterns.PageAndProcessAsync<EdFiSurvey>(
            httpClient,
            "ed-fi/surveys",
            survey =>
            {
                if (!IsFamilyEngagementSurvey(survey))
                    return;

                var administered = survey.NumberAdministered ?? 0;
                totalAdministered += administered;
                surveyDistribution[survey.SurveyIdentifier] = administered;
            },
            context,
            queryParams);

        context.Log($"Found {surveyDistribution.Count} K-12 family engagement survey(s) " +
                     $"with {totalAdministered:N0} total administered");
        context.ReportProgress(100, "Complete");

        if (surveyDistribution.Count == 0)
        {
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No surveys matching recognized K-12 family engagement instruments were found " +
                    "in the survey catalog."
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

    private static bool IsFamilyEngagementSurvey(EdFiSurvey survey)
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
