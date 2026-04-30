using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class StudentFteStatusEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Student FTE status";

    public string AssessmentDescription =>
        "Distribution of students by full-time equivalency from studentSchoolAssociations";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Querying API...");

        var distribution = new Dictionary<string, int>();
        var totalRecords = 0;
        var reportedCount = 0;

        await EdFiApiPatterns.PageAndProcessAsync<EdFiStudentSchoolAssociation>(
            httpClient,
            "ed-fi/studentSchoolAssociations",
            association =>
            {
                totalRecords++;

                var fte = association.FullTimeEquivalency;

                if (fte == null)
                {
                    distribution["Not Reported"] = distribution.GetValueOrDefault("Not Reported") + 1;
                }
                else
                {
                    reportedCount++;
                    var label = fte >= 1.0 ? "Full-Time (1.0)" : $"Part-Time ({fte:F2})";
                    distribution[label] = distribution.GetValueOrDefault(label) + 1;
                }
            },
            context
        );

        context.Log($"Found {reportedCount:N0} students with FTE reported out of {totalRecords:N0}");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Completeness(totalRecords, reportedCount, "FullTimeEquivalency"),
                new Distribution(distribution, "FTE Status")
            ]
        };
    }
}
