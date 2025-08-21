using EwFrameworkAnalysis.Common.Models;

namespace EwFrameworkAnalysis.Blazor.IntegrationTests;
public class EdFiDataAvailabilityTests : BaseApiTest
{
    public EdFiDataAvailabilityTests(ApiTestFixture fixture, ITestOutputHelper output)
        : base(fixture, output)
    {
    }

    [Fact]
    public async Task CheckDataAvailability_Attendance_ReturnsResultAsync()
    {
        var testDataItem = new DataElementDataItem
        {
            DataElement = new DataElement { Name = DataElementNames.Attendance, Category = "" }
        };

        var result = await Fixture.AvailabilityProvider!.CheckDataAvailability(testDataItem);

        // Assert
        Assert.NotNull(result);
    }
}
