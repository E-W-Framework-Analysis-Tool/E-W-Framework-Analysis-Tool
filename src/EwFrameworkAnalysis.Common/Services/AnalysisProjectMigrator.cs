using System.Text.Json;
using System.Text.Json.Nodes;

namespace EwFrameworkAnalysis.Common.Services;

public class MigrationResult
{
    public bool Success { get; }
    public string? Json { get; }
    public string? ErrorMessage { get; }
    public int MigrationsApplied { get; }

    private MigrationResult(bool success, string? json, string? errorMessage, int migrationsApplied)
    {
        Success = success;
        Json = json;
        ErrorMessage = errorMessage;
        MigrationsApplied = migrationsApplied;
    }

    public static MigrationResult Ok(string json, int migrationsApplied) =>
        new(true, json, null, migrationsApplied);

    public static MigrationResult Fail(string errorMessage) =>
        new(false, null, errorMessage, 0);
}


/// Migrates serialized AnalysisProject JSON from older schema versions to the current version.
/// Each migration step is a private method named MigrateV{n}ToV{n+1}, applied sequentially.
/// To add a migration: increment CurrentSchemaVersion, add a MigrateV{n}ToV{n+1} method,
/// and register it in the _migrations list.
/// </summary>
public static class AnalysisProjectMigrator
{
    public const int CurrentSchemaVersion = 2;

    // Each entry migrates from index N to N+1.
    // _migrations[0] = V1 → V2, _migrations[1] = V2 → V3, etc.
    private static readonly List<Func<JsonObject, JsonObject>> _migrations =
    [
        MigrateV1ToV2,
    ];

    /// <summary>
    /// Applies all necessary migrations to bring the JSON up to CurrentSchemaVersion.
    /// Returns a failed result if the JSON cannot be parsed.
    /// </summary>
    public static MigrationResult MigrateIfNeeded(string json)
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

        for (var v = version; v < CurrentSchemaVersion; v++)
        {
            Console.WriteLine($"Migrating project schema V{v} → V{v + 1}");
            node = _migrations[v - 1](node);
            node["schemaVersion"] = v + 1;
        }

        var migrated = node.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
        return MigrationResult.Ok(migrated, migrationsApplied: CurrentSchemaVersion - version);
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
}
