using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Services;

/// <summary>
/// Parses assessment result CSV files and maps to DataSourceAssessment objects
/// </summary>
public class DataSourceAssessmentFileParser
{
    /// <summary>
    /// Parses a CSV stream containing assessment results (async version for Blazor compatibility)
    /// </summary>
    /// <param name="stream">Stream containing CSV data</param>
    /// <param name="dataSourceId">The ID of the data source being assessed</param>
    /// <param name="hasHeaderRow">Whether the CSV includes a header row (default: true)</param>
    /// <param name="assessmentSession">Optional existing assessment session to add results to</param>
    /// <returns>A DataSourceAssessment with all parsed results</returns>
    public async Task<DataSourceAssessment> ParseAssessmentStreamAsync(
        Stream stream,
        bool hasHeaderRow = true,
        DataSourceAssessment? assessmentSession = null)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        if (!stream.CanRead)
        {
            throw new ArgumentException("Stream must be readable", nameof(stream));
        }

        // Create or use existing assessment session
        var assessment = assessmentSession ?? new DataSourceAssessment
        {
            Name = $"Assessment from stream",
            ConductedAt = DateTime.UtcNow
        };

        // Read and parse CSV
        var rows = await ReadCsvStreamAsync(stream, hasHeaderRow);

        if (rows.Count == 0)
        {
            throw new InvalidOperationException("CSV stream contains no data rows");
        }

        // Group by DataElementName
        var groupedByElement = rows.GroupBy(r => r.DataElementName);

        foreach (var elementGroup in groupedByElement)
        {
            var dataElementName = elementGroup.Key;
            var characteristics = new List<DataCharacteristicBase>();

            // Further group by CharacteristicType within each element
            var groupedByType = elementGroup.GroupBy(r => r.CharacteristicType);

            foreach (var typeGroup in groupedByType)
            {
                var characteristicType = typeGroup.Key;
                var characteristic = MapToCharacteristic(characteristicType, [.. typeGroup]);

                if (characteristic != null)
                {
                    characteristics.Add(characteristic);
                }
            }

            // Create DataElementAssessment
            var elementAssessment = new DataElementAssessment
            {
                AssessmentSessionId = assessment.Id,
                DataElementName = dataElementName,
                AssessedAt = DateTimeOffset.Now,
                Characteristics = characteristics,
                Remarks = ExtractRemarks(elementGroup)
            };

            assessment.DataElementAssessments.Add(elementAssessment);
        }

