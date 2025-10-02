using EwFrameworkAnalysis.Common.Models.Project;
using Microsoft.Extensions.Caching.Memory;

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
    public DataSource? TestDataSource { get; private set; }

    private readonly Lazy<Task> _initializationTask;

    public ApiTestFixture()
    {
        var baseUrl = Environment.GetEnvironmentVariable("EWFTOOL_EDFI_BASE_URL");
        var authUrl = Environment.GetEnvironmentVariable("EWFTOOL_EDFI_AUTH_URL");

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            Ready = false;
            _initializationTask = new Lazy<Task>(() => Task.CompletedTask);
            return;
        }

        if (!baseUrl.EndsWith("/"))
            baseUrl += "/";

        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri))
        {
            Ready = false;
            _initializationTask = new Lazy<Task>(() => Task.CompletedTask);
            return;
        }

        BaseUrl = uri;
        AuthUrl = authUrl ?? $"{BaseUrl.GetLeftPart(UriPartial.Authority)}/oauth/token";

        HttpClient = new HttpClient
        {
            BaseAddress = BaseUrl
        };

        Cache = new MemoryCache(new MemoryCacheOptions());

        TestDataSource = new DataSource
        {
            Id = Guid.NewGuid(),
            Name = "Test Ed-Fi API",
            Type = DataSourceType.EdFiApi,
            Enabled = true
        };

        ClientId = Environment.GetEnvironmentVariable("EWFTOOL_EDFI_CLIENT_ID");
        ClientSecret = Environment.GetEnvironmentVariable("EWFTOOL_EDFI_CLIENT_SECRET");

        if (!string.IsNullOrWhiteSpace(ClientId) && !string.IsNullOrWhiteSpace(ClientSecret))
        {
            Ready = true;
            _initializationTask = new Lazy<Task>(SetupAuthenticationAsync);
            return;
        }

        AccessToken = Environment.GetEnvironmentVariable("EWFTOOL_EDFI_ACCESS_TOKEN");
        if (!string.IsNullOrWhiteSpace(AccessToken))
        {
            HttpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);
            Ready = true;
            _initializationTask = new Lazy<Task>(() => Task.CompletedTask);
            return;
        }

        Ready = false;
        _initializationTask = new Lazy<Task>(() => Task.CompletedTask);
    }

    public Task InitializeAsync() => _initializationTask.Value;

    private async Task SetupAuthenticationAsync()
    {
        var tokenRequest = new HttpRequestMessage(HttpMethod.Post, AuthUrl);
        var formData = new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = ClientId!,
            ["client_secret"] = ClientSecret!
        };
        tokenRequest.Content = new FormUrlEncodedContent(formData);

        using var authClient = new HttpClient();
        var response = await authClient.SendAsync(tokenRequest);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"OAuth failed ({response.StatusCode}): {error}");
        }

        var json = await response.Content.ReadAsStringAsync();
        var token = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, System.Text.Json.JsonElement>>(json);

        if (token?.TryGetValue("access_token", out var accessToken) == true)
        {
            HttpClient!.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken.GetString());
        }
    }

    public void Dispose()
    {
        HttpClient?.Dispose();
    }
}
