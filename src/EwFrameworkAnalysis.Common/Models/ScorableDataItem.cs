namespace EwFrameworkAnalysis.Common.Models;

public interface IScorableDataItem : IEquatable<IScorableDataItem>
{
    public string Name { get; }
}

public class DataSourceSectorDataItem : IScorableDataItem
{
    public required DataSource DataSource { get; set; }
    public required Sector Sector { get; set; }
    public string Name => $"{DataSource} | {Sector}";

    public bool Equals(IScorableDataItem? other)
    {
        if (other == null) return false;

        if (other is DataSourceSectorDataItem otherDataSourceItem)
        {
            return DataSource == otherDataSourceItem.DataSource && Sector == otherDataSourceItem.Sector;
        }

        return false;
    }

    public override bool Equals(object? obj)
    {
        if (obj is DataSourceSectorDataItem otherDataSourceItem)
        {
            return Equals(otherDataSourceItem);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(DataSource, Sector);
    }
}

public class DataElementDataItem : IScorableDataItem
{
    public required DataElement DataElement { get; set; }
    public string Name => DataElement.Name;

    public bool Equals(IScorableDataItem? other)
    {
        if (other == null) return false;

        if (other is DataElementDataItem otherDataSourceItem)
        {
            return string.Equals(DataElement.Name, otherDataSourceItem.DataElement.Name, StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    public override bool Equals(object? obj)
    {
        if (obj is DataElementDataItem otherDataElementItem)
        {
            return Equals(otherDataElementItem);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return DataElement.Name.GetHashCode(StringComparison.OrdinalIgnoreCase);
    }
}
