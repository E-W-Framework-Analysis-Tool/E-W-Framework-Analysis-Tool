using EwFrameworkAnalysis.Common.Models;

namespace EwFrameworkAnalysis.Blazor.Services.DataAvailability;

public class UserInputDataItemProvider<T> : IDataAvailabilityProvider<T> where T : IScorableDataItem
{
    public static string ProviderName => "User-supplied checklist";
    public string Name => ProviderName;

    private readonly List<T> _availableDataItems;

    public UserInputDataItemProvider(List<T> availableDataItems)
    {
        _availableDataItems = availableDataItems;
    }

    public event Action<ProviderStatusUpdate> OnStatusUpdate = delegate { };
    public event Action OnCacheHit = delegate { };

    public Task<DataAvailabilityCheckResult> CheckDataAvailability(T dataItem)
    {
        return Task.FromResult(
            _availableDataItems.Contains(dataItem)
            ? DataAvailabilityCheckResult.DataAvailable()
            : DataAvailabilityCheckResult.DataUnavailable());
    }
}
