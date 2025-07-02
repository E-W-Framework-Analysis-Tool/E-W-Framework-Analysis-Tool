namespace EwFrameworkAnalysis.Common.Models;

public class Indicator
{
    public required string Name { get; set; }
    public required IndicatorType Type { get; set; }
    public required IndicatorDomain Domain { get; set; }
    public required string Definition { get; set; }
    public required string Significance { get; set; }
    public required string RecommendedMetrics { get; set; }
    public required string MeasurementNotes { get; set; }
    public required string SourceFrameworks { get; set; }
    public List<DataSource> DataSources
    {
        get; set;
    } = [];
    public List<Sector> Sectors
    {
        get; set;
    } = [];
    public List<string> DataElements
    {
        get; set;
    } = [];
}

public enum IndicatorType
{
    OutcomesMilestones,
    EWSystemConditions,
    AdjacentSystemConditions
}

public enum IndicatorDomain
{
    AcademicProgressCompletion,
    CareerReadinessEconomicSuccess,
    SocialEmotionalPhysicalWellbeing,
    CrossDomain
}
