namespace EwFrameworkAnalysis.IntegrationTests;

public abstract class BaseApiTest : IClassFixture<ApiTestFixture>, IAsyncLifetime
{
    protected readonly ApiTestFixture Fixture;
    protected readonly ITestOutputHelper Output;

    protected BaseApiTest(ApiTestFixture fixture, ITestOutputHelper output)
    {
        if (!fixture.Ready)
            Assert.Skip("API fixture not ready");

        Fixture = fixture;
        Output = output;
    }

    public async ValueTask InitializeAsync()
    {
        await Fixture.InitializeAsync();
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
