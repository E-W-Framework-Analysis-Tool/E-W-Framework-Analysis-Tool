using EwFrameworkAnalysis.Common.Assessors.EdFi;
using EwFrameworkAnalysis.Common.Services;
using EwFrameworkAnalysis.UI;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<EdFiAssessmentOrchestrator>();

var assessorAssembly = typeof(IEdFiAssessor).Assembly; // Gets the assembly where IEdFiAssessor is defined
var assessorImplementations = assessorAssembly
    .GetTypes()
    .Where(t => typeof(IEdFiAssessor).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

foreach (var implementation in assessorImplementations)
{
    builder.Services.AddScoped(typeof(IEdFiAssessor), implementation);
}

await builder.Build().RunAsync();
