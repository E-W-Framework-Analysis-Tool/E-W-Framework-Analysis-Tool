using EwFrameworkAnalysis.Blazor.Services.DataAvailability;
using Microsoft.Extensions.Caching.Memory;

namespace EwFrameworkAnalysis.Blazor.IntegrationTests;

public class ApiTestFixture : IDisposable
{
    public HttpClient? HttpClient { get; private set; }
    public IMemoryCache? Cache { get; private set; }
    public string? ClientId { get; }
    public string? ClientSecret { get; }
    public string? AuthUrl { get; }    
    public string? AccessToken { get; }
    public Uri? BaseUrl { get; private set; }
    public bool Ready { get; private set; }
    public EdFiApiDataItemAvailabilityProvider? AvailabilityProvider { get; }

    public ApiTestFixture()
    {                
        var baseUrl = Environment.GetEnvironmentVariable("EWFTOOL_EDFI_BASE_URL");
        var authUrl = Environment.GetEnvironmentVariable("EWFTOOLTESTING_EDFI_AUTH_URL");
        if (!string.IsNullOrWhiteSpace(baseUrl) && Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri))
        {
            BaseUrl = uri;
             
            AuthUrl = authUrl ?? $"{BaseUrl.OriginalString}/oauth/token";

            HttpClient = new HttpClient
            {
                BaseAddress = BaseUrl
            };

            Cache = new MemoryCache(new MemoryCacheOptions());

            // Mode 1: API Client ID and Secret
            ClientId = Environment.GetEnvironmentVariable("EWFTOOL_EDFI_CLIENT_ID");
            ClientSecret = Environment.GetEnvironmentVariable("EWFTOOL_EDFI_CLIENT_SECRET");

            if (!string.IsNullOrWhiteSpace(ClientId) && !string.IsNullOrWhiteSpace(ClientSecret))
            {
                AvailabilityProvider = new EdFiApiDataItemAvailabilityProvider(
                    HttpClient,
                    Cache,
                    BaseUrl.OriginalString,
                    ClientId, 
                    ClientSecret, 
                    AccessToken,
                    AuthUrl
                );
                Ready = true;
                return;
            }

            // Mode 2: API Access Token
            AccessToken = Environment.GetEnvironmentVariable("EWFTOOL_EDFI_ACCESS_TOKEN");
            if (!string.IsNullOrWhiteSpace(AccessToken))
            {
                AvailabilityProvider = new EdFiApiDataItemAvailabilityProvider(
                    HttpClient,
                    Cache,
                    BaseUrl.OriginalString,
                    null,
                    null,
                    AccessToken,
                    null
                );
                Ready = true;
                return;
            }
        }

        // Nothing usable → tests will skip
        Ready = false;
    }

    public void Dispose()
    {
        // Cleanup if needed
    }
}
