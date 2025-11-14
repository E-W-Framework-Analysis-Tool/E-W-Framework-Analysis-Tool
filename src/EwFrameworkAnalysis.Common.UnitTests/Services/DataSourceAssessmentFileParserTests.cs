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
    public void ParseAssessmentStream_WithHeaders_ShouldParseRecordCountCorrectly()
    {
        // Arrange
        var csvContent = @"DataElementName,CharacteristicType,Value,SubItemLabel,Remarks
Suspensions and Expulsions (Grades 1 and 2),RecordCount,5,,NULL
Suspensions and Expulsions (K-12),RecordCount,1020,,""No data found for the following grades: 10,11,12""";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var assessment = _parser.ParseAssessmentStream(
            stream,
            hasHeaderRow: true
        );

        // Assert
        Assert.NotNull(assessment);
        Assert.Equal(2, assessment.DataElementAssessments.Count);

        var grades12Assessment = assessment.DataElementAssessments
            .FirstOrDefault(a => a.DataElementName == "Suspensions and Expulsions (Grades 1 and 2)");
        Assert.NotNull(grades12Assessment);

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
    public void ParseAssessmentStream_WithoutHeaders_ShouldParseRecordCountCorrectly()
    {
        // Arrange - same data but without header row (5 columns now)
        var csvContent = @"Suspensions and Expulsions (Grades 1 and 2),RecordCount,5,,NULL
Suspensions and Expulsions (K-12),RecordCount,1020,,""No data found for the following grades: 10,11,12""";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var assessment = _parser.ParseAssessmentStream(
            stream,
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
    public void ParseAssessmentStream_WithIntegerRange_ShouldParseCorrectly()
    {
        // Arrange
        var csvContent = @"DataElementName,CharacteristicType,Value,SubItemLabel,Remarks
Student Age Range,IntegerRange,5,Minimum,Age of enrolled students
Student Age Range,IntegerRange,21,Maximum,";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var assessment = _parser.ParseAssessmentStream(
            stream,
            hasHeaderRow: true
        );

        // Assert
        Assert.Single(assessment.DataElementAssessments);

        var element = assessment.DataElementAssessments.First();
        Assert.Equal("Student Age Range", element.DataElementName);

        var range = element.Characteristics.OfType<IntegerRange>().FirstOrDefault();
        Assert.NotNull(range);
        Assert.Equal(5, range.Minimum);
        Assert.Equal(21, range.Maximum);
        Assert.Equal("Age of enrolled students", range.Label);
        Assert.Equal("Age of enrolled students", range.Remarks);

        _output.WriteLine($"Parsed IntegerRange: {range.Minimum} to {range.Maximum} ({range.Label})");
    }

    [Fact]
    public void ParseAssessmentStream_WithCompleteness_ShouldParseCorrectly()
    {
        // Arrange
        var csvContent = @"DataElementName,CharacteristicType,Value,SubItemLabel,Remarks
Student Contact Information,Completeness,850,PopulatedRecords,ElectronicMailAddress
Student Contact Information,Completeness,1000,TotalRecords,";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var assessment = _parser.ParseAssessmentStream(
            stream,
            hasHeaderRow: true
        );

        // Assert
        Assert.Single(assessment.DataElementAssessments);

        var element = assessment.DataElementAssessments.First();
        Assert.Equal("Student Contact Information", element.DataElementName);

        var completeness = element.Characteristics.OfType<Completeness>().FirstOrDefault();
        Assert.NotNull(completeness);
        Assert.Equal(1000, completeness.TotalRecords);
        Assert.Equal(850, completeness.PopulatedRecords);
        Assert.Equal("ElectronicMailAddress", completeness.AttributeName);
        Assert.Equal(85.0m, completeness.Percentage);
        Assert.Equal("ElectronicMailAddress", completeness.Remarks);

        _output.WriteLine($"Parsed Completeness: {completeness.PopulatedRecords}/{completeness.TotalRecords} " +
                         $"({completeness.Percentage:F1}%) for {completeness.AttributeName}");
    }

    [Fact]
    public void ParseAssessmentStream_WithDistribution_ShouldParseCorrectly()
    {
        // Arrange
        var csvContent = @"DataElementName,CharacteristicType,Value,SubItemLabel,Remarks
Discipline Action Types,Distribution,450,In-School Suspension,Distribution of disciplinary actions taken
Discipline Action Types,Distribution,320,Out-of-School Suspension,
Discipline Action Types,Distribution,125,Expulsion with Services,
Discipline Action Types,Distribution,95,Expulsion without Services,";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var assessment = _parser.ParseAssessmentStream(
            stream,
            hasHeaderRow: true
        );

        // Assert
        Assert.Single(assessment.DataElementAssessments);

        var element = assessment.DataElementAssessments.First();
        Assert.Equal("Discipline Action Types", element.DataElementName);

        var distribution = element.Characteristics.OfType<Distribution>().FirstOrDefault();
        Assert.NotNull(distribution);
        Assert.Equal("Distribution of disciplinary actions taken", distribution.Label);
        Assert.Equal(4, distribution.Counts.Count);
        Assert.Equal(450, distribution.Counts["In-School Suspension"]);
        Assert.Equal(320, distribution.Counts["Out-of-School Suspension"]);
        Assert.Equal(125, distribution.Counts["Expulsion with Services"]);
        Assert.Equal(95, distribution.Counts["Expulsion without Services"]);
        Assert.Equal(990, distribution.TotalCount);

        _output.WriteLine($"Parsed Distribution with {distribution.Counts.Count} categories:");
        foreach (var kvp in distribution.Counts.OrderByDescending(x => x.Value))
        {
            _output.WriteLine($"  {distribution.FormatItem(kvp.Key, kvp.Value)}");
        }
    }

    [Fact]
    public void ParseAssessmentStream_WithReportedAvailability_ShouldParseCorrectly()
    {
        // Arrange
        var csvContent = @"DataElementName,CharacteristicType,Value,SubItemLabel,Remarks
Special Education Services,ReportedAvailability,Available,,Availability based on record count thresholds
Gifted and Talented Programs,ReportedAvailability,PartiallyAvailable,,Only available for grades 3-8";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var assessment = _parser.ParseAssessmentStream(
            stream,
            hasHeaderRow: true
        );

        // Assert
        Assert.Equal(2, assessment.DataElementAssessments.Count);

        var specEdAssessment = assessment.DataElementAssessments
            .FirstOrDefault(a => a.DataElementName == "Special Education Services");
        Assert.NotNull(specEdAssessment);

        var availability = specEdAssessment.Characteristics.OfType<ReportedAvailability>().FirstOrDefault();
        Assert.NotNull(availability);
        Assert.Equal(AvailabilityJudgment.Available, availability.Value);
        Assert.Equal("Availability based on record count thresholds", availability.Remarks);

        var giftedAssessment = assessment.DataElementAssessments
            .FirstOrDefault(a => a.DataElementName == "Gifted and Talented Programs");
        Assert.NotNull(giftedAssessment);

        var giftedAvailability = giftedAssessment.Characteristics.OfType<ReportedAvailability>().FirstOrDefault();
        Assert.NotNull(giftedAvailability);
        Assert.Equal(AvailabilityJudgment.PartiallyAvailable, giftedAvailability.Value);

        _output.WriteLine($"Special Ed: {availability.Value}");
        _output.WriteLine($"Gifted: {giftedAvailability.Value}");
    }

    [Fact]
    public void ParseAssessmentStream_WithMultipleCharacteristicsForSameElement_ShouldGroupCorrectly()
    {
        // Arrange
        var csvContent = @"DataElementName,CharacteristicType,Value,SubItemLabel,Remarks
Suspensions (K-12),RecordCount,1020,,
Suspensions (K-12),Completeness,850,PopulatedRecords,IncidentDate
Suspensions (K-12),Completeness,1020,TotalRecords,";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var assessment = _parser.ParseAssessmentStream(
            stream,
            hasHeaderRow: true
        );

        // Assert
        Assert.Single(assessment.DataElementAssessments);

        var element = assessment.DataElementAssessments.First();
        Assert.Equal(2, element.Characteristics.Count);

        var recordCount = element.Characteristics.OfType<RecordCount>().FirstOrDefault();
        var completeness = element.Characteristics.OfType<Completeness>().FirstOrDefault();

        Assert.NotNull(recordCount);
        Assert.NotNull(completeness);
        Assert.Equal(1020, recordCount.Value);
        Assert.Equal(850, completeness.PopulatedRecords);
        Assert.Equal(1020, completeness.TotalRecords);
        Assert.Equal(83.3m, Math.Round(completeness.Percentage, 1));
        Assert.Equal("IncidentDate", completeness.AttributeName);

        _output.WriteLine($"Element has {element.Characteristics.Count} characteristics grouped correctly");
        _output.WriteLine($"  RecordCount: {recordCount.Value}");
        _output.WriteLine($"  Completeness: {completeness.Percentage:F1}% for {completeness.AttributeName}");
    }

    [Fact]
    public void ParseAssessmentStream_WithComplexScenario_ShouldParseAllTypesCorrectly()
    {
        // Arrange - Kitchen sink test with all characteristic types
        var csvContent = @"DataElementName,CharacteristicType,Value,SubItemLabel,Remarks
Student Demographics,RecordCount,15420,,Total student records
Student Demographics,IntegerRange,5,Minimum,Student Age Range
Student Demographics,IntegerRange,21,Maximum,
Student Contact Info,Completeness,12500,PopulatedRecords,ElectronicMailAddress
Student Contact Info,Completeness,15420,TotalRecords,
Discipline Types,Distribution,5200,In-School Suspension,Distribution of discipline actions
Discipline Types,Distribution,3100,Out-of-School Suspension,
Discipline Types,Distribution,890,Expulsion,
Special Programs,ReportedAvailability,Available,,Sufficient data for reporting";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var assessment = _parser.ParseAssessmentStream(
            stream,
            hasHeaderRow: true
        );

        // Assert
        Assert.Equal(4, assessment.DataElementAssessments.Count);

        // Verify RecordCount
        var demographics = assessment.DataElementAssessments
            .First(a => a.DataElementName == "Student Demographics");
        Assert.Equal(2, demographics.Characteristics.Count); // RecordCount + IntegerRange
        Assert.NotNull(demographics.Characteristics.OfType<RecordCount>().FirstOrDefault());
        Assert.NotNull(demographics.Characteristics.OfType<IntegerRange>().FirstOrDefault());

        // Verify Completeness
        var contactInfo = assessment.DataElementAssessments
            .First(a => a.DataElementName == "Student Contact Info");
        var completeness = contactInfo.Characteristics.OfType<Completeness>().First();
        Assert.Equal(81.1m, Math.Round(completeness.Percentage, 1));

        // Verify Distribution
        var disciplineTypes = assessment.DataElementAssessments
            .First(a => a.DataElementName == "Discipline Types");
        var distribution = disciplineTypes.Characteristics.OfType<Distribution>().First();
        Assert.Equal(3, distribution.Counts.Count);
        Assert.Equal(9190, distribution.TotalCount);

        // Verify ReportedAvailability
        var specialPrograms = assessment.DataElementAssessments
            .First(a => a.DataElementName == "Special Programs");
        var availability = specialPrograms.Characteristics.OfType<ReportedAvailability>().First();
        Assert.Equal(AvailabilityJudgment.Available, availability.Value);

        _output.WriteLine($"Successfully parsed complex assessment with {assessment.DataElementAssessments.Count} elements");
    }

    [Fact]
    public void ParseAssessmentStream_WithInvalidIntegerRange_ShouldThrowException()
    {
        // Arrange - Missing Maximum row
        var csvContent = @"DataElementName,CharacteristicType,Value,SubItemLabel,Remarks
Student Age Range,IntegerRange,5,Minimum,Age of enrolled students";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act & Assert
        var exception = Assert.Throws<FormatException>(() =>
            _parser.ParseAssessmentStream(stream, hasHeaderRow: true)
        );

        Assert.Contains("IntegerRange requires exactly 2 rows", exception.Message);
        _output.WriteLine($"Correctly threw exception: {exception.Message}");
    }

    [Fact]
    public void ParseAssessmentStream_WithInvalidCompleteness_ShouldThrowException()
    {
        // Arrange - Missing TotalRecords row
        var csvContent = @"DataElementName,CharacteristicType,Value,SubItemLabel,Remarks
Student Contact Information,Completeness,850,PopulatedRecords,ElectronicMailAddress";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act & Assert
        var exception = Assert.Throws<FormatException>(() =>
            _parser.ParseAssessmentStream(stream, hasHeaderRow: true)
        );

        Assert.Contains("Completeness requires exactly 2 rows", exception.Message);
        _output.WriteLine($"Correctly threw exception: {exception.Message}");
    }

    [Fact]
    public void ParseAssessmentStream_WithDistributionMissingSubItemLabel_ShouldThrowException()
    {
        // Arrange - Distribution row without SubItemLabel
        var csvContent = @"DataElementName,CharacteristicType,Value,SubItemLabel,Remarks
Discipline Types,Distribution,450,,Distribution of discipline actions";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act & Assert
        var exception = Assert.Throws<FormatException>(() =>
            _parser.ParseAssessmentStream(stream, hasHeaderRow: true)
        );

        Assert.Contains("Distribution rows must have a SubItemLabel", exception.Message);
        _output.WriteLine($"Correctly threw exception: {exception.Message}");
    }

    [Fact]
    public void ParseAssessmentStream_WithInvalidRecordCountValue_ShouldThrowException()
    {
        // Arrange - Non-integer value for RecordCount
        var csvContent = @"DataElementName,CharacteristicType,Value,SubItemLabel,Remarks
Suspensions,RecordCount,NOT_A_NUMBER,,";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act & Assert
        var exception = Assert.Throws<FormatException>(() =>
            _parser.ParseAssessmentStream(stream, hasHeaderRow: true)
        );

        Assert.Contains("Invalid RecordCount value", exception.Message);
        _output.WriteLine($"Correctly threw exception: {exception.Message}");
    }

    [Fact]
    public void ParseAssessmentStream_WithInvalidAvailabilityValue_ShouldThrowException()
    {
        // Arrange - Invalid enum value
        var csvContent = @"DataElementName,CharacteristicType,Value,SubItemLabel,Remarks
Special Education Services,ReportedAvailability,InvalidValue,,";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act & Assert
        var exception = Assert.Throws<FormatException>(() =>
            _parser.ParseAssessmentStream(stream, hasHeaderRow: true)
        );

        Assert.Contains("Invalid ReportedAvailability value", exception.Message);
        _output.WriteLine($"Correctly threw exception: {exception.Message}");
    }
}
