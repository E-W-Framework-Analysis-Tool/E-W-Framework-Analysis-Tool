using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class APIBDualCreditCourseCreditsEdFiAssessor : IEdFiAssessor
{
    private readonly EdFiCourseProvider _courseProvider;

    public APIBDualCreditCourseCreditsEdFiAssessor(EdFiCourseProvider courseProvider)
    {
        _courseProvider = courseProvider;
    }

    public string DataElementName => "AP, IB, or Dual Credit course credits";

    public string AssessmentDescription =>
        "Analyzes AP, IB, and Dual Credit courses for available credit information";

    private static bool IsAPOrIBOrDualCredit(EdFiCourse course)
    {
        return course.LevelCharacteristics?.Any(lc =>
        {
            var desc = lc.CourseLevelCharacteristicDescriptor ?? "";
            return desc.Contains("Advanced Placement", StringComparison.OrdinalIgnoreCase)
                || desc.Contains("International Baccalaureate", StringComparison.OrdinalIgnoreCase)
                || desc.Contains("Dual Credit", StringComparison.OrdinalIgnoreCase);
        }) == true;
    }

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading courses...");

        var data = await _courseProvider.GetDataAsync(httpClient, context);

        var totalCourses = data.Count;
        var matchingCourses = 0;
        var withCredits = 0;

        foreach (var course in data)
        {
            if (IsAPOrIBOrDualCredit(course))
            {
                matchingCourses++;
                if (course.MaximumAvailableCredits.HasValue)
                    withCredits++;
            }
        }

        context.Log($"Found {matchingCourses:N0} AP/IB/Dual Credit courses, {withCredits:N0} with credit information");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalCourses),
                new Completeness(totalCourses, matchingCourses, "AP/IB/Dual Credit designation"),
                new Completeness(matchingCourses, withCredits, "MaximumAvailableCredits")
            ],
            Remarks = AssessmentDescription
        };
    }
}
