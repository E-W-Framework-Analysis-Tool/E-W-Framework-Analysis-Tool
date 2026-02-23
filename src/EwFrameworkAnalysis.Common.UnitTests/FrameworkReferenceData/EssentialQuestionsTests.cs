using EwFrameworkAnalysis.Common.FrameworkReferenceData;

namespace EwFrameworkAnalysis.Common.UnitTests.FrameworkReferenceData;

public class EssentialQuestionsTests
{
    [Fact]
    public void EssentialQuestions_ShouldNotHaveUnknownRelatedIndicators()
    {
        var relatedIndicators = EwFrameworkEssentialQuestions.Questions
            .SelectMany(i => i.RelatedIndicatorNames)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var actualIndicators = EwFrameworkIndicators.Indicators
            .Select(i => i.Key)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var unknownRelatedIndicators = relatedIndicators
            .Where(i => !actualIndicators.Contains(i))
            .ToList();

        Assert.True(
            unknownRelatedIndicators.Count == 0,
            $"Expected zero but found {unknownRelatedIndicators.Count} unknown related indicators:\n{string.Join("\n", unknownRelatedIndicators)}");
    }

    [Fact]
    public void EssentialQuestions_ShouldNotHaveDuplicateRelatedIndicators()
    {
        var duplicateRelatedIndicators = EwFrameworkEssentialQuestions.Questions
            .Select(eq => new
            {
                eq.QuestionNumber,
                DuplicateRelatedIndicatorName = eq.RelatedIndicatorNames
                    .GroupBy(rin => rin, StringComparer.OrdinalIgnoreCase)
                    .Where(rin => rin.Count() > 1)
                    .Select(rin => rin.Key)
                    .ToList()
            })
            .Where(x => x.DuplicateRelatedIndicatorName.Count != 0)
            .ToDictionary(x => x.QuestionNumber, x => x.DuplicateRelatedIndicatorName);

        Assert.True(
            duplicateRelatedIndicators.Count == 0,
            $"Expected zero but found {duplicateRelatedIndicators.Count} duplicate related indicators for question:\n{string.Join("\n", duplicateRelatedIndicators.Select(dde => $"\"{dde.Key}\": \"{string.Join("\", \"", dde.Value)}\""))} ");
    }
}
