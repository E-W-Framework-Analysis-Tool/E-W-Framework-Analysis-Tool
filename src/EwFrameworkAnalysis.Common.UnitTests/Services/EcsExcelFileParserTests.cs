using ClosedXML.Excel;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.UnitTests.Services;

public class EcsExcelFileParserTests
{
    private readonly EcsExcelFileParser _parser = new();

    private static MemoryStream CreateTestWorkbook(Action<IXLWorksheet> configure)
    {
        var workbook = new XLWorkbook();
        var worksheet = workbook.AddWorksheet("Test");

        // Add headers
        worksheet.Cell(1, 1).Value = "Order Key";
        worksheet.Cell(1, 2).Value = "State";
        worksheet.Cell(1, 3).Value = "Essential Question";
        worksheet.Cell(1, 4).Value = "Question Order Key";
        worksheet.Cell(1, 5).Value = "Type";
        worksheet.Cell(1, 6).Value = "Domain";
        worksheet.Cell(1, 7).Value = "Sector";
        worksheet.Cell(1, 8).Value = "E-W Indicator";
        worksheet.Cell(1, 9).Value = "Metric Type";
        worksheet.Cell(1, 10).Value = "Metric or data element";
        worksheet.Cell(1, 11).Value = "Collected Code";
        worksheet.Cell(1, 21).Value = "Reported Code";

        configure(worksheet);

        var ms = new MemoryStream();
        workbook.SaveAs(ms);
        ms.Position = 0;
        return ms;
    }

    private static void AddDataElementRow(IXLWorksheet ws, int row, string state, string sector,
        string elementName, string collectedStatus, string? reportedStatus = null)
    {
        ws.Cell(row, 2).Value = state;
        ws.Cell(row, 7).Value = sector;
        ws.Cell(row, 9).Value = "Data element";
        ws.Cell(row, 10).Value = elementName;
        ws.Cell(row, 11).Value = collectedStatus;
        if (reportedStatus != null)
            ws.Cell(row, 21).Value = reportedStatus;
    }

    [Fact]
    public async Task Should_ReturnAvailableStates()
    {
        using var stream = CreateTestWorkbook(ws =>
        {
            AddDataElementRow(ws, 2, "Alabama", "K-12", "ACT completion", "Found");
            AddDataElementRow(ws, 3, "Alaska", "K-12", "ACT completion", "Found");
            AddDataElementRow(ws, 4, "Alabama", "K-12", "SAT completion", "Found");
        });

        var states = await _parser.GetAvailableStatesAsync(stream);

        Assert.Equal(2, states.Count);
        Assert.Contains("Alabama", states);
        Assert.Contains("Alaska", states);
    }

    [Fact]
    public async Task Should_MapFoundToAvailable()
    {
        using var stream = CreateTestWorkbook(ws =>
        {
            AddDataElementRow(ws, 2, "Alabama", "K-12", "ACT completion", "Found");
        });

        var (assessment, stats) = await _parser.ParseStateDataAsync(stream, "Alabama", EcsDataColumn.Collected);

        Assert.Single(assessment.DataElementAssessments);
        var element = assessment.DataElementAssessments[0];
        Assert.Equal("ACT completion", element.DataElementName);
        var availability = element.Characteristics.OfType<ReportedAvailability>().First();
        Assert.Equal(AvailabilityJudgment.Available, availability.Value);
    }

    [Fact]
    public async Task Should_MapPartialToPartiallyAvailable()
    {
        using var stream = CreateTestWorkbook(ws =>
        {
            AddDataElementRow(ws, 2, "Alabama", "K-12", "ACT completion", "Partial");
        });

        var (assessment, _) = await _parser.ParseStateDataAsync(stream, "Alabama", EcsDataColumn.Collected);

        var availability = assessment.DataElementAssessments[0].Characteristics.OfType<ReportedAvailability>().First();
        Assert.Equal(AvailabilityJudgment.PartiallyAvailable, availability.Value);
    }

    [Fact]
    public async Task Should_MapNotFoundToNotAvailable()
    {
        using var stream = CreateTestWorkbook(ws =>
        {
            AddDataElementRow(ws, 2, "Alabama", "K-12", "ACT completion", "Not Found");
        });

        var (assessment, _) = await _parser.ParseStateDataAsync(stream, "Alabama", EcsDataColumn.Collected);

        var availability = assessment.DataElementAssessments[0].Characteristics.OfType<ReportedAvailability>().First();
        Assert.Equal(AvailabilityJudgment.NotAvailable, availability.Value);
    }

