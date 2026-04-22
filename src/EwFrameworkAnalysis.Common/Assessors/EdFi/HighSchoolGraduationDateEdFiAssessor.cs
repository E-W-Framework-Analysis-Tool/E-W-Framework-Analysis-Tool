using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class HighSchoolGraduationDateEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "High school graduation date";

    public string AssessmentDescription =>
        "Counts diploma award dates from studentAcademicRecords and shows their year distribution " +
        "to verify graduation dates are populated with reasonable values.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Loading student academic records...");

        var diplomaCount = 0;
        var awardYearDistribution = new Dictionary<string, int>();
        var diplomaTypeDistribution = new Dictionary<string, int>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiStudentAcademicRecord>(
            httpClient,
            "ed-fi/studentAcademicRecords",
            record =>
            {
                if (record.Diplomas == null)
                    return;

                foreach (var diploma in record.Diplomas)
                {
                    diplomaCount++;

                    var year = diploma.DiplomaAwardDate.Year.ToString();
                    if (!awardYearDistribution.ContainsKey(year))
                        awardYearDistribution[year] = 0;
                    awardYearDistribution[year]++;

                    var diplomaType = EdFiDescriptorHelper.ParseDescriptorValue(diploma.DiplomaTypeDescriptor);
                    if (!diplomaTypeDistribution.ContainsKey(diplomaType))
                        diplomaTypeDistribution[diplomaType] = 0;
                    diplomaTypeDistribution[diplomaType]++;
                }
            },
            context
        );

        context.Log($"Found {diplomaCount:N0} diploma award dates");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(diplomaCount),
                new Distribution(awardYearDistribution, "Diploma Award Year"),
                new Distribution(diplomaTypeDistribution, "Diploma Type")
            ],
            Remarks = AssessmentDescription
        };
    }
}
