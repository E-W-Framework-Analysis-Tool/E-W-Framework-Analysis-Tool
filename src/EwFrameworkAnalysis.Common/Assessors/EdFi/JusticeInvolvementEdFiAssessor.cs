using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class JusticeInvolvementEdFiAssessor : IEdFiAssessor
{
    private static readonly string[] _justiceDescriptors =
    [
        "Law Enforcement",
        "Arrest",
        "Juvenile Justice",
        "Court",
        "Referral to Law Enforcement",
        "Referred to Law Enforcement",
        "School-Related Arrest"
    ];

    public string DataElementName => "Justice involvement";

    public string AssessmentDescription =>
        "Count and distribution of discipline actions classified as justice involvement " +
        "(law enforcement referrals, arrests, court involvement) from disciplineActions.";

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

                    if (IsJusticeRelated(value))
                    {
                        matchCount++;
                        distribution[value] = distribution.GetValueOrDefault(value) + 1;
                    }
                }
            },
            context
        );

        context.Log($"Found {matchCount:N0} justice-related records out of {totalRecords:N0} discipline actions");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(matchCount),
                new Distribution(distribution, "Discipline Type")
            ]
        };
    }

    private static bool IsJusticeRelated(string descriptorValue)
    {
        foreach (var keyword in _justiceDescriptors)
        {
            if (descriptorValue.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
