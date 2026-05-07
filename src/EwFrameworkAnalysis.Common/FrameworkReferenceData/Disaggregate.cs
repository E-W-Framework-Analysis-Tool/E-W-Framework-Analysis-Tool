using EwFrameworkAnalysis.Common.Models.Framework;

namespace EwFrameworkAnalysis.Common.FrameworkReferenceData;

public class Disaggregate
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required List<Sector> Sectors { get; init; }
    public List<string> DataElementNames { get; init; } = [];
}
