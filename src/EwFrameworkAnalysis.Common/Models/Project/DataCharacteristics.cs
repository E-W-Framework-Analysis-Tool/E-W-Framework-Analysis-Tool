namespace EwFrameworkAnalysis.Common.Models.Project;

public abstract class DataCharacteristicBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Remarks { get; set; }
    public DateTime MeasuredAt { get; set; } = DateTime.UtcNow;
}

public class RecordCount : DataCharacteristicBase
{
    public RecordCount(int value)
    {
        Value = value;
    }

    public int Value { get; }
}

public class ReportedAvailability : DataCharacteristicBase
{
    public ReportedAvailability(AvailabilityJudgment value)
    {
        Value = value;
    }

    public AvailabilityJudgment Value { get; }
}

public enum AvailabilityJudgment
{
    Available,
    PartiallyAvailable,
    NotAvailable,
    InsufficientData
}
