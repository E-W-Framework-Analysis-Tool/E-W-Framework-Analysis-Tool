namespace EwFrameworkAnalysis.IntegrationTests;

public abstract class BaseApiTest : IClassFixture<ApiTestFixture>
{
    protected readonly ApiTestFixture Fixture;
    protected readonly ITestOutputHelper Output;

    protected BaseApiTest(ApiTestFixture fixture, ITestOutputHelper output)
    {
        if (!fixture.Ready)
            Assert.Skip("API fixture not ready. These tests should be run from `/eng/run-integration.ps1` script.");

        Fixture = fixture;
        Output = output;
    }
}
