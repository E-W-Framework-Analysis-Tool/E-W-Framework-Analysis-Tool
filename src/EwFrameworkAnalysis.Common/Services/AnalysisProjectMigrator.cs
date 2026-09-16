using System.Text.Json;
using System.Text.Json.Nodes;
using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Services;

public class MigrationResult
{
    public bool Success { get; }
    public string? Json { get; }
    public string? ErrorMessage { get; }
    public int MigrationsApplied { get; }

    /// <summary>
    /// Human-readable descriptions of notable (e.g. destructive/data-altering) changes made
    /// during migration, such as data elements being renamed or split. Empty when no migration
    /// step had anything worth calling out.
    /// </summary>
    public IReadOnlyList<string> Notes { get; }

    private MigrationResult(bool success, string? json, string? errorMessage, int migrationsApplied, IReadOnlyList<string> notes)
    {
        Success = success;
        Json = json;
        ErrorMessage = errorMessage;
        MigrationsApplied = migrationsApplied;
        Notes = notes;
    }

    public static MigrationResult Ok(string json, int migrationsApplied, IReadOnlyList<string>? notes = null) =>
        new(true, json, null, migrationsApplied, notes ?? []);

    public static MigrationResult Fail(string errorMessage) =>
        new(false, null, errorMessage, 0, []);
}


/// Migrates serialized AnalysisProject JSON from older schema versions to the current version.
/// Each migration step is a private method named MigrateV{n}ToV{n+1}, applied sequentially.
/// To add a migration: increment CurrentSchemaVersion, add a MigrateV{n}ToV{n+1} method,
/// and register it in the _migrations list.
/// </summary>
public static class AnalysisProjectMigrator
{
    public const int CurrentSchemaVersion = 5;

    // Each entry migrates from index N to N+1. The Description is shown to the user as
    // migration progress (see MigrateIfNeededAsync); the Apply function's second parameter
    // collects human-readable notes about any notable/destructive change (e.g. a renamed or
    // split data element) so callers can warn the user — steps with nothing to call out can
    // ignore it. _migrations[0] = V1 → V2, _migrations[1] = V2 → V3, etc.
    private static readonly List<(string Description, Func<JsonObject, List<string>, JsonObject> Apply)> _migrations =
    [
        ("Updating numeric range format", (project, _) => MigrateV1ToV2(project)),
        ("Adding action item tracking", (project, _) => MigrateV2ToV3(project)),
        ("Recording data source versions", (project, _) => MigrateV3ToV4(project)),
        ("Updating data element definitions", MigrateV4ToV5),
    ];

    /// <summary>
    /// Applies all necessary migrations to bring the JSON up to CurrentSchemaVersion.
    /// Returns a failed result if the JSON cannot be parsed.
    /// </summary>
    public static MigrationResult MigrateIfNeeded(string json) =>
        MigrateIfNeededAsync(json).GetAwaiter().GetResult();

    /// <summary>
    /// Applies all necessary migrations to bring the JSON up to CurrentSchemaVersion, invoking
    /// <paramref name="onStep"/> (if provided) before each step so callers can show progress
    /// (e.g. "Step 2 of 4: Adding action item tracking") for projects with many steps to apply.
    /// Returns a failed result if the JSON cannot be parsed.
    /// </summary>
    public static async Task<MigrationResult> MigrateIfNeededAsync(
        string json, Func<int, int, string, Task>? onStep = null)
    {
        JsonObject? node;
        try
        {
            node = JsonNode.Parse(json)?.AsObject();
            if (node is null) return MigrationResult.Fail("JSON parsed to null.");
        }
        catch (JsonException ex)
        {
            return MigrationResult.Fail($"Invalid JSON: {ex.Message}");
        }

        var version = node["schemaVersion"]?.GetValue<int>() ?? 1;
        if (version >= CurrentSchemaVersion)
            return MigrationResult.Ok(json, migrationsApplied: 0);

        var notes = new List<string>();
        var totalSteps = CurrentSchemaVersion - version;
        var stepIndex = 0;
        for (var v = version; v < CurrentSchemaVersion; v++)
        {
            stepIndex++;
            var step = _migrations[v - 1];
            Console.WriteLine($"Migrating project schema V{v} → V{v + 1}: {step.Description}");
            if (onStep is not null)
                await onStep(stepIndex, totalSteps, step.Description);

            node = step.Apply(node, notes);
            node["schemaVersion"] = v + 1;
        }

        var migrated = node.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
        return MigrationResult.Ok(migrated, migrationsApplied: totalSteps, notes);
    }

    // -------------------------------------------------------------------------
    // V1 → V2: Renamed IntegerRange discriminator to NumericalRange
    // -------------------------------------------------------------------------
    private static JsonObject MigrateV1ToV2(JsonObject project)
    {
        RenameDiscriminator(project, "IntegerRange", "NumericalRange");
        return project;
    }

    // -------------------------------------------------------------------------
    // V2 → V3: Introduce top-level actionItems array
    // -------------------------------------------------------------------------
    private static JsonObject MigrateV2ToV3(JsonObject project)
    {
        // If the property is already present (e.g. partially migrated data),
        // leave it untouched.
        if (!project.ContainsKey("actionItems"))
            project["actionItems"] = new JsonArray();

        return project;
    }

