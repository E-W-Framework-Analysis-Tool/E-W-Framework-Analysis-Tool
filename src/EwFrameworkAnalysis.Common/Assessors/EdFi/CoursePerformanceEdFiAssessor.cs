using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class CoursePerformanceEdFiAssessor : IEdFiAssessor
{
    // Academic subjects that scope this data element to English and Math.
    private const string ENGLISH_SUBJECT_DESCRIPTOR =
        "uri://ed-fi.org/AcademicSubjectDescriptor#English Language Arts";
    private const string MATH_SUBJECT_DESCRIPTOR =
        "uri://ed-fi.org/AcademicSubjectDescriptor#Mathematics";

    private static readonly string[] _englishMathSubjectDescriptors =
    [
        ENGLISH_SUBJECT_DESCRIPTOR,
        MATH_SUBJECT_DESCRIPTOR
    ];

    // Course attempt results, counted per course via the API's totalCount header.
    private static readonly string[] _resultDescriptors =
    [
        "uri://ed-fi.org/CourseAttemptResultDescriptor#Pass",
        "uri://ed-fi.org/CourseAttemptResultDescriptor#Fail",
        "uri://ed-fi.org/CourseAttemptResultDescriptor#Incomplete",
        "uri://ed-fi.org/CourseAttemptResultDescriptor#Withdrawn"
    ];

    private const string NOT_SPECIFIED_LABEL = "Not Specified";

    private readonly EdFiCourseProvider _courseProvider;

    public CoursePerformanceEdFiAssessor(EdFiCourseProvider courseProvider)
    {
        _courseProvider = courseProvider;
    }

    public string DataElementName => "Course performance (English and Math)";

    public string AssessmentDescription =>
        "Record count and distribution (by subject and by pass/fail attempt result) of course " +
        "transcripts for English and Math courses, counted via API totalCount headers.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading courses...");

        // Filter the cached course catalog to English/Math courses
        var courses = await _courseProvider.GetDataAsync(httpClient, context);

        var englishMathCourses = courses
            .Where(c => c.AcademicSubjects?.Any(s =>
                _englishMathSubjectDescriptors.Contains(s.AcademicSubjectDescriptor)) == true)
            .Where(c => !string.IsNullOrWhiteSpace(c.CourseCode))
            .Select(c => (
                c.CourseCode,
                c.EducationOrganizationReference.EducationOrganizationId,
                SubjectLabel: ClassifySubject(c)))
            .Distinct()
            .ToList();

        context.Log($"Found {englishMathCourses.Count:N0} English/Math courses in the catalog");

        // Count transcripts per course straight from the API's totalCount header.
        var totalRecords = 0;
        
        var subjectDistribution = new Dictionary<string, int>
        {
            [EdFiDescriptorHelper.ParseDescriptorValue(ENGLISH_SUBJECT_DESCRIPTOR)] = 0,
            [EdFiDescriptorHelper.ParseDescriptorValue(MATH_SUBJECT_DESCRIPTOR)] = 0
        };
        
        var resultDistribution = _resultDescriptors.ToDictionary(
            EdFiDescriptorHelper.ParseDescriptorValue, _ => 0);
        var resultBucketTotal = 0;

        var processed = 0;
        foreach (var (courseCode, educationOrganizationId, subjectLabel) in englishMathCourses)
        {
            var courseTotal = await EdFiApiPatterns.CountFromHeaderAsync(
                httpClient, "ed-fi/courseTranscripts", CourseFilter(courseCode, educationOrganizationId));

            totalRecords += courseTotal;
            subjectDistribution[subjectLabel] += courseTotal;

            // Per-result counts for the pass/fail distribution (one header count each).
            foreach (var descriptor in _resultDescriptors)
            {
                var filter = CourseFilter(courseCode, educationOrganizationId);
                filter["courseAttemptResultDescriptor"] = descriptor;

                var count = await EdFiApiPatterns.CountFromHeaderAsync(
                    httpClient, "ed-fi/courseTranscripts", filter);

                resultDistribution[EdFiDescriptorHelper.ParseDescriptorValue(descriptor)] += count;
                resultBucketTotal += count;
            }

            processed++;
            context.ReportProgress(
                processed * 100 / englishMathCourses.Count,
                $"Course {processed:N0}/{englishMathCourses.Count:N0}");
        }

        // Transcripts whose result is absent or outside the known set.
        var notSpecified = totalRecords - resultBucketTotal;
        if (notSpecified > 0)
            resultDistribution[NOT_SPECIFIED_LABEL] = notSpecified;

        context.Log($"Found {totalRecords:N0} English/Math course transcripts");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Distribution(subjectDistribution, "Subject Area"),
                new Distribution(resultDistribution, "Course Attempt Result")
            ],
            Remarks = AssessmentDescription
        };
    }

    // Builds the course-scoped query filter; courseCode is only unique within an education organization.
    private static Dictionary<string, string> CourseFilter(string courseCode, long educationOrganizationId) =>
        new()
        {
            ["courseCode"] = courseCode,
            ["courseEducationOrganizationId"] = educationOrganizationId.ToString()
        };

    // Classifies an English/Math course into a single subject bucket, English taking precedence.
    private static string ClassifySubject(EdFiCourse course)
    {
        var hasEnglish = course.AcademicSubjects?.Any(s =>
            s.AcademicSubjectDescriptor == ENGLISH_SUBJECT_DESCRIPTOR) == true;

        return EdFiDescriptorHelper.ParseDescriptorValue(
            hasEnglish ? ENGLISH_SUBJECT_DESCRIPTOR : MATH_SUBJECT_DESCRIPTOR);
    }
}
