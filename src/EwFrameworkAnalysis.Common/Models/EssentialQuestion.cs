namespace EwFrameworkAnalysis.Common.Models;

//
public class EssentialQuestion
{
    public required string Question { get; set; }
    public required List<Sector> Sectors { get; set; }
    public required List<string> RelatedIndicators { get; set; }
}

public class EssentialQuestionWithIndicators
{
    public required string Question { get; set; }
    public required List<Sector> Sectors { get; set; }
    public required List<Indicator> RelatedIndicators { get; set; }
}
