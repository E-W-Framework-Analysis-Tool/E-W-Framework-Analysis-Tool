using Microsoft.JSInterop;

namespace EwFrameworkAnalysis.UI.Services;

public class ScrollService(IJSRuntime js)
{
    public ValueTask ScrollToAsync(string elementId) =>
        js.InvokeVoidAsync("scrollToSection", elementId);
}
