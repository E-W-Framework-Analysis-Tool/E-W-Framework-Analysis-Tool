using SoloX.CodeQuality.Playwright;

namespace EwFrameworkAnalysis.E2ETests;

public class PlaywrightTestFixture : IAsyncLifetime, IDisposable
{
    public IPlaywrightTestBuilder? Builder { get; private set; }
    public string? RootPath { get; private set; }
    public string AxeScript { get; private set; } = "";
    public bool Ready { get; private set; }
    public bool A11yReady { get; private set; }
    public Uri? BaseUrl { get; private set; }

    /// Where test artifacts should be written.
    public string ArtifactsBasePath =>
        !string.IsNullOrWhiteSpace(RootPath) ? RootPath! : AppContext.BaseDirectory;

    public async ValueTask InitializeAsync()
    {
        // Mode 1: external server already running
        var baseUrl = Environment.GetEnvironmentVariable("EWFTOOL_E2E_BASE_URL");
        if (!string.IsNullOrWhiteSpace(baseUrl) && Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri))
        {
            BaseUrl = uri;
            Builder = PlaywrightTestBuilder.Create()
                .WithOnLineHost(baseUrl.ToString())
                .WithPlaywrightOptions(ConfigurePlaywright);
            await LoadAxeFromOutputAsync();
            Ready = true;
            return;
        }

        // Mode 2: use a published folder (set by build script)
        var publishRoot = Environment.GetEnvironmentVariable("EWFTOOL_E2E_PUBLISH_PATH");
        if (!string.IsNullOrWhiteSpace(publishRoot) && Directory.Exists(Path.Combine(publishRoot, "wwwroot")))
        {
            RootPath = Path.Combine(publishRoot, "wwwroot");

            // Serve over plain HTTP. Over HTTPS the host negotiates HTTP/2, and Firefox aborts
            // part of the ~100 parallel _framework asset requests the Blazor runtime issues at
            // boot ("Error in mono_download_assets: AbortError"), leaving the app stuck on the
            // loading splash. localhost is still a secure browser context without TLS.
            Builder = PlaywrightTestBuilder.Create()
                .WithLocalHost(lb =>
                    lb.UsePortRange(new PortRange(5000, 6000))
                      .UseHttps(false)
                      .UseWebHostWithWwwRoot(RootPath, "index.html"))
                .WithPlaywrightOptions(ConfigurePlaywright);

            await LoadAxeFromOutputAsync();
            Ready = true;
            return;
        }

        // Nothing usable → tests will skip
        Ready = false;
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    public void Dispose() { }

    private static void ConfigurePlaywright(Microsoft.Playwright.BrowserTypeLaunchOptions opt)
    {
        var headless = Environment.GetEnvironmentVariable("EWFTOOL_E2E_HEADLESS");
        opt.Headless = string.IsNullOrEmpty(headless) || headless.Equals("true", StringComparison.OrdinalIgnoreCase);

        var slow = Environment.GetEnvironmentVariable("EWFTOOL_E2E_SLOWMO");
        if (int.TryParse(slow, out var ms) && ms > 0) opt.SlowMo = ms;
    }

    /// Securely load axe from test output (not from CDN, not from wwwroot).
    private async Task LoadAxeFromOutputAsync()
    {
        // Mark as <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
        var path = Path.Combine(AppContext.BaseDirectory, "Assets", "axe.4.10.3.min.js");
        if (!File.Exists(path))
        {
            A11yReady = false; // a11y tests will Assert.Skip
            return;
        }

        AxeScript = await File.ReadAllTextAsync(path);
        A11yReady = !string.IsNullOrWhiteSpace(AxeScript);
    }
}
