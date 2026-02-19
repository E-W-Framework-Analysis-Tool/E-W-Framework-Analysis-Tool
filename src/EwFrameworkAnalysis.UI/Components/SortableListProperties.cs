namespace EwFrameworkAnalysis.UI.Components;

public sealed record class SortOptions<T>(
    IEnumerable<SortColumn<T>> Columns,
    IEnumerable<T> List,
    Func<string, Task> Toggle,
    Func<string, bool, bool> IsActive,
    string? ActiveColumn,
    bool Ascending
);

public class SortColumn<T>
{
    public string Key { get; set; } = default!;

    public Func<T, object?> Selector { get; set; } = default!;

    /// <summary>
    /// Whether the default sort order for this column is ascending
    /// </summary>
    public bool DefaultAscending { get; set; } = true;
}
