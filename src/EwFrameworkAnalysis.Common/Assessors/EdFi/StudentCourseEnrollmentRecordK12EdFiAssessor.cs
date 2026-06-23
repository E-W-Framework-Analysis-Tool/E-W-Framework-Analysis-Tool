using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class StudentCourseEnrollmentRecordK12EdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Student course enrollment record (K-12)";

    public string AssessmentDescription =>
        "Analyzes studentSectionAssociations for course enrollments excluding homerooms";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Counting student section associations...");

        // Use total-count headers instead of paging/deserializing the full collection.
        // Two requests replace the previous 1 + ceil(N/pageSize) paged calls.
        var totalRecords = await EdFiApiPatterns.CountFromHeaderAsync(
            httpClient,
            "ed-fi/studentSectionAssociations");

        context.ReportProgress(50, "Counting homeroom enrollments...");

        // Only homeroomIndicator=true rows are homerooms; everything else (false or
        // unset) is a non-homeroom enrollment, matching the prior "!= true" semantics.
        var homeroomRecords = await EdFiApiPatterns.CountFromHeaderAsync(
            httpClient,
            "ed-fi/studentSectionAssociations",
            new Dictionary<string, string> { ["homeroomIndicator"] = "true" });

        var nonHomeroomRecords = totalRecords - homeroomRecords;

        context.Log($"Found {nonHomeroomRecords:N0} non-homeroom enrollments out of {totalRecords:N0} section associations");
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
