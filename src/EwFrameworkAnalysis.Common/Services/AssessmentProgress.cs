using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Services;

/// <summary>
/// Overall progress report for the assessment orchestration.
/// </summary>
public class AssessmentProgress
{
    /// <summary>
    /// Total number of assessors to run.
    /// </summary>
    public int TotalAssessors { get; set; }

    /// <summary>
    /// Number of assessors that have completed (successfully or with errors).
    /// </summary>
    public int CompletedAssessors { get; set; }

    /// <summary>
    /// Individual assessor progress reports.
    /// </summary>
    public List<AssessorProgress> AssessorProgresses { get; set; } = [];

    /// <summary>
    /// Calculates the percentage complete (0-100).
    /// </summary>
    public int PercentComplete => TotalAssessors == 0 ? 0 : (CompletedAssessors * 100) / TotalAssessors;
}

/// <summary>
/// Status of an individual assessor execution.
/// </summary>
public enum AssessorStatus
{
    Pending,
    Running,
    Completed,
    Failed
}

/// <summary>
/// Progress report for an individual assessor execution.
/// </summary>
public class AssessorProgress
{
    /// <summary>
    /// The name of the data element being assessed.
    /// </summary>
    public string DataElementName { get; set; } = string.Empty;

    /// <summary>
    /// The current status of the assessment.
    /// </summary>
    public AssessorStatus Status { get; set; }

    /// <summary>
    /// Optional error message if the assessment failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// The completed assessment result, if successful.
    /// </summary>
    public DataElementAssessment? Result { get; set; }

    /// <summary>
    /// Current progress percentage (0-100) for this assessor, if available.
    /// </summary>
    public int? ProgressPercentage { get; set; }

    /// <summary>
    /// Current status message for this assessor (e.g., "Processing page 3 of 10").
    /// </summary>
    public string? StatusMessage { get; set; }

    /// <summary>
    /// Log messages from the assessor execution.
    /// </summary>
    public List<string> LogMessages { get; set; } = [];
}

/// <summary>
/// Context object passed to assessors for reporting progress and logging.
/// </summary>
public class AssessorContext
{
    private readonly Action<int?, string?>? _progressCallback;
    private readonly Action<string>? _logCallback;

    public AssessorContext(Action<int?, string?>? progressCallback = null, Action<string>? logCallback = null)
    {
        _progressCallback = progressCallback;
        _logCallback = logCallback;
    }

    /// <summary>
    /// Reports progress percentage and optional status message.
    /// </summary>
    /// <param name="percentage">Progress percentage (0-100), or null to clear.</param>
    /// <param name="statusMessage">Optional status message to display.</param>
    public void ReportProgress(int? percentage, string? statusMessage = null)
    {
        _progressCallback?.Invoke(percentage, statusMessage);
    }

    /// <summary>
    /// Logs a message for debugging or display purposes.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Log(string message)
    {
        _logCallback?.Invoke(message);
    }
}
