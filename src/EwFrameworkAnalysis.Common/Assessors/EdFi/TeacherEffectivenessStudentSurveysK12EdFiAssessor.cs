using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class TeacherEffectivenessStudentSurveysK12EdFiAssessor : IEdFiAssessor
{
    private const string StudentCategoryDescriptor =
        "uri://ed-fi.org/SurveyCategoryDescriptor#Student";

    private static readonly string[] _surveyKeywords =
    [
        "teacher effectiveness",
        "teaching effectiveness",
        "student perception",
        "student perception of teaching",

        "Danielson",
        "Marzano",
        "TNTP",
        "Tripod",
        "MET Project",
        "Measures of Effective Teaching",
        "student voice",
        "instructional practice"
    ];

    public string DataElementName => "Teacher effectiveness student surveys (K-12)";

    public string AssessmentDescription =>
        "Identifies student-category surveys linked to K-12 teacher effectiveness perception " +
        "instruments (e.g., Tripod, TNTP Insight, Danielson-aligned student surveys, MET Project " +
        "instruments) by filtering on the Student survey category descriptor and matching " +
        "well-known instrument names and teacher effectiveness keywords in the Ed-Fi surveys catalog.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching student surveys for K-12 teacher effectiveness instruments...");

        var totalAdministered = 0;
        var surveyDistribution = new Dictionary<string, int>();

        var queryParams = new Dictionary<string, string>
        {
            ["surveyCategoryDescriptor"] = StudentCategoryDescriptor
        };

        await EdFiApiPatterns.PageAndProcessAsync<EdFiSurvey>(
            httpClient,
            "ed-fi/surveys",
            survey =>
            {
                if (!IsTeacherEffectivenessSurvey(survey))
                    return;

                var administered = survey.NumberAdministered ?? 0;
                totalAdministered += administered;
                surveyDistribution[survey.SurveyIdentifier] = administered;
            },
            context,
            queryParams);

        context.Log($"Found {surveyDistribution.Count} K-12 teacher effectiveness student survey(s) " +
                     $"with {totalAdministered:N0} total administered");
        context.ReportProgress(100, "Complete");

        if (surveyDistribution.Count == 0)
        {
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No surveys matching recognized K-12 teacher effectiveness student perception " +
                    "instruments were found in the survey catalog."
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

    private static bool IsTeacherEffectivenessSurvey(EdFiSurvey survey)
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
