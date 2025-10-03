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

public class IntegerRange : DataCharacteristicBase
{
    public IntegerRange(int minimum, int maximum, string label)
    {
        if (maximum < minimum)
            throw new ArgumentException("Maximum must be greater than or equal to minimum");

        Minimum = minimum;
        Maximum = maximum;
        Label = label;
    }

    public int Minimum { get; }
    public int Maximum { get; }
    public string Label { get; }
}

public class Completeness : DataCharacteristicBase
{
    public Completeness(int totalRecords, int populatedRecords, string attributeName)
    {
        if (totalRecords < 0)
            throw new ArgumentException("Total records cannot be negative", nameof(totalRecords));
        if (populatedRecords < 0)
            throw new ArgumentException("Populated records cannot be negative", nameof(populatedRecords));
        if (populatedRecords > totalRecords)
            throw new ArgumentException("Populated records cannot exceed total records");

        TotalRecords = totalRecords;
        PopulatedRecords = populatedRecords;
        AttributeName = attributeName;
    }

    public int TotalRecords { get; }
    public int PopulatedRecords { get; }
    public string AttributeName { get; }
    public decimal Percentage => (decimal)(TotalRecords == 0 ? 0 : (PopulatedRecords * 100.0) / TotalRecords);
}

public class Distribution : DataCharacteristicBase
{
    public Distribution(Dictionary<string, int> counts, string label)
    {
        Counts = counts ?? throw new ArgumentNullException(nameof(counts));
        Label = label ?? throw new ArgumentNullException(nameof(label));
        TotalCount = counts.Values.Sum();
    }

    public Dictionary<string, int> Counts { get; }
    public string Label { get; }
    public int TotalCount { get; }

    public string FormatItem(string key, int count)
    {
        var percentage = TotalCount == 0 ? 0 : (count * 100.0 / TotalCount);
        return $"{key}: {count:N0} ({percentage:F1}%)";
    }
}
