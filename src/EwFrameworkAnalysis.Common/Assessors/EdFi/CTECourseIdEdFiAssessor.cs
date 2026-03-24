using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class CTECourseIdEdFiAssessor : IEdFiAssessor
{
    private readonly EdFiCourseProvider _courseProvider;

    public CTECourseIdEdFiAssessor(EdFiCourseProvider courseProvider)
    {
        _courseProvider = courseProvider;
    }

    public string DataElementName => "CTE course ID or course title";

    public string AssessmentDescription =>
        "Analyzes courses for CTE designation via CareerPathwayDescriptor";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Loading courses...");

        var data = await _courseProvider.GetDataAsync(httpClient, context);

        var totalCourses = data.Count;
        var cteCourses = 0;
        var pathwayDistribution = new Dictionary<string, int>();

        foreach (var course in data)
        {
            if (!string.IsNullOrWhiteSpace(course.CareerPathwayDescriptor))
            {
                cteCourses++;
                var pathway = EdFiDescriptorHelper.ParseDescriptorValue(course.CareerPathwayDescriptor);
                if (!pathwayDistribution.ContainsKey(pathway))
                    pathwayDistribution[pathway] = 0;
                pathwayDistribution[pathway]++;
            }
        }

        context.Log($"Found {cteCourses:N0} of {totalCourses:N0} courses with CareerPathwayDescriptor (CTE courses)");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalCourses),
                new Distribution(pathwayDistribution, "Career Pathway"),
                new Completeness(totalCourses, cteCourses, "CareerPathwayDescriptor")
            ],
            Remarks = AssessmentDescription
        };
    }
}
