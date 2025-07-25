using FluentAssertions;
using SoloX.CodeQuality.Playwright;

namespace EwFrameworkAnalysis.Blazor.E2ETests;

public class PlaywrightTestBuilderLocalTest : IClassFixture<PlaywrightTestFixture>
{
    private readonly IPlaywrightTestBuilder? _builder;

    public PlaywrightTestBuilderLocalTest(PlaywrightTestFixture fixture)
    {
        _builder = fixture.Builder;
    }

    [Theory]
    [InlineData(Browser.Chromium, null)]
    [InlineData(Browser.Firefox, null)]
    [InlineData(Browser.Webkit, null)]
    public async Task HomePageLoad_Should_Display_Correct_Title_Async(Browser browser, string? deviceName)
    {

        if (_builder == null)
        {
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
}
