using EwFrameworkAnalysis.Common.Models.Framework;

namespace EwFrameworkAnalysis.Common.FrameworkReferenceData;

public class Indicator
{
    public required string Name { get; set; }
    public required IndicatorType Type { get; set; }
    public required IndicatorDomain Domain { get; set; }
    public required string Definition { get; set; }
    public required string RecommendedMetrics { get; set; }
    public List<DataCategory> DataNeeded { get; set; } = [];
    public List<Sector> Sectors { get; set; } = [];
    public List<string> DataElementNames { get; set; } = [];
}
