
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Models.Scoring;
using EwFrameworkAnalysis.Common.Scoring;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.UnitTests.ScoringRules;

public class ScoringTestFixture : IDisposable
{
    public DataElementScoringRuleRegistry RuleRegistry { get; }
    public DataElementScoringService ScoringService { get; }
    public List<DataSource> DataSources { get; set; } = [];

    public ScoringTestFixture()
    {
        var rules = new IDataElementScoringRule[]
        {
            new ReportedAndCountScoringRule()
        };

        RuleRegistry = new DataElementScoringRuleRegistry(rules);
        ScoringService = new DataElementScoringService(RuleRegistry);
    }

    public void Dispose()
    {
        // Cleanup resources if needed
    }
}
