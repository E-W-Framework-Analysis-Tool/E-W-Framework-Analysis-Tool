using Blazored.LocalStorage;
using EwFrameworkAnalysis.Blazor.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

namespace EwFrameworkAnalysis.Blazor;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        builder.Services.AddScoped<EwFrameworkService>();
        builder.Services.AddScoped<QuestionScoringService>();
        builder.Services.AddMemoryCache();
        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddMudServices();

        await builder.Build().RunAsync();
    }
}
