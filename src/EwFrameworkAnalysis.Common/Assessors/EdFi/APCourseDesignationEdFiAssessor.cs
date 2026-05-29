using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class APCourseDesignationEdFiAssessor : IEdFiAssessor
{
    private const string AP_DESCRIPTOR_URI = "uri://ed-fi.org/CourseLevelCharacteristicDescriptor#Advanced Placement";

    private readonly EdFiCourseProvider _courseProvider;

    public APCourseDesignationEdFiAssessor(EdFiCourseProvider courseProvider)
    {
        _courseProvider = courseProvider;
    }

    public string DataElementName => "AP course designation";

    public string AssessmentDescription =>
        "Counts courses designated Advanced Placement via their CourseLevelCharacteristic descriptor.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading courses...");

        var data = await _courseProvider.GetDataAsync(httpClient, context);

        var apCourses = data.Count(course =>
            course.LevelCharacteristics?.Any(lc =>
                string.Equals(lc.CourseLevelCharacteristicDescriptor, AP_DESCRIPTOR_URI, StringComparison.OrdinalIgnoreCase)) == true);

        context.Log($"Found {apCourses:N0} courses with Advanced Placement designation");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(apCourses)
            ],
            Remarks = AssessmentDescription
        };
    }
}
