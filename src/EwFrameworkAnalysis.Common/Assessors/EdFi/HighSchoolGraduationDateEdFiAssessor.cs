using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class HighSchoolGraduationDateEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "High school graduation date";

    public string AssessmentDescription =>
        "Analyzes studentAcademicRecords for diploma types and CTE completers";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Loading student academic records...");

        var totalRecords = 0;
        var recordsWithDiplomas = 0;
        var diplomaTypeDistribution = new Dictionary<string, int>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiStudentAcademicRecord>(
            httpClient,
            "ed-fi/studentAcademicRecords",
            record =>
            {
                totalRecords++;

                if (record.Diplomas != null && record.Diplomas.Count > 0)
                {
                    recordsWithDiplomas++;
                    foreach (var diploma in record.Diplomas)
                    {
                        var diplomaType = EdFiDescriptorHelper.ParseDescriptorValue(diploma.DiplomaTypeDescriptor);
                        if (!diplomaTypeDistribution.ContainsKey(diplomaType))
                            diplomaTypeDistribution[diplomaType] = 0;
                        diplomaTypeDistribution[diplomaType]++;
                    }
                }
            },
            context
        );

        context.Log($"Found {recordsWithDiplomas:N0} of {totalRecords:N0} academic records with diplomas");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Distribution(diplomaTypeDistribution, "Diploma Type"),
                new Completeness(totalRecords, recordsWithDiplomas, "Diplomas")
            ],
            Remarks = AssessmentDescription
        };
    }
}
