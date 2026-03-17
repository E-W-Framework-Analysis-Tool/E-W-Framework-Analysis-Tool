using EwFrameworkAnalysis.Common.FrameworkReferenceData;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Models.Scoring;
using Microsoft.JSInterop;

namespace EwFrameworkAnalysis.UI.Services;

public record DataSourceScore(string Name, double Score);

public class PdfReportService(IJSRuntime jsRuntime)
{
    public async Task GenerateReportAsync(
        List<QuestionScore> questionScores,
        List<SectorReadinessResult> sectorReadinessScores,
        OverallReadinessResults overallReadinessScores,
        string? projectTitle,
        List<DataSourceScore> dataSourceScores,
        List<IndicatorScore> indicatorScores)
    {

        var eqBands = new[]
        {
            new { label = "90\u2013100%", count = questionScores.Count(q => q.ReadinessScore >= 0.90m) },
            new { label = "80\u201390%",  count = questionScores.Count(q => q.ReadinessScore is >= 0.80m and < 0.90m) },
            new { label = "70\u201380%",  count = questionScores.Count(q => q.ReadinessScore is >= 0.70m and < 0.80m) },
            new { label = "60\u201370%",  count = questionScores.Count(q => q.ReadinessScore is >= 0.60m and < 0.70m) },
            new { label = "50\u201360%",  count = questionScores.Count(q => q.ReadinessScore is >= 0.50m and < 0.60m) },
            new { label = "<50%",         count = questionScores.Count(q => q.ReadinessScore < 0.50m) },
        };

        var sectorReadiness = sectorReadinessScores
            .OrderByDescending(s => s.ReadinessScore)
            .Select(s => new { sector = s.Sector.GetDisplayName(), score = (double)s.ReadinessScore })
            .ToArray();

        var questions = EwFrameworkEssentialQuestions.Questions
            .Select(eq =>
            {
                var qs = questionScores.FirstOrDefault(q => q.QuestionNumber == eq.QuestionNumber);

                // For each distinct data element, take the best (lowest enum value) availability across all indicators
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
                    readinessScore = qs != null ? (double)qs.ReadinessScore : 0.0,
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
                                readinessScore = (double)scored.ReadinessScore,
                                sectors = scored.Sectors.Select(s => s.ToString()).ToArray(),
                            };
                        }

                        var fallbackSectors = EwFrameworkIndicators.Indicators.TryGetValue(indName, out var def)
                            ? def.Sectors.Select(s => s.ToString()).ToArray()
                            : [];
                        return new { name = indName, readinessScore = 0.0, sectors = fallbackSectors };
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

        var roiItems = indicatorScores
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
                sectorReadiness,
                dataSourceReadiness = dataSourceScores
                    .Select(ds => new { name = ds.Name, score = ds.Score })
                    .ToArray(),
            },
            overallReadiness = new
            {
                manual = (double)overallReadinessScores.CustomDataSourceReadiness,
                automated = (double)overallReadinessScores.AutomatedDataSourceReadiness,
                ecs = (double)overallReadinessScores.EcsReadiness,
                combined = (double)overallReadinessScores.CombinedReadiness,
                roiItems,
            },
            questions,
        };

        await jsRuntime.InvokeVoidAsync("generatePdfReport", reportData);
    }
}
