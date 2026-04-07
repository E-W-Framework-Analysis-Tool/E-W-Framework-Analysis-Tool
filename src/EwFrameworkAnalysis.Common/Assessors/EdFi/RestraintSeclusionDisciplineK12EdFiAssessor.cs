using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class RestraintSeclusionDisciplineK12EdFiAssessor : IEdFiAssessor
{
    private static readonly string _restraint = "Restraint";
    private static readonly string _seclusion = "Seclusion";

    public string DataElementName => "Restraint and seclusion for discipline (K-12)";

    public string AssessmentDescription =>
        "Count and distribution of disciplinary restraint and seclusion actions from disciplineActions";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Querying API...");

        var distribution = new Dictionary<string, int>
        {
            ["Restraint"] = 0,
            ["Seclusion"] = 0
        };
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

                    if (value.Contains(_restraint, StringComparison.OrdinalIgnoreCase))
                    {
                        matchCount++;
                        distribution["Restraint"]++;
                    }
                    else if (value.Contains(_seclusion, StringComparison.OrdinalIgnoreCase))
                    {
                        matchCount++;
                        distribution["Seclusion"]++;
                    }
                }
            },
            context
        );

        context.Log($"Found {matchCount:N0} restraint/seclusion discipline records out of {totalRecords:N0} discipline actions");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(matchCount),
                new Distribution(distribution, "Restraint/Seclusion Type")
            ]
        };
    }
}
