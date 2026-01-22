using System.Text.Json;
using System.Xml.Linq;
using EwFrameworkAnalysis.Common.FrameworkReferenceData;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Models.Scoring;
using EwFrameworkAnalysis.Common.Scoring;
using Microsoft.JSInterop;

namespace EwFrameworkAnalysis.UI;

public class AnalysisProjectService
{
    private readonly IJSRuntime _jsRuntime;
    private const string STORAGE_KEY = "ewframework_analysis_project";
    private readonly DataElementScoringRuleRegistry _ruleRegistry;

    public AnalysisProject Project { get; private set; } = new();
    public IReadOnlyList<DataSource> DataSources => Project.DataSources;
    public IReadOnlyList<DataSourceAssessment> Assessments => DataSources.SelectMany(x => x.Assessments).ToList();

    // Track newly added items for UI highlighting
    public Guid? NewlyAddedDataSourceId { get; private set; }
    public Guid? NewlyAddedAssessmentId { get; private set; }

    public event Action? Changed;

    public AnalysisProjectService(IJSRuntime jsRuntime, DataElementScoringRuleRegistry ruleRegistry)
    {
        _jsRuntime = jsRuntime;
        _ruleRegistry = ruleRegistry;
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
                Project = new AnalysisProject();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
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
            assessment.Name = $"Assessment {DateTimeOffset.Now:yyyy-MM-dd HH:mm}";
        }

        dataSource.Assessments.Insert(0, assessment);
        Project.LastModifiedAt = DateTimeOffset.Now;

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

    public async Task SetActiveAssessmentAsync(DataSourceAssessment activeAssessment)
    {
        var dataSource = DataSources
            .FirstOrDefault(ds => ds.Assessments.Any(a => a.Id == activeAssessment.Id));

        if (dataSource is null)
            return;

        foreach (var assessment in dataSource.Assessments)
        {
            assessment.Active = assessment.Id == activeAssessment.Id;
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
    public async Task<bool> ImportProjectAsync(string json)
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
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to import project: {ex.Message}");
        }
        return false;
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
