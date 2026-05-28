using EwFrameworkAnalysis.Common.FrameworkReferenceData;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.UnitTests.Services;

public class LlmDataProfilingPromptBuilderTests
{
    [Fact]
    public void Prompt_ContainsAllDataElementNames()
    {
        foreach (var name in EwFrameworkDataElements.Elements.Keys)
            Assert.Contains(name, LlmDataProfilingPromptBuilder.Prompt);
    }

    [Fact]
    public void Prompt_ContainsRequiredSections()
    {
        var prompt = LlmDataProfilingPromptBuilder.Prompt;

        Assert.Contains("## AVAILABILITY JUDGMENT RULES", prompt);
        Assert.Contains("## OUTPUT FORMAT", prompt);
        Assert.Contains("## DATA ELEMENTS", prompt);
        Assert.Contains("## DATA SOURCE SCHEMA", prompt);
    }

    [Fact]
    public void Prompt_ContainsJsonTemplate()
    {
        Assert.Contains("\"schemaVersion\": 4", LlmDataProfilingPromptBuilder.Prompt);
    }

    [Fact]
    public void Prompt_IsNotEmptyOrWhitespace()
    {
        Assert.False(string.IsNullOrWhiteSpace(LlmDataProfilingPromptBuilder.Prompt));
    }
}