        return assessment;
    }

    /// <summary>
    /// Parses a CSV stream containing assessment results (synchronous version for backward compatibility)
    /// </summary>
    public DataSourceAssessment ParseAssessmentStream(
        Stream stream,
        bool hasHeaderRow = true,
        DataSourceAssessment? assessmentSession = null)
    {
        return ParseAssessmentStreamAsync(stream, hasHeaderRow, assessmentSession)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    /// Parses a CSV file containing assessment results (convenience method)
    /// </summary>
    /// <param name="csvFilePath">Path to the CSV file exported from the database</param>
    /// <param name="dataSourceId">The ID of the data source being assessed</param>
    /// <param name="hasHeaderRow">Whether the CSV includes a header row (default: true)</param>
    /// <param name="assessmentSession">Optional existing assessment session to add results to</param>
    /// <returns>A DataSourceAssessment with all parsed results</returns>
    public DataSourceAssessment ParseAssessmentFile(
        string csvFilePath,
        bool hasHeaderRow = true,
        DataSourceAssessment? assessmentSession = null)
    {
        if (!File.Exists(csvFilePath))
        {
            throw new FileNotFoundException($"Assessment file not found: {csvFilePath}");
        }

        using var stream = File.OpenRead(csvFilePath);

        var assessment = ParseAssessmentStream(stream, hasHeaderRow, assessmentSession);

        // Update name if creating new assessment
        if (assessmentSession == null)
        {
            assessment.Name = $"Assessment from {Path.GetFileName(csvFilePath)}";
        }

        return assessment;
    }

    /// <summary>
    /// Reads CSV stream and returns parsed rows (async version)
    /// </summary>
    private async Task<List<AssessmentResultRow>> ReadCsvStreamAsync(Stream stream, bool hasHeaderRow)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = hasHeaderRow,
            TrimOptions = TrimOptions.Trim,
            MissingFieldFound = null // Ignore missing fields
        };

        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, config);

        List<AssessmentResultRow> rows = [];

        if (hasHeaderRow)
        {
            // Standard parsing with headers
            await foreach (var record in csv.GetRecordsAsync<AssessmentResultRow>())
            {
                rows.Add(record);
            }
        }
        else
        {
            // Manual parsing without headers - assume column order
            // DataElementName, CharacteristicType, Value, SubItemLabel, Remarks
            while (await csv.ReadAsync())
            {
                var row = new AssessmentResultRow
                {
                    DataElementName = csv.GetField<string>(0) ?? string.Empty,
                    CharacteristicType = csv.GetField<string>(1) ?? string.Empty,
                    Value = csv.GetField<string>(2) ?? string.Empty,
                    SubItemLabel = csv.GetField<string>(3),
                    Remarks = csv.GetField<string>(4)
                };
                rows.Add(row);
            }
        }

        return rows;
    }

    /// <summary>
    /// Maps CSV rows for a characteristic type to the appropriate DataCharacteristicBase subclass
    /// </summary>
    private DataCharacteristicBase? MapToCharacteristic(string characteristicType, List<AssessmentResultRow> rows)
    {
        if (string.IsNullOrWhiteSpace(characteristicType) || rows.Count == 0)
        {
            return null;
        }

        var type = characteristicType.Trim();
        var firstRow = rows[0];

        switch (type)
        {
            case "RecordCount":
                return ParseRecordCount(firstRow);

            case "ReportedAvailability":
                return ParseReportedAvailability(firstRow);

            case "IntegerRange":
                return ParseIntegerRange(rows);

            case "Completeness":
                return ParseCompleteness(rows);

            case "Distribution":
                return ParseDistribution(rows);

            default:
                throw new NotSupportedException(
                    $"Unknown CharacteristicType: '{type}'. " +
                    $"Supported types: RecordCount, ReportedAvailability, IntegerRange, Completeness, Distribution");
        }
    }

    private RecordCount ParseRecordCount(AssessmentResultRow row)
    {
        if (!int.TryParse(row.Value, out var count))
        {
            throw new FormatException(
                $"Invalid RecordCount value for '{row.DataElementName}': '{row.Value}'. Expected integer.");
        }
        return new RecordCount(count) { Remarks = row.Remarks };
    }

    private ReportedAvailability ParseReportedAvailability(AssessmentResultRow row)
    {
        if (!Enum.TryParse<AvailabilityJudgment>(row.Value, ignoreCase: true, out var judgment))
        {
            throw new FormatException(
                $"Invalid ReportedAvailability value for '{row.DataElementName}': '{row.Value}'. " +
                $"Expected one of: {string.Join(", ", Enum.GetNames<AvailabilityJudgment>())}");
        }
        return new ReportedAvailability(judgment) { Remarks = row.Remarks };
    }

    private IntegerRange ParseIntegerRange(List<AssessmentResultRow> rows)
    {
        if (rows.Count != 2)
        {
            throw new FormatException(
                $"IntegerRange requires exactly 2 rows (Minimum and Maximum), but found {rows.Count} for '{rows[0].DataElementName}'.");
        }

        var minRow = rows.FirstOrDefault(r => r.SubItemLabel?.Equals("Minimum", StringComparison.OrdinalIgnoreCase) == true);
        var maxRow = rows.FirstOrDefault(r => r.SubItemLabel?.Equals("Maximum", StringComparison.OrdinalIgnoreCase) == true);

        if (minRow == null || maxRow == null)
        {
            throw new FormatException(
                $"IntegerRange requires rows with SubItemLabel 'Minimum' and 'Maximum' for '{rows[0].DataElementName}'.");
        }

        if (!int.TryParse(minRow.Value, out var minimum))
        {
            throw new FormatException(
                $"Invalid Minimum value for '{rows[0].DataElementName}': '{minRow.Value}'. Expected integer.");
        }

        if (!int.TryParse(maxRow.Value, out var maximum))
        {
            throw new FormatException(
                $"Invalid Maximum value for '{rows[0].DataElementName}': '{maxRow.Value}'. Expected integer.");
        }

        // Use the label from Remarks, or default to the data element name
        var label = minRow.Remarks ?? maxRow.Remarks ?? rows[0].DataElementName;

        return new IntegerRange(minimum, maximum, label)
        {
            Remarks = string.IsNullOrWhiteSpace(minRow.Remarks) ? maxRow.Remarks : minRow.Remarks
        };
    }

    private Completeness ParseCompleteness(List<AssessmentResultRow> rows)
    {
        if (rows.Count != 2)
        {
            throw new FormatException(
                $"Completeness requires exactly 2 rows (PopulatedRecords and TotalRecords), but found {rows.Count} for '{rows[0].DataElementName}'.");
        }

        var populatedRow = rows.FirstOrDefault(r =>
            r.SubItemLabel?.Equals("PopulatedRecords", StringComparison.OrdinalIgnoreCase) == true);
        var totalRow = rows.FirstOrDefault(r =>
            r.SubItemLabel?.Equals("TotalRecords", StringComparison.OrdinalIgnoreCase) == true);

        if (populatedRow == null || totalRow == null)
        {
            throw new FormatException(
                $"Completeness requires rows with SubItemLabel 'PopulatedRecords' and 'TotalRecords' for '{rows[0].DataElementName}'.");
        }

        if (!int.TryParse(populatedRow.Value, out var populated))
        {
            throw new FormatException(
                $"Invalid PopulatedRecords value for '{rows[0].DataElementName}': '{populatedRow.Value}'. Expected integer.");
        }

        if (!int.TryParse(totalRow.Value, out var total))
        {
            throw new FormatException(
                $"Invalid TotalRecords value for '{rows[0].DataElementName}': '{totalRow.Value}'. Expected integer.");
        }

        // Use the attribute name from Remarks, or default to the data element name
        var attributeName = populatedRow.Remarks ?? totalRow.Remarks ?? rows[0].DataElementName;

        return new Completeness(total, populated, attributeName)
        {
            Remarks = string.IsNullOrWhiteSpace(populatedRow.Remarks) ? totalRow.Remarks : populatedRow.Remarks
        };
    }

    private Distribution ParseDistribution(List<AssessmentResultRow> rows)
    {
        if (rows.Count == 0)
        {
            throw new FormatException("Distribution requires at least 1 row.");
        }

        var counts = new Dictionary<string, int>();

        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.SubItemLabel))
            {
                throw new FormatException(
                    $"Distribution rows must have a SubItemLabel for '{row.DataElementName}'.");
            }

            if (!int.TryParse(row.Value, out var count))
            {
                throw new FormatException(
                    $"Invalid Distribution value for '{row.DataElementName}' item '{row.SubItemLabel}': '{row.Value}'. Expected integer.");
            }

            counts[row.SubItemLabel] = count;
        }

        // Use the label from Remarks of first row, or default to the data element name
        var label = rows.FirstOrDefault(r => !string.IsNullOrWhiteSpace(r.Remarks))?.Remarks
                    ?? rows[0].DataElementName;

        return new Distribution(counts, label)
        {
            Remarks = rows.FirstOrDefault(r => !string.IsNullOrWhiteSpace(r.Remarks))?.Remarks
        };
    }

    /// <summary>
    /// Extracts remarks from the first non-null remark in the group, if any
    /// </summary>
    private string? ExtractRemarks(IGrouping<string, AssessmentResultRow> group)
    {
        return group
            .Select(r => r.Remarks)
            .FirstOrDefault(r => !string.IsNullOrWhiteSpace(r));
    }
}

/// <summary>
/// Represents a single row from the assessment results CSV
/// </summary>
public class AssessmentResultRow
{
    public string DataElementName { get; set; } = string.Empty;
    public string CharacteristicType { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? SubItemLabel { get; set; }
    public string? Remarks { get; set; }
}
