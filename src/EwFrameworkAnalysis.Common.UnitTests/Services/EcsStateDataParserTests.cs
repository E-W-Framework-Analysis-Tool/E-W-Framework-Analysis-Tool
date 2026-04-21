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
            new("K-12", "Discipline indicator", "Suspensions and expulsions (K-12)", null),
            new("K-12", "Discipline indicator", "Suspensions and expulsions (K-12)", null),
            new("K-12", "Discipline indicator", "Suspensions and expulsions (K-12)", null)
        };

        var (assessment, _) = _parser.ProcessStateData(records);

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
            new("K-12", "ACT indicator", "ACT completion", null),
            new("K-12", "SAT indicator", "SAT completion", null)
        };

        var (assessment, stats) = _parser.ProcessStateData(records);

        Assert.Single(assessment.DataElementAssessments);
        Assert.Equal(1, stats.DataElementsSkipped);
    }

    [Fact]
    public void Should_ProcessStateData_IncludeUnknownElements()
    {
        // Elements not in the EW Framework dictionary are included in the assessment.
        var records = new List<EcsStateDataRecord>
        {
            new("K-12", "Unknown indicator", "Some Unknown Element", null),
            new("K-12", "ACT indicator", "ACT completion", null)
        };

        var (assessment, stats) = _parser.ProcessStateData(records);

        Assert.Equal(2, assessment.DataElementAssessments.Count);
        Assert.Equal(2, stats.DataElementsProcessed);
        Assert.Contains(assessment.DataElementAssessments, a => a.DataElementName == "Some Unknown Element");
    }
}
