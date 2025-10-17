// Services/AnalysisProjectService.cs
using System.Text.Json;
using EwFrameworkAnalysis.Common.Models.Project;
using Microsoft.JSInterop;

namespace EwFrameworkAnalysis.UI;

public class AnalysisProjectService
{
    private readonly IJSRuntime _jsRuntime;
    private const string STORAGE_KEY = "ewframework_analysis_project";

    public AnalysisProject Project { get; private set; } = new();
    public IReadOnlyList<DataSource> DataSources => Project.DataSources;
    public IReadOnlyList<DataSourceAssessment> Assessments => Project.DataSourceAssessments;

    // Track newly added items for UI highlighting
    public Guid? NewlyAddedDataSourceId { get; private set; }
    public Guid? NewlyAddedAssessmentId { get; private set; }

    public event Action? Changed;

    public AnalysisProjectService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Initialize the service - load from localStorage or create new project.
    /// Called once on app startup from Program.cs
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", STORAGE_KEY);
            if (!string.IsNullOrEmpty(json))
            {
                var project = JsonSerializer.Deserialize<AnalysisProject>(json, GetJsonOptions());
                Project = project ?? new AnalysisProject();
            }
            else
            {
                // No saved project, start fresh
                Project = new AnalysisProject();
            }
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
            // Deserialization failed, start with empty project
            Project = new AnalysisProject();
        }

        Notify();
    }

    public async Task<bool> LoadProjectAsync(string json)
    {
        try
        {
            var project = JsonSerializer.Deserialize<AnalysisProject>(json, GetJsonOptions());
            if (project != null)
            {
                Project = project;
                await SaveAsync();
                Notify();
                return true;
            }
        }
        catch { }
        return false;
    }

    public Task<string> ExportProjectAsync()
    {
        return Task.FromResult(JsonSerializer.Serialize(Project, GetJsonOptions()));
    }

    private async Task SaveAsync()
    {
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

    #region Data Source Management

    public DataSource? GetDataSource(Guid id) => DataSources.FirstOrDefault(d => d.Id == id);

    public DataSource? GetDataSource(string name) => DataSources.FirstOrDefault(d => d.Name == name);

    public async Task<Guid> AddDataSourceAsync(DataSourceType dataSourceType)
    {
        var dataSource = CreateDataSource(dataSourceType);
        Project.DataSources.Insert(0, dataSource);
        Project.LastModifiedAt = DateTime.UtcNow;

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
            Project.LastModifiedAt = DateTime.UtcNow;
            await SaveAsync();
            Notify();
        }
    }

    public async Task SetEnabledAsync(Guid id, bool enabled)
    {
        var ds = DataSources.FirstOrDefault(d => d.Id == id);
        if (ds is null) return;

        ds.Enabled = enabled;
        Project.LastModifiedAt = DateTime.UtcNow;
        await SaveAsync();
        Notify();
    }

    public async Task DeleteDataSourceAsync(Guid id)
    {
        var index = Project.DataSources.FindIndex(d => d.Id == id);
        if (index >= 0)
        {
            Project.DataSources.RemoveAt(index);
            Project.LastModifiedAt = DateTime.UtcNow;
            await SaveAsync();
            Notify();
        }
    }

    #endregion

    #region Assessment Management

    public DataSourceAssessment? GetAssessment(Guid id) =>
        Assessments.FirstOrDefault(a => a.Id == id);

    public IEnumerable<DataSourceAssessment> GetAssessmentsForDataSource(Guid dataSourceId)
    {
        return Assessments.Where(a =>
            a.DataElementAssessments.Any(dea => dea.DataSourceId == dataSourceId));
    }

    public IEnumerable<DataElementAssessment> GetDataElementAssessments(Guid assessmentId)
    {
        var assessment = GetAssessment(assessmentId);
        return assessment?.DataElementAssessments ?? Enumerable.Empty<DataElementAssessment>();
    }

    public async Task<Guid> AddAssessmentAsync(DataSourceAssessment assessment)
    {
        if (string.IsNullOrWhiteSpace(assessment.Name))
        {
            assessment.Name = $"Assessment {DateTime.UtcNow:yyyy-MM-dd HH:mm}";
        }

        Project.DataSourceAssessments.Insert(0, assessment);
        Project.LastModifiedAt = DateTime.UtcNow;

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
        var idx = Project.DataSourceAssessments.FindIndex(a => a.Id == updated.Id);
        if (idx >= 0)
        {
            Project.DataSourceAssessments[idx] = updated;
            Project.LastModifiedAt = DateTime.UtcNow;
            await SaveAsync();
            Notify();
        }
    }

    public async Task DeleteAssessmentAsync(Guid id)
    {
        var index = Project.DataSourceAssessments.FindIndex(a => a.Id == id);
        if (index >= 0)
        {
            Project.DataSourceAssessments.RemoveAt(index);
            Project.LastModifiedAt = DateTime.UtcNow;
            await SaveAsync();
            Notify();
        }
    }

    #endregion

    #region Project Management

    public async Task NewProjectAsync()
    {
        Project = new AnalysisProject
        {
            Id = Guid.NewGuid().ToString(),
            Title = "New Analysis Project",
            CreatedAt = DateTime.UtcNow,
            LastModifiedAt = DateTime.UtcNow,
            DataSources = [],
            DataSourceAssessments = []
        };
        await SaveAsync();
        Notify();
    }

    public async Task UpdateProjectDetailsAsync(string? title)
    {
        Project.Title = title;
        Project.LastModifiedAt = DateTime.UtcNow;
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
            Enabled = true
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
