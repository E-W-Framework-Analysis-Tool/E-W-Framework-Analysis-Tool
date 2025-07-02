namespace EwFrameworkAnalysis.Common.Models;

public class DataAvailabilityCheckResult
{
    public DataAvailabilityStatus Status { get; init; }
    public string? Remarks { get; init; }
    public int? TotalCount { get; init; }

    public static DataAvailabilityCheckResult NotSupported => new() { Status = DataAvailabilityStatus.NotSupported };
    public static DataAvailabilityCheckResult DataAvailable(int? count = null, string? remarks = null)
    {
        return new DataAvailabilityCheckResult
        {
            Status = DataAvailabilityStatus.DataAvailable,
            TotalCount = count,
            Remarks = remarks
        };
    }
    public static DataAvailabilityCheckResult DataUnavailable(string? remarks = null)
    {
        return new DataAvailabilityCheckResult
        {
            Status = DataAvailabilityStatus.DataUnavailable,
            Remarks = remarks
        };
    }
}

public enum DataAvailabilityStatus
{
    DataAvailable,
    DataUnavailable,
    NotSupported
}
