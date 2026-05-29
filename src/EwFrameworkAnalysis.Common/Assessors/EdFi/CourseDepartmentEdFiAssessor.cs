using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class CourseDepartmentEdFiAssessor : IEdFiAssessor
{
    private readonly EdFiCourseProvider _courseProvider;

    public CourseDepartmentEdFiAssessor(EdFiCourseProvider courseProvider)
    {
        _courseProvider = courseProvider;
    }

    public string DataElementName => "Course department";

    public string AssessmentDescription =>
        "Analyzes courses for academic subject (department) distribution and completeness";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading courses...");

        var data = await _courseProvider.GetDataAsync(httpClient, context);

        var totalCourses = data.Count;
        var coursesWithSubject = 0;
        var subjectDistribution = new Dictionary<string, int>();

        foreach (var course in data)
        {
            if (course.AcademicSubjects != null && course.AcademicSubjects.Count > 0)
            {
                coursesWithSubject++;

                foreach (var subject in course.AcademicSubjects)
                {
                    var subjectValue = EdFiDescriptorHelper.ParseDescriptorValue(subject.AcademicSubjectDescriptor);
                    if (!subjectDistribution.ContainsKey(subjectValue))
                        subjectDistribution[subjectValue] = 0;
                    subjectDistribution[subjectValue]++;
                }
            }
        }

        context.Log($"Found {coursesWithSubject:N0} of {totalCourses:N0} courses with academic subject (department)");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalCourses),
                new Distribution(subjectDistribution, "Academic Subject"),
                new Completeness(totalCourses, coursesWithSubject, "AcademicSubject")
            ],
            Remarks = AssessmentDescription
        };
    }
}
