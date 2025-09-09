using EwFrameworkAnalysis.Common.Models;

namespace EwFrameworkAnalysis.Common.Services;

public interface IDataAvailabilityProvider<T> where T : IScorableDataItem
{
    // Encouraged to define a static string named "ProviderName" on the implementation class, and use
    // that to implement this string name. It is helpful to refer to a well defined provider name like that
    // in some scenarios.
    public string Name { get; }
    public Task<DataAvailabilityCheckResult> CheckDataAvailability(T dataItem);
    event Action<ProviderStatusUpdate> OnStatusUpdate;
    event Action OnCacheHit;
}

public record ProviderStatusUpdate(string ProviderName, string StatusMessage);

