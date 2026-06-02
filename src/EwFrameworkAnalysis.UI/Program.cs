using EwFrameworkAnalysis.Common.Assessors.Ceds;
using EwFrameworkAnalysis.Common.Assessors.EdFi;
using EwFrameworkAnalysis.Common.Mapping;
using EwFrameworkAnalysis.Common.Models.Scoring;
using EwFrameworkAnalysis.Common.Scoring;
using EwFrameworkAnalysis.Common.Services;
using EwFrameworkAnalysis.UI;
using EwFrameworkAnalysis.UI.Options;
using EwFrameworkAnalysis.UI.Pages.Survey;
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

var visualizationsOptions = new VisualizationsOptions();
builder.Configuration.GetSection(VisualizationsOptions.SectionName).Bind(visualizationsOptions);
builder.Services.AddSingleton(Options.Create(visualizationsOptions));

var contactInfoOptions = new ContactInfoOptions();
builder.Configuration.GetSection(ContactInfoOptions.SectionName).Bind(contactInfoOptions);
builder.Services.AddSingleton(Options.Create(contactInfoOptions));

builder.Services.AddScoped<EdFiAssessmentOrchestrator>();
builder.Services.AddScoped<EdFiStudentDemographicsProvider>();
builder.Services.AddScoped<EdFiCTEProgramProvider>();
builder.Services.AddScoped<EdFiCourseProvider>();
builder.Services.AddScoped<CedsDWAssessmentOrchestrator>();
builder.Services.AddScoped<DataSourceAssessmentFileParser>();
builder.Services.AddScoped<EcsStateDataParser>();
builder.Services.AddSingleton<EcsStateDataProvider>();
builder.Services.AddSingleton<AnalysisProjectService>();
builder.Services.AddScoped<PdfReportService>();
builder.Services.AddSingleton<AssessorMappingService>();
builder.Services.AddSingleton<WalkthroughService>();
builder.Services.AddScoped<ScrollService>();
builder.Services.AddSingleton<BusyService>();

builder.Services.AddSingleton<SurveyBreakdownService>();

// Use HttpClient in scoped situations, use IHttpClientFactory pattern for Singletons
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddHttpClient(string.Empty, client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

// Register Scoring Rules
builder.Services.AddSingleton<FrameworkCoverageService>();
builder.Services.AddSingleton<IDataElementScoringRule, ReportedAndCountScoringRule>();
builder.Services.AddSingleton(sp =>
{
    var rules = sp.GetServices<IDataElementScoringRule>();
    return new DataElementScoringRuleRegistry(rules);
});

// Register Ed-Fi assessors
var assessorAssembly = typeof(IEdFiAssessor).Assembly;

var edFiImplementations = assessorAssembly
    .GetTypes()
    .Where(t => typeof(IEdFiAssessor).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
foreach (var implementation in edFiImplementations)
{
    builder.Services.AddScoped(typeof(IEdFiAssessor), implementation);
}

// Register CEDS assessors
var cedsImplementations = assessorAssembly
    .GetTypes()
    .Where(t => typeof(ICedsDWAssessor).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
foreach (var implementation in cedsImplementations)
{
    builder.Services.AddScoped(typeof(ICedsDWAssessor), implementation);
}

var host = builder.Build();

// Initialize the project service after the app is built (JSRuntime is now available)
var projectService = host.Services.GetRequiredService<AnalysisProjectService>();
await projectService.InitializeAsync();
await host.RunAsync();
