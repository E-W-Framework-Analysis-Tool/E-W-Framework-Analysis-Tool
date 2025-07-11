using EwFrameworkAnalysis.Common.Models;

namespace EwFrameworkAnalysis.Blazor.Tests;

[Trait("TestPhase", "OnBuild")]
public class DataSourceSectorDataItemTests
{
    [Fact]
    public void DataSourceSectorDataItem_WhenDuplicatesInList_HandlesDistinct()
    {
        // Not implementing Object.Equals override bit me...
        var items = new List<DataSourceSectorDataItem>()
        {
            new() { DataSource = DataSource.Rubrics, Sector = Sector.PK },
            new() { DataSource = DataSource.Rubrics, Sector = Sector.PK },
            new() { DataSource = DataSource.Rubrics, Sector = Sector.K12 },
        };

        var distinctItems = items.Distinct();

        Assert.Equal(2, distinctItems.Count());
    }
}
