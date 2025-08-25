using System.Text.Json;
using FluentAssertions;
using Microsoft.Playwright;

namespace EwFrameworkAnalysis.Blazor.E2ETests;

public static class AccessibilityHelper
{
    public enum FailLevel
    {
        Critical,
        Serious,
        Moderate,
        Minor
    }

    public static FailLevel GetFailLevelFromEnvironment()
    {
        var envValue = Environment.GetEnvironmentVariable("EWFTOOL_A11Y_FAIL_LEVEL")?.ToLowerInvariant();
        return envValue switch
        {
            "critical" => FailLevel.Critical,
            "serious" => FailLevel.Serious,
            "moderate" => FailLevel.Moderate,
            "minor" => FailLevel.Minor,
            _ => FailLevel.Serious // Default to serious
        };
    }

    public static string[] GetFailImpacts(FailLevel failLevel)
    {
        return failLevel switch
        {
            FailLevel.Critical => ["critical"],
            FailLevel.Serious => ["critical", "serious"],
            FailLevel.Moderate => ["critical", "serious", "moderate"],
            FailLevel.Minor => ["critical", "serious", "moderate", "minor"],
            _ => ["critical", "serious"]
        };
    }

    public static async Task<AccessibilityTestResult> RunAccessibilityTestAsync(
        IPage page,
        string axeScript,
        FailLevel? failLevel = null)
    {
        // Use provided fail level or get from environment
        var effectiveFailLevel = failLevel ?? GetFailLevelFromEnvironment();
        var failImpacts = GetFailImpacts(effectiveFailLevel);

        // Inject axe and run
        await page.EvaluateAsync(axeScript);
        var resultsJson = await page.EvaluateAsync<string>("async () => JSON.stringify(await axe.run(document))");

        using var results = JsonDocument.Parse(resultsJson);
        var root = results.RootElement;

        // Convert JsonElements to plain objects before the JsonDocument is disposed
        var violations = root.GetProperty("violations")
                             .EnumerateArray()
                             .Where(v => v.TryGetProperty("impact", out var impact)
                                      && impact.ValueKind == JsonValueKind.String
                                      && failImpacts.Contains(impact.GetString() ?? ""))
                             .Select(ConvertViolationToObject)
                             .ToArray();

        return new AccessibilityTestResult
        {
            FailLevel = effectiveFailLevel,
            Violations = violations,
            RawResultsJson = resultsJson
        };
    }

    public static async Task WriteAccessibilityReportAsync(
        AccessibilityTestResult result,
        string artifactsBasePath,
        string browser,
        ITestOutputHelper? output = null)
    {
        var reportDir = Path.Combine(artifactsBasePath, "accessibility-reports");
        Directory.CreateDirectory(reportDir);

        var safeBrowser = browser.ToLowerInvariant();
        var stamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
        var mdPath = Path.Combine(reportDir, $"report-{safeBrowser}-{stamp}.md");
        var jsonPath = Path.Combine(reportDir, $"report-{safeBrowser}-{stamp}.json");

        // Write JSON report
        await File.WriteAllTextAsync(jsonPath, result.RawResultsJson);

        // Write Markdown report
        await using var writer = new StreamWriter(mdPath);
        await writer.WriteLineAsync("# Axe Accessibility Report");
        await writer.WriteLineAsync($"Generated: {DateTime.Now}");
        await writer.WriteLineAsync($"Fail Level: {result.FailLevel}");
        await writer.WriteLineAsync($"Browser: {browser}");
        await writer.WriteLineAsync();

        if (result.Violations.Length == 0)
        {
            await writer.WriteLineAsync("✅ No accessibility violations found at the specified fail level.");
        }
        else
        {
            await writer.WriteLineAsync($"❌ Found {result.Violations.Length} accessibility violation(s):");
            await writer.WriteLineAsync();

            foreach (var violation in result.Violations)
            {
                await writer.WriteLineAsync($"## ❌ {violation.Id}: {violation.Description}");
                await writer.WriteLineAsync($"**Impact**: {violation.Impact}");
                await writer.WriteLineAsync($"**Help**: [{violation.Help}]({violation.HelpUrl})");
                await writer.WriteLineAsync();
                await writer.WriteLineAsync("### Affected Nodes:");

                foreach (var node in violation.Nodes)
                {
                    await writer.WriteLineAsync($"- **HTML**: `{Truncate(node.Html, 500)}`");
                    await writer.WriteLineAsync($"  - **Target**: `{node.Target}`");
                    await writer.WriteLineAsync();
                }
            }
        }

        // Log to test output

        var colorOutput = new ColorOutputHelper(output);

        if (result.Violations.Length > 0)
        {
            colorOutput.WriteLine($"A11y violations (impact >= {result.FailLevel}). See: {mdPath}", ConsoleColor.Red);

            colorOutput.WriteLine($"X Found {result.Violations.Length} accessibility violation(s):", ConsoleColor.Red);

            foreach (var violation in result.Violations)
            {
                colorOutput.WriteLine($"## X {violation.Id}: {violation.Description}", ConsoleColor.Red);
                colorOutput.WriteLine($"**Impact**: {violation.Impact}", ConsoleColor.Red);
                colorOutput.WriteLine("", ConsoleColor.Red);
            }
        }
        else
        {
            colorOutput.WriteLine("✅ No accessibility violations found at the specified fail level.", ConsoleColor.Green);
        }
    }

    public static void AssertNoViolations(AccessibilityTestResult result, string reportPath)
    {
        result.Violations.Length.Should().Be(0,
            $"Accessibility issues found (impact >= {result.FailLevel}). See report: {reportPath}");
    }

    private static AccessibilityViolation ConvertViolationToObject(JsonElement violation)
    {
        var nodes = violation.GetProperty("nodes")
                            .EnumerateArray()
                            .Select(node => new AccessibilityNode
                            {
                                Html = node.GetProperty("html").GetString() ?? "",
                                Target = string.Join(" > ", node.GetProperty("target").EnumerateArray().Select(t => t.GetString()))
                            })
                            .ToArray();

        return new AccessibilityViolation
        {
            Id = violation.GetProperty("id").GetString() ?? "",
            Description = violation.GetProperty("description").GetString() ?? "",
            Impact = violation.GetProperty("impact").GetString() ?? "",
            Help = violation.GetProperty("help").GetString() ?? "",
            HelpUrl = violation.GetProperty("helpUrl").GetString() ?? "",
            Nodes = nodes
        };
    }

    private static string Truncate(string s, int max) => s.Length <= max ? s : s[..max] + "...";
}

public class AccessibilityTestResult
{
    public required AccessibilityHelper.FailLevel FailLevel { get; init; }
    public required AccessibilityViolation[] Violations { get; init; }
    public required string RawResultsJson { get; init; }
}

public class AccessibilityViolation
{
    public required string Id { get; init; }
    public required string Description { get; init; }
    public required string Impact { get; init; }
    public required string Help { get; init; }
    public required string HelpUrl { get; init; }
    public required AccessibilityNode[] Nodes { get; init; }
}

public class AccessibilityNode
{
    public required string Html { get; init; }
    public required string Target { get; init; }
}
