using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class IBCourseDesignationEdFiAssessor : IEdFiAssessor
{
    private readonly EdFiCourseProvider _courseProvider;

    public IBCourseDesignationEdFiAssessor(EdFiCourseProvider courseProvider)
    {
        _courseProvider = courseProvider;
    }

    public string DataElementName => "IB course designation";

    public string AssessmentDescription =>
        "Analyzes courses for International Baccalaureate designation via CourseLevelCharacteristic";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading courses...");

        var data = await _courseProvider.GetDataAsync(httpClient, context);

        var totalCourses = data.Count;
        var ibCourses = 0;

        foreach (var course in data)
        {
            if (course.LevelCharacteristics?.Any(lc =>
                    lc.CourseLevelCharacteristicDescriptor?.Contains("International Baccalaureate", StringComparison.OrdinalIgnoreCase) ?? false) == true)
            {
                ibCourses++;
            }
        }

        context.Log($"Found {ibCourses:N0} of {totalCourses:N0} courses with International Baccalaureate designation");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalCourses),
                new Completeness(totalCourses, ibCourses, "International Baccalaureate designation")
            ],
            Remarks = AssessmentDescription
        };
    }
}
