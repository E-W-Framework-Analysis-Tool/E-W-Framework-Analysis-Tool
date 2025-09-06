using FluentAssertions;
using SoloX.CodeQuality.Playwright;

namespace EwFrameworkAnalysis.Blazor.E2ETests;

[Trait("Category", "E2E")]
public class HomePageTests : BaseE2ETest
{
    public HomePageTests(PlaywrightTestFixture fixture, ITestOutputHelper output)
        : base(fixture, output)
    {
    }

    [Theory]
    [MemberData(nameof(Browsers))]
    public async Task HomePageLoad_Should_Display_Correct_Title_Async(Browser browser, string? deviceName)
    {
        await using var test = await Fixture.Builder!.BuildAsync(browser, deviceName: deviceName);

        await test.GotoPageAsync("/", async page =>
        {
            await page.Locator("body").WaitForAsync();

            var title = await page.TitleAsync();
            title.Should().Be("E-W Framework Analysis Tool");
        });
    }

    [Theory]
    [MemberData(nameof(Browsers))]
    [Trait("Kind", "A11y")]
    public async Task HomePage_A11y_Async(Browser browser, string? deviceName)
    {
        await using var test = await Fixture.Builder!.BuildAsync(browser, deviceName: deviceName);

        await test.GotoPageAsync("/", async page =>
        {
            await page.Locator("body").WaitForAsync();

            await AssertNoAccessibilityViolationsAsync(page, browser, "Home Page A11y Test");
        });
    }
}
