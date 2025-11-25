using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Services;

/// <summary>
/// Parses assessment result CSV files and maps to DataSourceAssessment objects
/// </summary>
public class DataSourceAssessmentFileParser
{
    private static readonly string[] _expectedHeaders =
    [
        "DataElementName",
        "CharacteristicType",
        "Value",
        "SubItemLabel",
        "Remarks"
    ];

    /// <summary>
    /// Parses a CSV stream containing assessment results (async version for Blazor compatibility)
    /// </summary>
    /// <param name="stream">Stream containing CSV data</param>
    /// <param name="dataSourceId">The ID of the data source being assessed</param>
    /// <param name="hasHeaderRow">Whether the CSV includes a header row (null for auto-detection, default: null)</param>
    /// <param name="assessmentSession">Optional existing assessment session to add results to</param>
    /// <returns>A tuple containing the DataSourceAssessment and parsing statistics</returns>
    public async Task<(DataSourceAssessment Assessment, AssessmentParsingStats Stats)> ParseAssessmentStreamAsync(
        Stream stream,
        bool? hasHeaderRow = null,
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

        // Initialize stats
        var stats = new AssessmentParsingStats();

        // For non-seekable streams (like Blazor file uploads), buffer the content
        var workingStream = stream;
        if (!stream.CanSeek)
        {
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            workingStream = memoryStream;
        }

        // Auto-detect header if not specified
        var headerDetected = hasHeaderRow ?? await DetectHeaderAsync(workingStream);
        stats.HasHeaderRow = headerDetected;

        // Create or use existing assessment session
        var assessment = assessmentSession ?? new DataSourceAssessment
        {
            Name = $"Assessment from stream",
            ConductedAt = DateTime.UtcNow
        };

        // Read and parse CSV
        var rows = await ReadCsvStreamAsync(workingStream, headerDetected);
        stats.TotalRowsRead = rows.Count;

        // Clean up the buffered stream if we created one
        if (workingStream != stream)
        {
            await workingStream.DisposeAsync();
        }

        if (rows.Count == 0)
        {
            throw new InvalidOperationException("CSV stream contains no data rows");
        }

        // Group by DataElementName
        var groupedByElement = rows.GroupBy(r => r.DataElementName);
        stats.TotalDataElements = groupedByElement.Count();

        foreach (var elementGroup in groupedByElement)
        {
            var dataElementName = elementGroup.Key;
            var characteristics = new List<DataCharacteristicBase>();

            // Further group by CharacteristicType within each element
            var groupedByType = elementGroup.GroupBy(r => r.CharacteristicType);

            foreach (var typeGroup in groupedByType)
            {
                var characteristicType = typeGroup.Key;
                var characteristic = MapToCharacteristic(characteristicType, [.. typeGroup], stats);

                if (characteristic != null)
                {
                    characteristics.Add(characteristic);
                    stats.CharacteristicsProcessed++;
                }
            }

            // Only add data elements that have at least one valid characteristic
            if (characteristics.Count > 0)
            {
                var elementAssessment = new DataElementAssessment
                {
                    DataElementName = dataElementName,
                    AssessedAt = DateTimeOffset.Now,
                    Characteristics = characteristics,
                    Remarks = ExtractRemarks(elementGroup)
                };

                assessment.DataElementAssessments.Add(elementAssessment);
                stats.DataElementsProcessed++;
            }
            else
            {
                stats.DataElementsSkipped++;
                stats.SkippedDataElements.Add(dataElementName);
            }
        }

        return (assessment, stats);
    }

    /// <summary>
    /// Detects whether the CSV has a header row by examining the first row
    /// </summary>
    private async Task<bool> DetectHeaderAsync(Stream stream)
    {
        // Remember position to reset after detection
        var originalPosition = stream.Position;

        try
        {
            using var reader = new StreamReader(stream, leaveOpen: true);
            var firstLine = await reader.ReadLineAsync();

            if (string.IsNullOrWhiteSpace(firstLine))
            {
                return true; // Default to true if empty
            }

            // Parse the first line
            var fields = ParseCsvLine(firstLine);

            if (fields.Length < 3)
            {
                return true; // Not enough fields, assume header
            }

            // Check if first row matches expected headers
            // We'll check the first 3 required columns for a match
            var matchCount = 0;
            for (var i = 0; i < Math.Min(3, fields.Length); i++)
            {
                if (i < _expectedHeaders.Length &&
                    fields[i].Equals(_expectedHeaders[i], StringComparison.OrdinalIgnoreCase))
                {
                    matchCount++;
                }
            }

            // If at least 2 of the first 3 columns match expected headers, it's a header row
            return matchCount >= 2;
        }
        finally
        {
            // Reset stream position
            stream.Position = originalPosition;
        }
    }

