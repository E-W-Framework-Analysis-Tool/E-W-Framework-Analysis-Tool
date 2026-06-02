using System.Text;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.UnitTests.Services;

public class DataSourceAssessmentFileParserTests
{
    private readonly ITestOutputHelper _output;
    private readonly DataSourceAssessmentFileParser _parser;

    public DataSourceAssessmentFileParserTests(ITestOutputHelper output)
    {
        _output = output;
        _parser = new DataSourceAssessmentFileParser();
    }

    [Fact]
    public void ParseAssessmentStream_WithHeaders_ShouldParseRecordCountCorrectly()
    {
        var csvContent = @"DataElementName,CharacteristicType,Value,SubItemLabel,Remarks
Suspensions and Expulsions (Grades 1 and 2),RecordCount,5,,NULL
Suspensions and Expulsions (K-12),RecordCount,1020,,""No data found for the following grades: 10,11,12""";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var (assessment, stats) = _parser.ParseAssessmentStream(stream, hasHeaderRow: true);

        Assert.NotNull(assessment);
        Assert.NotNull(stats);
        Assert.True(stats.HasHeaderRow);
        Assert.Equal(2, stats.TotalRowsRead);
        Assert.Equal(2, stats.DataElementsProcessed);
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
        var csvContent = @"Suspensions and Expulsions (Grades 1 and 2),RecordCount,5,,NULL
Suspensions and Expulsions (K-12),RecordCount,1020,,""No data found for the following grades: 10,11,12""";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var (assessment, stats) = _parser.ParseAssessmentStream(stream);

        Assert.NotNull(assessment);
        Assert.NotNull(stats);
        Assert.False(stats.HasHeaderRow);
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
    public void ParseAssessmentStream_WithNumericalRange_ShouldParseCorrectly()
    {
        var csvContent = @"DataElementName,CharacteristicType,Value,SubItemLabel,Remarks
Student Age Range,NumericalRange,5,Minimum,Age of enrolled students
Student Age Range,NumericalRange,21,Maximum,";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var (assessment, stats) = _parser.ParseAssessmentStream(stream, hasHeaderRow: true);

        Assert.Single(assessment.DataElementAssessments);
        Assert.Equal(1, stats.CharacteristicsProcessed);

        var element = assessment.DataElementAssessments.First();
        Assert.Equal("Student Age Range", element.DataElementName);

        var range = element.Characteristics.OfType<NumericalRange>().FirstOrDefault();
        Assert.NotNull(range);
        Assert.Equal(5, range.Minimum);
        Assert.Equal(21, range.Maximum);
        Assert.Equal("Age of enrolled students", range.Label);
        Assert.Equal("Age of enrolled students", range.Remarks);

        _output.WriteLine($"Parsed NumericalRange: {range.Minimum} to {range.Maximum} ({range.Label})");
    }

    [Fact]
    public void ParseAssessmentStream_WithCompleteness_ShouldParseCorrectly()
    {
        var csvContent = @"DataElementName,CharacteristicType,Value,SubItemLabel,Remarks
Student Contact Information,Completeness,850,PopulatedRecords,ElectronicMailAddress
Student Contact Information,Completeness,1000,TotalRecords,";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var (assessment, stats) = _parser.ParseAssessmentStream(stream, hasHeaderRow: true);

        Assert.Single(assessment.DataElementAssessments);
        Assert.Equal(1, stats.CharacteristicsProcessed);

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
        var csvContent = @"DataElementName,CharacteristicType,Value,SubItemLabel,Remarks
Discipline Action Types,Distribution,450,In-School Suspension,Distribution of disciplinary actions taken
Discipline Action Types,Distribution,320,Out-of-School Suspension,
Discipline Action Types,Distribution,125,Expulsion with Services,
Discipline Action Types,Distribution,95,Expulsion without Services,";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var (assessment, stats) = _parser.ParseAssessmentStream(stream, hasHeaderRow: true);

        Assert.Single(assessment.DataElementAssessments);
        Assert.Equal(1, stats.CharacteristicsProcessed);

        var distribution = assessment.DataElementAssessments.First()
            .Characteristics.OfType<Distribution>().FirstOrDefault();
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
            _output.WriteLine($"  {distribution.FormatItem(kvp.Key, kvp.Value)}");
    }

    [Fact]
    public void ParseAssessmentStream_WithReportedAvailability_ShouldParseCorrectly()
    {
        var csvContent = @"DataElementName,CharacteristicType,Value,SubItemLabel,Remarks
Special Education Services,ReportedAvailability,Available,,Availability based on record count thresholds
Gifted and Talented Programs,ReportedAvailability,PartiallyAvailable,,Only available for grades 3-8";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var (assessment, stats) = _parser.ParseAssessmentStream(stream, hasHeaderRow: true);

        Assert.Equal(2, assessment.DataElementAssessments.Count);
        Assert.Equal(2, stats.CharacteristicsProcessed);

        var specEd = assessment.DataElementAssessments
            .FirstOrDefault(a => a.DataElementName == "Special Education Services");
        Assert.NotNull(specEd);
        var availability = specEd.Characteristics.OfType<ReportedAvailability>().FirstOrDefault();
        Assert.NotNull(availability);
        Assert.Equal(AvailabilityJudgment.Available, availability.Value);
        Assert.Equal("Availability based on record count thresholds", availability.Remarks);

        var gifted = assessment.DataElementAssessments
            .FirstOrDefault(a => a.DataElementName == "Gifted and Talented Programs");
        Assert.NotNull(gifted);
        var giftedAvailability = gifted.Characteristics.OfType<ReportedAvailability>().FirstOrDefault();
        Assert.NotNull(giftedAvailability);
        Assert.Equal(AvailabilityJudgment.PartiallyAvailable, giftedAvailability.Value);

        _output.WriteLine($"Special Ed: {availability.Value}");
        _output.WriteLine($"Gifted: {giftedAvailability.Value}");
    }

    [Fact]
    public void ParseAssessmentStream_WithMultipleCharacteristicsForSameElement_ShouldGroupCorrectly()
    {
        var csvContent = @"DataElementName,CharacteristicType,Value,SubItemLabel,Remarks
Suspensions (K-12),RecordCount,1020,,
Suspensions (K-12),Completeness,850,PopulatedRecords,IncidentDate
Suspensions (K-12),Completeness,1020,TotalRecords,";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var (assessment, stats) = _parser.ParseAssessmentStream(stream, hasHeaderRow: true);

        Assert.Single(assessment.DataElementAssessments);
        Assert.Equal(2, stats.CharacteristicsProcessed);

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
    }

    [Fact]
    public void ParseAssessmentStream_WithComplexScenario_ShouldParseAllTypesCorrectly()
    {
        var csvContent = @"DataElementName,CharacteristicType,Value,SubItemLabel,Remarks
Student Demographics,RecordCount,15420,,Total student records
Student Demographics,NumericalRange,5,Minimum,Student Age Range
Student Demographics,NumericalRange,21,Maximum,
Student Contact Info,Completeness,12500,PopulatedRecords,ElectronicMailAddress
Student Contact Info,Completeness,15420,TotalRecords,
Discipline Types,Distribution,5200,In-School Suspension,Distribution of discipline actions
Discipline Types,Distribution,3100,Out-of-School Suspension,
Discipline Types,Distribution,890,Expulsion,
Special Programs,ReportedAvailability,Available,,Sufficient data for reporting";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var (assessment, stats) = _parser.ParseAssessmentStream(stream, hasHeaderRow: true);

        Assert.Equal(4, assessment.DataElementAssessments.Count);
        Assert.Equal(5, stats.CharacteristicsProcessed);
        Assert.Equal(0, stats.CharacteristicsSkipped);
        Assert.False(stats.HasErrors);

        var demographics = assessment.DataElementAssessments
            .First(a => a.DataElementName == "Student Demographics");
        Assert.Equal(2, demographics.Characteristics.Count);
        Assert.NotNull(demographics.Characteristics.OfType<RecordCount>().FirstOrDefault());
        Assert.NotNull(demographics.Characteristics.OfType<NumericalRange>().FirstOrDefault());

        var completeness = assessment.DataElementAssessments
            .First(a => a.DataElementName == "Student Contact Info")
            .Characteristics.OfType<Completeness>().First();
        Assert.Equal(81.1m, Math.Round(completeness.Percentage, 1));

        var distribution = assessment.DataElementAssessments
            .First(a => a.DataElementName == "Discipline Types")
            .Characteristics.OfType<Distribution>().First();
        Assert.Equal(3, distribution.Counts.Count);
        Assert.Equal(9190, distribution.TotalCount);

        var availability = assessment.DataElementAssessments
            .First(a => a.DataElementName == "Special Programs")
            .Characteristics.OfType<ReportedAvailability>().First();
        Assert.Equal(AvailabilityJudgment.Available, availability.Value);

        _output.WriteLine($"Successfully parsed complex assessment with {assessment.DataElementAssessments.Count} elements");
    }

    [Fact]
    public void ParseAssessmentStream_WithNullValues_ShouldSkipCharacteristicsGracefully()
    {
        var csvContent = @"DataElementName,CharacteristicType,Value,SubItemLabel,Remarks
Student Age,RecordCount,100,,Valid record count
Student Age,Completeness,NULL,PopulatedRecords,BirthDate
Student Age,Completeness,NULL,TotalRecords,
Student Age,NumericalRange,NULL,Minimum,Age Range
Student Age,NumericalRange,NULL,Maximum,
Suspensions,RecordCount,0,,Zero records is valid";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var (assessment, stats) = _parser.ParseAssessmentStream(stream, hasHeaderRow: true);

        Assert.Equal(2, assessment.DataElementAssessments.Count);
        Assert.Equal(2, stats.DataElementsProcessed);
        Assert.Equal(0, stats.DataElementsSkipped);
        Assert.Equal(2, stats.CharacteristicsProcessed);
        Assert.Equal(2, stats.CharacteristicsSkipped);
        Assert.False(stats.HasErrors);

        var studentAge = assessment.DataElementAssessments
            .FirstOrDefault(a => a.DataElementName == "Student Age");
        Assert.NotNull(studentAge);
        Assert.Single(studentAge.Characteristics);
        Assert.IsType<RecordCount>(studentAge.Characteristics[0]);

        Assert.True(stats.HasWarnings);
        Assert.Contains(stats.SkippedReasons, r => r.Contains("Student Age") && r.Contains("Completeness"));
        Assert.Contains(stats.SkippedReasons, r => r.Contains("Student Age") && r.Contains("NumericalRange"));

        _output.WriteLine($"Stats: {stats.CharacteristicsProcessed} processed, {stats.CharacteristicsSkipped} skipped");
        foreach (var reason in stats.SkippedReasons)
            _output.WriteLine($"  - {reason}");
    }

    [Fact]
    public void ParseAssessmentStream_WithDataElementHavingNoValidCharacteristics_ShouldSkipElement()
    {
        var csvContent = @"DataElementName,CharacteristicType,Value,SubItemLabel,Remarks
Student Age,Completeness,NULL,PopulatedRecords,BirthDate
Student Age,Completeness,NULL,TotalRecords,
Student Age,NumericalRange,NULL,Minimum,Age Range
Student Age,NumericalRange,NULL,Maximum,
Valid Element,RecordCount,100,,This one is fine";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var (assessment, stats) = _parser.ParseAssessmentStream(stream, hasHeaderRow: true);

        Assert.Single(assessment.DataElementAssessments);
        Assert.Equal(1, stats.DataElementsProcessed);
        Assert.Equal(1, stats.DataElementsSkipped);
        Assert.Single(stats.SkippedDataElements);
        Assert.Contains("Student Age", stats.SkippedDataElements);

        var validElement = assessment.DataElementAssessments.First();
        Assert.Equal("Valid Element", validElement.DataElementName);

        _output.WriteLine($"Correctly skipped data element with no valid characteristics");
    }

    [Fact]
    public void ParseAssessmentStream_WithPartialDistribution_ShouldIncludeValidItems()
    {
        var csvContent = @"DataElementName,CharacteristicType,Value,SubItemLabel,Remarks
Discipline Types,Distribution,450,In-School Suspension,Distribution of actions
Discipline Types,Distribution,NULL,Out-of-School Suspension,
Discipline Types,Distribution,125,Expulsion,";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var (assessment, stats) = _parser.ParseAssessmentStream(stream, hasHeaderRow: true);

        Assert.Single(assessment.DataElementAssessments);
        Assert.Equal(1, stats.CharacteristicsProcessed);
        Assert.Equal(0, stats.CharacteristicsSkipped);

        var distribution = assessment.DataElementAssessments.First()
            .Characteristics.OfType<Distribution>().First();
        Assert.Equal(2, distribution.Counts.Count);
        Assert.Equal(450, distribution.Counts["In-School Suspension"]);
        Assert.Equal(125, distribution.Counts["Expulsion"]);
        Assert.False(distribution.Counts.ContainsKey("Out-of-School Suspension"));

        Assert.Contains(stats.SkippedReasons,
            r => r.Contains("Discipline Types") && r.Contains("Distribution") && r.Contains("1 item(s) skipped"));

        _output.WriteLine($"Distribution has {distribution.Counts.Count} valid items");
    }

    [Fact]
    public void ParseAssessmentStream_WithInvalidNumericalRange_ShouldCollectErrorAndContinue()
    {
        // Missing Maximum row — parser should collect the error and continue
        var csvContent = @"DataElementName,CharacteristicType,Value,SubItemLabel,Remarks
Student Age Range,NumericalRange,5,Minimum,Age of enrolled students
Valid Element,RecordCount,100,,";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var (assessment, stats) = _parser.ParseAssessmentStream(stream, hasHeaderRow: true);

        Assert.True(stats.HasErrors);
        Assert.Single(stats.ParseErrors);
        Assert.Equal("Student Age Range", stats.ParseErrors[0].DataElementName);
        Assert.Equal("NumericalRange", stats.ParseErrors[0].CharacteristicType);
        Assert.Contains("exactly 2 rows", stats.ParseErrors[0].Message);

        // Valid element should still be present
        Assert.Single(assessment.DataElementAssessments);
        Assert.Equal("Valid Element", assessment.DataElementAssessments[0].DataElementName);

        _output.WriteLine($"Collected error: {stats.ParseErrors[0].Message}");
    }

    [Fact]
    public void ParseAssessmentStream_WithInvalidCompleteness_ShouldCollectErrorAndContinue()
    {
        // Missing TotalRecords row
        var csvContent = @"DataElementName,CharacteristicType,Value,SubItemLabel,Remarks
Student Contact Information,Completeness,850,PopulatedRecords,ElectronicMailAddress
Valid Element,RecordCount,100,,";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var (assessment, stats) = _parser.ParseAssessmentStream(stream, hasHeaderRow: true);

        Assert.True(stats.HasErrors);
        Assert.Single(stats.ParseErrors);
        Assert.Equal("Student Contact Information", stats.ParseErrors[0].DataElementName);
        Assert.Equal("Completeness", stats.ParseErrors[0].CharacteristicType);
        Assert.Contains("exactly 2 rows", stats.ParseErrors[0].Message);

        Assert.Single(assessment.DataElementAssessments);
        Assert.Equal("Valid Element", assessment.DataElementAssessments[0].DataElementName);

        _output.WriteLine($"Collected error: {stats.ParseErrors[0].Message}");
    }

    [Fact]
    public void ParseAssessmentStream_WithDistributionMissingSubItemLabel_ShouldCollectErrorAndContinue()
    {
        var csvContent = @"DataElementName,CharacteristicType,Value,SubItemLabel,Remarks
Discipline Types,Distribution,450,,Distribution of discipline actions
Valid Element,RecordCount,100,,";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var (assessment, stats) = _parser.ParseAssessmentStream(stream, hasHeaderRow: true);

        Assert.True(stats.HasErrors);
        Assert.Single(stats.ParseErrors);
        Assert.Equal("Discipline Types", stats.ParseErrors[0].DataElementName);
        Assert.Equal("Distribution", stats.ParseErrors[0].CharacteristicType);
        Assert.Contains("SubItemLabel", stats.ParseErrors[0].Message);

        Assert.Single(assessment.DataElementAssessments);
        Assert.Equal("Valid Element", assessment.DataElementAssessments[0].DataElementName);

        _output.WriteLine($"Collected error: {stats.ParseErrors[0].Message}");
    }

    [Fact]
    public void ParseAssessmentStream_WithInvalidRecordCountValue_ShouldCollectErrorAndContinue()
    {
        var csvContent = @"DataElementName,CharacteristicType,Value,SubItemLabel,Remarks
Suspensions,RecordCount,NOT_A_NUMBER,,
Valid Element,RecordCount,100,,";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var (assessment, stats) = _parser.ParseAssessmentStream(stream, hasHeaderRow: true);

        Assert.True(stats.HasErrors);
        Assert.Single(stats.ParseErrors);
        Assert.Equal("Suspensions", stats.ParseErrors[0].DataElementName);
        Assert.Equal("RecordCount", stats.ParseErrors[0].CharacteristicType);
        Assert.Contains("Invalid RecordCount value", stats.ParseErrors[0].Message);

        Assert.Single(assessment.DataElementAssessments);
        Assert.Equal("Valid Element", assessment.DataElementAssessments[0].DataElementName);

        _output.WriteLine($"Collected error: {stats.ParseErrors[0].Message}");
    }

    [Fact]
    public void ParseAssessmentStream_WithInvalidAvailabilityValue_ShouldCollectErrorAndContinue()
    {
        var csvContent = @"DataElementName,CharacteristicType,Value,SubItemLabel,Remarks
Special Education Services,ReportedAvailability,InvalidValue,,
Valid Element,RecordCount,100,,";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var (assessment, stats) = _parser.ParseAssessmentStream(stream, hasHeaderRow: true);

        Assert.True(stats.HasErrors);
        Assert.Single(stats.ParseErrors);
        Assert.Equal("Special Education Services", stats.ParseErrors[0].DataElementName);
        Assert.Equal("ReportedAvailability", stats.ParseErrors[0].CharacteristicType);
        Assert.Contains("Invalid ReportedAvailability value", stats.ParseErrors[0].Message);

        Assert.Single(assessment.DataElementAssessments);
        Assert.Equal("Valid Element", assessment.DataElementAssessments[0].DataElementName);

        _output.WriteLine($"Collected error: {stats.ParseErrors[0].Message}");
    }

    [Fact]
    public void ParseAssessmentStream_StatsTracking_ShouldReportAccurately()
    {
        // Element1: valid RecordCount
        // Element2: NULL RecordCount (skipped), valid Completeness
        // Element3: NULL NumericalRange (skipped) → element excluded
        // Element4: valid ReportedAvailability
        var csvContent = @"DataElementName,CharacteristicType,Value,SubItemLabel,Remarks
Element1,RecordCount,100,,
Element2,RecordCount,NULL,,
Element2,Completeness,50,PopulatedRecords,
Element2,Completeness,100,TotalRecords,
Element3,NumericalRange,NULL,Minimum,
Element3,NumericalRange,NULL,Maximum,
Element4,ReportedAvailability,Available,,";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var (assessment, stats) = _parser.ParseAssessmentStream(stream, hasHeaderRow: true);

        Assert.Equal(7, stats.TotalRowsRead);
        Assert.Equal(4, stats.TotalDataElements);
        Assert.Equal(3, stats.DataElementsProcessed); // Element1, Element2, Element4
        Assert.Equal(1, stats.DataElementsSkipped);   // Element3
        Assert.Equal(3, stats.CharacteristicsProcessed); // Element1 RecordCount, Element2 Completeness, Element4 Availability
        Assert.Equal(2, stats.CharacteristicsSkipped);   // Element2 RecordCount (NULL), Element3 NumericalRange (NULL)
        Assert.False(stats.HasErrors); // NULL skips go to CharacteristicsSkipped, not ParseErrors
        Assert.True(stats.HasWarnings);
        Assert.Single(stats.SkippedDataElements);
        Assert.Contains("Element3", stats.SkippedDataElements);

        Assert.Equal(3, assessment.DataElementAssessments.Count);
        Assert.Contains(assessment.DataElementAssessments, e => e.DataElementName == "Element1");
        Assert.Contains(assessment.DataElementAssessments, e => e.DataElementName == "Element2");
        Assert.Contains(assessment.DataElementAssessments, e => e.DataElementName == "Element4");
        Assert.DoesNotContain(assessment.DataElementAssessments, e => e.DataElementName == "Element3");

        // Element2 has only Completeness — RecordCount was skipped due to NULL
        var element2 = assessment.DataElementAssessments.First(e => e.DataElementName == "Element2");
        Assert.Single(element2.Characteristics);
        Assert.IsType<Completeness>(element2.Characteristics[0]);

        _output.WriteLine($"Total rows: {stats.TotalRowsRead}");
        _output.WriteLine($"Elements: {stats.DataElementsProcessed} processed, {stats.DataElementsSkipped} skipped");
        _output.WriteLine($"Characteristics: {stats.CharacteristicsProcessed} processed, {stats.CharacteristicsSkipped} skipped");
        _output.WriteLine($"Parse errors: {stats.ParseErrors.Count}");
    }

    [Fact]
    public void ParseAssessmentStream_WithMixedErrorsAndValid_ShouldCollectAllErrors()
    {
        // Multiple errored elements alongside valid ones — all errors collected, all valid elements imported
        var csvContent = @"DataElementName,CharacteristicType,Value,SubItemLabel,Remarks
Bad Range,NumericalRange,5,Minimum,
Bad Completeness,Completeness,100,PopulatedRecords,
Good Element,RecordCount,50,,
Bad Availability,ReportedAvailability,NotARealValue,,";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var (assessment, stats) = _parser.ParseAssessmentStream(stream, hasHeaderRow: true);

        Assert.True(stats.HasErrors);
        Assert.Equal(3, stats.ParseErrors.Count);
        Assert.Contains(stats.ParseErrors, e => e.DataElementName == "Bad Range");
        Assert.Contains(stats.ParseErrors, e => e.DataElementName == "Bad Completeness");
        Assert.Contains(stats.ParseErrors, e => e.DataElementName == "Bad Availability");

        // Only the good element makes it through
        Assert.Single(assessment.DataElementAssessments);
        Assert.Equal("Good Element", assessment.DataElementAssessments[0].DataElementName);

        _output.WriteLine($"Collected {stats.ParseErrors.Count} errors, imported {assessment.DataElementAssessments.Count} element");
        foreach (var err in stats.ParseErrors)
            _output.WriteLine($"  [{err.DataElementName} / {err.CharacteristicType}] {err.Message}");
    }
}
