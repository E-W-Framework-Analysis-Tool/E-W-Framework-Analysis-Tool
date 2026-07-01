using FluentAssertions;
using Microsoft.Playwright;
using SoloX.CodeQuality.Playwright;

namespace EwFrameworkAnalysis.E2ETests;

[Trait("Category", "E2E")]
public class AnalysisPageTests : BaseE2ETest
{
    public AnalysisPageTests(PlaywrightTestFixture fixture, ITestOutputHelper output)
        : base(fixture, output)
    {
    }

    [Theory]
    [MemberData(nameof(Browsers))]
    public async Task AnalysisPageLoad_Should_Display_Correct_Heading_Async(Browser browser, string? deviceName)
    {
        await using var test = await Fixture.Builder!.BuildAsync(browser, deviceName: deviceName);

        await test.GotoPageAsync("/", async page =>
        {
            await GoToAnalysisAsync(page);

            var heading = page.GetByRole(AriaRole.Heading, new() { Name = "E-W Framework Coverage Analysis" });
            (await heading.IsVisibleAsync()).Should().BeTrue();
        });
    }

    [Theory]
    [MemberData(nameof(Browsers))]
    [Trait("Kind", "A11y")]
    public async Task AnalysisPage_A11y_Async(Browser browser, string? deviceName)
    {
        await using var test = await Fixture.Builder!.BuildAsync(browser, deviceName: deviceName);

        await test.GotoPageAsync("/", async page =>
        {
            await GoToAnalysisAsync(page);

            await AssertNoAccessibilityViolationsAsync(page, browser, "Analysis Page A11y Test");
        });
    }

    [Theory]
    [MemberData(nameof(Browsers))]
    [Trait("Kind", "A11y")]
    public async Task AnalysisPageExpandedContent_A11y_Async(Browser browser, string? deviceName)
    {
        await using var test = await Fixture.Builder!.BuildAsync(browser, deviceName: deviceName);

        await test.GotoPageAsync("/", async page =>
        {
            await GoToAnalysisAsync(page);

            // Expand the first Essential Question row and open the indicator details modal so the
            // collapsed report content and the dialog are also covered by the accessibility scan.
            await page.Locator("input[name='question-check-1']").CheckAsync();
            await page.Locator(".collapse-content .card").First.ClickAsync();
            await page.Locator("[role='dialog']").WaitForAsync();

            await AssertNoAccessibilityViolationsAsync(page, browser, "Analysis Page Expanded Content A11y Test");
        });
    }

    /// <summary>
    /// Navigates from the home page to the Analysis page via the nav link (client-side routing)
    /// and waits for the Essential Questions report grid to render.
    /// </summary>
    private static async Task GoToAnalysisAsync(IPage page)
    {
        await page.Locator("body").WaitForAsync();
        await page.GetByRole(AriaRole.Link, new() { Name = "Reports & Analysis" }).ClickAsync();
        await page.Locator("input[name='question-check-1']").WaitForAsync();
    }
}
