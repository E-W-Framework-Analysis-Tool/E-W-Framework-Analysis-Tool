using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.UnitTests.Services;

public class EcsStateDataParserTests
{
    private readonly EcsStateDataParser _parser = new();

    [Fact]
    public void Should_ProcessStateData_WithReportedColumn()
    {
        var records = new List<EcsStateDataRecord>
        {
            new("K-12", "ACT completion indicator", "ACT completion", AvailabilityJudgment.NotAvailable)
        };

        var (assessment, _) = _parser.ProcessStateData(records);

        var availability = assessment.DataElementAssessments[0].Characteristics
            .OfType<ReportedAvailability>().First();
        Assert.Equal(AvailabilityJudgment.NotAvailable, availability.Value);
    }

    [Fact]
    public void Should_ProcessStateData_ConsolidateElements_KeepingBestJudgment()
    {
        // Same element name appears three times with different judgments —
        // the best (lowest ordinal = most available) should win.
        var records = new List<EcsStateDataRecord>
        {
            new("K-12", "Discipline indicator", "Suspensions and expulsions (K-12)", AvailabilityJudgment.NotAvailable),
            new("K-12", "Discipline indicator", "Suspensions and expulsions (K-12)", AvailabilityJudgment.Available),
            new("K-12", "Discipline indicator", "Suspensions and expulsions (K-12)", AvailabilityJudgment.PartiallyAvailable)
        };

        var (assessment, _) = _parser.ProcessStateData(records);

        Assert.Single(assessment.DataElementAssessments);
        Assert.Equal("Suspensions and expulsions (K-12)", assessment.DataElementAssessments[0].DataElementName);
        var availability = assessment.DataElementAssessments[0].Characteristics
            .OfType<ReportedAvailability>().First();
        Assert.Equal(AvailabilityJudgment.Available, availability.Value);
    }

    [Fact]
    public void Should_ProcessStateData_SkipNullStatus()
    {
        // One valid record and one null — only the valid one should be assessed.
        var records = new List<EcsStateDataRecord>
        {
            new("K-12", "ACT indicator",  "ACT completion", AvailabilityJudgment.Available),
            new("K-12", "SAT indicator",  "SAT completion", null)
        };

        var (assessment, stats) = _parser.ProcessStateData(records);

        Assert.Single(assessment.DataElementAssessments);
        Assert.Equal(1, stats.DataElementsSkipped);
        Assert.Equal(1, stats.DataElementsProcessed);
    }

    [Fact]
    public void Should_ProcessStateData_IncludeUnknownElements()
    {
        // Elements not in any known framework dictionary are still included.
        var records = new List<EcsStateDataRecord>
        {
            new("K-12", "Unknown indicator", "Some Unknown Element", AvailabilityJudgment.Available),
            new("K-12", "ACT indicator",     "ACT completion",       AvailabilityJudgment.NotAvailable)
        };

        var (assessment, stats) = _parser.ProcessStateData(records);

        Assert.Equal(2, assessment.DataElementAssessments.Count);
        Assert.Equal(2, stats.DataElementsProcessed);
        Assert.Contains(assessment.DataElementAssessments,
            a => a.DataElementName == "Some Unknown Element");
    }

    [Fact]
    public void Should_ProcessStateData_SkipEmptyElementName()
    {
        var records = new List<EcsStateDataRecord>
        {
            new("K-12", "Some indicator", "",    AvailabilityJudgment.Available),
            new("K-12", "Some indicator", "   ", AvailabilityJudgment.Available),
            new("K-12", "ACT indicator",  "ACT completion", AvailabilityJudgment.Available)
        };

        var (assessment, stats) = _parser.ProcessStateData(records);

        Assert.Single(assessment.DataElementAssessments);
        Assert.Equal(2, stats.DataElementsSkipped);
    }

    [Fact]
    public void Should_ProcessStateData_ReturnEmptyAssessment_WhenAllRecordsNull()
    {
        var records = new List<EcsStateDataRecord>
        {
            new("K-12", "ACT indicator", "ACT completion", null),
            new("K-12", "SAT indicator", "SAT completion", null)
        };

        var (assessment, stats) = _parser.ProcessStateData(records);

        Assert.Empty(assessment.DataElementAssessments);
        Assert.Equal(2, stats.DataElementsSkipped);
        Assert.Equal(0, stats.DataElementsProcessed);
    }

    [Fact]
    public void Should_ProcessStateData_ReturnEmptyAssessment_WhenRecordsEmpty()
    {
        var (assessment, stats) = _parser.ProcessStateData([]);

        Assert.Empty(assessment.DataElementAssessments);
        Assert.Equal(0, stats.TotalRowsRead);
    }
}
