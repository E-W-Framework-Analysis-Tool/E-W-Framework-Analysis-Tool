
using SoloX.CodeQuality.Playwright;

namespace EwFrameworkAnalysis.Blazor.E2ETests;

public class PlaywrightTestFixture : IDisposable
{
    public IPlaywrightTestBuilder? Builder { get; }
    public string? RootPath { get; }

    public PlaywrightTestFixture()
    {
        var root = Environment.GetEnvironmentVariable("EWFTOOLTESTING_PUBLISH_PATH");

        if (string.IsNullOrWhiteSpace(root) || !Directory.Exists(Path.Combine(root, "wwwroot")))
        {
            Console.WriteLine("Publish path variable has not been set or does not exist, tests will be skipped.");
            return;
        }

        RootPath = Path.Combine(root, "wwwroot");

        Builder = PlaywrightTestBuilder.Create()
            .WithLocalHost(localHostBuilder =>
            {
                localHostBuilder
                    .UsePortRange(new PortRange(5000, 6000))
                    .UseWebHostWithWwwRoot(RootPath, "index.html");
            })
            .WithPlaywrightOptions(opt =>
            {
                //opt.Headless = false;
                //opt.SlowMo = 5000;
                //opt.Timeout = 60000;
            });
    }

    public void Dispose()
    {
        // Cleanup if needed
    }
}
