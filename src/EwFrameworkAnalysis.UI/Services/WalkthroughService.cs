namespace EwFrameworkAnalysis.UI.Services;

public class WalkthroughService
{
    public bool IsActive { get; private set; }
    public int CurrentStepIndex { get; private set; }
    public WalkthroughStepDefinition? CurrentStep =>
        IsActive && CurrentStepIndex < _steps.Count ? _steps[CurrentStepIndex] : null;
    public int TotalSteps => _steps.Count;

    private readonly List<WalkthroughStepDefinition> _steps = [.. WalkthroughSteps.All];

    public event Action? Changed;

    public void Start() { IsActive = true; CurrentStepIndex = 0; Changed?.Invoke(); }
    public void Next() { if (CurrentStepIndex < _steps.Count - 1) CurrentStepIndex++; else Stop(); Changed?.Invoke(); }
    public void Previous() { if (CurrentStepIndex > 0) CurrentStepIndex--; Changed?.Invoke(); }
    public void Stop() { IsActive = false; Changed?.Invoke(); }
}

public record WalkthroughStepDefinition(
    string StepId,           // matches registered element
    string Title,
    string Body,
    string? RequiredRoute,   // navigate here if not already on it
    TooltipPosition Position = TooltipPosition.Bottom
);

public enum TooltipPosition { Top, Bottom, Left, Right }

public static class WalkthroughSteps
{
    public static readonly List<WalkthroughStepDefinition> All =
    [
        new WalkthroughStepDefinition(
            StepId: "project-file-ops",
            Title: "Your Project Lives in a File",
            Body: "New, Save, and Load let you manage your project file. " +
                  "Everything in this tool — your data sources, assessments, and results — " +
                  "lives in a single JSON file on your machine. " +
                  "Your progress is cached in the browser so you won't lose work if you navigate away, " +
                  "but you should save to file regularly.",
            RequiredRoute: "/dashboard",
            Position: TooltipPosition.Bottom
        ),
        new WalkthroughStepDefinition(
            StepId: "data-sources-list",
            Title: "Add Your Data Sources",
            Body: "Data sources represent the systems you want to analyze for framework coverage — " +
                  "such as an Ed-Fi API or a CEDS data warehouse. " +
                  "Add one or more here, then run an assessment on each to discover what data elements are present.",
            RequiredRoute: "/dashboard",
            Position: TooltipPosition.Right
        )
    ];
}
