using System.ComponentModel.DataAnnotations;
namespace EwFrameworkAnalysis.Common.Models.Project;

/// <summary>
/// A data source that can be assessed (API, SQL, Manual)
/// </summary>
public class DataSource
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool Enabled { get; set; } = true;
    public DataSourceType Type { get; set; }

    /// <summary>
    /// The version of this data source, if applicable. Populated at creation time
    /// for versioned source types (e.g. <see cref="CedsDwVersions"/>, <see cref="EdFiVersions"/>).
    /// Null for unversioned types such as <see cref="DataSourceType.Custom"/>.
    /// Use <see cref="DataSourceVersionRegistry.GetVersions"/> to determine whether
    /// a given type supports versioning.
    /// </summary>
    public string? Version { get; set; }

    public List<DataSourceAssessment> Assessments { get; set; } = [];
}

public enum DataSourceType
{
    [Display(Name = "Ed-Fi ODS API", Description = "Ed-Fi ODS API endpoint for automated data discovery")]
    EdFiApi,
    [Display(Name = "CEDS Data Warehouse", Description = "Upload SQL result sets from CEDS DW")]
    CedsDw,
    [Display(Name = "Manual Entry", Description = "Flexible checklist for manual review")]
    Custom,
    [Display(Name = "ECS State Reference Profile", Description = "State-specific data that research shows is reported by the state")]
    EcsState
}

public static class CedsDwVersions
{
    public const string V13 = "v13";
    public const string V14 = "v14";

    /// <summary>Ordered from earliest to latest.</summary>
    public static readonly IReadOnlyList<string> All = [V13, V14];
}

public static class EdFiVersions
{
    public const string V73 = "7.3";

    /// <summary>Ordered from earliest to latest.</summary>
    public static readonly IReadOnlyList<string> All = [V73];
}

/// <summary>
/// Maps <see cref="DataSourceType"/> values to their known ordered version lists.
/// Types absent from this registry do not support versioning.
/// </summary>
public static class DataSourceVersionRegistry
{
    private static readonly Dictionary<DataSourceType, List<string>> _versions = new()
    {
        [DataSourceType.CedsDw] = [.. CedsDwVersions.All],
        [DataSourceType.EdFiApi] = [.. EdFiVersions.All],
    };

    /// <summary>
    /// Returns the ordered version list for <paramref name="type"/>,
    /// or null if the type does not support versioning.
    /// </summary>
    public static IReadOnlyList<string>? GetVersions(DataSourceType type) =>
        _versions.TryGetValue(type, out var versions) ? versions : null;

    public static bool IsVersioned(DataSourceType type) =>
        _versions.ContainsKey(type);

    /// <summary>
    /// Returns true if <paramref name="target"/> falls within [<paramref name="min"/>, <paramref name="maxInclusive"/>]
    /// according to the declared sort order for <paramref name="type"/>.
    /// </summary>
    public static bool IsInRange(DataSourceType type, string target, string min, string? maxInclusive)
    {
        if (!_versions.TryGetValue(type, out var all)) return false;

        var targetIdx = all.IndexOf(target);
        var minIdx = all.IndexOf(min);
        if (minIdx == -1 || targetIdx == -1) return false;
        var maxIdx = maxInclusive is null ? int.MaxValue : all.IndexOf(maxInclusive);

        return targetIdx >= minIdx && targetIdx <= maxIdx;
    }
}
