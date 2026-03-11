using EwFrameworkAnalysis.Common.Assessors.EdFi;
using EwFrameworkAnalysis.Common.Models.Scoring;
using EwFrameworkAnalysis.Common.Scoring;
using EwFrameworkAnalysis.Common.Services;
using EwFrameworkAnalysis.UI;
using EwFrameworkAnalysis.UI.Options;
using EwFrameworkAnalysis.UI.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Options;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register Options (e.g. from appsettings.json)
var demoApiOptions = new DemoEdFiApiOptions();
builder.Configuration.GetSection("DemoEdFiApi").Bind(demoApiOptions);
builder.Services.AddSingleton(Options.Create(demoApiOptions));

var demoCedsOptions = new DemoCedsOptions();
builder.Configuration.GetSection("DemoCeds").Bind(demoCedsOptions);
builder.Services.AddSingleton(Options.Create(demoCedsOptions));

var deploymentInfoOptions = new DeploymentInfoOptions();
builder.Configuration.GetSection("DeploymentInfo").Bind(deploymentInfoOptions);
builder.Services.AddSingleton(Options.Create(deploymentInfoOptions));

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<EdFiAssessmentOrchestrator>();
builder.Services.AddScoped<CedsDWAssessmentOrchestrator>();
builder.Services.AddScoped<DataSourceAssessmentFileParser>();
builder.Services.AddScoped<EcsStateDataParser>();
builder.Services.AddSingleton<EcsStateDataProvider>();
builder.Services.AddSingleton<AnalysisProjectService>();
builder.Services.AddScoped<PdfReportService>();
builder.Services.AddSingleton<WalkthroughService>();
builder.Services.AddScoped<ScrollService>();

// Register Scoring Rules
builder.Services.AddSingleton<DataElementScoringService>();
builder.Services.AddSingleton<IDataElementScoringRule, ReportedAndCountScoringRule>();
builder.Services.AddSingleton(sp =>
{
    var rules = sp.GetServices<IDataElementScoringRule>();
    return new DataElementScoringRuleRegistry(rules);
});

var assessorAssembly = typeof(IEdFiAssessor).Assembly;
var assessorImplementations = assessorAssembly
    .GetTypes()
    .Where(t => typeof(IEdFiAssessor).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

foreach (var implementation in assessorImplementations)
{
    builder.Services.AddScoped(typeof(IEdFiAssessor), implementation);
}

var host = builder.Build();

// Initialize the project service after the app is built (JSRuntime is now available)
var projectService = host.Services.GetRequiredService<AnalysisProjectService>();
await projectService.InitializeAsync();

await host.RunAsync();
