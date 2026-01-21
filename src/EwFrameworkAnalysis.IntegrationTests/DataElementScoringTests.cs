
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Models.Scoring;
using EwFrameworkAnalysis.Common.Scoring;

namespace EwFrameworkAnalysis.IntegrationTests;

public class DataElementScoringTests
{
    public DataElementScoringTests()
    {
    }

    [Fact]
    public void Registry_Resolves_Rule_By_Name()
    {
        var rules = new IDataElementScoringRule[]
        {
            new ReportedAndCountScoringRule()
        };

        var registry = new DataElementScoringRuleRegistry(rules);

        var rule = registry.Resolve("reportedonly");

        Assert.IsType<ReportedAndCountScoringRule>(rule);
    }

    [Fact]
    public void ReportedOnly_Selects_Highest_Score_From_All_Sources()
    {
        var request = new DataElementScoringRequest
        {
            DataElementName = "StudentGPA",
            ScoringRuleName = "ReportedOnly",
            Matches =
            {
                //TestData.ManualAssessment(AvailabilityJudgment.PartiallyAvailable),
                //TestData.EdFiAssessment(recordCount: 0)
            }
        };

        var rule = new ReportedAndCountScoringRule();

        var result = rule.Score(request);

        Assert.Equal(0.5m, result.AvailabilityScore);
        Assert.Equal("Manual", result.SelectedSource);
        Assert.Equal(2, result.SourceScores.Count);
    }
}
