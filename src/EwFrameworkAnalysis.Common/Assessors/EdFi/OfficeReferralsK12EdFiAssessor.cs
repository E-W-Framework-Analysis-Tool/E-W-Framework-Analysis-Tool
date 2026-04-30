using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class OfficeReferralsK12EdFiAssessor : IEdFiAssessor
{
    private static readonly string _referralDescriptor = "Removal from Classroom";

    public string DataElementName => "Office referrals (K-12)";

    public string AssessmentDescription =>
        "Count and distribution of discipline actions classified as office referrals from disciplineActions";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Querying API...");

        var distribution = new Dictionary<string, int>();
        var totalRecords = 0;
        var matchCount = 0;

        await EdFiApiPatterns.PageAndProcessAsync<EdFiDisciplineAction>(
            httpClient,
            "ed-fi/disciplineActions",
            action =>
            {
                totalRecords++;

                if (action.Disciplines == null)
                    return;

                foreach (var discipline in action.Disciplines)
                {
                    if (discipline.DisciplineDescriptor == null)
                        continue;

                    var value = EdFiDescriptorHelper.ParseDescriptorValue(discipline.DisciplineDescriptor);

                    if (value.Contains(_referralDescriptor, StringComparison.OrdinalIgnoreCase))
                    {
                        matchCount++;
                        distribution[value] = distribution.GetValueOrDefault(value) + 1;
                    }
                }
            },
            context
        );

        context.Log($"Found {matchCount:N0} office referral records out of {totalRecords:N0} discipline actions");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(matchCount),
                new Distribution(distribution, "Referral Type")
            ]
        };
    }
}
