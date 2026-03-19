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
    private static readonly string _dataSourceRoute =
        AppRoutes.DataSourceDetails.ForDataSource(new Guid("f33342b7-8bcd-4fe2-9b61-4fadb9298a45"));

    public static readonly List<WalkthroughStepDefinition> All =
    [
        new WalkthroughStepDefinition(
            StepId: StepIds.ProjectFileOperations,
            Title: "Your Project Lives in a File",
            Body: "New, Save, and Load let you manage your project file. " +
                  "Everything in this tool — your data sources, data profiles, and results — " +
                  "lives in a single JSON file on your machine. " +
                  "Your progress is cached in the browser so you won't lose work if you navigate away, " +
                  "but you should save to file regularly.",
            RequiredRoute: "/dashboard",
            Position: TooltipPosition.Bottom
        ),
        new WalkthroughStepDefinition(
            StepId: StepIds.DataSourcesList,
            Title: "Add Your Data Sources",
            Body: "Data sources represent the systems you want to analyze for Framework coverage — " +
                  "such as an Ed-Fi API or a CEDS data warehouse. " +
                  "Add one or more here, then run a scan or manual entry on each to discover what data elements are present.",
            RequiredRoute: "/dashboard",
            Position: TooltipPosition.Right
        ),
        new WalkthroughStepDefinition(
            StepId: StepIds.DataSourceItem,
            Title: "View Data Source Details",
            Body: "Click a Data Source here to navigate to the Data Source details page.",
            RequiredRoute: "/dashboard",
            Position: TooltipPosition.Right
        ),
        new WalkthroughStepDefinition(
            StepId: StepIds.RunAssessmentButton,
            Title: "Profile Your Data Source",
            Body: "In the Data Source details page, run an automated scan or update a manual entry to profile the data elements " +
                  "present in your system. This process inspects your source and maps what it finds " +
                  "against the Framework's expected elements.",
            RequiredRoute: _dataSourceRoute,
            Position: TooltipPosition.Bottom
        ),
        new WalkthroughStepDefinition(
            StepId: StepIds.DataSourceAssessmentHistory,
            Title: "Data Profile History",
            Body: "Each time you profile a data source, a snapshot is saved here. " +
                  "You can compare profiles over time to track how your framework coverage changes " +
                  "as your systems evolve.",
            RequiredRoute: _dataSourceRoute,
            Position: TooltipPosition.Right
        ),
        new WalkthroughStepDefinition(
            StepId: StepIds.DataSourceAssessmentDetails,
            Title: "Data Profile Details",
            Body: "Drill into an individual data profile to see exactly which data elements were found, " +
                  "which were missing, and any issues encountered during profiling.",
            RequiredRoute: _dataSourceRoute,
            Position: TooltipPosition.Right
        ),
        new WalkthroughStepDefinition(
            StepId: StepIds.AnalysisNavLink,
            Title: "Analysis Page",
            Body: "Navigate to the Reports & Analysis page after adding data sources to view Framework insights.",
            RequiredRoute: _dataSourceRoute,
            Position: TooltipPosition.Right
        ),
        new WalkthroughStepDefinition(
            StepId: StepIds.EssentialQuestionsScoreGrid,
            Title: "Essential Questions Coverage",
            Body: "This grid shows how well your data sources collectively support each of the " +
                  "Framework's Essential Questions — the high-level outcomes the Framework is designed to answer.",
            RequiredRoute: AppRoutes.Analysis,
            Position: TooltipPosition.Top
        ),
        new WalkthroughStepDefinition(
            StepId: StepIds.ScoreGridSelector,
            Title: "Filter the Score Grid",
            Body: "Switch between views to explore coverage by Essential Question, Indicator, or Disaggregate. " +
                  "Use this to identify where gaps are concentrated.",
            RequiredRoute: AppRoutes.Analysis,
            Position: TooltipPosition.Bottom
        ),
        new WalkthroughStepDefinition(
            StepId: StepIds.DownloadReportButton,
            Title: "Download Your Report",
            Body: "Export a full analysis report summarizing your Framework coverage across all " +
                  "data sources. Share this with stakeholders or use it to guide your data improvement efforts.",
            RequiredRoute: AppRoutes.Analysis,
            Position: TooltipPosition.Bottom
        ),
    ];

    public static class StepIds
    {
        public const string ProjectFileOperations = "project-file-ops";
        public const string DataSourcesList = "data-sources-list";
        public const string RunAssessmentButton = "run-assessment";
        public const string DataSourceAssessmentHistory = "data-source-assessment-history";
        public const string DataSourceAssessmentDetails = "data-source-assessment-details";
        public const string DownloadReportButton = "analysis-report-download";
        public const string EssentialQuestionsScoreGrid = "eq-score-grid";
        public const string ScoreGridSelector = "score-grid-selector";
        public const string DashboardNavLink = "dashboard-link";
        public const string AnalysisNavLink = "analysis-link";
        public const string DataSourceItem = "data-source-item";
    }
}

public static class DemoProject
{
    public static async Task<AnalysisProject?> LoadAsync(HttpClient http)
    {
        return await http.GetFromJsonAsync<AnalysisProject>("Walkthrough_Demo.json");
    }
}