    // -------------------------------------------------------------------------
    // V3 → V4: Backfill Version on existing versioned data sources.
    //   - cedsDw  → "v13" (all existing data was against CEDS DW v13)
    //   - edFiApi → "7.3" (assumed baseline; not yet used by the assessment runner)
    // -------------------------------------------------------------------------
    private static JsonObject MigrateV3ToV4(JsonObject project)
    {
        if (project["dataSources"] is not JsonArray dataSources)
            return project;

        foreach (var node in dataSources)
        {
            if (node is not JsonObject ds) continue;

            // Skip if version is already set (defensive — shouldn't happen at V3)
            if (ds["version"] is not null) continue;

            var type = ds["type"]?.GetValue<int>();
            ds["version"] = (DataSourceType?)type switch
            {
                DataSourceType.CedsDw => CedsDWVersions.V13,
                DataSourceType.EdFiApi => EdFiVersions.V73,
                _ => null
            };
        }

        return project;
    }

    // -------------------------------------------------------------------------
    // V4 → V5: Reference data element renames/splits.
    //   - "EAP or mental health services provided" renamed to
    //     "EAP or mental health services provided (Workforce)"
    //   - "Enlistment in the military" renamed to "Military enlistment date"
    //   - "Mental health services offered" renamed to
    //     "Mental health services offered (PK, K-12, Postsecondary)"
    //   - "Student course enrollment record" renamed to
    //     "Student course enrollment record (K-12)" (removed as a duplicate of the latter)
    //   - "Course performance (English and Math)" split into "Course outcome"
    //     and "Course subject area" (existing assessment data is copied to both)
    // -------------------------------------------------------------------------
    private static readonly Dictionary<string, string> _renamedDataElements = new()
    {
        ["EAP or mental health services provided"] = "EAP or mental health services provided (Workforce)",
        ["Enlistment in the military"] = "Military enlistment date",
        ["Mental health services offered"] = "Mental health services offered (PK, K-12, Postsecondary)",
        ["Student course enrollment record"] = "Student course enrollment record (K-12)",
    };

    private static readonly Dictionary<string, string[]> _splitDataElements = new()
    {
        ["Course performance (English and Math)"] = ["Course outcome", "Course subject area"],
    };

    private static JsonObject MigrateV4ToV5(JsonObject project, List<string> notes)
    {
        var renamed = new HashSet<string>();
        RenameDataElement(project, _renamedDataElements, renamed);
        foreach (var oldName in renamed)
            notes.Add($"Renamed data element \"{oldName}\" to \"{_renamedDataElements[oldName]}\".");

        var split = new HashSet<string>();
        SplitDataElement(project, _splitDataElements, split);
        foreach (var oldName in split)
            notes.Add(
                $"Split data element \"{oldName}\" into \"{string.Join("\" and \"", _splitDataElements[oldName])}\". " +
                "Any existing assessment data for it has been copied to both new elements.");

        return project;
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    /// <summary>
    /// Walks the entire JSON tree and replaces a $type discriminator value wherever it appears.
    /// </summary>
    private static void RenameDiscriminator(JsonNode? node, string oldValue, string newValue)
    {
        if (node is JsonObject obj)
        {
            if (obj["$type"]?.GetValue<string>() == oldValue)
                obj["$type"] = newValue;

            foreach (var prop in obj)
                RenameDiscriminator(prop.Value, oldValue, newValue);
        }
        else if (node is JsonArray arr)
        {
            foreach (var item in arr)
                RenameDiscriminator(item, oldValue, newValue);
        }
    }

    /// <summary>
    /// Walks the entire JSON tree and renames "dataElementName" values matching a key in <paramref name="renames"/>.
    /// </summary>
    private static void RenameDataElement(JsonNode? node, Dictionary<string, string> renames, HashSet<string> found)
    {
        if (node is JsonObject obj)
        {
            if (obj["dataElementName"]?.GetValue<string>() is { } name && renames.TryGetValue(name, out var newName))
            {
                obj["dataElementName"] = newName;
                found.Add(name);
            }

            foreach (var prop in obj)
                RenameDataElement(prop.Value, renames, found);
        }
        else if (node is JsonArray arr)
        {
            foreach (var item in arr)
                RenameDataElement(item, renames, found);
        }
    }

    /// <summary>
    /// Walks the entire JSON tree and, wherever an array item has a "dataElementName" matching a
    /// key in <paramref name="splits"/>, replaces that single item with one clone per new name
    /// (each carrying a fresh id, since the original id can no longer correctly identify either).
    /// </summary>
    private static void SplitDataElement(JsonNode? node, Dictionary<string, string[]> splits, HashSet<string> found)
    {
        if (node is JsonObject obj)
        {
            foreach (var prop in obj)
                SplitDataElement(prop.Value, splits, found);
        }
        else if (node is JsonArray arr)
        {
            for (var i = 0; i < arr.Count; i++)
            {
                SplitDataElement(arr[i], splits, found);

                if (arr[i] is not JsonObject item ||
                    item["dataElementName"]?.GetValue<string>() is not { } name ||
                    !splits.TryGetValue(name, out var newNames))
                    continue;

                var originalJson = item.ToJsonString();
                arr.RemoveAt(i);

                for (var j = 0; j < newNames.Length; j++)
                {
                    var clone = JsonNode.Parse(originalJson)!.AsObject();
                    clone["dataElementName"] = newNames[j];
                    if (clone.ContainsKey("id"))
                        clone["id"] = Guid.NewGuid().ToString();

                    arr.Insert(i + j, clone);
                }

                found.Add(name);
                i += newNames.Length - 1;
            }
        }
    }
}
