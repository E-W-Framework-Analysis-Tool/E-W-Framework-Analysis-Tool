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
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        if (!stream.CanRead) throw new ArgumentException("Stream must be readable", nameof(stream));

        var stats = new AssessmentParsingStats();

        var workingStream = stream;
        if (!stream.CanSeek)
        {
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            workingStream = memoryStream;
        }

        var headerDetected = hasHeaderRow ?? await DetectHeaderAsync(workingStream);
        stats.HasHeaderRow = headerDetected;

        var assessment = assessmentSession ?? new DataSourceAssessment
        {
            Name = "Assessment from stream",
            ConductedAt = DateTime.UtcNow
        };

        var rows = await ReadCsvStreamAsync(workingStream, headerDetected);
        stats.TotalRowsRead = rows.Count;

        if (workingStream != stream)
            await workingStream.DisposeAsync();

        if (rows.Count == 0)
            throw new InvalidOperationException("CSV stream contains no data rows");

        var groupedByElement = rows.GroupBy(r => r.DataElementName);
        stats.TotalDataElements = groupedByElement.Count();

        foreach (var elementGroup in groupedByElement)
        {
            var dataElementName = elementGroup.Key;
            var characteristics = new List<DataCharacteristicBase>();
            var groupedByType = elementGroup.GroupBy(r => r.CharacteristicType);

            foreach (var typeGroup in groupedByType)
            {
                var characteristicType = typeGroup.Key;
                DataCharacteristicBase? characteristic = null;

                try
                {
                    characteristic = MapToCharacteristic(characteristicType, [.. typeGroup], stats);
                }
                catch (Exception ex)
                {
                    stats.ParseErrors.Add(new AssessmentParseError(
                        dataElementName,
                        characteristicType,
                        ex.Message
                    ));
                    continue;
                }

                if (characteristic != null)
                {
                    characteristics.Add(characteristic);
                    stats.CharacteristicsProcessed++;
                }
            }

            if (characteristics.Count > 0)
            {
                assessment.DataElementAssessments.Add(new DataElementAssessment
                {
                    DataElementName = dataElementName,
                    AssessedAt = DateTimeOffset.Now,
                    Characteristics = characteristics,
                    Remarks = ExtractRemarks(elementGroup)
                });
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

    public (DataSourceAssessment Assessment, AssessmentParsingStats Stats) ParseAssessmentStream(
        Stream stream,
        bool? hasHeaderRow = null,
        DataSourceAssessment? assessmentSession = null)
        => ParseAssessmentStreamAsync(stream, hasHeaderRow, assessmentSession).GetAwaiter().GetResult();

    public (DataSourceAssessment Assessment, AssessmentParsingStats Stats) ParseAssessmentFile(
        string csvFilePath,
        bool? hasHeaderRow = null,
        DataSourceAssessment? assessmentSession = null)
    {
        if (!File.Exists(csvFilePath))
            throw new FileNotFoundException($"Assessment file not found: {csvFilePath}");

        using var stream = File.OpenRead(csvFilePath);
        var (assessment, stats) = ParseAssessmentStream(stream, hasHeaderRow, assessmentSession);

        if (assessmentSession == null)
            assessment.Name = $"Assessment from {Path.GetFileName(csvFilePath)}";

        return (assessment, stats);
    }

    private async Task<bool> DetectHeaderAsync(Stream stream)
    {
        var originalPosition = stream.Position;
        try
        {
            using var reader = new StreamReader(stream, leaveOpen: true);
            var firstLine = await reader.ReadLineAsync();

            if (string.IsNullOrWhiteSpace(firstLine)) return true;

            var fields = ParseCsvLine(firstLine);
            if (fields.Length < 3) return true;

            var matchCount = 0;
            for (var i = 0; i < Math.Min(3, fields.Length); i++)
            {
                if (i < _expectedHeaders.Length &&
                    fields[i].Equals(_expectedHeaders[i], StringComparison.OrdinalIgnoreCase))
                    matchCount++;
            }
            return matchCount >= 2;
        }
        finally
        {
            stream.Position = originalPosition;
        }
    }

    private string[] ParseCsvLine(string line)
    {
        var fields = new List<string>();
        var inQuotes = false;
        var currentField = new StringBuilder();

        foreach (var c in line)
        {
            if (c == '"') inQuotes = !inQuotes;
            else if (c == ',' && !inQuotes) { fields.Add(currentField.ToString().Trim()); currentField.Clear(); }
            else currentField.Append(c);
        }

        fields.Add(currentField.ToString().Trim());
        return [.. fields];
    }

    private async Task<List<AssessmentResultRow>> ReadCsvStreamAsync(Stream stream, bool hasHeaderRow)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = hasHeaderRow,
            TrimOptions = TrimOptions.Trim,
            MissingFieldFound = null
        };

        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, config);
        var rows = new List<AssessmentResultRow>();

        if (hasHeaderRow)
        {
            await foreach (var record in csv.GetRecordsAsync<AssessmentResultRow>())
            {
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
            while (await csv.ReadAsync())
            {
                rows.Add(new AssessmentResultRow
                {
                    DataElementName = NormalizeValue(csv.GetField<string>(0)),
                    CharacteristicType = NormalizeValue(csv.GetField<string>(1)),
                    Value = NormalizeValue(csv.GetField<string>(2)),
                    SubItemLabel = NormalizeNullableValue(csv.GetField<string>(3)),
                    Remarks = NormalizeNullableValue(csv.GetField<string>(4))
                });
            }
        }

        return rows;
    }

    /// <summary>
    /// Normalizes CSV field values, treating "NULL" as empty string
    /// </summary>
    private string NormalizeValue(string? value)
        => string.IsNullOrWhiteSpace(value) || value.Equals("NULL", StringComparison.OrdinalIgnoreCase)
            ? string.Empty
            : value.Trim();

    /// <summary>
    /// Normalizes nullable CSV field values, treating "NULL" as null
    /// </summary>
    private string? NormalizeNullableValue(string? value)
        => string.IsNullOrWhiteSpace(value) || value.Equals("NULL", StringComparison.OrdinalIgnoreCase)
            ? null
            : value.Trim();

    /// <summary>
    /// Maps CSV rows for a characteristic type to the appropriate DataCharacteristicBase subclass
    /// </summary>
    private DataCharacteristicBase? MapToCharacteristic(
        string characteristicType,
        List<AssessmentResultRow> rows,
        AssessmentParsingStats stats)
    {
        if (string.IsNullOrWhiteSpace(characteristicType) || rows.Count == 0) return null;

        return characteristicType.Trim() switch
        {
            "RecordCount" => ParseRecordCount(rows[0], stats),
            "ReportedAvailability" => ParseReportedAvailability(rows[0], stats),
            "NumericalRange" => ParseNumericalRange(rows, stats),
            "Completeness" => ParseCompleteness(rows, stats),
            "Distribution" => ParseDistribution(rows, stats),
            var t => throw new NotSupportedException(
                $"Unknown CharacteristicType: '{t}'. " +
                $"Supported types: RecordCount, ReportedAvailability, NumericalRange, Completeness, Distribution")
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
            throw new FormatException($"Invalid RecordCount value: '{row.Value}'. Expected integer.");

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
            throw new FormatException(
                $"Invalid ReportedAvailability value: '{row.Value}'. " +
                $"Expected one of: {string.Join(", ", Enum.GetNames<AvailabilityJudgment>())}");

        return new ReportedAvailability(judgment) { Remarks = row.Remarks };
    }

    private NumericalRange? ParseNumericalRange(List<AssessmentResultRow> rows, AssessmentParsingStats stats)
    {
        if (rows.Count != 2)
            throw new FormatException(
                $"NumericalRange requires exactly 2 rows (Minimum and Maximum), but found {rows.Count}.");

        var minRow = rows.FirstOrDefault(r => r.SubItemLabel?.Equals("Minimum", StringComparison.OrdinalIgnoreCase) == true);
        var maxRow = rows.FirstOrDefault(r => r.SubItemLabel?.Equals("Maximum", StringComparison.OrdinalIgnoreCase) == true);

        if (minRow == null || maxRow == null)
            throw new FormatException("NumericalRange requires rows with SubItemLabel 'Minimum' and 'Maximum'.");

        // Skip if either value is NULL/empty
        if (string.IsNullOrWhiteSpace(minRow.Value) || string.IsNullOrWhiteSpace(maxRow.Value))
        {
            stats.CharacteristicsSkipped++;
            stats.SkippedReasons.Add($"{rows[0].DataElementName} (NumericalRange): Missing Minimum or Maximum value");
            return null;
        }

        if (!decimal.TryParse(minRow.Value, out var minimum))
        {
            throw new FormatException(
                $"Invalid Minimum value for '{rows[0].DataElementName}': '{minRow.Value}'. Expected number.");
        }

        if (!decimal.TryParse(maxRow.Value, out var maximum))
        {
            throw new FormatException(
                $"Invalid Maximum value for '{rows[0].DataElementName}': '{maxRow.Value}'. Expected number.");
        }

        // Use the label from Remarks, or default to the data element name
        var label = minRow.Remarks ?? maxRow.Remarks ?? rows[0].DataElementName;
        return new NumericalRange(minimum, maximum, label)
        {
            Remarks = string.IsNullOrWhiteSpace(minRow.Remarks) ? maxRow.Remarks : minRow.Remarks
        };
    }

    private Completeness? ParseCompleteness(List<AssessmentResultRow> rows, AssessmentParsingStats stats)
    {
        if (rows.Count != 2)
            throw new FormatException(
                $"Completeness requires exactly 2 rows (TotalRecords and PopulatedRecords), but found {rows.Count}.");

        var populatedRow = rows.FirstOrDefault(r => r.SubItemLabel?.Equals("PopulatedRecords", StringComparison.OrdinalIgnoreCase) == true);
        var totalRow = rows.FirstOrDefault(r => r.SubItemLabel?.Equals("TotalRecords", StringComparison.OrdinalIgnoreCase) == true);

        if (populatedRow == null || totalRow == null)
            throw new FormatException("Completeness requires rows with SubItemLabel 'PopulatedRecords' and 'TotalRecords'.");

        // Skip if either value is NULL/empty
        if (string.IsNullOrWhiteSpace(populatedRow.Value) || string.IsNullOrWhiteSpace(totalRow.Value))
        {
            stats.CharacteristicsSkipped++;
            stats.SkippedReasons.Add($"{rows[0].DataElementName} (Completeness): Missing PopulatedRecords or TotalRecords value");
            return null;
        }

        if (!int.TryParse(populatedRow.Value, out var populated))
            throw new FormatException($"Invalid PopulatedRecords value: '{populatedRow.Value}'. Expected integer.");
        if (!int.TryParse(totalRow.Value, out var total))
            throw new FormatException($"Invalid TotalRecords value: '{totalRow.Value}'. Expected integer.");

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
            throw new FormatException("Distribution requires at least 1 row.");

        var counts = new Dictionary<string, int>();
        var skippedItems = 0;

        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.SubItemLabel))
                throw new FormatException($"Distribution row is missing a SubItemLabel.");

            if (string.IsNullOrWhiteSpace(row.Value)) { skippedItems++; continue; }
            // Skip rows with NULL/empty values

            if (!int.TryParse(row.Value, out var count))
                throw new FormatException(
                    $"Invalid Distribution value for item '{row.SubItemLabel}': '{row.Value}'. Expected integer.");

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
            stats.SkippedReasons.Add($"{rows[0].DataElementName} (Distribution): {skippedItems} item(s) skipped due to missing values");

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
        => group.Select(r => r.Remarks).FirstOrDefault(r => !string.IsNullOrWhiteSpace(r));
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
/// Represents a parse error on a single row from the assessment results CSV
/// </summary>
public class AssessmentParseError
{
    public string DataElementName { get; }
    public string CharacteristicType { get; }
    public string Message { get; }

    public AssessmentParseError(string dataElementName, string characteristicType, string message)
    {
        DataElementName = dataElementName;
        CharacteristicType = characteristicType;
        Message = message;
    }
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
    public List<AssessmentParseError> ParseErrors { get; set; } = [];

    public bool HasWarnings => DataElementsSkipped > 0 || CharacteristicsSkipped > 0;
    public bool HasErrors => ParseErrors.Count > 0;
}
