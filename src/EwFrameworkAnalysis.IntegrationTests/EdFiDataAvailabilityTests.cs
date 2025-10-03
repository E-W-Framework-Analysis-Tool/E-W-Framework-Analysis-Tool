using EwFrameworkAnalysis.Common.Assessors.EdFi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.IntegrationTests;

public class EdFiAssessorTests : BaseApiTest
{
    public EdFiAssessorTests(ApiTestFixture fixture, ITestOutputHelper output)
        : base(fixture, output)
    {
    }

    [Fact]
    public async Task SuspensionsExpulsionsK12EdFiAssessor_ReturnsExpectedRecordCountAsync()
    {
        // Arrange
        var assessor = new SuspensionsExpulsionsK12EdFiAssessor();
        Assert.NotNull(assessor);

        // Create context that logs to test output
        var context = CreateTestContext();

        // Act
        var result = await assessor.AssessAsync(Fixture.HttpClient!, Fixture.TestDataSource!, context);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Suspensions and Expulsions (K-12)", result.DataElementName);
        Assert.Equal(Fixture.TestDataSource!.Id, result.DataSourceId);

        var recordCount = result.Characteristics.OfType<RecordCount>().FirstOrDefault();
        Assert.NotNull(recordCount);
        Output.WriteLine($"Found {recordCount.Value} suspension/expulsion records");

        // Verify we got at least the expected minimum count
        Assert.Equal(17, recordCount.Value);
    }

    [Fact]
    public async Task PreKEnrollmentsEdFiAssessor_ReturnsExpectedRecordCountAsync()
    {
        // Arrange
        var assessor = new PreKEnrollmentsEdFiAssessor();
        Assert.NotNull(assessor);

        // Create context that logs to test output
        var context = CreateTestContext();

        // Act
        var result = await assessor.AssessAsync(Fixture.HttpClient!, Fixture.TestDataSource!, context);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Pre-K Enrollments", result.DataElementName);
        Assert.Equal(Fixture.TestDataSource!.Id, result.DataSourceId);

        var recordCount = result.Characteristics.OfType<RecordCount>().FirstOrDefault();
        Assert.NotNull(recordCount);
        Output.WriteLine($"Found {recordCount.Value} Pre-K enrollment records");

        // Grand Bend doesn't have PK enrollments
        Assert.Equal(0, recordCount.Value);
    }

    /// <summary>
    /// Creates an AssessorContext that writes progress and logs to the test output.
    /// </summary>
    private AssessorContext CreateTestContext()
    {
        return new AssessorContext(
            progressCallback: (percentage, statusMessage) =>
            {
                if (percentage.HasValue)
                    Output.WriteLine($"Progress: {percentage}% - {statusMessage ?? ""}");
                else if (statusMessage != null)
                    Output.WriteLine($"Status: {statusMessage}");
            },
            logCallback: (message) =>
            {
                Output.WriteLine($"Log: {message}");
            }
        );
    }
}
