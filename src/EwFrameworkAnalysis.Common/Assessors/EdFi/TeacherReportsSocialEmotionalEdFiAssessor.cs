using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class TeacherReportsSocialEmotionalEdFiAssessor : IEdFiAssessor
{
    /// <summary>
    /// SurveyCategoryDescriptor values that indicate SEL / social-emotional teacher reports.
    /// Ed-Fi does not standardize a single category — districts map local frameworks
    /// (CASEL, DECA, SAEBRS, Panorama, Second Step) using local descriptors.
    /// We match the standard "Teacher" category plus common local patterns.
    /// </summary>
    private static readonly HashSet<string> _selSurveyCategoryKeywords =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "teacher",
            "sel",
            "social",
            "emotional",
            "socialemotional",
            "socialemotionallearning",
            "socialemotionaldevelopment",
            "schoolclimate",
            "casel",
            "deca",
            "saebrs",
            "panorama",
            "secondstep"
        };

    public string DataElementName => "Teacher reports of social-emotional development";

    public string AssessmentDescription =>
        "Queries the Ed-Fi Survey API (ed-fi/surveys and ed-fi/surveyResponses) to count surveys and " +
        "responses where the surveyCategoryDescriptor indicates teacher-reported SEL data. " +
        "Matches standard Ed-Fi 'Teacher' category and common SEL framework categories " +
        "(CASEL, DECA, SAEBRS, Panorama, Second Step).";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Querying Ed-Fi surveys...");

        // Step 1: Fetch all surveys and identify SEL teacher-report surveys
        var matchingSurveyKeys = new HashSet<string>();
        var surveyCategoryDistribution = new Dictionary<string, int>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiSurvey>(
            httpClient,
            "ed-fi/surveys",
            survey =>
            {
                var category = EdFiDescriptorHelper.ParseDescriptorValue(survey.SurveyCategoryDescriptor);
                var categoryNormalized = category.Replace(" ", "").Replace("-", "").Replace("_", "");

                if (_selSurveyCategoryKeywords.Any(kw =>
                    categoryNormalized.Contains(kw, StringComparison.OrdinalIgnoreCase)))
                {
                    var key = $"{survey.Namespace}|{survey.SurveyIdentifier}";
                    matchingSurveyKeys.Add(key);

                    if (!surveyCategoryDistribution.ContainsKey(category))
                        surveyCategoryDistribution[category] = 0;
                    surveyCategoryDistribution[category]++;
                }
            },
            context);

        context.Log($"Found {matchingSurveyKeys.Count} SEL-related survey definition(s)");

        if (matchingSurveyKeys.Count == 0)
        {
            context.ReportProgress(100, "Complete — no matching surveys found");

            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics =
                [
                    new RecordCount(0)
                ],
                Remarks = "No surveys found with SEL-related category descriptors (Teacher, SEL, " +
                           "Social-Emotional, CASEL, DECA, SAEBRS, Panorama, Second Step). " +
                           "Districts must map their SEL survey frameworks using local surveyCategoryDescriptor values."
            };
        }

        // Step 2: Count survey responses that belong to the matching surveys
        context.ReportProgress(40, "Counting survey responses...");

        var totalResponses = 0;
        var responsesWithStudentRef = 0;

        await EdFiApiPatterns.PageAndProcessAsync<EdFiSurveyResponse>(
            httpClient,
            "ed-fi/surveyResponses",
            response =>
            {
                var surveyRef = response.SurveyReference;
                if (surveyRef == null) return;

                var key = $"{surveyRef.Namespace}|{surveyRef.SurveyIdentifier}";
                if (!matchingSurveyKeys.Contains(key)) return;

                totalResponses++;
                if (response.StudentReference != null)
                    responsesWithStudentRef++;
            },
            context);

        context.Log($"Found {totalResponses:N0} survey responses for SEL surveys, " +
                     $"{responsesWithStudentRef:N0} linked to students");
        context.ReportProgress(100, "Complete");

        var characteristics = new List<DataCharacteristicBase>
        {
            new RecordCount(totalResponses),
            new Distribution(surveyCategoryDistribution, "Survey Category")
        };

        if (totalResponses > 0)
        {
            characteristics.Add(
                new Completeness(totalResponses, responsesWithStudentRef, "Student Reference"));
        }

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics = characteristics,
            Remarks = AssessmentDescription
        };
    }
}
