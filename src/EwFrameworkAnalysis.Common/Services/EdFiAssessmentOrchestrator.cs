using System.Collections.Concurrent;
using EwFrameworkAnalysis.Common.Assessors.EdFi;
using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Services;
/// <summary>
/// Orchestrates the execution of multiple Ed-Fi assessors against a data source.
/// </summary>
public class EdFiAssessmentOrchestrator
{
    private readonly IEnumerable<IEdFiAssessor> _assessors;
    private readonly EdFiStudentDemographicsProvider? _demographicsProvider;
    private readonly EdFiCTEProgramProvider? _cteProgramProvider;
    private readonly EdFiCourseProvider? _courseProvider;
    private readonly EdFiStudentAssessmentProvider? _studentAssessmentProvider;
    private readonly int _maxDegreeOfParallelism;

    /// <summary>
    /// Initializes a new instance of the orchestrator.
    /// </summary>
    /// <param name="assessors">Collection of all available Ed-Fi assessors to run.</param>
    /// <param name="demographicsProvider">
    /// Optional shared demographics provider whose cache is cleared at the start of each run.
    /// </param>
    /// <param name="cteProgramProvider">
    /// Optional shared CTE program provider whose cache is cleared at the start of each run.
    /// </param>
    /// <param name="courseProvider">
    /// Optional shared course provider whose cache is cleared at the start of each run.
    /// </param>
    /// <param name="studentAssessmentProvider">
    /// Optional shared student assessment provider whose cache is cleared at the start of each run.
    /// </param>
    /// <param name="maxDegreeOfParallelism">
    /// Maximum number of assessors to run concurrently.
    /// Default is 4, which balances throughput with API rate limiting concerns.
    /// Use 1 for sequential execution.
    /// </param>
    public EdFiAssessmentOrchestrator(
        IEnumerable<IEdFiAssessor> assessors,
        EdFiStudentDemographicsProvider? demographicsProvider = null,
        EdFiCTEProgramProvider? cteProgramProvider = null,
        EdFiCourseProvider? courseProvider = null,
        EdFiStudentAssessmentProvider? studentAssessmentProvider = null,
        int maxDegreeOfParallelism = 4)
    {
        _assessors = assessors ?? throw new ArgumentNullException(nameof(assessors));
        _demographicsProvider = demographicsProvider;
        _cteProgramProvider = cteProgramProvider;
        _courseProvider = courseProvider;
        _studentAssessmentProvider = studentAssessmentProvider;

        if (maxDegreeOfParallelism < 1)
            throw new ArgumentOutOfRangeException(nameof(maxDegreeOfParallelism), "Must be at least 1");

        _maxDegreeOfParallelism = maxDegreeOfParallelism;
    }

    /// <summary>
    /// Executes all assessors against the specified data source and collects results.
    /// </summary>
    /// <param name="httpClient">
    /// An HTTP client configured with the Ed-Fi API base address and authentication.
    /// </param>
    /// <param name="dataSource">The data source configuration being assessed.</param>
    /// <param name="assessmentName">Optional name for this assessment session.</param>
    /// <param name="notes">Optional notes about this assessment session.</param>
    /// <param name="progress">
    /// Optional progress reporter for tracking assessment execution in real-time.
    /// </param>
    /// <param name="continueOnError">
    /// If true, continues executing remaining assessors even if some fail.
    /// If false, stops execution on the first error. Default is true.
    /// </param>
    /// <param name="ct">Cancellation token to cancel the assessment operation.</param>
    /// <returns>
    /// A completed DataSourceAssessment containing all successful assessment results.
    /// Failed assessments are omitted from the results but reported via progress.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when httpClient or dataSource is null.
    /// </exception>
    /// <exception cref="TaskCanceledException">
    /// Thrown when the operation is cancelled via the cancellation token.
    /// </exception>
    /// <remarks>
    /// This method executes assessors in parallel (up to maxDegreeOfParallelism).
    /// Progress reports are thread-safe and can be monitored in real-time.
    /// For Blazor WebAssembly scenarios, consider using maxDegreeOfParallelism=1
    /// to avoid browser threading limitations.
    /// </remarks>
    public async Task<DataSourceAssessment> RunAssessmentsAsync(
        HttpClient httpClient,
        DataSource dataSource,
        string? assessmentName = null,
        IProgress<AssessmentProgress>? progress = null,
        bool continueOnError = true,
        CancellationToken ct = default)
    {
        if (httpClient == null)
            throw new ArgumentNullException(nameof(httpClient));
        if (dataSource == null)
            throw new ArgumentNullException(nameof(dataSource));

        // Clear cached data from previous runs so assessors fetch fresh data
        _demographicsProvider?.ClearCache();
        _cteProgramProvider?.ClearCache();
        _courseProvider?.ClearCache();
        _studentAssessmentProvider?.ClearCache();

        var assessment = new DataSourceAssessment
        {
            Name = assessmentName ?? $"Ed-Fi Assessment - {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}",
            ConductedAt = DateTime.UtcNow
        };

        var assessorList = _assessors.ToList();
        var progressState = new AssessmentProgress
        {
            TotalAssessors = assessorList.Count,
            CompletedAssessors = 0,
            AssessorProgresses = [.. assessorList.Select(a => new AssessorProgress
            {
                DataElementName = a.DataElementName,
                Status = AssessorStatus.Pending
            })]
        };

        // Report initial state
        progress?.Report(progressState);

        // Thread-safe collection for results
        var results = new ConcurrentBag<DataElementAssessment>();
        var progressLock = new object();

        // Execute assessors with controlled parallelism
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = _maxDegreeOfParallelism,
            CancellationToken = ct
        };

