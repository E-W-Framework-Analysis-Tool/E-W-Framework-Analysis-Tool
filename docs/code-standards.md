# Code Standards

This document outlines the coding standards and conventions used in this project.

## General .NET Standards

### EditorConfig and Analyzers

We use a comprehensive `.editorconfig` file to enforce consistent code style across the solution. The configuration
includes:

- **Modern C# patterns** - Collection expressions, primary constructors, simplified object creation
- **Null safety** - Null-conditional operators, pattern matching over traditional null checks
- **Code quality** - Readonly fields, expression-bodied members, consistent modifier ordering
- **Warnings as errors** - Strict enforcement of style rules to maintain code quality

### Build Configuration

**Directory.Build.props** is used to apply consistent settings across all projects:

```xml
<PropertyGroup>
  <EnableNETAnalyzers>true</EnableNETAnalyzers>
  <AnalysisLevel>latest</AnalysisLevel>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
</PropertyGroup>
```

### Analyzers

We use Microsoft's built-in .NET analyzers rather than legacy tools like StyleCop. These provide:

- Built-in code analysis rules (CA rules)
- Performance and security guidance
- Modern C# best practices
- Integration with the build system

### Naming Conventions

- **Private readonly fields:** `_camelCase`
- **Private fields:** `_camelCase`
- **Constants:** `UPPER_CASE`
- **Public members:** `PascalCase`
- **Parameters/locals:** `camelCase`
- **Interfaces:** `IPascalCase`

## Blazor-Specific Standards

### UI Project Structure

The UI project distinguishes between three kinds of Razor files, organized by their scope and reusability:

**Pages** (`Pages/`) are top-level routable components. Each page has a corresponding `@page` directive and is
registered in the application's routing. Pages are responsible for orchestrating their layout and composing
page-specific components.

```text
Pages/
  DashboardPage.razor
  DataSourceDetailsPage.razor
  AnalysisPage.razor
  MappingOverview.razor
  VisualizationsPage.razor
  ...
```

**Page-specific components** live in a subdirectory named after their parent page. These components are tightly coupled
to a single page's concerns and are not intended for reuse elsewhere. Co-locating them with their page makes the
relationship explicit and keeps page-level complexity manageable.

```text
Pages/
  DataSourceDetails/
    AssessmentDetailsView.razor
    AssessmentHistoryCard.razor
    AssessmentRunnerModal.razor
    DataElementCard.razor
    DataElementList.razor
    DataSourceHeader.razor
    ...
  Dashboard/
    ...
  Analysis/
    ...
```

**Shared components** (`Components/`) are page-agnostic and reusable across the application. A component belongs here
when it has no dependency on a specific page's data model or workflow — it receives everything it needs through
parameters.

```text
Components/
  Card.razor
  CollapsibleSection.razor
  EditableField.razor
  Modal.razor
  StatCard.razor
  WalkthroughOverlay.razor
  ...
```

When building a new feature, prefer page-specific components until a clear reuse need emerges. Promote to `Components/`
only when the component is genuinely page-agnostic, not simply because it might be used again someday.

### Component File Structure

Organize Blazor components with this consistent structure:

```razor
@page "/my-component"
@using MyNamespace.Models
@using MyNamespace.Services
@inject IMyService MyService
@inject IJSRuntime JSRuntime
@inherits ComponentBase
@implements IDisposable

<MudContainer>
    <!-- Razor markup here -->
</MudContainer>

<style>
    .my-component-class {
        /* Component-specific styles */
    }
</style>

@code {
    // 1. Parameters (with EditorRequired for required ones)
    [Parameter, EditorRequired]
    public string RequiredProperty { get; set; } = null!;

    [Parameter]
    public string? OptionalProperty { get; set; }

    // 2. Private fields
    private bool _isLoading = true;
    private List<MyModel> _items = [];

    // 3. Lifecycle methods
    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        // Handle parameter changes
    }

    // 4. Other methods
    private async Task LoadDataAsync()
    {
        // Implementation
    }

    public void Dispose()
    {
        // Cleanup
    }
}
```

### MudBlazor Guidelines

**CSS Isolation:** Avoid CSS isolation files (`.razor.css`) as they can conflict with MudBlazor's theming system.
Instead:

- Use `<style>` tags within components for component-specific styles
- Leverage MudBlazor's built-in styling system and CSS classes
- Use global stylesheets for application-wide styles

**Component Parameters:** Always use `[EditorRequired]` for required parameters to catch missing bindings at design
time:

```razor
@code {
    [Parameter, EditorRequired]
    public string Title { get; set; } = null!;

    [Parameter]
    public EventCallback<string> OnValueChanged { get; set; }
}
```

**Event Handling:** Prefer async event handlers and use proper error handling:

```razor
<MudButton OnClick="HandleClickAsync">Click Me</MudButton>

@code {
    private async Task HandleClickAsync()
    {
        try
        {
            await SomeAsyncOperation();
        }
        catch (Exception ex)
        {
            // Handle error appropriately
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
```

### Performance Considerations

**StateHasChanged:** Only call explicitly when necessary, as Blazor handles most updates automatically.

**Dispose Pattern:** Always implement `IDisposable` for components that:

- Subscribe to events
- Use timers
- Hold unmanaged resources
- Subscribe to services with event handlers

```csharp
public void Dispose()
{
    MyService.OnDataChanged -= HandleDataChanged;
    _timer?.Dispose();
}
```

### Dependency Injection

Use constructor injection in code-behind classes and `@inject` directives in `.razor` files:

```razor
@inject IMyService MyService
@inject IJSRuntime JSRuntime

@code {
    // Use injected services directly
    private async Task LoadData()
    {
        var data = await MyService.GetDataAsync();
    }
}
```

## Testing Standards

### Unit Tests

- Use descriptive test method names: `Should_ReturnExpectedResult_When_GivenValidInput`
- Follow AAA pattern: Arrange, Act, Assert
- Use FluentAssertions for readable assertions
- Mock external dependencies using interfaces

### E2E Tests

- Inherit from `BaseE2ETest` for consistent setup
- Use descriptive test names that include browser and scenario
- Group related tests with `[Trait]` attributes for better organization
- Follow accessibility testing guidelines with appropriate fail levels

```csharp
[Theory]
[MemberData(nameof(Browsers))]
public async Task HomePage_Should_Display_Correct_Title_Async(Browser browser, string? deviceName)
{
    await using var test = await Fixture.Builder!.BuildAsync(browser, deviceName: deviceName);

    await test.GotoPageAsync("/", async page =>
    {
        await page.Locator("body").WaitForAsync();
        var title = await page.TitleAsync();
        title.Should().Be("E-W Framework Analysis Tool");
    });
}
```
