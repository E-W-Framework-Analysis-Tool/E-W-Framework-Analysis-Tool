using EwFrameworkAnalysis.Common.Assessors.EdFi;
using EwFrameworkAnalysis.Common.Services;
using EwFrameworkAnalysis.UI;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<AnalysisProjectService>();

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
