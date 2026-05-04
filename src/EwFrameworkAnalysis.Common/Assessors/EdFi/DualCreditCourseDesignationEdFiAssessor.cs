using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class DualCreditCourseDesignationEdFiAssessor : IEdFiAssessor
{
    private readonly EdFiCourseProvider _courseProvider;

    public DualCreditCourseDesignationEdFiAssessor(EdFiCourseProvider courseProvider)
    {
        _courseProvider = courseProvider;
    }

    public string DataElementName => "Dual credit course designation";

    public string AssessmentDescription =>
        "Analyzes courses for Dual Credit designation via CourseLevelCharacteristic";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading courses...");

        var data = await _courseProvider.GetDataAsync(httpClient, context);

        var totalCourses = data.Count;
        var dualCreditCourses = 0;

        foreach (var course in data)
        {
            if (course.LevelCharacteristics?.Any(lc =>
                    lc.CourseLevelCharacteristicDescriptor?.Contains("Dual Credit", StringComparison.OrdinalIgnoreCase) ?? false) == true)
            {
                dualCreditCourses++;
            }
        }

        context.Log($"Found {dualCreditCourses:N0} of {totalCourses:N0} courses with Dual Credit designation");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalCourses),
                new Completeness(totalCourses, dualCreditCourses, "Dual Credit designation")
            ],
            Remarks = AssessmentDescription
        };
    }
}
