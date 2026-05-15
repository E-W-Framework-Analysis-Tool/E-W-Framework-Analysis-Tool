using EwFrameworkAnalysis.Common.Models.Framework;

namespace EwFrameworkAnalysis.Common.FrameworkReferenceData;

public class Disaggregate
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required List<Sector> Sectors { get; set; }
    public List<string> DataElementNames { get; set; } = [];
}
