using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class StudentGPAEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Grade point average (K-12)";

    public string AssessmentDescription =>
        "Measures the number of students with GPA ";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Fetching total student count ...");

        var totalStudents = await EdFiApiPatterns.CountFromHeaderAsync(
            httpClient,
            "ed-fi/students"
        );

        context.ReportProgress(50, "Query student academic records ...");

        var studentUniqueIds = new HashSet<string>();
        var termDistribution = new Dictionary<string, int>();
        await EdFiApiPatterns.PageAndProcessAsync<EdFiStudentAcademicRecord>(
            httpClient,
            "ed-fi/studentAcademicRecords",
            record =>
            {
                if (record.GradePointAverages.Count <= 0)
                {
                    return;
                }

                studentUniqueIds.Add(record.StudentReference.StudentUniqueId);

                var term = record.TermDescriptor ?? "Unknown";
                var termName = term.Split('#').LastOrDefault() ?? term;

                termDistribution.TryAdd(termName, 0);
                termDistribution[termName]++;
            },
            context
        );

        var studentsWithGpa = studentUniqueIds.Count;
        var completeness = new Completeness(totalStudents, studentsWithGpa, "GPA");

        context.Log($"Found {studentsWithGpa:N0} of {totalStudents:N0} students ({completeness.Percentage:F1}%) with GPA");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalStudents),
                new Distribution(termDistribution, "Term GPA")
            ]
        };
    }
}
