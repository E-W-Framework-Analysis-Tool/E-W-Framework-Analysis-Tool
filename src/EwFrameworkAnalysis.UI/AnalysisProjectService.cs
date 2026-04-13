using System.Text.Json;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;
using Microsoft.JSInterop;

namespace EwFrameworkAnalysis.UI;

public class AnalysisProjectService
{
    private readonly IJSRuntime _jsRuntime;
    private const string STORAGE_KEY = "ewframework_analysis_project";
    private const string WALKTHROUGH_BACKUP_KEY = "ewframework_analysis_project_backup";

    public AnalysisProject Project { get; private set; } = new();
    public IReadOnlyList<DataSource> DataSources => Project.DataSources;
    public IReadOnlyList<DataSourceAssessment> Assessments => DataSources.SelectMany(x => x.Assessments).ToList();

    // Track newly added items for UI highlighting
    public Guid? NewlyAddedDataSourceId { get; private set; }
    public Guid? NewlyAddedAssessmentId { get; private set; }
    public bool IsDemoActive { get; private set; }
    private bool _initialized = false;

    public event Action? Changed;

    public AnalysisProjectService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    private AnalysisProject? DeserializeProject(string json)
    {
        var migration = AnalysisProjectMigrator.MigrateIfNeeded(json);
        if (!migration.Success)
        {
            Console.WriteLine($"Migration failed: {migration.ErrorMessage}");
            return null;
        }

        if (migration.MigrationsApplied > 0)
            Console.WriteLine($"Applied {migration.MigrationsApplied} migration(s) before deserializing.");

        return JsonSerializer.Deserialize<AnalysisProject>(migration.Json!, GetJsonOptions());
    }

