using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class IBCourseDesignationEdFiAssessor : IEdFiAssessor
{
    private const string IB_DESCRIPTOR_URI = "uri://ed-fi.org/ProgramTypeDescriptor#International Baccalaureate";

    public string DataElementName => "IB course designation";

    public string AssessmentDescription =>
        "Analyzes courses for International Baccalaureate designation via CourseLevelCharacteristic";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Counting courses...");

        var totalCourses = await EdFiApiPatterns.CountFromHeaderAsync(
            httpClient,
            "ed-fi/courses");

        context.ReportProgress(50, "Counting International Baccalaureate courses...");

        var ibCourses = await EdFiApiPatterns.CountFromHeaderAsync(
            httpClient,
            "ed-fi/courses",
            new Dictionary<string, string>
            {
                ["courseLevelCharacteristicDescriptor"] = IB_DESCRIPTOR_URI
            });

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
