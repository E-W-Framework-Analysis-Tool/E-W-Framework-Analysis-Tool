using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class StudentCourseEnrollmentEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Student course enrollment record";

    public string AssessmentDescription =>
        "Analyzes studentSectionAssociations for course enrollments excluding homerooms";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Loading student section associations...");

        var totalRecords = 0;
        var nonHomeroomRecords = 0;
        var uniqueStudents = new HashSet<string>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiStudentSectionAssociation>(
            httpClient,
            "ed-fi/studentSectionAssociations",
            association =>
            {
                totalRecords++;

                if (association.HomeroomIndicator != true)
                {
                    nonHomeroomRecords++;
                    var studentId = association.StudentReference?.StudentUniqueId;
                    if (!string.IsNullOrEmpty(studentId))
                        uniqueStudents.Add(studentId);
                }
            },
            context
        );

        context.Log($"Found {nonHomeroomRecords:N0} non-homeroom enrollments across {uniqueStudents.Count:N0} unique students");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Completeness(totalRecords, nonHomeroomRecords, "Non-Homeroom Enrollments")
            ],
            Remarks = AssessmentDescription
        };
    }
}
