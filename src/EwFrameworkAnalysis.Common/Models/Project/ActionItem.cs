namespace EwFrameworkAnalysis.Common.Models.Project;

public class ActionItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public DateTimeOffset? TargetDate { get; set; }
    public bool IsResolved { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsSuggested { get; set; }
    public string? SuggestionKey { get; set; }
    public bool IsDismissed { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
}

public static class SuggestionKeys
{
    public const string NoDataSources = "no-data-sources";
    public const string NoEcsDataSource = "no-ecs-data-source";

    public static string StaleProfile(Guid dataSourceId) => $"stale-profile:{dataSourceId}";
    public static string SourceNotProfiled(Guid dataSourceId) => $"source-not-profiled:{dataSourceId}";
}
