using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class StateAssessmentsK12Algebra : IEdFiAssessor
{
    private const string AcademicSubjectDescriptor = "uri://ed-fi.org/AcademicSubjectDescriptor#Mathematics";

    private const string GradeLevelDescriptor = "uri://ed-fi.org/GradeLevelDescriptor#Ninth grade";

    private static readonly HashSet<string> _algebraICourseCodes = ["ALG-1"];

    public string DataElementName => "Course identifier or title";

    public string AssessmentDescription => "Count of unique Course Identifiers for Algebra I (Grade 9)";

    // CourseTranscript.Course.CourseCode
    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Counting unique Course Identifiers for Algebra I (Grade 9)");

        var courseCodes = new HashSet<string>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiCourseTranscript>(
            httpClient,
            "ed-fi/courseTranscripts",
            courseTranscript =>
            {
                if (!courseTranscript.AcademicSubjects
                    .Select(a => a.AcademicSubjectDescriptor)
                    .Contains(GradeLevelDescriptor))
                {
                    return;
                }

                if (!courseTranscript.WhenTakenGradeLevelDescriptor.Contains(AcademicSubjectDescriptor))
                {
                    return;
                }

                if (!_algebraICourseCodes.Contains(courseTranscript.CourseReference.CourseCode))
                {
                    return;
                }

                courseCodes.Add(courseTranscript.CourseReference.CourseCode);
            },
            context
        );
        var totalCourses = courseCodes.Count;

        context.Log($"Found {totalCourses:N0} unique course codes");

        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalCourses)
            ]
        };
    }
}
