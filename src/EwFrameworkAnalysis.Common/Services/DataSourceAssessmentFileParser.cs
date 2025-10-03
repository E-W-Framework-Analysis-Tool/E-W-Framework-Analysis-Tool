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
    /// Parses a CSV stream containing assessment results
    /// </summary>
    /// <param name="stream">Stream containing CSV data</param>
    /// <param name="dataSourceId">The ID of the data source being assessed</param>
    /// <param name="hasHeaderRow">Whether the CSV includes a header row (default: true)</param>
    /// <param name="assessmentSession">Optional existing assessment session to add results to</param>
    /// <returns>A DataSourceAssessment with all parsed results</returns>
    public DataSourceAssessment ParseAssessmentStream(
        Stream stream,
        Guid dataSourceId,
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
        var rows = ReadCsvStream(stream, hasHeaderRow);

        if (rows.Count == 0)
        {
            throw new InvalidOperationException("CSV stream contains no data rows");
        }

        // Group by DataElementName
        var groupedResults = rows.GroupBy(r => r.DataElementName);

        foreach (var group in groupedResults)
        {
            var dataElementName = group.Key;
            var characteristics = new List<DataCharacteristicBase>();

            foreach (var row in group)
            {
                var characteristic = MapToCharacteristic(row);
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
                DataSourceId = dataSourceId,
                AssessedAt = DateTime.UtcNow,
                Characteristics = characteristics,
                Remarks = ExtractRemarks(group)
            };

            assessment.DataElementAssessments.Add(elementAssessment);
        }

        return assessment;
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
        Guid dataSourceId,
        bool hasHeaderRow = true,
        DataSourceAssessment? assessmentSession = null)
    {
        if (!File.Exists(csvFilePath))
        {
            throw new FileNotFoundException($"Assessment file not found: {csvFilePath}");
        }

        using var stream = File.OpenRead(csvFilePath);

        var assessment = ParseAssessmentStream(stream, dataSourceId, hasHeaderRow, assessmentSession);

        // Update name if creating new assessment
        if (assessmentSession == null)
        {
            assessment.Name = $"Assessment from {Path.GetFileName(csvFilePath)}";
        }

        return assessment;
    }

    /// <summary>
    /// Reads CSV stream and returns parsed rows
    /// </summary>
    private List<AssessmentResultRow> ReadCsvStream(Stream stream, bool hasHeaderRow)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = hasHeaderRow,
            TrimOptions = TrimOptions.Trim,
            MissingFieldFound = null // Ignore missing fields
        };

        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, config);

        List<AssessmentResultRow> rows;

        if (hasHeaderRow)
        {
            // Standard parsing with headers
            rows = [.. csv.GetRecords<AssessmentResultRow>()];
        }
        else
        {
            // Manual parsing without headers - assume column order
            // DataElementName, CharacteristicType, Value, Remarks
            rows = [];

            while (csv.Read())
            {
                var row = new AssessmentResultRow
                {
                    DataElementName = csv.GetField<string>(0) ?? string.Empty,
                    CharacteristicType = csv.GetField<string>(1) ?? string.Empty,
                    Value = csv.GetField<string>(2) ?? string.Empty,
                    Remarks = csv.GetField<string>(3)
                };
                rows.Add(row);
            }
        }

        return rows;
    }

    /// <summary>
    /// Maps a CSV row to the appropriate DataCharacteristicBase subclass
    /// </summary>
    private DataCharacteristicBase? MapToCharacteristic(AssessmentResultRow row)
    {
        if (string.IsNullOrWhiteSpace(row.CharacteristicType))
        {
            return null;
        }

        var type = row.CharacteristicType.Trim();

        switch (type)
        {
            case "RecordCount":
                if (!int.TryParse(row.Value, out var count))
                {
                    throw new FormatException(
                        $"Invalid RecordCount value for '{row.DataElementName}': '{row.Value}'. Expected integer.");
                }
                return new RecordCount(count) { Remarks = row.Remarks };

            case "ReportedAvailability":
                if (!Enum.TryParse<AvailabilityJudgment>(row.Value, ignoreCase: true, out var judgment))
                {
                    throw new FormatException(
                        $"Invalid ReportedAvailability value for '{row.DataElementName}': '{row.Value}'. " +
                        $"Expected one of: {string.Join(", ", Enum.GetNames<AvailabilityJudgment>())}");
                }
                return new ReportedAvailability(judgment) { Remarks = row.Remarks };

            //case "Completeness":
            //    if (!decimal.TryParse(row.Value, out var score))
            //    {
            //        throw new FormatException(
            //            $"Invalid CompletenessScore value for '{row.DataElementName}': '{row.Value}'. Expected decimal.");
            //    }
            //    return new Completeness(score) { Remarks = row.Remarks };

            default:
                throw new NotSupportedException(
                    $"Unknown CharacteristicType: '{type}'. " +
                    $"Supported types: RecordCount, ReportedAvailability, CompletenessScore");
        }
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
    public string? Remarks { get; set; }
}
