namespace EwFrameworkAnalysis.Common.Models;

public class QuestionWithScore<T> where T : IScorableDataItem
{
    public required string Question { get; set; }
    public required List<Sector> Sectors { get; set; }
    public required List<IndicatorWithScore<T>> Indicators { get; set; }
    public decimal Score => Indicators.Count != 0 ? Indicators.Average(x => x.Score) : 0;
}

public class IndicatorWithScore<T> where T : IScorableDataItem
{
    public required Indicator Indicator { get; set; }
    public decimal Score => DataAvailabilityResults.Count != 0 ? DataAvailabilityResults.Average(x => x.Value.IsAvailable ? (decimal)1 : 0) : 0;
    public required Dictionary<string, DataAvailabilityResult<T>> DataAvailabilityResults { get; set; }
}

public class DataAvailabilityResult<T> where T : IScorableDataItem
{
    public required T DataItem { get; set; }
    public required List<ProviderDataDetails> ProviderResults { get; set; }
    public bool IsAvailable => ProviderResults.Any(x => x.DataAvailability.Status == DataAvailabilityStatus.DataAvailable);
}

public class ProviderDataDetails
{
    public required string ProviderName { get; set; }
    public required DataAvailabilityCheckResult DataAvailability { get; set; }
}
