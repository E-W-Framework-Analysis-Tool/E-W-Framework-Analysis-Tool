using System.Text.Json;
using System.Text.Json.Nodes;
using EwFrameworkAnalysis.Common.Services;
using Xunit.Abstractions;

namespace EwFrameworkAnalysis.Common.UnitTests.Services;

public class AnalysisProjectMigratorTests
{
    private readonly ITestOutputHelper _output;

    public AnalysisProjectMigratorTests(ITestOutputHelper output)
    {
        _output = output;
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static string LoadFixture(string filename)
    {
        var assembly = typeof(AnalysisProjectMigratorTests).Assembly;
        var resourceName = $"{assembly.GetName().Name}.Fixtures.Migrations.{filename}";
        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException(
                $"Fixture not found: '{resourceName}'. " +
                $"Available: {string.Join(", ", assembly.GetManifestResourceNames())}");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private static int GetSchemaVersion(string json)
    {
        var node = JsonNode.Parse(json)?.AsObject();
        return node?["schemaVersion"]?.GetValue<int>() ?? 1;
    }

    private static bool ContainsDiscriminator(string json, string discriminator)
    {
        var node = JsonNode.Parse(json);
        return ContainsDiscriminatorRecursive(node, discriminator);
    }

    private static bool ContainsDiscriminatorRecursive(JsonNode? node, string discriminator)
    {
        if (node is JsonObject obj)
        {
            if (obj["$type"]?.GetValue<string>() == discriminator) return true;
            foreach (var prop in obj)
                if (ContainsDiscriminatorRecursive(prop.Value, discriminator)) return true;
        }
        else if (node is JsonArray arr)
        {
            foreach (var item in arr)
                if (ContainsDiscriminatorRecursive(item, discriminator)) return true;
        }
        return false;
    }

    // -------------------------------------------------------------------------
    // Version detection
    // -------------------------------------------------------------------------

    [Fact]
    public void MigrateIfNeeded_AlreadyCurrentVersion_ReturnsUnchanged()
    {
        // project_v3.json is the current version — must not be migrated
        var json = LoadFixture("project_v3.json");
        var result = AnalysisProjectMigrator.MigrateIfNeeded(json);

        Assert.True(result.Success);
        Assert.Equal(0, result.MigrationsApplied);
        Assert.Equal(
            JsonNode.Parse(json)!.ToJsonString(),
            JsonNode.Parse(result.Json!)!.ToJsonString()
        );
        _output.WriteLine("V3 passed through unchanged.");
    }

    [Fact]
    public void MigrateIfNeeded_MissingSchemaVersion_TreatedAsV1()
    {
        var json = """{ "title": "Old Project", "dataSources": [] }""";
        var result = AnalysisProjectMigrator.MigrateIfNeeded(json);

        Assert.True(result.Success);
        Assert.Equal(AnalysisProjectMigrator.CurrentSchemaVersion, GetSchemaVersion(result.Json!));
        _output.WriteLine($"No-version project migrated to V{GetSchemaVersion(result.Json!)}.");
    }

    [Fact]
    public void MigrateIfNeeded_InvalidJson_ReturnsFailedResult()
    {
        var result = AnalysisProjectMigrator.MigrateIfNeeded("this is not json");
        Assert.False(result.Success);
        Assert.Null(result.Json);
        Assert.NotNull(result.ErrorMessage);
        _output.WriteLine($"Invalid JSON returned failed result: {result.ErrorMessage}");
    }

    // -------------------------------------------------------------------------
    // V1 → V2: IntegerRange → NumericalRange
    // -------------------------------------------------------------------------

    [Fact]
    public void MigrateV1ToV2_RenamesIntegerRangeDiscriminator()
    {
        var v1 = LoadFixture("project_v1.json");
        Assert.Equal(1, GetSchemaVersion(v1));

        var result = AnalysisProjectMigrator.MigrateIfNeeded(v1);

        Assert.False(ContainsDiscriminator(result.Json!, "IntegerRange"),
            "Output should not contain 'IntegerRange' discriminator");
        Assert.True(ContainsDiscriminator(result.Json!, "NumericalRange"),
            "Output should contain 'NumericalRange' discriminator");
        _output.WriteLine("IntegerRange → NumericalRange rename confirmed.");
    }

    [Fact]
    public void MigrateV1_ProducesCurrentSchemaVersion()
    {
        // Running V1 through the full migrator should reach CurrentSchemaVersion,
        // not stop at V2. The old test asserting == 2 was wrong once V3 existed.
        var v1 = LoadFixture("project_v1.json");
        var result = AnalysisProjectMigrator.MigrateIfNeeded(v1);

        Assert.Equal(AnalysisProjectMigrator.CurrentSchemaVersion, GetSchemaVersion(result.Json!));
        _output.WriteLine($"V1 migrated to current version V{GetSchemaVersion(result.Json!)}.");
    }

    [Fact]
    public void MigrateV1_ReportsCorrectMigrationsApplied()
    {
        var v1 = LoadFixture("project_v1.json");
        var result = AnalysisProjectMigrator.MigrateIfNeeded(v1);

        // V1 → V2 → V3 = 2 migrations
        Assert.Equal(AnalysisProjectMigrator.CurrentSchemaVersion - 1, result.MigrationsApplied);
        _output.WriteLine($"{result.MigrationsApplied} migration(s) applied from V1.");
    }

    [Fact]
    public void MigrateV1_ProducesExpectedOutput()
    {
        var v1 = LoadFixture("project_v1.json");
        var result = AnalysisProjectMigrator.MigrateIfNeeded(v1);

        Assert.True(result.Success);
        Assert.Equal(AnalysisProjectMigrator.CurrentSchemaVersion, GetSchemaVersion(result.Json!));
        Assert.False(ContainsDiscriminator(result.Json!, "IntegerRange"));
        Assert.True(ContainsDiscriminator(result.Json!, "NumericalRange"));

        var node = JsonNode.Parse(result.Json!)!.AsObject();
        Assert.Equal("fixture-project-001", node["id"]?.GetValue<string>());
        Assert.Equal("Migration Fixture Project", node["title"]?.GetValue<string>());

        _output.WriteLine("Migrated V1 has correct structure and preserved data.");
    }

    [Fact]
    public void MigrateV1_PreservesNonDiscriminatorData()
    {
        var v1 = LoadFixture("project_v1.json");
        var v1Node = JsonNode.Parse(v1)!.AsObject();
        var result = JsonNode.Parse(AnalysisProjectMigrator.MigrateIfNeeded(v1).Json!)!.AsObject();

        Assert.Equal(v1Node["title"]?.GetValue<string>(), result["title"]?.GetValue<string>());
        Assert.Equal(v1Node["id"]?.GetValue<string>(), result["id"]?.GetValue<string>());

        var v1Sources = v1Node["dataSources"]?.AsArray();
        var resultSources = result["dataSources"]?.AsArray();
        Assert.Equal(v1Sources?.Count, resultSources?.Count);
        _output.WriteLine("Non-discriminator data preserved through migration.");
    }

    [Fact]
    public void MigrateV1ToV2_WithNoIntegerRangeCharacteristics_StillReachesCurrentVersion()
    {
        var json = """
            {
              "schemaVersion": 1,
              "id": "test-id",
              "title": "No Ranges Project",
              "dataSources": []
            }
            """;

        var result = AnalysisProjectMigrator.MigrateIfNeeded(json);

        Assert.Equal(AnalysisProjectMigrator.CurrentSchemaVersion, GetSchemaVersion(result.Json!));
        Assert.False(ContainsDiscriminator(result.Json!, "IntegerRange"));
        _output.WriteLine("Version reached current even with no IntegerRange characteristics present.");
    }

    // -------------------------------------------------------------------------
    // V2 → V3: actionItems array added
    // -------------------------------------------------------------------------

    [Fact]
    public void MigrateV2ToV3_AddsActionItemsArray()
    {
        var v2 = LoadFixture("project_v2.json");
        Assert.Equal(2, GetSchemaVersion(v2));

        var result = AnalysisProjectMigrator.MigrateIfNeeded(v2);

        var node = JsonNode.Parse(result.Json!)!.AsObject();
        Assert.True(node.ContainsKey("actionItems"), "actionItems key should be present after migration");
        Assert.Equal(JsonValueKind.Array, node["actionItems"]!.GetValueKind());
        _output.WriteLine("actionItems array added by V2→V3 migration.");
    }

    [Fact]
    public void MigrateV2ToV3_ActionItemsIsEmptyArray_WhenNotPreviouslyPresent()
    {
        var v2 = LoadFixture("project_v2.json");
        var result = AnalysisProjectMigrator.MigrateIfNeeded(v2);

        var node = JsonNode.Parse(result.Json!)!.AsObject();
        var actionItems = node["actionItems"]!.AsArray();
        Assert.Empty(actionItems);
        _output.WriteLine("actionItems defaults to empty array.");
    }

    [Fact]
    public void MigrateV2ToV3_PreservesExistingActionItems_WhenAlreadyPresent()
    {
        // Defensive: if somehow actionItems already exists (e.g. partial migration),
        // the migration should not clobber it.
        var json = """
            {
              "schemaVersion": 2,
              "id": "test-id",
              "title": "Pre-existing Items",
              "dataSources": [],
              "actionItems": [
                { "id": "abc", "title": "Existing item", "isResolved": false }
              ]
            }
            """;

        var result = AnalysisProjectMigrator.MigrateIfNeeded(json);

        var node = JsonNode.Parse(result.Json!)!.AsObject();
        var actionItems = node["actionItems"]!.AsArray();
        Assert.Single(actionItems);
        Assert.Equal("Existing item", actionItems[0]!["title"]?.GetValue<string>());
        _output.WriteLine("Pre-existing actionItems preserved through V2→V3 migration.");
    }

    [Fact]
    public void MigrateV2ToV3_UpdatesSchemaVersion()
    {
        var v2 = LoadFixture("project_v2.json");
        var result = AnalysisProjectMigrator.MigrateIfNeeded(v2);

        Assert.Equal(3, GetSchemaVersion(result.Json!));
        _output.WriteLine($"Schema version updated to {GetSchemaVersion(result.Json!)}.");
    }

    [Fact]
    public void MigrateV2ToV3_PreservesOtherData()
    {
        var v2 = LoadFixture("project_v2.json");
        var v2Node = JsonNode.Parse(v2)!.AsObject();
        var result = JsonNode.Parse(AnalysisProjectMigrator.MigrateIfNeeded(v2).Json!)!.AsObject();

        Assert.Equal(v2Node["title"]?.GetValue<string>(), result["title"]?.GetValue<string>());
        Assert.Equal(v2Node["id"]?.GetValue<string>(), result["id"]?.GetValue<string>());

        var v2Sources = v2Node["dataSources"]?.AsArray();
        var resultSources = result["dataSources"]?.AsArray();
        Assert.Equal(v2Sources?.Count, resultSources?.Count);
        _output.WriteLine("Non-actionItems data preserved through V2→V3 migration.");
    }

    [Fact]
    public void MigrateV2_ReportsOneMigrationApplied()
    {
        var v2 = LoadFixture("project_v2.json");
        var result = AnalysisProjectMigrator.MigrateIfNeeded(v2);

        Assert.Equal(1, result.MigrationsApplied);
        _output.WriteLine("Exactly 1 migration applied from V2.");
    }

    // -------------------------------------------------------------------------
    // Idempotency
    // -------------------------------------------------------------------------

    [Fact]
    public void MigrateIfNeeded_RunTwice_IsIdempotent()
    {
        var v1 = LoadFixture("project_v1.json");
        var once = AnalysisProjectMigrator.MigrateIfNeeded(v1);
        var twice = AnalysisProjectMigrator.MigrateIfNeeded(once.Json!);

        Assert.Equal(
            JsonNode.Parse(once.Json!)!.ToJsonString(),
            JsonNode.Parse(twice.Json!)!.ToJsonString()
        );
        _output.WriteLine("Double migration is idempotent.");
    }
}