    [Fact]
    public async Task Should_UseReportedColumn_When_ReportedSelected()
    {
        using var stream = CreateTestWorkbook(ws =>
        {
            AddDataElementRow(ws, 2, "Alabama", "K-12", "ACT completion", "Found", "Not Found");
        });

        var (assessment, _) = await _parser.ParseStateDataAsync(stream, "Alabama", EcsDataColumn.Reported);

        var availability = assessment.DataElementAssessments[0].Characteristics.OfType<ReportedAvailability>().First();
        Assert.Equal(AvailabilityJudgment.NotAvailable, availability.Value);
    }

    [Fact]
    public async Task Should_FilterByState()
    {
        using var stream = CreateTestWorkbook(ws =>
        {
            AddDataElementRow(ws, 2, "Alabama", "K-12", "ACT completion", "Found");
            AddDataElementRow(ws, 3, "Alaska", "K-12", "SAT completion", "Found");
        });

        var (assessment, stats) = await _parser.ParseStateDataAsync(stream, "Alabama", EcsDataColumn.Collected);

        Assert.Single(assessment.DataElementAssessments);
        Assert.Equal("ACT completion", assessment.DataElementAssessments[0].DataElementName);
    }

    [Fact]
    public async Task Should_OnlyProcessDataElementRows()
    {
        using var stream = CreateTestWorkbook(ws =>
        {
            // Metric row (should be skipped)
            ws.Cell(2, 2).Value = "Alabama";
            ws.Cell(2, 7).Value = "K-12";
            ws.Cell(2, 9).Value = "Metric";
            ws.Cell(2, 10).Value = "Some metric";
            ws.Cell(2, 11).Value = "Found";

            // Data element row (should be processed)
            AddDataElementRow(ws, 3, "Alabama", "K-12", "ACT completion", "Found");
        });

        var (assessment, stats) = await _parser.ParseStateDataAsync(stream, "Alabama", EcsDataColumn.Collected);

        Assert.Single(assessment.DataElementAssessments);
        Assert.Equal(1, stats.TotalRowsRead);
    }

    [Fact]
    public async Task Should_ConsolidateMultipleEcsElements_When_MappedToSameFrameworkElement()
    {
        using var stream = CreateTestWorkbook(ws =>
        {
            AddDataElementRow(ws, 2, "Alabama", "K-12", "In school suspension", "Not Found");
            AddDataElementRow(ws, 3, "Alabama", "K-12", "Out of school suspension", "Found");
            AddDataElementRow(ws, 4, "Alabama", "K-12", "Expulsions", "Partial");
        });

        var (assessment, _) = await _parser.ParseStateDataAsync(stream, "Alabama", EcsDataColumn.Collected);

        // All three should consolidate to "Suspensions and expulsions (K-12)"
        Assert.Single(assessment.DataElementAssessments);
        var element = assessment.DataElementAssessments[0];
        Assert.Equal("Suspensions and expulsions (K-12)", element.DataElementName);

        // Best status wins: Found (Available) > Partial > Not Found
        var availability = element.Characteristics.OfType<ReportedAvailability>().First();
        Assert.Equal(AvailabilityJudgment.Available, availability.Value);
    }

    [Fact]
    public async Task Should_TrackUnmappedElements()
    {
        using var stream = CreateTestWorkbook(ws =>
        {
            AddDataElementRow(ws, 2, "Alabama", "K-12", "Some Unknown Element", "Found");
            AddDataElementRow(ws, 3, "Alabama", "K-12", "ACT completion", "Found");
        });

        var (assessment, stats) = await _parser.ParseStateDataAsync(stream, "Alabama", EcsDataColumn.Collected);

        Assert.Single(assessment.DataElementAssessments);
        Assert.Equal(1, stats.DataElementsUnmapped);
        Assert.Contains("Some Unknown Element", stats.UnmappedElements);
    }

    [Fact]
    public async Task Should_MapSectorSpecificElements()
    {
        using var stream = CreateTestWorkbook(ws =>
        {
            AddDataElementRow(ws, 2, "Alabama", "Pre-K", "Student attendance rate", "Found");
        });

        var (assessment, _) = await _parser.ParseStateDataAsync(stream, "Alabama", EcsDataColumn.Collected);

        Assert.Single(assessment.DataElementAssessments);
        Assert.Equal("Student attendance rate (PK)", assessment.DataElementAssessments[0].DataElementName);
    }

