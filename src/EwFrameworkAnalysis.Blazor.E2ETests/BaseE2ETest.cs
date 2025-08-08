using Microsoft.Playwright;
using SoloX.CodeQuality.Playwright;

namespace EwFrameworkAnalysis.Blazor.E2ETests;

public abstract class BaseE2ETest : IClassFixture<PlaywrightTestFixture>
{
    protected readonly PlaywrightTestFixture Fixture;
    protected readonly ITestOutputHelper Output;

    public static TheoryData<Browser, string?> Browsers
    {
        get
        {
            var data = new TheoryData<Browser, string?>
            {
                // Always include Chromium
                { Browser.Chromium, null }
            };

            // Add other browsers only if environment variable is set to "true"
            var allBrowsers = Environment.GetEnvironmentVariable("EWFTOOL_E2E_ALL_BROWSERS");
            if ("true".Equals(allBrowsers, StringComparison.OrdinalIgnoreCase))
            {
                data.Add(Browser.Firefox, null);
                data.Add(Browser.Webkit, null);
            }

            return data;
        }
    }

    protected BaseE2ETest(PlaywrightTestFixture fixture, ITestOutputHelper output)
    {
        if (!fixture.Ready)
            Assert.Skip("E2E fixture not ready (no published app or E2E_BASE_URL). These tests should be run from `/eng/run-e2e.ps1` script.");

        Fixture = fixture;
        Output = output;
    }

    /// <summary>
    /// Runs accessibility test on the current page and asserts no violations at the configured fail level.
    /// </summary>
    protected async Task AssertNoAccessibilityViolationsAsync(IPage page, Browser browser, AccessibilityHelper.FailLevel? failLevel = null)
    {
        if (!Fixture.A11yReady)
            Assert.Skip("axe.min.js not found in test output; a11y tests skipped.");

        var result = await AccessibilityHelper.RunAccessibilityTestAsync(page, Fixture.AxeScript, failLevel);

        var reportDir = Path.Combine(Fixture.ArtifactsBasePath, "accessibility-reports");
        var safeBrowser = browser.ToString().ToLowerInvariant();
        var stamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
        var reportPath = Path.Combine(reportDir, $"report-{safeBrowser}-{stamp}.md");

        await AccessibilityHelper.WriteAccessibilityReportAsync(result, Fixture.ArtifactsBasePath, browser.ToString(), Output);

        AccessibilityHelper.AssertNoViolations(result, reportPath);
    }

    /// <summary>
    /// Runs accessibility test and returns the result without asserting. Useful for custom validation logic.
    /// </summary>
    protected async Task<AccessibilityTestResult> RunAccessibilityTestAsync(IPage page, AccessibilityHelper.FailLevel? failLevel = null)
    {
        if (!Fixture.A11yReady)
            throw new InvalidOperationException("axe.min.js not found in test output; accessibility testing not available.");

        return await AccessibilityHelper.RunAccessibilityTestAsync(page, Fixture.AxeScript, failLevel);
    }
}
