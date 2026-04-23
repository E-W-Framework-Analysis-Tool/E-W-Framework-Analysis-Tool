using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class HighSchoolDiplomaTypeEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "High school diploma type";

    public string AssessmentDescription =>
        "Distribution of diploma types from studentAcademicRecords";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Querying API...");

        var distribution = new Dictionary<string, int>();
        var totalRecords = 0;
        var recordsWithDiplomas = 0;

        await EdFiApiPatterns.PageAndProcessAsync<EdFiStudentAcademicRecord>(
            httpClient,
            "ed-fi/studentAcademicRecords",
            record =>
            {
                totalRecords++;

                if (record.Diplomas == null || record.Diplomas.Count == 0)
                    return;

                recordsWithDiplomas++;

                foreach (var diploma in record.Diplomas)
                {
                    var diplomaType = diploma.DiplomaTypeDescriptor;
                    var label = !string.IsNullOrWhiteSpace(diplomaType)
                        ? EdFiDescriptorHelper.ParseDescriptorValue(diplomaType)
                        : "Not Specified";

                    distribution[label] = distribution.GetValueOrDefault(label) + 1;
                }
            },
            context
        );

        context.Log($"Found {recordsWithDiplomas:N0} records with diplomas out of {totalRecords:N0} academic records");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(recordsWithDiplomas),
                new Distribution(distribution, "Diploma Type")
            ]
        };
    }
}
