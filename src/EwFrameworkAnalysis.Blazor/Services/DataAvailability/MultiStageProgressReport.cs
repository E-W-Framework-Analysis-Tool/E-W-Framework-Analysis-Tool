using System.Collections.Concurrent;

namespace EwFrameworkAnalysis.Blazor.Services.DataAvailability;

public class MultiStageProgressReport
{
    public ConcurrentDictionary<string, ProviderProgressReport> ProviderReports { get; set; } = new ConcurrentDictionary<string, ProviderProgressReport>();

    // Calculated properties
    public int Errors => ProviderReports.Values.Sum(p => p.Errors);
    public int CompletedAvailabilityChecks => ProviderReports.Values.Sum(p => p.CompletedAvailabilityChecks);
    public int TotalAvailabilityChecks => ProviderReports.Values.Sum(p => p.TotalAvailabilityChecks);
    public int CacheHits => ProviderReports.Values.Sum(p => p.CacheHits);
    public string LatestStatus => ProviderReports.Values.OrderByDescending(p => p.LastUpdated).FirstOrDefault()?.LatestStatus ?? "No status available";

    public decimal OverallProgress => TotalAvailabilityChecks == 0 ? 0 : (decimal)CompletedAvailabilityChecks / TotalAvailabilityChecks;
}

public class ProviderProgressReport
{
    public required string ProviderName { get; set; }

    public int Errors { get; set; }
    public int CompletedAvailabilityChecks { get; set; }
    public int TotalAvailabilityChecks { get; set; }

    public int UnavailableResults { get; set; }
    public int AvailableResults { get; set; }

    public int CacheHits { get; set; }

    public string LatestStatus { get; set; } = "No status";
    public DateTime LastUpdated { get; private set; } = DateTime.UtcNow;

    // Calculated property
    public decimal Progress => TotalAvailabilityChecks == 0 ? 0 : (decimal)CompletedAvailabilityChecks / TotalAvailabilityChecks;
    public decimal Availability => AvailableResults == 0 ? 0 : (decimal)AvailableResults / (AvailableResults + UnavailableResults);

    public void UpdateStatus(string status)
    {
        LatestStatus = status;
        LastUpdated = DateTime.UtcNow;
    }

    public void IncrementAvailableResults()
    {
        AvailableResults++;
        LastUpdated = DateTime.UtcNow;
    }

    public void IncrementUnavailableResults()
    {
        UnavailableResults++;
        LastUpdated = DateTime.UtcNow;
    }

    public void IncrementCompletedAvailabilityChecks()
    {
        CompletedAvailabilityChecks++;
        LastUpdated = DateTime.UtcNow;
    }

    public void IncrementErrors()
    {
        Errors++;
        LastUpdated = DateTime.UtcNow;
    }

    public void SetTotalAvailabilityChecks(int total)
    {
        TotalAvailabilityChecks = total;
        LastUpdated = DateTime.UtcNow;
    }

    public void IncrementCacheHits()
    {
        CacheHits++;
        LastUpdated = DateTime.UtcNow;
    }
}
