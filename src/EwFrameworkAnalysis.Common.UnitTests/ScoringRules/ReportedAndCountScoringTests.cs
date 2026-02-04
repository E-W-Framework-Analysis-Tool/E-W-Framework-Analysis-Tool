
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Models.Scoring;
using EwFrameworkAnalysis.Common.Scoring;

namespace EwFrameworkAnalysis.Common.UnitTests.ScoringRules;

public class ReportedAndCountScoringTests : IClassFixture<ScoringTestFixture>
{
    protected readonly ScoringTestFixture Fixture;

    public ReportedAndCountScoringTests(ScoringTestFixture fixture)
    {
        Fixture = fixture;
    }

    [Fact]
    public void Registry_Resolves_Rule_By_Name()
    {
        var rules = new IDataElementScoringRule[]
        {
            new ReportedAndCountScoringRule()
        };

        var registry = new DataElementScoringRuleRegistry(rules);

        var rule = registry.Resolve("ReportedAndCount");

        Assert.IsType<ReportedAndCountScoringRule>(rule);
    }

    [Fact]
    public void ReportedAndCount_Selects_Highest_Score_From_All_Assessments()
    {
        var dataElementName = "Years of teaching experience";

        var manualAssessment = new DataElementAssessmentContext
        {
            DataSourceType = DataSourceType.Custom,
            Assessment = new DataElementAssessment
            {
                DataElementName = dataElementName,
                Characteristics =
                    {
                        new ReportedAvailability(AvailabilityJudgment.PartiallyAvailable)
                    }
            }
        };

        var edfiAssessment = new DataElementAssessmentContext
        {
            DataSourceType = DataSourceType.EdFiApi,
            Assessment = new DataElementAssessment
            {
                DataElementName = dataElementName,
                Characteristics =
                    {
                        new RecordCount(20)
                    }
            }
        };

        var cedsAssessment = new DataElementAssessmentContext
        {
            DataSourceType = DataSourceType.CedsDw,
            Assessment = new DataElementAssessment
            {
                DataElementName = dataElementName,
                Characteristics =
                    {
                        new RecordCount(0)
                    }
            }
        };

        var request = new DataElementScoringRequest
        {
            DataElementName = dataElementName,
            ScoringRuleName = "ReportedAndCount",
            Matches =
            {
                edfiAssessment,
                manualAssessment,
                cedsAssessment
            }
        };

        var rule = Fixture.RuleRegistry.Resolve(request.ScoringRuleName);

        var result = rule.Score(request);

        Assert.Equal(AvailabilityJudgment.Available, result.AvailabilityScore);
        Assert.Equal(DataSourceType.EdFiApi.ToString(), result.SelectedSource);
        Assert.Equal(3, result.SourceScores.Count);
    }

    [Fact]
    public void ReportedAndCount_Selects_Manual_Source_When_All_Have_Same_Score()
    {
        var dataElementName = "Years of teaching experience";

        var manualAssessment = new DataElementAssessmentContext
        {
            DataSourceType = DataSourceType.Custom,
            Assessment = new DataElementAssessment
            {
                DataElementName = dataElementName,
                Characteristics =
                    {
                        new ReportedAvailability(AvailabilityJudgment.Available)
                    }
            }
        };

        var edfiAssessment = new DataElementAssessmentContext
        {
            DataSourceType = DataSourceType.EdFiApi,
            Assessment = new DataElementAssessment
            {
                DataElementName = dataElementName,
                Characteristics =
                    {
                        new RecordCount(20)
                    }
            }
        };

        var cedsAssessment = new DataElementAssessmentContext
        {
            DataSourceType = DataSourceType.CedsDw,
            Assessment = new DataElementAssessment
            {
                DataElementName = dataElementName,
                Characteristics =
                    {
                        new RecordCount(1)
                    }
            }
        };

        var request = new DataElementScoringRequest
        {
            DataElementName = dataElementName,
            ScoringRuleName = "ReportedAndCount",
            Matches =
            {
                edfiAssessment,
                manualAssessment,
                cedsAssessment
            }
        };

        var rule = Fixture.RuleRegistry.Resolve(request.ScoringRuleName);

        var result = rule.Score(request);

        Assert.Equal(AvailabilityJudgment.Available, result.AvailabilityScore);
        Assert.Equal(DataSourceType.Custom.ToString(), result.SelectedSource);
        Assert.Equal(3, result.SourceScores.Count);
    }
}
