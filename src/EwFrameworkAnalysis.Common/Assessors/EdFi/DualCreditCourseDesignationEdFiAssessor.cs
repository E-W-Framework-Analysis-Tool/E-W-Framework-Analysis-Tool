using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class DualCreditCourseDesignationEdFiAssessor : IEdFiAssessor
{
    private const string DUAL_CREDIT_DESCRIPTOR_URI = "uri://ed-fi.org/CourseLevelCharacteristicDescriptor#Dual Credit";

    private readonly EdFiCourseProvider _courseProvider;

    public DualCreditCourseDesignationEdFiAssessor(EdFiCourseProvider courseProvider)
    {
        _courseProvider = courseProvider;
    }

    public string DataElementName => "Dual credit course designation";

    public string AssessmentDescription =>
        "Counts courses designated Dual Credit via their CourseLevelCharacteristic descriptor.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading courses...");

        var data = await _courseProvider.GetDataAsync(httpClient, context);

        var dualCreditCourses = data.Count(course =>
            course.LevelCharacteristics?.Any(lc =>
                string.Equals(lc.CourseLevelCharacteristicDescriptor, DUAL_CREDIT_DESCRIPTOR_URI, StringComparison.OrdinalIgnoreCase)) == true);

        context.Log($"Found {dualCreditCourses:N0} courses with Dual Credit designation");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(dualCreditCourses)
            ],
            Remarks = AssessmentDescription
        };
    }
}
