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
            new("K-12", "ACT completion indicator", "ACT completion", AvailabilityJudgment.Available, AvailabilityJudgment.NotAvailable),
            new("K-12", "SAT completion indicator", "SAT completion", AvailabilityJudgment.PartiallyAvailable, AvailabilityJudgment.Available)
        };

        var (assessment, stats) = _parser.ProcessStateData(records, EcsDataColumn.Collected);

        Assert.Equal(2, assessment.DataElementAssessments.Count);
        Assert.Equal(2, stats.DataElementsProcessed);

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
            new("K-12", "ACT completion indicator", "ACT completion", AvailabilityJudgment.Available, AvailabilityJudgment.NotAvailable)
        };

        var (assessment, _) = _parser.ProcessStateData(records, EcsDataColumn.Reported);

        var availability = assessment.DataElementAssessments[0].Characteristics.OfType<ReportedAvailability>().First();
        Assert.Equal(AvailabilityJudgment.NotAvailable, availability.Value);
    }

    [Fact]
    public void Should_ProcessStateData_ConsolidateElements()
    {
        // Multiple records with the same element name are consolidated,
        // keeping the best (most available) judgment.
        var records = new List<EcsStateDataRecord>
        {
            new("K-12", "Discipline indicator", "Suspensions and expulsions (K-12)", AvailabilityJudgment.NotAvailable, null),
            new("K-12", "Discipline indicator", "Suspensions and expulsions (K-12)", AvailabilityJudgment.Available, null),
            new("K-12", "Discipline indicator", "Suspensions and expulsions (K-12)", AvailabilityJudgment.PartiallyAvailable, null)
        };

        var (assessment, _) = _parser.ProcessStateData(records, EcsDataColumn.Collected);

        Assert.Single(assessment.DataElementAssessments);
        Assert.Equal("Suspensions and expulsions (K-12)", assessment.DataElementAssessments[0].DataElementName);

        var availability = assessment.DataElementAssessments[0].Characteristics.OfType<ReportedAvailability>().First();
        Assert.Equal(AvailabilityJudgment.Available, availability.Value);
    }

    [Fact]
    public void Should_ProcessStateData_SkipNullStatus()
    {
        var records = new List<EcsStateDataRecord>
        {
            new("K-12", "ACT indicator", "ACT completion", null, null),
            new("K-12", "SAT indicator", "SAT completion", AvailabilityJudgment.Available, null)
        };

        var (assessment, stats) = _parser.ProcessStateData(records, EcsDataColumn.Collected);

        Assert.Single(assessment.DataElementAssessments);
        Assert.Equal(1, stats.DataElementsSkipped);
    }

    [Fact]
    public void Should_ProcessStateData_IncludeUnknownElements()
    {
        // Elements not in the EW Framework dictionary are included in the assessment.
        var records = new List<EcsStateDataRecord>
        {
            new("K-12", "Unknown indicator", "Some Unknown Element", AvailabilityJudgment.Available, null),
            new("K-12", "ACT indicator", "ACT completion", AvailabilityJudgment.Available, null)
        };

        var (assessment, stats) = _parser.ProcessStateData(records, EcsDataColumn.Collected);

        Assert.Equal(2, assessment.DataElementAssessments.Count);
        Assert.Equal(2, stats.DataElementsProcessed);
        Assert.Contains(assessment.DataElementAssessments, a => a.DataElementName == "Some Unknown Element");
    }
}
