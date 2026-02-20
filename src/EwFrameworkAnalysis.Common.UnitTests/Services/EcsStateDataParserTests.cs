using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.UnitTests.Services;

public class EcsStateDataParserTests
{
    private readonly EcsStateDataParser _parser = new();

    [Fact]
    public void Should_ProcessStateData_WithCollectedColumn()
    {
        var records = new List<EcsStateDataRecord>
        {
            new("K-12", "ACT completion indicator", "ACT completion", "Found", "Not Found"),
            new("K-12", "SAT completion indicator", "SAT completion", "Partial", "Found")
        };

        var (assessment, stats) = _parser.ProcessStateData(records, EcsDataColumn.Collected);

        Assert.Equal(2, assessment.DataElementAssessments.Count);
        Assert.Equal(2, stats.DataElementsMapped);

        var act = assessment.DataElementAssessments.First(a => a.DataElementName == "ACT completion");
        var actAvailability = act.Characteristics.OfType<ReportedAvailability>().First();
        Assert.Equal(AvailabilityJudgment.Available, actAvailability.Value);

        var sat = assessment.DataElementAssessments.First(a => a.DataElementName == "SAT completion");
        var satAvailability = sat.Characteristics.OfType<ReportedAvailability>().First();
        Assert.Equal(AvailabilityJudgment.PartiallyAvailable, satAvailability.Value);
    }

    [Fact]
    public void Should_ProcessStateData_WithReportedColumn()
    {
        var records = new List<EcsStateDataRecord>
        {
            new("K-12", "ACT completion indicator", "ACT completion", "Found", "Not Found")
        };

        var (assessment, _) = _parser.ProcessStateData(records, EcsDataColumn.Reported);

        var availability = assessment.DataElementAssessments[0].Characteristics.OfType<ReportedAvailability>().First();
        Assert.Equal(AvailabilityJudgment.NotAvailable, availability.Value);
    }

    [Fact]
    public void Should_ProcessStateData_ConsolidateElements()
    {
        // Records from the JSON already carry the framework element name; multiple
        // records mapping to the same framework element should be consolidated
        // by keeping the best (highest) availability judgment.
        var records = new List<EcsStateDataRecord>
        {
            new("K-12", "Discipline indicator", "Suspensions and expulsions (K-12)", "Not Found", ""),
            new("K-12", "Discipline indicator", "Suspensions and expulsions (K-12)", "Found", ""),
            new("K-12", "Discipline indicator", "Suspensions and expulsions (K-12)", "Partial", "")
        };

        var (assessment, _) = _parser.ProcessStateData(records, EcsDataColumn.Collected);

        Assert.Single(assessment.DataElementAssessments);
        Assert.Equal("Suspensions and expulsions (K-12)", assessment.DataElementAssessments[0].DataElementName);

        var availability = assessment.DataElementAssessments[0].Characteristics.OfType<ReportedAvailability>().First();
        Assert.Equal(AvailabilityJudgment.Available, availability.Value);
    }

    [Fact]
    public void Should_ProcessStateData_SkipEmptyStatus()
    {
        var records = new List<EcsStateDataRecord>
        {
            new("K-12", "ACT indicator", "ACT completion", "", ""),
            new("K-12", "SAT indicator", "SAT completion", "Found", "")
        };

        var (assessment, stats) = _parser.ProcessStateData(records, EcsDataColumn.Collected);

        Assert.Single(assessment.DataElementAssessments);
        Assert.Equal(1, stats.DataElementsSkipped);
    }

    [Fact]
    public void Should_ProcessStateData_TrackUnmappedElements()
    {
        var records = new List<EcsStateDataRecord>
        {
            new("K-12", "Unknown indicator", "Some Unknown Element", "Found", ""),
            new("K-12", "ACT indicator", "ACT completion", "Found", "")
        };

        var (assessment, stats) = _parser.ProcessStateData(records, EcsDataColumn.Collected);

        Assert.Single(assessment.DataElementAssessments);
        Assert.Equal(1, stats.DataElementsUnmapped);
        Assert.Contains("Some Unknown Element", stats.UnmappedElements);
    }
}
