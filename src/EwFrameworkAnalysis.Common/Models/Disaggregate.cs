namespace EwFrameworkAnalysis.Common.Models;

public class Disaggregate
{
    public required string Name { get; set; }
    public List<Sector> Sectors
    {
        get; set;
    } = [];
    public List<string> DataElements
    {
        get; set;
    } = [];
}
