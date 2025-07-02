using EwFrameworkAnalysis.Blazor.Services;
using EwFrameworkAnalysis.Blazor.Services.DataAvailability;
using EwFrameworkAnalysis.Common.Models;

public class QuestionScoringService
{
    public QuestionScoringService(EwFrameworkService ewFrameworkService)
    {
        _ewFrameworkService = ewFrameworkService;
    }

    private readonly EwFrameworkService _ewFrameworkService;
    private const decimal MISSING_REQUIREMENT_DEFAULT_SCORE = 0;

    public async Task<List<QuestionWithScore<T>>> CalculateScoresForAvailableData<T>(
        IIndicatorDataRequirementsProvider<T> dataRequirementProvider,
        List<IDataAvailabilityProvider<T>> dataAvailabilityProviders,
        IProgress<MultiStageProgressReport>? progress = null) where T : IScorableDataItem
    {
        // Prepare data structures
        var questions = await _ewFrameworkService.GetEssentialQuestionWithIndicators();
        var questionScores = InitializeQuestionScores(questions, dataRequirementProvider);
        var progressReport = InitializeProgressReport(dataAvailabilityProviders, progress);
        SetTotalChecksPerProvider(dataAvailabilityProviders, questionScores, progressReport);

        // Send initial progress update
        progress?.Report(progressReport);

        // Issue availability checks for each provider and update the questionScores
        await PerformDataAvailabilityChecks(dataAvailabilityProviders, questionScores, progressReport, progress);

        return [.. questionScores.OrderByDescending(x => x.Score)];
    }

    private static List<QuestionWithScore<T>> InitializeQuestionScores<T>(
        List<EssentialQuestionWithIndicators> questions,
        IIndicatorDataRequirementsProvider<T> dataRequirementProvider) where T : IScorableDataItem
    {
        return [.. questions.Select(question =>
        {
            var indicatorScores = question.RelatedIndicators.Select(indicator =>
            {
                var dataRequirements = dataRequirementProvider.WithQuestionContext(question).GetScorableDataItems(indicator);

                var availabilityResults = dataRequirements.ToDictionary(
                    dataItem => dataItem.Name,
                    dataItem => new DataAvailabilityResult<T>
                    {
                        DataItem = dataItem,
                        ProviderResults = []
                    }
                );

                return new IndicatorWithScore<T>
                {
                    Indicator = indicator,
                    DataAvailabilityResults = availabilityResults
                };
            }).ToList();

            return new QuestionWithScore<T>
            {
                Question = question.Question,
                Indicators = indicatorScores,
                Sectors = question.Sectors
            };
        })];
    }

    private static MultiStageProgressReport InitializeProgressReport<T>(
        List<IDataAvailabilityProvider<T>> dataAvailabilityProviders,
        IProgress<MultiStageProgressReport>? progress) where T : IScorableDataItem
    {
        var progressReport = new MultiStageProgressReport();

        foreach (var provider in dataAvailabilityProviders)
        {
            progressReport.ProviderReports[provider.Name] = new ProviderProgressReport
            {
                ProviderName = provider.Name
            };

            provider.OnStatusUpdate += update =>
            {
                var providerReport = progressReport.ProviderReports[provider.Name];
                providerReport.UpdateStatus(update.StatusMessage);
                progress?.Report(progressReport);
            };

            provider.OnCacheHit += () =>
            {
                var providerReport = progressReport.ProviderReports[provider.Name];
                providerReport.IncrementCacheHits();
                progress?.Report(progressReport);
            };
        }

        return progressReport;
    }

    private static void SetTotalChecksPerProvider<T>(
        List<IDataAvailabilityProvider<T>> dataAvailabilityProviders,
        List<QuestionWithScore<T>> questionScores,
        MultiStageProgressReport progressReport) where T : IScorableDataItem
    {
        var totalChecks = questionScores.Sum(q => q.Indicators.Sum(i => i.DataAvailabilityResults.Count));

        foreach (var provider in dataAvailabilityProviders)
        {
            var providerReport = progressReport.ProviderReports[provider.Name];
            providerReport.SetTotalAvailabilityChecks(totalChecks);
        }
    }

    private static async Task PerformDataAvailabilityChecks<T>(
        List<IDataAvailabilityProvider<T>> dataAvailabilityProviders,
        List<QuestionWithScore<T>> questionScores,
        MultiStageProgressReport progressReport,
        IProgress<MultiStageProgressReport>? progress) where T : IScorableDataItem
    {
        var tasks = dataAvailabilityProviders.Select(provider =>
            ProcessProviderDataChecks(provider, questionScores, progressReport, progress)
        );

        await Task.WhenAll(tasks);
    }

    private static async Task ProcessProviderDataChecks<T>(
    IDataAvailabilityProvider<T> provider,
    List<QuestionWithScore<T>> questionScores,
    MultiStageProgressReport progressReport,
    IProgress<MultiStageProgressReport>? progress) where T : IScorableDataItem
    {
        var providerReport = progressReport.ProviderReports[provider.Name];

        foreach (var question in questionScores)
        {
            foreach (var indicator in question.Indicators)
            {
                foreach (var kvp in indicator.DataAvailabilityResults)
                {
                    var dataItemKey = kvp.Key;
                    var dataAvailabilityResult = kvp.Value;

                    try
                    {
                        var result = await provider.CheckDataAvailability(dataAvailabilityResult.DataItem) ?? throw new ApplicationException("Data availability check produced null result");
                        if (result?.Status == DataAvailabilityStatus.DataAvailable)
                        {
                            providerReport.IncrementAvailableResults();
                        }
                        else
                        {
                            providerReport.IncrementUnavailableResults();
                        }

                        dataAvailabilityResult.ProviderResults.Add(new ProviderDataDetails
                        {
                            ProviderName = provider.Name,
                            DataAvailability = result!
                        });
                    }
                    catch
                    {
                        providerReport.IncrementErrors();
                    }
                    finally
                    {
                        providerReport.IncrementCompletedAvailabilityChecks();
                        progress?.Report(progressReport);
                    }
                }
            }
        }
    }
}