        try
        {
            await Parallel.ForEachAsync(
                assessorList.Select((assessor, index) => (assessor, index)),
                parallelOptions,
                async (item, token) =>
                {
                    var (assessor, index) = item;

                    // Update status to running
                    UpdateProgress(index, AssessorStatus.Running);

                    // Create context for this assessor
                    var context = new AssessorContext(
                        progressCallback: (percentage, statusMessage) =>
                        {
                            UpdateProgressDetails(index, percentage, statusMessage);
                        },
                        logCallback: (message) =>
                        {
                            AddLogMessage(index, message);
                        }
                    );

                    try
                    {
                        var result = await assessor.AssessAsync(httpClient, dataSource, context);
                        results.Add(result);

                        // Update status to completed
                        UpdateProgress(index, AssessorStatus.Completed, result: result);
                    }
                    catch (Exception ex)
                    {
                        // Update status to failed
                        UpdateProgress(index, AssessorStatus.Failed, errorMessage: ex.Message);

                        if (!continueOnError)
                            throw;
                    }
                });
        }
        catch (TaskCanceledException)
        {
            // Should add logging
            throw;
        }

        // Collect all successful results
        assessment.DataElementAssessments = [.. results.OrderBy(r => r.DataElementName)];

        return assessment;

        void UpdateProgress(int index, AssessorStatus status, string? errorMessage = null, DataElementAssessment? result = null)
        {
            if (progress == null) return;

            lock (progressLock)
            {
                progressState.AssessorProgresses[index].Status = status;
                progressState.AssessorProgresses[index].ErrorMessage = errorMessage;
                progressState.AssessorProgresses[index].Result = result;

                if (status is AssessorStatus.Completed or AssessorStatus.Failed)
                {
                    progressState.CompletedAssessors++;
                }

                ReportProgress();
            }
        }

        void UpdateProgressDetails(int index, int? percentage, string? statusMessage)
        {
            if (progress == null) return;

            lock (progressLock)
            {
                progressState.AssessorProgresses[index].ProgressPercentage = percentage;
                progressState.AssessorProgresses[index].StatusMessage = statusMessage;
                ReportProgress();
            }
        }

        void AddLogMessage(int index, string message)
        {
            if (progress == null) return;

            lock (progressLock)
            {
                progressState.AssessorProgresses[index].LogMessages.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
                ReportProgress();
            }
        }

        void ReportProgress()
        {
            // Create a deep copy for thread safety
            var reportCopy = new AssessmentProgress
            {
                TotalAssessors = progressState.TotalAssessors,
                CompletedAssessors = progressState.CompletedAssessors,
                AssessorProgresses = [.. progressState.AssessorProgresses.Select(ap => new AssessorProgress
                {
                    DataElementName = ap.DataElementName,
                    Status = ap.Status,
                    ErrorMessage = ap.ErrorMessage,
                    Result = ap.Result,
                    ProgressPercentage = ap.ProgressPercentage,
                    StatusMessage = ap.StatusMessage,
                    LogMessages = [.. ap.LogMessages]
                })]
            };

            progress.Report(reportCopy);
        }
    }
}
