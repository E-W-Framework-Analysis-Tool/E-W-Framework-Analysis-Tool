using System.Text;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;
using Xunit.Abstractions;

namespace EwFrameworkAnalysis.Common.UnitTests.Services;

public class DataSourceAssessmentFileParserTests
{
    private readonly ITestOutputHelper _output;
    private readonly DataSourceAssessmentFileParser _parser;
    private readonly Guid _testDataSourceId = Guid.NewGuid();

    public DataSourceAssessmentFileParserTests(ITestOutputHelper output)
    {
        _output = output;
        _parser = new DataSourceAssessmentFileParser();
    }

    [Fact]
    public void ParseAssessmentStream_WithHeaders_ShouldParseCorrectly()
    {
        // Arrange
        var csvContent = @"DataElementName,CharacteristicType,Value,Remarks
Suspensions and Expulsions (Grades 1 and 2),RecordCount,5,NULL
Suspensions and Expulsions (K-12),RecordCount,1020,""No data found for the following grades: 10,11,12""";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var assessment = _parser.ParseAssessmentStream(
            stream,
            _testDataSourceId,
            hasHeaderRow: true
        );

        // Assert
        Assert.NotNull(assessment);
        Assert.Equal(2, assessment.DataElementAssessments.Count);

        var grades12Assessment = assessment.DataElementAssessments
            .FirstOrDefault(a => a.DataElementName == "Suspensions and Expulsions (Grades 1 and 2)");
        Assert.NotNull(grades12Assessment);
        Assert.Equal(_testDataSourceId, grades12Assessment.DataSourceId);

        var recordCount = grades12Assessment.Characteristics.OfType<RecordCount>().FirstOrDefault();
        Assert.NotNull(recordCount);
        Assert.Equal(5, recordCount.Value);

        var k12Assessment = assessment.DataElementAssessments
            .FirstOrDefault(a => a.DataElementName == "Suspensions and Expulsions (K-12)");
        Assert.NotNull(k12Assessment);

        var k12RecordCount = k12Assessment.Characteristics.OfType<RecordCount>().FirstOrDefault();
        Assert.NotNull(k12RecordCount);
        Assert.Equal(1020, k12RecordCount.Value);
        Assert.Equal("No data found for the following grades: 10,11,12", k12RecordCount.Remarks);

        _output.WriteLine($"Parsed {assessment.DataElementAssessments.Count} data elements");
        _output.WriteLine($"Grades 1-2: {recordCount.Value} records");
        _output.WriteLine($"K-12: {k12RecordCount.Value} records with remarks: {k12RecordCount.Remarks}");
    }

    [Fact]
    public void ParseAssessmentStream_WithoutHeaders_ShouldParseCorrectly()
    {
        // Arrange - same data but without header row
        var csvContent = @"Suspensions and Expulsions (Grades 1 and 2),RecordCount,5,NULL
Suspensions and Expulsions (K-12),RecordCount,1020,""No data found for the following grades: 10,11,12""";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var assessment = _parser.ParseAssessmentStream(
            stream,
            _testDataSourceId,
            hasHeaderRow: false
        );

        // Assert
        Assert.NotNull(assessment);
        Assert.Equal(2, assessment.DataElementAssessments.Count);

        var k12Assessment = assessment.DataElementAssessments
            .FirstOrDefault(a => a.DataElementName == "Suspensions and Expulsions (K-12)");
        Assert.NotNull(k12Assessment);

        var recordCount = k12Assessment.Characteristics.OfType<RecordCount>().FirstOrDefault();
        Assert.NotNull(recordCount);
        Assert.Equal(1020, recordCount.Value);
        Assert.Equal("No data found for the following grades: 10,11,12", recordCount.Remarks);

        _output.WriteLine($"Parsed {assessment.DataElementAssessments.Count} data elements without headers");
        _output.WriteLine($"K-12 with remarks: {recordCount.Remarks}");
    }

    [Fact]
    public void ParseAssessmentStream_WithNullRemark_ShouldHandleCorrectly()
    {
        // Arrange
        var csvContent = @"DataElementName,CharacteristicType,Value,Remarks
Suspensions and Expulsions (Grades 1 and 2),RecordCount,5,NULL
Suspensions and Expulsions (K-12),RecordCount,1020,""No data found for the following grades: 10,11,12""";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var assessment = _parser.ParseAssessmentStream(
            stream,
            _testDataSourceId,
            hasHeaderRow: true
        );

        // Assert
        var element = assessment.DataElementAssessments.FirstOrDefault();
        Assert.NotNull(element);

        var recordCount = element.Characteristics.OfType<RecordCount>().FirstOrDefault();
        Assert.NotNull(recordCount);
        Assert.Equal(5, recordCount.Value);

        // "NULL" string should be treated as a remark, not null
        // If you want it treated as null, the parser would need to handle that
        Assert.Equal("NULL", recordCount.Remarks);

        _output.WriteLine($"Remark value: '{recordCount.Remarks}'");
    }

    [Fact]
    public void ParseAssessmentStream_WithMultipleCharacteristicsForSameElement_ShouldGroupCorrectly()
    {
        // Arrange
        var csvContent = @"DataElementName,CharacteristicType,Value,Remarks
Suspensions (K-12),RecordCount,1020,
Suspensions (K-12),CompletenessScore,87.5,""Based on StudentId, IncidentDate, DisciplinaryActionTaken""";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var assessment = _parser.ParseAssessmentStream(
            stream,
            _testDataSourceId,
            hasHeaderRow: true
        );

        // Assert
        Assert.Single(assessment.DataElementAssessments);

        var element = assessment.DataElementAssessments.First();
        Assert.Equal(2, element.Characteristics.Count);

        var recordCount = element.Characteristics.OfType<RecordCount>().FirstOrDefault();
        var completeness = element.Characteristics.OfType<CompletenessScore>().FirstOrDefault();

        Assert.NotNull(recordCount);
        Assert.NotNull(completeness);
        Assert.Equal(1020, recordCount.Value);
        Assert.Equal(87.5m, completeness.Score);
        Assert.Equal("Based on StudentId, IncidentDate, DisciplinaryActionTaken", completeness.Remarks);

        _output.WriteLine($"Element has {element.Characteristics.Count} characteristics grouped correctly");
    }
}