    /// <summary>
    /// Initialize the service - load from localStorage or create new project.
    /// Called once on app startup from Program.cs
    /// </summary>
    public async Task InitializeAsync()
    {
        if (_initialized) return;

        try
        {
            // If a backup exists, a walkthrough crashed — restore it unconditionally
            var backup = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", WALKTHROUGH_BACKUP_KEY);
            if (!string.IsNullOrEmpty(backup))
            {
                Console.WriteLine("Walkthrough backup detected, restoring real project.");
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", STORAGE_KEY, backup);
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", WALKTHROUGH_BACKUP_KEY);
            }

            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", STORAGE_KEY);
            if (!string.IsNullOrEmpty(json))
            {
                var project = DeserializeProject(json);
                Project = project ?? new AnalysisProject();
            }
            else
            {
                Project = new AnalysisProject();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Project = new AnalysisProject();
        }

        _initialized = true;
        Notify();
    }

    public Task<string> ExportProjectAsync()
    {
        return Task.FromResult(JsonSerializer.Serialize(Project, GetJsonOptions()));
    }

    private async Task SaveAsync()
    {
        // Safe-guard to prevent overwriting during demo walkthrough
        if (IsDemoActive) return;
        try
        {
            var json = JsonSerializer.Serialize(Project, GetJsonOptions());
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", STORAGE_KEY, json);
        }
        catch
        {
            // Handle save errors gracefully
        }
    }

    public async Task ActivateDemoProjectAsync(AnalysisProject demoProject)
    {
        var realJson = await ExportProjectAsync();
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", WALKTHROUGH_BACKUP_KEY, realJson);
        IsDemoActive = true;
        Project = demoProject;
        Notify();
    }

    public async Task DeactivateDemoProjectAsync()
    {
        var backup = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", WALKTHROUGH_BACKUP_KEY);
        if (!string.IsNullOrEmpty(backup))
        {
            var project = DeserializeProject(backup);
            Project = project ?? new AnalysisProject();
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", WALKTHROUGH_BACKUP_KEY);
        }
        IsDemoActive = false;
        Notify();
    }

    #region Data Source Management

    public DataSource? GetDataSource(Guid id) => DataSources.FirstOrDefault(d => d.Id == id);

    public DataSource? GetDataSource(string name) => DataSources.FirstOrDefault(d => d.Name == name);

    public async Task<Guid> AddDataSourceAsync(DataSourceType dataSourceType)
    {
        var dataSource = CreateDataSource(dataSourceType);
        Project.DataSources.Insert(0, dataSource);
        Project.LastModifiedAt = DateTimeOffset.Now;

        NewlyAddedDataSourceId = dataSource.Id;

        await SaveAsync();
        Notify();

        _ = Task.Run(async () =>
        {
            await Task.Delay(3000);
            NewlyAddedDataSourceId = null;
            Notify();
        });

        return dataSource.Id;
    }

    public async Task UpdateDataSourceAsync(DataSource updated)
    {
        var idx = Project.DataSources.FindIndex(d => d.Id == updated.Id);
        if (idx >= 0)
        {
            Project.DataSources[idx] = updated;
            Project.LastModifiedAt = DateTimeOffset.Now;
            await SaveAsync();
            Notify();
        }
    }

    public async Task SetEnabledAsync(Guid id, bool enabled)
    {
        var ds = DataSources.FirstOrDefault(d => d.Id == id);
        if (ds is null) return;

        ds.Enabled = enabled;
        Project.LastModifiedAt = DateTimeOffset.Now;
        await SaveAsync();
        Notify();
    }

    public async Task DeleteDataSourceAsync(Guid id)
    {
        var index = Project.DataSources.FindIndex(d => d.Id == id);
        if (index >= 0)
        {
            Project.DataSources.RemoveAt(index);
            Project.LastModifiedAt = DateTimeOffset.Now;
            await SaveAsync();
            Notify();
        }
    }

    #endregion

    #region Assessment Management

    public DataSourceAssessment? GetAssessment(Guid assessmentId)
    {
        foreach (var dataSource in DataSources)
        {
            var assessment = dataSource.Assessments.FirstOrDefault(a => a.Id == assessmentId);
            if (assessment != null)
                return assessment;
        }
        return null;
    }

    public IEnumerable<DataSourceAssessment> GetAssessmentsForDataSource(Guid dataSourceId)
    {
        var dataSource = GetDataSource(dataSourceId);
        return dataSource?.Assessments ?? [];
    }

    public IEnumerable<DataElementAssessment> GetDataElementAssessments(Guid assessmentId)
    {
        var assessment = GetAssessment(assessmentId);
        return assessment?.DataElementAssessments ?? [];
    }

    public async Task<Guid> AddAssessmentAsync(DataSourceAssessment assessment, Guid dataSourceId)
    {
        var dataSource = GetDataSource(dataSourceId);
        if (dataSource is null)
            throw new InvalidOperationException($"Data source {dataSourceId} not found");

        if (string.IsNullOrWhiteSpace(assessment.Name))
        {
            assessment.Name = $"Data Profile {DateTimeOffset.Now:yyyy-MM-dd HH:mm}";
        }

        dataSource.Assessments.Insert(0, assessment);
        Project.LastModifiedAt = DateTimeOffset.Now;

        await SetActiveAssessmentAsync(assessment.Id);
        NewlyAddedAssessmentId = assessment.Id;

        await SaveAsync();
        Notify();

        _ = Task.Run(async () =>
        {
            await Task.Delay(3000);
            NewlyAddedAssessmentId = null;
            Notify();
        });

        return assessment.Id;
    }

    public async Task UpdateAssessmentAsync(DataSourceAssessment updated)
    {
        foreach (var dataSource in DataSources)
        {
            var idx = dataSource.Assessments.FindIndex(a => a.Id == updated.Id);
            if (idx >= 0)
            {
                dataSource.Assessments[idx] = updated;
                Project.LastModifiedAt = DateTimeOffset.Now;
                await SaveAsync();
                Notify();
                return;
            }
        }
    }

    public async Task DeleteAssessmentAsync(Guid assessmentId)
    {
        foreach (var dataSource in DataSources)
        {
            var idx = dataSource.Assessments.FindIndex(a => a.Id == assessmentId);
            if (idx >= 0)
            {
                dataSource.Assessments.RemoveAt(idx);
                Project.LastModifiedAt = DateTimeOffset.Now;
                await SaveAsync();
                Notify();
                return;
            }
        }
    }

    public async Task SetActiveAssessmentAsync(Guid assessmentId)
    {
        var dataSource = DataSources
            .FirstOrDefault(ds => ds.Assessments.Any(a => a.Id == assessmentId));

        if (dataSource is null)
            return;

        foreach (var assessment in dataSource.Assessments)
        {
            assessment.Active = assessment.Id == assessmentId;
        }

        Project.LastModifiedAt = DateTimeOffset.Now;
        await SaveAsync();
        Notify();
    }

    /// <summary>
    /// Get all assessments across all data sources (flattened)
    /// </summary>
    public IEnumerable<DataSourceAssessment> GetAllAssessments()
    {
        return DataSources.SelectMany(ds => ds.Assessments);
    }

    #endregion

    #region Project Management

    /// <summary>
    /// Create a brand new empty project
    /// </summary>
    public async Task CreateNewProjectAsync()
    {
        Project = new AnalysisProject
        {
            Id = Guid.NewGuid().ToString(),
            Title = "New Analysis Project",
            CreatedAt = DateTimeOffset.Now,
            LastModifiedAt = DateTimeOffset.Now,
            DataSources = []
        };
        await SaveAsync();
        Notify();
    }

    /// <summary>
    /// Import a project from JSON string
    /// </summary>
    public async Task<ImportProjectResult> ImportProjectAsync(string json)
    {
        try
        {
            var project = DeserializeProject(json);
            if (project is null)
                return ImportProjectResult.Fail("The file could not be read as a project.");

            Project = project;
            await SaveAsync();
            Notify();
            return ImportProjectResult.Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to import project: {ex.Message}");
            return ImportProjectResult.Fail(ex.Message);
        }
    }

    public async Task UpdateProjectDetailsAsync(string? title)
    {
        Project.Title = title;
        Project.LastModifiedAt = DateTimeOffset.Now;
        await SaveAsync();
        Notify();
    }

    #endregion

    #region Private Helpers

    private void Notify() => Changed?.Invoke();

    private DataSource CreateDataSource(DataSourceType type)
    {
        return new DataSource
        {
            Id = Guid.NewGuid(),
            Name = type.GetDisplayName(),
            Description = type.GetDisplayDescription(),
            Type = type,
            Enabled = true,
            Assessments = []
        };
    }

    private JsonSerializerOptions GetJsonOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    #endregion
}

public class ImportProjectResult
{
    public bool Success { get; }
    public string? ErrorMessage { get; }

    private ImportProjectResult(bool success, string? errorMessage)
    {
        Success = success;
        ErrorMessage = errorMessage;
    }

    public static ImportProjectResult Ok() => new(true, null);
    public static ImportProjectResult Fail(string errorMessage) => new(false, errorMessage);
}
