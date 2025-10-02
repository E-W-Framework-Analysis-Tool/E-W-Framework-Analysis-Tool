using EwFrameworkAnalysis.Common.Assessors.EdFi;
using EwFrameworkAnalysis.Common.Models.Project;

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

        // Act
        var result = await assessor.AssessAsync(Fixture.HttpClient!, Fixture.TestDataSource!);

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

        // Act
        var result = await assessor.AssessAsync(Fixture.HttpClient!, Fixture.TestDataSource!);

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
}
