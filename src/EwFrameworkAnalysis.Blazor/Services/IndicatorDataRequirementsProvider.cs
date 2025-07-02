using EwFrameworkAnalysis.Common.Models;

namespace EwFrameworkAnalysis.Blazor.Services;

public interface IIndicatorDataRequirementsProvider<T> where T : IScorableDataItem
{
    public List<T> GetScorableDataItems(Indicator indicator);
    public IIndicatorDataRequirementsProvider<T> WithQuestionContext(EssentialQuestionWithIndicators question);
}

public class DataElementIndicatorDataRequirementsProvider : IIndicatorDataRequirementsProvider<DataElementDataItem>
{
    private readonly List<DataElement> _allDataItems;

    public DataElementIndicatorDataRequirementsProvider(List<DataElement> allDataItems)
    {
        _allDataItems = allDataItems;
    }

    public List<DataElementDataItem> GetScorableDataItems(Indicator indicator)
    {
        return [.. indicator.DataElements.Select(x => new DataElementDataItem()
        {
            DataElement = _allDataItems.Single(di => di.Name == x)
        })];
    }

    public IIndicatorDataRequirementsProvider<DataElementDataItem> WithQuestionContext(EssentialQuestionWithIndicators question) => this;
}

public class DataSourceSectorIndicatorDataRequirementsProvider : IIndicatorDataRequirementsProvider<DataSourceSectorDataItem>
{
    private readonly EssentialQuestionWithIndicators? _questionContext;

    public DataSourceSectorIndicatorDataRequirementsProvider(EssentialQuestionWithIndicators? questionContext = null)
    {
        _questionContext = questionContext;
    }

    public List<DataSourceSectorDataItem> GetScorableDataItems(Indicator indicator)
    {
        var applicableSectors = _questionContext?.Sectors ?? [.. Enum.GetValues<Sector>()];
        return [.. indicator.DataSources.SelectMany(ds => indicator.Sectors.Where(s => applicableSectors.Contains(s)).Select(s => new DataSourceSectorDataItem
        {
            DataSource = ds,
            Sector = s
        }))];
    }

    public IIndicatorDataRequirementsProvider<DataSourceSectorDataItem> WithQuestionContext(EssentialQuestionWithIndicators question)
    {
        return new DataSourceSectorIndicatorDataRequirementsProvider(question);
    }
}
