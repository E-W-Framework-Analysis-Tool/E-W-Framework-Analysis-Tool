using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class DiplomaAwardDateEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Diploma or credential award date";

    public string AssessmentDescription =>
        "Count and completeness of diploma award dates from studentAcademicRecords";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Querying API...");

        var totalRecords = 0;
        var recordsWithDiplomas = 0;
        var diplomaCount = 0;

        await EdFiApiPatterns.PageAndProcessAsync<EdFiStudentAcademicRecord>(
            httpClient,
            "ed-fi/studentAcademicRecords",
            record =>
            {
                totalRecords++;

                if (record.Diplomas == null || record.Diplomas.Count == 0)
                    return;

                recordsWithDiplomas++;
                diplomaCount += record.Diplomas.Count;
            },
            context
        );

        context.Log($"Found {diplomaCount:N0} diplomas across {recordsWithDiplomas:N0} of {totalRecords:N0} academic records");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(diplomaCount),
                new Completeness(totalRecords, recordsWithDiplomas, "Diplomas")
            ]
        };
    }
}
