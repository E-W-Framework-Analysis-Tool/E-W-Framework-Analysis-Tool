using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class StudentCoursesMathematics : IEdFiAssessor
{
    private const string AcademicSubjectDescriptor = "uri://ed-fi.org/AcademicSubjectDescriptor#Mathematics";

    private const string GradeLevelDescriptor = "uri://ed-fi.org/GradeLevelDescriptor#Ninth grade";

    public string RelatedIndicatorName => "Successful completion of Algebra I by 9th grade";

    public string DataElementName => "Course identifier or title";

    public string AssessmentDescription => "Count of unique Course Identifiers in Algebra I by 9th grade";

    // CourseTranscript.Course.CourseCode
    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Counting unique Course Identifiers in Mathematics");

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
