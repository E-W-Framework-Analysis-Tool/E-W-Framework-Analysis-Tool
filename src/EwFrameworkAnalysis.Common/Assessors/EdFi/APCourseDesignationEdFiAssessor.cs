using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class APCourseDesignationEdFiAssessor : IEdFiAssessor
{
    private readonly EdFiCourseProvider _courseProvider;

    public APCourseDesignationEdFiAssessor(EdFiCourseProvider courseProvider)
    {
        _courseProvider = courseProvider;
    }

    public string DataElementName => "AP course designation";

    public string AssessmentDescription =>
        "Analyzes courses for Advanced Placement designation via CourseLevelCharacteristic";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading courses...");

        var data = await _courseProvider.GetDataAsync(httpClient, context);

        var totalCourses = data.Count;
        var apCourses = 0;

        foreach (var course in data)
        {
            if (course.LevelCharacteristics?.Any(lc =>
                    lc.CourseLevelCharacteristicDescriptor?.Contains("Advanced Placement", StringComparison.OrdinalIgnoreCase) ?? false) == true)
            {
                apCourses++;
            }
        }

        context.Log($"Found {apCourses:N0} of {totalCourses:N0} courses with Advanced Placement designation");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalCourses),
                new Completeness(totalCourses, apCourses, "Advanced Placement designation")
            ],
            Remarks = AssessmentDescription
        };
    }
}
