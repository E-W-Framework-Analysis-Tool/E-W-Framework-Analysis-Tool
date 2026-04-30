using EwFrameworkAnalysis.Common.FrameworkReferenceData;

namespace EwFrameworkAnalysis.Common.UnitTests.FrameworkReferenceData;

public class DisaggregatesTests
{
    [Fact]
    public void Disaggregates_ShouldNotBeEmpty()
    {
        Assert.True(
            EwFrameworkDisaggregates.Disaggregates.Count > 0,
            "Expected at least one disaggregate");
    }

    [Fact]
    public void Disaggregates_ShouldAllHaveAtLeastOneSector()
    {
        var missing = EwFrameworkDisaggregates.Disaggregates
            .Where(d => d.Sectors.Count == 0)
            .Select(d => d.Name)
            .ToList();

        Assert.True(
            missing.Count == 0,
            $"Expected all disaggregates to have at least one sector, but these do not:\n{string.Join("\n", missing)}");
    }

    [Fact]
    public void Disaggregates_MappedDataElementsShouldExistInDataElements()
    {
        var unknownMappings = EwFrameworkDisaggregates.Disaggregates
            .SelectMany(d => d.DataElementNames
                .Where(name => !EwFrameworkDataElements.Elements.ContainsKey(name))
                .Select(name => $"\"{d.Name}\" -> \"{name}\""))
            .ToList();

        Assert.True(
            unknownMappings.Count == 0,
            $"Expected all mapped data element names to exist in DataElements, but these do not:\n{string.Join("\n", unknownMappings)}");
    }

    [Fact]
    public void Disaggregates_ShouldNotHaveDuplicateNames()
    {
        var duplicates = EwFrameworkDisaggregates.Disaggregates
            .GroupBy(d => d.Name, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        Assert.True(
            duplicates.Count == 0,
            $"Expected no duplicate disaggregate names, but found:\n{string.Join("\n", duplicates)}");
    }
}
