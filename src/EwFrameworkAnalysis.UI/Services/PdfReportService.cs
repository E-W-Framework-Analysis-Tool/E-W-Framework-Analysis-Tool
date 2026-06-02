using EwFrameworkAnalysis.Common.FrameworkReferenceData;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Models.Scoring;
using Microsoft.JSInterop;

namespace EwFrameworkAnalysis.UI.Services;

public record DataSourceScore(string Name, double Score);

public class PdfReportService(IJSRuntime jsRuntime)
{
    public async Task GenerateReportAsync(
        FrameworkCoverage coverage,
        string? projectTitle,
        List<DataSourceScore> dataSourceScores)
    {
        var eqBands = new[]
        {
            new { label = "90\u2013100%", count = coverage.QuestionScores.Count(q => q.CoverageScore >= 0.90m) },
            new { label = "80\u201390%",  count = coverage.QuestionScores.Count(q => q.CoverageScore is >= 0.80m and < 0.90m) },
            new { label = "70\u201380%",  count = coverage.QuestionScores.Count(q => q.CoverageScore is >= 0.70m and < 0.80m) },
            new { label = "60\u201370%",  count = coverage.QuestionScores.Count(q => q.CoverageScore is >= 0.60m and < 0.70m) },
            new { label = "50\u201360%",  count = coverage.QuestionScores.Count(q => q.CoverageScore is >= 0.50m and < 0.60m) },
            new { label = "<50%",         count = coverage.QuestionScores.Count(q => q.CoverageScore < 0.50m) },
        };

        var sectorCoverage = coverage.BySector
            .OrderByDescending(s => s.CoverageScore)
            .Select(s => new { sector = s.Sector.GetDisplayName(), score = (double)s.CoverageScore })
            .ToArray();

        var questions = EwFrameworkEssentialQuestions.Questions
            .Select(eq =>
            {
                var qs = coverage.QuestionScores.FirstOrDefault(q => q.QuestionNumber == eq.QuestionNumber);

                var distinctAvailability = qs?.IndicatorScores
                    .SelectMany(i => i.DataElementScores)
                    .GroupBy(de => de.DataElementName)
                    .Select(g => g.Min(de => de.AvailabilityScore))
                    .ToList() ?? [];

                var scoredByName = qs?.IndicatorScores
                    .ToDictionary(i => i.IndicatorCode) ?? [];

                return new
                {
                    number = eq.QuestionNumber,
                    question = eq.Question,
                    summary = eq.QuestionSummary,
                    sectors = eq.ApplicableSectors.Select(s => s.GetDisplayName()).ToArray(),
                    coverageScore = qs != null ? (double)qs.CoverageScore : 0.0,
                    indicatorCount = qs?.IndicatorScores.Count ?? 0,
                    dataElements = new
                    {
                        available = distinctAvailability.Count(a => a == AvailabilityJudgment.Available),
                        partial = distinctAvailability.Count(a => a == AvailabilityJudgment.PartiallyAvailable),
                        notAvailable = distinctAvailability.Count(a => a is AvailabilityJudgment.NotAvailable or AvailabilityJudgment.InsufficientData),
                    },
                    indicators = eq.RelatedIndicatorNames.Select(indName =>
                    {
                        if (scoredByName.TryGetValue(indName, out var scored))
                        {
                            return new
                            {
                                name = indName,
                                coverageScore = (double)scored.CoverageScore,
                                sectors = scored.Sectors.Select(s => s.ToString()).ToArray(),
                            };
                        }

                        var fallbackSectors = EwFrameworkIndicators.Indicators.TryGetValue(indName, out var def)
                            ? def.Sectors.Select(s => s.ToString()).ToArray()
                            : [];
                        return new { name = indName, coverageScore = 0.0, sectors = fallbackSectors };
                    }).ToArray(),
                    distinctDataElements = qs?.IndicatorScores
                        .SelectMany(i => i.DataElementScores)
                        .GroupBy(de => de.DataElementName)
                        .Select(g => (name: g.Key, score: g.Min(de => de.AvailabilityScore)))
                        .OrderBy(x => x.score)
                        .ThenBy(x => x.name)
                        .Select(x => new { x.name, availability = x.score.ToString() })
                        .ToArray() ?? [],
                };
            })
            .ToArray();

        var roiItems = coverage.IndicatorScores
            .DistinctBy(i => i.IndicatorCode)
            .SelectMany(i => i.DataElementScores
                .Where(de => de.QualityScore < 1.0m)
                .Select(de => new { i.IndicatorCode, DataElement = de }))
            .GroupBy(x => x.DataElement.DataElementName)
            .Select(g => new
            {
                name = g.Key,
                indicators = g.Select(x => x.IndicatorCode).Distinct().OrderBy(x => x).ToList(),
                potentialImpact = (double)g.Sum(x => 1.0m - x.DataElement.QualityScore),
            })
            .OrderByDescending(x => x.potentialImpact)
            .ThenByDescending(x => x.indicators.Count)
            .Take(5)
            .ToArray();

        var reportData = new
        {
            projectTitle = projectTitle ?? string.Empty,
            summary = new
            {
                eqBands,
                sectorCoverage,
                dataSourceCoverage = dataSourceScores
                    .Select(ds => new { name = ds.Name, score = ds.Score })
                    .ToArray(),
            },
            overallCoverage = new
            {
                manual = (double)coverage.BySourceType.Manual,
                automated = (double)coverage.BySourceType.Automated,
                ecs = (double)coverage.BySourceType.Ecs,
                combined = (double)coverage.OverallCoverage,
                roiItems,
            },
            questions,
        };

        await jsRuntime.InvokeVoidAsync("generatePdfReport", reportData);
    }
}
