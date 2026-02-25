using EwFrameworkAnalysis.Common.FrameworkReferenceData;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Models.Scoring;
using Microsoft.JSInterop;

namespace EwFrameworkAnalysis.UI.Services;

public class PdfReportService(IJSRuntime jsRuntime)
{
    public async Task GenerateReportAsync(
        List<QuestionScore> questionScores,
        List<SectorReadinessResult> sectorReadinessScores,
        OverallReadinessResults overallReadinessScores,
        List<DataSource> activeDataSources)
    {
        var ecsActive = activeDataSources.Any(ds => ds.Type == DataSourceType.EcsState);

        var eqBands = new[]
        {
            new { label = "100% Readiness", count = questionScores.Count(q => q.ReadinessScore >= 0.9m) },
            new { label = "80\u201390% Readiness", count = questionScores.Count(q => q.ReadinessScore is >= 0.80m and < 0.90m) },
            new { label = "50\u201379% Readiness", count = questionScores.Count(q => q.ReadinessScore is >= 0.50m and < 0.80m) },
            new { label = "Under 50% Readiness", count = questionScores.Count(q => q.ReadinessScore < 0.50m) },
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
                                sectors = scored.Sectors.Select(s => s.GetDisplayName()).ToArray(),
                            };
                        }

                        var fallbackSectors = EwFrameworkIndicators.Indicators.TryGetValue(indName, out var def)
                            ? def.Sectors.Select(s => s.GetDisplayName()).ToArray()
                            : [];
                        return new { name = indName, readinessScore = 0.0, sectors = fallbackSectors };
                    }).ToArray(),
                    distinctDataElements = qs?.IndicatorScores
                        .SelectMany(i => i.DataElementScores)
                        .GroupBy(de => de.DataElementName)
                        .Select(g => new
                        {
                            name = g.Key,
                            availability = g.Min(de => de.AvailabilityScore).ToString(),
                        })
                        .OrderBy(de => de.name)
                        .ToArray() ?? [],
                };
            })
            .ToArray();

        var reportData = new
        {
            summary = new
            {
                eqBands,
                sectorReadiness,
                dataSourceReadiness = new
                {
                    custom = (double)overallReadinessScores.CustomDataSourceReadiness,
                    automated = (double)overallReadinessScores.AutomatedDataSourceReadiness,
                    ecs = (double)overallReadinessScores.EcsReadiness,
                    ecsActive,
                },
            },
            questions,
        };

        await jsRuntime.InvokeVoidAsync("generatePdfReport", reportData);
    }
}
