using EwFrameworkAnalysis.Common.FrameworkReferenceData;

namespace EwFrameworkAnalysis.Common.UnitTests.FrameworkReferenceData;

public class IndicatorsTests
{
    [Fact]
    public void Indicators_ShouldNotHaveUnknownDataElements()
    {
        var dataElements = EwFrameworkIndicators.Indicators.Values
            .SelectMany(e => e.DataElementNames)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var actualDataElements = EwFrameworkDataElements.Elements
            .Select(i => i.Key)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var unknownDataElements = dataElements
            .Where(i => !actualDataElements.Contains(i))
            .ToList();

        Assert.True(
            unknownDataElements.Count == 0,
            $"Expected zero but found {unknownDataElements.Count} unknown data elements:\n{string.Join("\n", unknownDataElements)}");
    }

    [Fact]
    public void Indicators_ShouldNotHaveDuplicateDataElements()
    {
        var duplicateDataElements = EwFrameworkIndicators.Indicators
            .Select(kvp => new
            {
                kvp.Key,
                DuplicateDataElements = kvp.Value.DataElementNames
                    .GroupBy(de => de, StringComparer.OrdinalIgnoreCase)
                    .Where(de => de.Count() > 1)
                    .Select(de => de.Key)
                    .ToList()
            })
            .Where(x => x.DuplicateDataElements.Count != 0)
            .ToDictionary(x => x.Key, x => x.DuplicateDataElements);

        Assert.True(
            duplicateDataElements.Count == 0,
            $"Expected zero but found {duplicateDataElements.Count} duplicate data elements for indicators:\n{string.Join("\n", duplicateDataElements.Select(dde => $"\"{dde.Key}\": \"{string.Join("\", \"", dde.Value)}\""))} ");
    }

    [Fact]
    public void Indicators_ShouldBelongToAtLeastOneEssentialQuestion()
    {
        var indicators = EwFrameworkIndicators.Indicators
            .Select(i => i.Key)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var relatedIndicators = EwFrameworkEssentialQuestions.Questions
            .SelectMany(i => i.RelatedIndicatorNames)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var orphanedIndicators = indicators
            .Where(i => !relatedIndicators.Contains(i))
            .ToList();

        Assert.True(
            orphanedIndicators.Count == 0,
            $"Expected zero but found {orphanedIndicators.Count} orphaned indicators that do not belong to an essential question:\n{string.Join("\n", orphanedIndicators)}");
    }
}