    /// <summary>
    /// Simple CSV line parser for header detection
    /// </summary>
    private string[] ParseCsvLine(string line)
    {
        var fields = new List<string>();
        var inQuotes = false;
        var currentField = new StringBuilder();

        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];

            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(currentField.ToString().Trim());
                currentField.Clear();
            }
            else
            {
                currentField.Append(c);
            }
        }

        fields.Add(currentField.ToString().Trim());
        return [.. fields];
    }

    /// <summary>
    /// Parses a CSV stream containing assessment results (synchronous version for backward compatibility)
    /// </summary>
    public (DataSourceAssessment Assessment, AssessmentParsingStats Stats) ParseAssessmentStream(
        Stream stream,
        bool? hasHeaderRow = null,
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
    /// <param name="hasHeaderRow">Whether the CSV includes a header row (null for auto-detection, default: null)</param>
    /// <param name="assessmentSession">Optional existing assessment session to add results to</param>
    /// <returns>A tuple containing the DataSourceAssessment and parsing statistics</returns>
    public (DataSourceAssessment Assessment, AssessmentParsingStats Stats) ParseAssessmentFile(
        string csvFilePath,
        bool? hasHeaderRow = null,
        DataSourceAssessment? assessmentSession = null)
    {
        if (!File.Exists(csvFilePath))
        {
            throw new FileNotFoundException($"Assessment file not found: {csvFilePath}");
        }

        using var stream = File.OpenRead(csvFilePath);

        var (assessment, stats) = ParseAssessmentStream(stream, hasHeaderRow, assessmentSession);

        // Update name if creating new assessment
        if (assessmentSession == null)
        {
            assessment.Name = $"Assessment from {Path.GetFileName(csvFilePath)}";
        }

        return (assessment, stats);
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

        var rows = new List<AssessmentResultRow>();

        if (hasHeaderRow)
        {
            // Standard parsing with headers - but we still need to normalize values
            await foreach (var record in csv.GetRecordsAsync<AssessmentResultRow>())
            {
                // Normalize NULL strings to proper empty/null values
                record.DataElementName = NormalizeValue(record.DataElementName);
                record.CharacteristicType = NormalizeValue(record.CharacteristicType);
                record.Value = NormalizeValue(record.Value);
                record.SubItemLabel = NormalizeNullableValue(record.SubItemLabel);
                record.Remarks = NormalizeNullableValue(record.Remarks);

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
                    DataElementName = NormalizeValue(csv.GetField<string>(0)),
                    CharacteristicType = NormalizeValue(csv.GetField<string>(1)),
                    Value = NormalizeValue(csv.GetField<string>(2)),
                    SubItemLabel = NormalizeNullableValue(csv.GetField<string>(3)),
                    Remarks = NormalizeNullableValue(csv.GetField<string>(4))
                };
                rows.Add(row);
            }
        }

        return rows;
    }

    /// <summary>
    /// Normalizes CSV field values, treating "NULL" as empty string
    /// </summary>
    private string NormalizeValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Equals("NULL", StringComparison.OrdinalIgnoreCase))
        {
            return string.Empty;
        }
        return value.Trim();
    }

    /// <summary>
    /// Normalizes nullable CSV field values, treating "NULL" as null
    /// </summary>
    private string? NormalizeNullableValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Equals("NULL", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }
        return value.Trim();
    }

    /// <summary>
    /// Maps CSV rows for a characteristic type to the appropriate DataCharacteristicBase subclass
    /// </summary>
    private DataCharacteristicBase? MapToCharacteristic(
        string characteristicType,
        List<AssessmentResultRow> rows,
        AssessmentParsingStats stats)
    {
        if (string.IsNullOrWhiteSpace(characteristicType) || rows.Count == 0)
        {
            return null;
        }

        var type = characteristicType.Trim();
        var firstRow = rows[0];

        return type switch
        {
            "RecordCount" => ParseRecordCount(firstRow, stats),
            "ReportedAvailability" => ParseReportedAvailability(firstRow, stats),
            "IntegerRange" => ParseIntegerRange(rows, stats),
            "Completeness" => ParseCompleteness(rows, stats),
            "Distribution" => ParseDistribution(rows, stats),
            _ => throw new NotSupportedException(
                $"Unknown CharacteristicType: '{type}'. " +
                $"Supported types: RecordCount, ReportedAvailability, IntegerRange, Completeness, Distribution")
        };
    }

    private RecordCount? ParseRecordCount(AssessmentResultRow row, AssessmentParsingStats stats)
    {
        if (string.IsNullOrWhiteSpace(row.Value))
        {
            stats.CharacteristicsSkipped++;
            stats.SkippedReasons.Add($"{row.DataElementName} (RecordCount): Missing value");
            return null;
        }

        if (!int.TryParse(row.Value, out var count))
        {
            throw new FormatException(
                $"Invalid RecordCount value for '{row.DataElementName}': '{row.Value}'. Expected integer.");
        }
        return new RecordCount(count) { Remarks = row.Remarks };
    }

    private ReportedAvailability? ParseReportedAvailability(AssessmentResultRow row, AssessmentParsingStats stats)
    {
        if (string.IsNullOrWhiteSpace(row.Value))
        {
            stats.CharacteristicsSkipped++;
            stats.SkippedReasons.Add($"{row.DataElementName} (ReportedAvailability): Missing value");
            return null;
        }

        if (!Enum.TryParse<AvailabilityJudgment>(row.Value, ignoreCase: true, out var judgment))
        {
            throw new FormatException(
                $"Invalid ReportedAvailability value for '{row.DataElementName}': '{row.Value}'. " +
                $"Expected one of: {string.Join(", ", Enum.GetNames<AvailabilityJudgment>())}");
        }
        return new ReportedAvailability(judgment) { Remarks = row.Remarks };
    }

    private IntegerRange? ParseIntegerRange(List<AssessmentResultRow> rows, AssessmentParsingStats stats)
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

        // Skip if either value is NULL/empty
        if (string.IsNullOrWhiteSpace(minRow.Value) || string.IsNullOrWhiteSpace(maxRow.Value))
        {
            stats.CharacteristicsSkipped++;
            stats.SkippedReasons.Add($"{rows[0].DataElementName} (IntegerRange): Missing Minimum or Maximum value");
            return null;
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

    private Completeness? ParseCompleteness(List<AssessmentResultRow> rows, AssessmentParsingStats stats)
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

        // Skip if either value is NULL/empty
        if (string.IsNullOrWhiteSpace(populatedRow.Value) || string.IsNullOrWhiteSpace(totalRow.Value))
        {
            stats.CharacteristicsSkipped++;
            stats.SkippedReasons.Add($"{rows[0].DataElementName} (Completeness): Missing PopulatedRecords or TotalRecords value");
            return null;
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

    private Distribution? ParseDistribution(List<AssessmentResultRow> rows, AssessmentParsingStats stats)
    {
        if (rows.Count == 0)
        {
            throw new FormatException("Distribution requires at least 1 row.");
        }

        var counts = new Dictionary<string, int>();
        var skippedItems = 0;

        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.SubItemLabel))
            {
                throw new FormatException(
                    $"Distribution rows must have a SubItemLabel for '{row.DataElementName}'.");
            }

            // Skip rows with NULL/empty values
            if (string.IsNullOrWhiteSpace(row.Value))
            {
                skippedItems++;
                continue;
            }

            if (!int.TryParse(row.Value, out var count))
            {
                throw new FormatException(
                    $"Invalid Distribution value for '{row.DataElementName}' item '{row.SubItemLabel}': '{row.Value}'. Expected integer.");
            }

            counts[row.SubItemLabel] = count;
        }

        // If no valid rows after filtering, return null
        if (counts.Count == 0)
        {
            stats.CharacteristicsSkipped++;
            stats.SkippedReasons.Add($"{rows[0].DataElementName} (Distribution): All items had missing values");
            return null;
        }

        // Track partially skipped distributions
        if (skippedItems > 0)
        {
            stats.SkippedReasons.Add($"{rows[0].DataElementName} (Distribution): {skippedItems} item(s) skipped due to missing values");
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

/// <summary>
/// Statistics about the assessment parsing process
/// </summary>
public class AssessmentParsingStats
{
    public bool HasHeaderRow { get; set; }
    public int TotalRowsRead { get; set; }
    public int TotalDataElements { get; set; }
    public int DataElementsProcessed { get; set; }
    public int DataElementsSkipped { get; set; }
    public int CharacteristicsProcessed { get; set; }
    public int CharacteristicsSkipped { get; set; }
    public List<string> SkippedDataElements { get; set; } = [];
    public List<string> SkippedReasons { get; set; } = [];

    public bool HasWarnings => DataElementsSkipped > 0 || CharacteristicsSkipped > 0;
}
