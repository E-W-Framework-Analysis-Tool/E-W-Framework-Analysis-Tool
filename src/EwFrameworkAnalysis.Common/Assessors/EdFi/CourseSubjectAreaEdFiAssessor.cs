using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class CourseSubjectAreaEdFiAssessor : IEdFiAssessor
{
    private readonly EdFiCourseProvider _courseProvider;

    public CourseSubjectAreaEdFiAssessor(EdFiCourseProvider courseProvider)
    {
        _courseProvider = courseProvider;
    }

    public string DataElementName => "Course subject area";

    public string AssessmentDescription =>
        "Analyzes courses for academic subject distribution and title completeness";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Loading courses...");

        var data = await _courseProvider.GetDataAsync(httpClient, context);

        var totalRecords = data.Count;
        var recordsWithTitle = 0;
        var subjectDistribution = new Dictionary<string, int>();

        foreach (var course in data)
        {
            if (!string.IsNullOrWhiteSpace(course.CourseTitle))
                recordsWithTitle++;

            if (course.AcademicSubjects != null)
            {
                foreach (var subject in course.AcademicSubjects)
                {
                    var subjectValue = EdFiDescriptorHelper.ParseDescriptorValue(subject.AcademicSubjectDescriptor);
                    if (!subjectDistribution.ContainsKey(subjectValue))
                        subjectDistribution[subjectValue] = 0;
                    subjectDistribution[subjectValue]++;
                }
            }
        }

        context.Log($"Found {recordsWithTitle:N0} of {totalRecords:N0} courses with titles");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Distribution(subjectDistribution, "Academic Subject"),
                new Completeness(totalRecords, recordsWithTitle, "CourseTitle")
            ],
            Remarks = AssessmentDescription
        };
    }
}
