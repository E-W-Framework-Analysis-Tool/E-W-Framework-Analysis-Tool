using System.Text.Json;
using FluentAssertions;
using SoloX.CodeQuality.Playwright;
using Xunit.Abstractions;

namespace EwFrameworkAnalysis.Blazor.E2ETests;

public class PlaywrightTestBuilderLocalTest : IClassFixture<PlaywrightTestFixture>
{
    private readonly IPlaywrightTestBuilder? _builder;
    private readonly ITestOutputHelper _output;
    private readonly string _rootPath;
    private readonly string _axeScript;

    public PlaywrightTestBuilderLocalTest(PlaywrightTestFixture fixture, ITestOutputHelper output)
    {
        _builder = fixture.Builder;
        _rootPath = fixture.RootPath ?? "";
        _axeScript = fixture.AxeScript;
        _output = output;
    }

    [Theory]
    [InlineData(Browser.Chromium, null)]
    [InlineData(Browser.Firefox, null)]
    [InlineData(Browser.Webkit, null)]
    public async Task HomePageLoad_Should_Display_Correct_Title_Async(Browser browser, string? deviceName)
    {

        if (_builder == null)
        {
            _output.WriteLine("Skipping: Fixture not initialized. Upgrade to xUnit v3 for graceful dynamic skipping.");
            return;
        }

        var playwrightTest = await _builder
            .BuildAsync(browser, deviceName: deviceName)
            .ConfigureAwait(true);

        await using var _ = playwrightTest.ConfigureAwait(false);

        await playwrightTest
            .GotoPageAsync(
                "/",
                async (page) =>
                {
                    var body = page.Locator("body");

                    await body.WaitForAsync().ConfigureAwait(true);

                    var title = await page.TitleAsync().ConfigureAwait(true);

                    title.Should().Be("E-W Framework Analysis Tool");
                }).ConfigureAwait(true);
    }


    [Theory]
    [InlineData(Browser.Chromium, null)]
    [InlineData(Browser.Firefox, null)]
    [InlineData(Browser.Webkit, null)]
    public async Task HomePage_Identify_Accessibility_Issues_In_Home_Page_Async(Browser browser, string? deviceName)
    {
        if (_builder == null)
        {
            _output.WriteLine("Skipping: Fixture not initialized. Upgrade to xUnit v3 for graceful dynamic skipping.");
            return;
        }

        if (_rootPath == null)
        {
            _output.WriteLine("Skipping: Fixture not initialized. Upgrade to xUnit v3 for graceful dynamic skipping.");
            return;
        }

        var playwrightTest = await _builder
            .BuildAsync(browser, deviceName: deviceName)
            .ConfigureAwait(true);

        await using var _ = playwrightTest.ConfigureAwait(false);

        await playwrightTest
            .GotoPageAsync(
                "/",
                async (page) =>
                {
                    // Inject axe-core script loaded from CDN
                    await page.EvaluateAsync(_axeScript);

                    // Run axe
                    var resultsJson = await page.EvaluateAsync<string>(@"async () => {
                        return JSON.stringify(await axe.run(document));
                    }");

                    // Parse results
                    var results = JsonDocument.Parse(resultsJson);
                    var violations = results.RootElement.GetProperty("violations");

                    // Save to readable file if violations exist
                    if (violations.GetArrayLength() > 0)
                    {
                        var safeBrowserName = browser.ToString().ToLowerInvariant();
                        var reportDirectory = Path.Combine(_rootPath, "accessibility-reports");

                        // Ensure the directory exists
                        Directory.CreateDirectory(reportDirectory); // This does nothing if it already exists

                        var reportPath = Path.Combine(reportDirectory, $"report-{safeBrowserName}-{DateTime.Now:yyyyMMdd-HHmmss}.md");

                        using (var writer = new StreamWriter(reportPath))
                        {
                            writer.WriteLine("# Axe Accessibility Report");
                            writer.WriteLine($"Generated: {DateTime.Now}");
                            writer.WriteLine();

                            foreach (var violation in violations.EnumerateArray())
                            {
                                writer.WriteLine($"## ❌ {violation.GetProperty("id")}: {violation.GetProperty("description")}");
                                writer.WriteLine($"**Impact**: {violation.GetProperty("impact")}");
                                writer.WriteLine($"**Help**: [{violation.GetProperty("help")}]({violation.GetProperty("helpUrl")})");
                                writer.WriteLine();
                                writer.WriteLine("### Affected Nodes:");
                                foreach (var node in violation.GetProperty("nodes").EnumerateArray())
                                {
                                    writer.WriteLine($"- **HTML**: `{node.GetProperty("html")}`");
                                    writer.WriteLine($"  - **Target**: `{string.Join(" > ", node.GetProperty("target").EnumerateArray().Select(t => t.GetString()))}`");
                                    writer.WriteLine();
                                }
                            }
                        }
                        _output.WriteLine($"Accessibility violations found. See report at: {reportPath}");
                    }

                    violations.GetArrayLength().Should().Be(0);

                }).ConfigureAwait(true);
    }
}
