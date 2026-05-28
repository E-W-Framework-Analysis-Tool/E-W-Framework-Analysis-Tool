using EwFrameworkAnalysis.Common.Models.Framework;

namespace EwFrameworkAnalysis.Common.FrameworkReferenceData;

public class EssentialQuestion
{
    public int QuestionNumber { get; set; }
    public string Question { get; set; } = string.Empty;
    public string QuestionSummary { get; set; } = string.Empty;
    public List<Sector> ApplicableSectors { get; set; } = [];
    public List<string> RelatedIndicatorNames { get; set; } = [];
}
