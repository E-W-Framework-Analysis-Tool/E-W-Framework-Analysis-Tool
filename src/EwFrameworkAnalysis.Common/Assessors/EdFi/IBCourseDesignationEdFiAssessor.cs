using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class IBCourseDesignationEdFiAssessor : IEdFiAssessor
{
    private const string IB_DESCRIPTOR_URI = "uri://ed-fi.org/CourseLevelCharacteristicDescriptor#International Baccalaureate";

    private readonly EdFiCourseProvider _courseProvider;

    public IBCourseDesignationEdFiAssessor(EdFiCourseProvider courseProvider)
    {
        _courseProvider = courseProvider;
    }

    public string DataElementName => "IB course designation";

    public string AssessmentDescription =>
        "Counts courses designated International Baccalaureate via their CourseLevelCharacteristic descriptor.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading courses...");

        var data = await _courseProvider.GetDataAsync(httpClient, context);

        var ibCourses = data.Count(course =>
            course.LevelCharacteristics?.Any(lc =>
                string.Equals(lc.CourseLevelCharacteristicDescriptor, IB_DESCRIPTOR_URI, StringComparison.OrdinalIgnoreCase)) == true);

        context.Log($"Found {ibCourses:N0} courses with International Baccalaureate designation");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(ibCourses)
            ],
            Remarks = AssessmentDescription
        };
    }
}
