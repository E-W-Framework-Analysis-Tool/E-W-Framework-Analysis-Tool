using System.Net.Http.Json;
using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.UI.Services;

public class WalkthroughService
{
    public bool IsActive { get; private set; }
    public int CurrentStepIndex { get; private set; }
    public WalkthroughStepDefinition? CurrentStep =>
        IsActive && CurrentStepIndex < _steps.Count ? _steps[CurrentStepIndex] : null;
    public int TotalSteps => _steps.Count;

    private readonly List<WalkthroughStepDefinition> _steps = [.. WalkthroughSteps.All];
    private readonly AnalysisProjectService _projectSvc;
    private readonly IHttpClientFactory _httpClientFactory;

    public event Action? Changed;

    public WalkthroughService(AnalysisProjectService projectSvc, IHttpClientFactory httpClientFactory)
    {
        _projectSvc = projectSvc;
        _httpClientFactory = httpClientFactory;
    }

    public async Task StartAsync()
    {
        using var http = _httpClientFactory.CreateClient();
        var demoProject = await DemoProject.LoadAsync(http);
        await _projectSvc.ActivateDemoProjectAsync(demoProject ?? new AnalysisProject());
        IsActive = true;
        CurrentStepIndex = 0;
        Changed?.Invoke();
    }

    public async Task StopAsync()
    {
        IsActive = false;
        await _projectSvc.DeactivateDemoProjectAsync();
        Changed?.Invoke();
    }
    public async Task NextAsync()
    {
        if (CurrentStepIndex < _steps.Count - 1)
            CurrentStepIndex++;
        else
            await StopAsync();
        Changed?.Invoke();
    }

    public Task PreviousAsync()
    {
        if (CurrentStepIndex > 0)
            CurrentStepIndex--;
        Changed?.Invoke();

        return Task.CompletedTask;
    }
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

public static class DemoProject
{
    public static async Task<AnalysisProject?> LoadAsync(HttpClient http)
    {
        return await http.GetFromJsonAsync<AnalysisProject>("Walkthrough_Demo.json");
    }
}
