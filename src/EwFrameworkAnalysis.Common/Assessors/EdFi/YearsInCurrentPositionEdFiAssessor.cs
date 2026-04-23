using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class YearsInCurrentPositionEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Years in current position";

    public string AssessmentDescription =>
        "Distribution of staff by years in current position calculated from beginDate in staffEducationOrganizationAssignmentAssociations";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Querying API...");

        var distribution = new Dictionary<string, int>();
        var totalRecords = 0;
        var reportedCount = 0;
        var today = DateOnly.FromDateTime(DateTime.Today);

        await EdFiApiPatterns.PageAndProcessAsync<EdFiStaffEducationOrganizationAssignmentAssociation>(
            httpClient,
            "ed-fi/staffEducationOrganizationAssignmentAssociations",
            association =>
            {
                totalRecords++;

                if (association.EndDate != null)
                    return;

                reportedCount++;
                var beginDate = association.BeginDate;
                var years = today.Year - beginDate.Year;
                if (today < beginDate.AddYears(years))
                    years--;

                var label = years switch
                {
                    < 1 => "< 1 year",
                    >= 1 and <= 3 => "1-3 years",
                    >= 4 and <= 5 => "4-5 years",
                    >= 6 and <= 10 => "6-10 years",
                    _ => "10+ years"
                };

                distribution[label] = distribution.GetValueOrDefault(label) + 1;
            },
            context
        );

        context.Log($"Found {reportedCount:N0} active assignments out of {totalRecords:N0} total");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Distribution(distribution, "Years in Position")
            ]
        };
    }
}
