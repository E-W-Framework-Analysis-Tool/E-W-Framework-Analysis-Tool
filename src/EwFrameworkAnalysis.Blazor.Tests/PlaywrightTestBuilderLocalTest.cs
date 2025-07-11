using FluentAssertions;
using SoloX.CodeQuality.Playwright;

namespace EwFrameworkAnalysis.Blazor.Tests;

[Trait("TestPhase", "PostPublish")]
public class PlaywrightTestBuilderLocalTest
{
    private readonly IPlaywrightTestBuilder _builder;

    public PlaywrightTestBuilderLocalTest()
    {
        var root = Environment.GetEnvironmentVariable("PUBLISH_PATH")
           ?? throw new InvalidOperationException("PUBLISH_PATH not set");

        var path = Path.Combine(root, "wwwroot");

        _builder = PlaywrightTestBuilder.Create()
            .WithLocalHost(localHostBuilder =>
            {
                localHostBuilder
                    .UsePortRange(new PortRange(5000, 6000))
                    .UseWebHostWithWwwRoot(path, "index.html")
                    .UseWebHostBuilder(builder =>
                    {
                        //builder.ConfigureServices(services =>
                        //{
                        //    services.AddTransient<IService, ServiceMock>();
                        //})
                        //.ConfigureAppConfiguration((app, conf) =>
                        //{
                        //    conf.AddJsonFile("appsettings.Test.json");
                        //})
                        //.UseSetting("SomeKey", "SomeValue");
                    });
            })
            .WithPlaywrightOptions(opt =>
            {
                //opt.Headless = false;
                //opt.SlowMo = 5000;
                //opt.Timeout = 60000;
            })
            .WithPlaywrightNewContextOptions(opt =>
            {
                //opt.ViewportSize = new Microsoft.Playwright.ViewportSize() { Height = 800, Width = 1000 };
                //opt.StorageStatePath = "State Json file";
            });
    }

    [Theory]
    [InlineData(Browser.Chromium, null)]
    [InlineData(Browser.Firefox, null)]
    [InlineData(Browser.Webkit, null)]
    public async Task HomePageLoad_ShouldDisplayCorrectTitleAsync(Browser browser, string? deviceName)
    {
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