    [Fact]
    public async Task Should_SkipRows_When_StatusValueEmpty()
    {
        using var stream = CreateTestWorkbook(ws =>
        {
            AddDataElementRow(ws, 2, "Alabama", "K-12", "ACT completion", "");
        });

        var (assessment, stats) = await _parser.ParseStateDataAsync(stream, "Alabama", EcsDataColumn.Collected);

        Assert.Empty(assessment.DataElementAssessments);
        Assert.Equal(1, stats.DataElementsSkipped);
    }

    [Fact]
    public void Should_ProcessStateData_WithCollectedColumn()
    {
        var records = new List<EcsStateDataRecord>
        {
            new("K-12", "ACT completion indicator", "ACT completion", "Found", "Not Found"),
            new("K-12", "SAT completion indicator", "SAT completion", "Partial", "Found")
        };

        var (assessment, stats) = _parser.ProcessStateData(records, EcsDataColumn.Collected);

        Assert.Equal(2, assessment.DataElementAssessments.Count);
        Assert.Equal(2, stats.DataElementsMapped);

        var act = assessment.DataElementAssessments.First(a => a.DataElementName == "ACT completion");
        var actAvailability = act.Characteristics.OfType<ReportedAvailability>().First();
        Assert.Equal(AvailabilityJudgment.Available, actAvailability.Value);

        var sat = assessment.DataElementAssessments.First(a => a.DataElementName == "SAT completion");
        var satAvailability = sat.Characteristics.OfType<ReportedAvailability>().First();
        Assert.Equal(AvailabilityJudgment.PartiallyAvailable, satAvailability.Value);
    }

    [Fact]
    public void Should_ProcessStateData_WithReportedColumn()
    {
        var records = new List<EcsStateDataRecord>
        {
            new("K-12", "ACT completion indicator", "ACT completion", "Found", "Not Found")
        };

        var (assessment, _) = _parser.ProcessStateData(records, EcsDataColumn.Reported);

        var availability = assessment.DataElementAssessments[0].Characteristics.OfType<ReportedAvailability>().First();
        Assert.Equal(AvailabilityJudgment.NotAvailable, availability.Value);
    }

    [Fact]
    public void Should_ProcessStateData_ConsolidateElements()
    {
        var records = new List<EcsStateDataRecord>
        {
            new("K-12", "Discipline indicator", "In school suspension", "Not Found", ""),
            new("K-12", "Discipline indicator", "Out of school suspension", "Found", ""),
            new("K-12", "Discipline indicator", "Expulsions", "Partial", "")
        };

        var (assessment, _) = _parser.ProcessStateData(records, EcsDataColumn.Collected);

        Assert.Single(assessment.DataElementAssessments);
        Assert.Equal("Suspensions and expulsions (K-12)", assessment.DataElementAssessments[0].DataElementName);

        var availability = assessment.DataElementAssessments[0].Characteristics.OfType<ReportedAvailability>().First();
        Assert.Equal(AvailabilityJudgment.Available, availability.Value);
    }

    [Fact]
    public void Should_ProcessStateData_SkipEmptyStatus()
    {
        var records = new List<EcsStateDataRecord>
        {
            new("K-12", "ACT indicator", "ACT completion", "", ""),
            new("K-12", "SAT indicator", "SAT completion", "Found", "")
        };

        var (assessment, stats) = _parser.ProcessStateData(records, EcsDataColumn.Collected);

        Assert.Single(assessment.DataElementAssessments);
        Assert.Equal(1, stats.DataElementsSkipped);
    }

    [Fact]
    public void Should_ProcessStateData_TrackUnmappedElements()
    {
        var records = new List<EcsStateDataRecord>
        {
            new("K-12", "Unknown indicator", "Some Unknown Element", "Found", ""),
            new("K-12", "ACT indicator", "ACT completion", "Found", "")
        };

        var (assessment, stats) = _parser.ProcessStateData(records, EcsDataColumn.Collected);

        Assert.Single(assessment.DataElementAssessments);
        Assert.Equal(1, stats.DataElementsUnmapped);
        Assert.Contains("Some Unknown Element", stats.UnmappedElements);
    }
}
