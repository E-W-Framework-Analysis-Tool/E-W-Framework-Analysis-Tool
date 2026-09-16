using System.Text.Json;
using System.Text.Json.Nodes;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;
using FluentAssertions;

namespace EwFrameworkAnalysis.Common.UnitTests.Services;

public class AnalysisProjectMigratorTests
{
    private readonly ITestOutputHelper _output;

    public AnalysisProjectMigratorTests(ITestOutputHelper output)
    {
        _output = output;
    }

    // -------------------------------------------------------------------------
    // Enum integer values — mirrors the DataSourceType declaration order.
    // If the enum order ever changes, update these constants and the tests
    // will catch any migrator breakage automatically.
    // -------------------------------------------------------------------------
    private static readonly int EdFiApiTypeValue = (int)DataSourceType.EdFiApi;
    private static readonly int CedsDWTypeValue = (int)DataSourceType.CedsDw;
    private static readonly int CustomTypeValue = (int)DataSourceType.Custom;
    private static readonly int EcsStateTypeValue = (int)DataSourceType.EcsState;

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

    private static string MakeMinimalProject(int schemaVersion, string? extra = null) =>
        $$"""
        {
          "schemaVersion": {{schemaVersion}},
          "id": "test-id",
          "title": "Test Project",
          "dataSources": []
          {{(extra is null ? "" : "," + extra)}}
        }
        """;

    private static int GetSchemaVersion(string json)
    {
        var node = JsonNode.Parse(json)?.AsObject();
        return node?["schemaVersion"]?.GetValue<int>() ?? 1;
    }

    private static JsonObject ParseResult(MigrationResult result)
    {
        result.Success.Should().BeTrue(because: result.ErrorMessage ?? "migration failed");
        return JsonNode.Parse(result.Json!)!.AsObject();
    }

    private static bool ContainsDiscriminator(string json, string discriminator)
    {
        return ContainsDiscriminatorRecursive(JsonNode.Parse(json), discriminator);
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
    // Version detection / general behaviour
    // -------------------------------------------------------------------------

    [Fact]
    public void MigrateIfNeeded_AlreadyCurrentVersion_ReturnsUnchanged()
    {
        var json = MakeMinimalProject(AnalysisProjectMigrator.CurrentSchemaVersion);
        var result = AnalysisProjectMigrator.MigrateIfNeeded(json);

        result.Success.Should().BeTrue();
        result.MigrationsApplied.Should().Be(0);
        JsonNode.Parse(result.Json!)!.ToJsonString()
            .Should().Be(JsonNode.Parse(json)!.ToJsonString());
    }

    [Fact]
    public void MigrateIfNeeded_MissingSchemaVersion_TreatedAsV1_ReachesCurrentVersion()
    {
        var json = """{ "title": "Old Project", "dataSources": [] }""";
        var result = AnalysisProjectMigrator.MigrateIfNeeded(json);

        result.Success.Should().BeTrue();
        GetSchemaVersion(result.Json!).Should().Be(AnalysisProjectMigrator.CurrentSchemaVersion);
    }

    [Fact]
    public void MigrateIfNeeded_InvalidJson_ReturnsFailedResult()
    {
        var result = AnalysisProjectMigrator.MigrateIfNeeded("this is not json");

        result.Success.Should().BeFalse();
        result.Json.Should().BeNull();
        result.ErrorMessage.Should().NotBeNull();
    }

    [Fact]
    public void MigrateIfNeeded_AnyVersion_AlwaysReachesCurrentSchemaVersion()
    {
        for (var v = 1; v < AnalysisProjectMigrator.CurrentSchemaVersion; v++)
        {
            var json = MakeMinimalProject(v);
            var result = AnalysisProjectMigrator.MigrateIfNeeded(json);

            result.Success.Should().BeTrue(because: $"V{v} migration should succeed");
            GetSchemaVersion(result.Json!).Should()
                .Be(AnalysisProjectMigrator.CurrentSchemaVersion,
                    because: $"V{v} should reach current version");

            _output.WriteLine($"V{v} → V{AnalysisProjectMigrator.CurrentSchemaVersion} ok.");
        }
    }

    [Fact]
    public void MigrateIfNeeded_MigrationsApplied_EqualsVersionDelta()
    {
        for (var v = 1; v < AnalysisProjectMigrator.CurrentSchemaVersion; v++)
        {
            var json = MakeMinimalProject(v);
            var result = AnalysisProjectMigrator.MigrateIfNeeded(json);
            var expected = AnalysisProjectMigrator.CurrentSchemaVersion - v;

            result.MigrationsApplied.Should().Be(expected,
                because: $"migrating from V{v} should apply {expected} step(s)");
        }
    }

    [Fact]
    public void MigrateIfNeeded_RunTwice_IsIdempotent()
    {
        var json = MakeMinimalProject(1);
        var once = AnalysisProjectMigrator.MigrateIfNeeded(json);
        var twice = AnalysisProjectMigrator.MigrateIfNeeded(once.Json!);

        JsonNode.Parse(twice.Json!)!.ToJsonString()
            .Should().Be(JsonNode.Parse(once.Json!)!.ToJsonString());
        twice.MigrationsApplied.Should().Be(0);
    }

    // -------------------------------------------------------------------------
    // V1 → V2: IntegerRange → NumericalRange
    // -------------------------------------------------------------------------

    [Fact]
    public void MigrateV1ToV2_RenamesIntegerRangeDiscriminator()
    {
        var v1 = LoadFixture("project_v1.json");
        v1.Should().Contain("IntegerRange", because: "fixture must contain the old discriminator");

        var result = AnalysisProjectMigrator.MigrateIfNeeded(v1);

        ContainsDiscriminator(result.Json!, "IntegerRange").Should().BeFalse();
        ContainsDiscriminator(result.Json!, "NumericalRange").Should().BeTrue();
    }

    [Fact]
    public void MigrateV1ToV2_WithNoIntegerRangePresent_DoesNotFail()
    {
        var json = MakeMinimalProject(1);
        var result = AnalysisProjectMigrator.MigrateIfNeeded(json);

        result.Success.Should().BeTrue();
        ContainsDiscriminator(result.Json!, "IntegerRange").Should().BeFalse();
    }

    [Fact]
    public void MigrateV1_PreservesNonDiscriminatorData()
    {
        var v1 = LoadFixture("project_v1.json");
        var v1Node = JsonNode.Parse(v1)!.AsObject();
        var resultNode = ParseResult(AnalysisProjectMigrator.MigrateIfNeeded(v1));

        resultNode["title"]?.GetValue<string>().Should().Be(v1Node["title"]?.GetValue<string>());
        resultNode["id"]?.GetValue<string>().Should().Be(v1Node["id"]?.GetValue<string>());
        resultNode["dataSources"]?.AsArray().Count.Should()
            .Be(v1Node["dataSources"]?.AsArray().Count);
    }

    // -------------------------------------------------------------------------
    // V2 → V3: actionItems array added
    // -------------------------------------------------------------------------

    [Fact]
    public void MigrateV2ToV3_AddsEmptyActionItemsArray()
    {
        var json = MakeMinimalProject(2);
        var resultNode = ParseResult(AnalysisProjectMigrator.MigrateIfNeeded(json));

        resultNode.ContainsKey("actionItems").Should().BeTrue();
        resultNode["actionItems"]!.GetValueKind().Should().Be(JsonValueKind.Array);
        resultNode["actionItems"]!.AsArray().Should().BeEmpty();
    }

    [Fact]
    public void MigrateV2ToV3_PreservesExistingActionItems_WhenAlreadyPresent()
    {
        var json = """
            {
              "schemaVersion": 2,
              "id": "test-id",
              "title": "Test Project",
              "dataSources": [],
              "actionItems": [
                { "id": "abc", "title": "Existing item", "isResolved": false }
              ]
            }
            """;

        var resultNode = ParseResult(AnalysisProjectMigrator.MigrateIfNeeded(json));
        var items = resultNode["actionItems"]!.AsArray();

        items.Should().ContainSingle();
        items[0]!["title"]?.GetValue<string>().Should().Be("Existing item");
    }

    [Fact]
    public void MigrateV2ToV3_PreservesOtherData()
    {
        var json = MakeMinimalProject(2);
        var original = JsonNode.Parse(json)!.AsObject();
        var resultNode = ParseResult(AnalysisProjectMigrator.MigrateIfNeeded(json));

        resultNode["title"]?.GetValue<string>().Should().Be(original["title"]?.GetValue<string>());
        resultNode["id"]?.GetValue<string>().Should().Be(original["id"]?.GetValue<string>());
    }

    // -------------------------------------------------------------------------
    // V3 → V4: Version backfill on data sources
    // Type is serialized as an integer matching the DataSourceType enum value.
    // -------------------------------------------------------------------------

    [Fact]
    public void MigrateV3ToV4_BackfillsCedsDwVersion()
    {
        var json = $$"""
            {
              "schemaVersion": 3,
              "id": "test-id",
              "title": "Test",
              "dataSources": [
                { "id": "ds-1", "type": {{CedsDWTypeValue}}, "name": "My CEDS DW" }
              ],
              "actionItems": []
            }
            """;

        var resultNode = ParseResult(AnalysisProjectMigrator.MigrateIfNeeded(json));
        var ds = resultNode["dataSources"]!.AsArray()[0]!.AsObject();

        ds["version"]?.GetValue<string>().Should().Be(CedsDWVersions.V13);
    }

    [Fact]
    public void MigrateV3ToV4_BackfillsEdFiVersion()
    {
        var json = $$"""
            {
              "schemaVersion": 3,
              "id": "test-id",
              "title": "Test",
              "dataSources": [
                { "id": "ds-1", "type": {{EdFiApiTypeValue}}, "name": "My Ed-Fi API" }
              ],
              "actionItems": []
            }
            """;

        var resultNode = ParseResult(AnalysisProjectMigrator.MigrateIfNeeded(json));
        var ds = resultNode["dataSources"]!.AsArray()[0]!.AsObject();

        ds["version"]?.GetValue<string>().Should().Be(EdFiVersions.V73);
    }

    [Fact]
    public void MigrateV3ToV4_LeavesVersionNull_ForUnversionedTypes()
    {
        var json = $$"""
            {
              "schemaVersion": 3,
              "id": "test-id",
              "title": "Test",
              "dataSources": [
                { "id": "ds-1", "type": {{CustomTypeValue}},   "name": "Manual" },
                { "id": "ds-2", "type": {{EcsStateTypeValue}}, "name": "ECS" }
              ],
              "actionItems": []
            }
            """;

        var resultNode = ParseResult(AnalysisProjectMigrator.MigrateIfNeeded(json));
        var sources = resultNode["dataSources"]!.AsArray();

        foreach (var ds in sources)
        {
            ds!.AsObject().TryGetPropertyValue("version", out var v);
            (v is null || v.GetValueKind() == JsonValueKind.Null).Should().BeTrue(
                because: $"unversioned type should not have a version");
        }
    }

    [Fact]
    public void MigrateV3ToV4_DoesNotOverwriteExistingVersion()
    {
        var json = $$"""
            {
              "schemaVersion": 3,
              "id": "test-id",
              "title": "Test",
              "dataSources": [
                { "id": "ds-1", "type": {{CedsDWTypeValue}}, "name": "My CEDS DW", "version": "v14" }
              ],
              "actionItems": []
            }
            """;

        var resultNode = ParseResult(AnalysisProjectMigrator.MigrateIfNeeded(json));
        var ds = resultNode["dataSources"]!.AsArray()[0]!.AsObject();

        ds["version"]?.GetValue<string>().Should().Be("v14",
            because: "a pre-existing version value should not be overwritten");
    }

    [Fact]
    public void MigrateV3ToV4_HandlesEmptyDataSources()
    {
        var json = """
            {
              "schemaVersion": 3,
              "id": "test-id",
              "title": "Test Project",
              "dataSources": [],
              "actionItems": []
            }
            """;

        var result = AnalysisProjectMigrator.MigrateIfNeeded(json);

        result.Success.Should().BeTrue();
        GetSchemaVersion(result.Json!).Should().Be(AnalysisProjectMigrator.CurrentSchemaVersion);
    }

    [Fact]
    public void MigrateV3ToV4_MixedDataSources_BackfillsCorrectly()
    {
        var json = $$"""
            {
              "schemaVersion": 3,
              "id": "test-id",
              "title": "Test",
              "dataSources": [
                { "id": "ds-1", "type": {{CedsDWTypeValue}},   "name": "CEDS" },
                { "id": "ds-2", "type": {{EdFiApiTypeValue}},  "name": "EdFi" },
                { "id": "ds-3", "type": {{CustomTypeValue}},   "name": "Manual" }
              ],
              "actionItems": []
            }
            """;

        var resultNode = ParseResult(AnalysisProjectMigrator.MigrateIfNeeded(json));
        var sources = resultNode["dataSources"]!.AsArray();

        sources[0]!["version"]?.GetValue<string>().Should().Be(CedsDWVersions.V13);
        sources[1]!["version"]?.GetValue<string>().Should().Be(EdFiVersions.V73);

        sources[2]!.AsObject().TryGetPropertyValue("version", out var customVersion);
        (customVersion is null || customVersion.GetValueKind() == JsonValueKind.Null)
            .Should().BeTrue();
    }

    // -------------------------------------------------------------------------
    // V4 → V5: Data element renames/splits
    // -------------------------------------------------------------------------

    private static string MakeV4ProjectWithDataElements(params (string id, string dataElementName)[] elements)
    {
        var elementsJson = string.Join(",", elements.Select(e => $$"""
            { "id": "{{e.id}}", "dataElementName": "{{e.dataElementName}}", "remarks": "kept-{{e.id}}" }
            """));

        return $$"""
            {
              "schemaVersion": 4,
              "id": "test-id",
              "title": "Test",
              "dataSources": [
                {
                  "id": "ds-1",
                  "name": "Test Source",
                  "type": {{CustomTypeValue}},
                  "assessments": [
                    { "id": "as-1", "dataElementAssessments": [ {{elementsJson}} ] }
                  ]
                }
              ],
              "actionItems": []
            }
            """;
    }

    private static JsonArray GetDataElementAssessments(JsonObject project) =>
        project["dataSources"]![0]!["assessments"]![0]!["dataElementAssessments"]!.AsArray();

    [Fact]
    public void MigrateV4ToV5_RenamesEapDataElement()
    {
        var json = MakeV4ProjectWithDataElements(("de-1", "EAP or mental health services provided"));
        var resultNode = ParseResult(AnalysisProjectMigrator.MigrateIfNeeded(json));
        var elements = GetDataElementAssessments(resultNode);

        elements.Should().ContainSingle();
        elements[0]!["dataElementName"]!.GetValue<string>()
            .Should().Be("EAP or mental health services provided (Workforce)");
        elements[0]!["id"]!.GetValue<string>().Should().Be("de-1", because: "a plain rename should keep the original id");
    }

    [Fact]
    public void MigrateV4ToV5_RenamesMilitaryEnlistmentDataElement()
    {
        var json = MakeV4ProjectWithDataElements(("de-1", "Enlistment in the military"));
        var resultNode = ParseResult(AnalysisProjectMigrator.MigrateIfNeeded(json));
        var elements = GetDataElementAssessments(resultNode);

        elements.Should().ContainSingle();
        elements[0]!["dataElementName"]!.GetValue<string>().Should().Be("Military enlistment date");
        elements[0]!["id"]!.GetValue<string>().Should().Be("de-1", because: "a plain rename should keep the original id");
    }

    [Fact]
    public void MigrateV4ToV5_SplitsCourseElement_CopyingDataToBothNewElements()
    {
        var json = MakeV4ProjectWithDataElements(("de-1", "Course performance (English and Math)"));
        var resultNode = ParseResult(AnalysisProjectMigrator.MigrateIfNeeded(json));
        var elements = GetDataElementAssessments(resultNode);

        elements.Should().HaveCount(2);
        var names = elements.Select(e => e!["dataElementName"]!.GetValue<string>()).ToList();
        names.Should().BeEquivalentTo(["Course outcome", "Course subject area"]);

        foreach (var element in elements)
        {
            element!["remarks"]!.GetValue<string>().Should().Be("kept-de-1",
                because: "existing assessment data should carry over to both new elements");
        }

        var ids = elements.Select(e => e!["id"]!.GetValue<string>()).ToHashSet();
        ids.Should().HaveCount(2, because: "each split element needs its own unique id");
        ids.Should().NotContain("de-1", because: "the original id can no longer correctly identify either new element");
    }

    [Fact]
    public void MigrateV4ToV5_LeavesUnrelatedDataElementsUntouched()
    {
        var json = MakeV4ProjectWithDataElements(("de-1", "ACT completion"));
        var resultNode = ParseResult(AnalysisProjectMigrator.MigrateIfNeeded(json));
        var elements = GetDataElementAssessments(resultNode);

        elements.Should().ContainSingle();
        elements[0]!["dataElementName"]!.GetValue<string>().Should().Be("ACT completion");
        elements[0]!["id"]!.GetValue<string>().Should().Be("de-1");
    }

    [Fact]
    public void MigrateIfNeeded_V4ToV5_ReportsNotesForRenamedAndSplitElements()
    {
        var json = MakeV4ProjectWithDataElements(
            ("de-1", "EAP or mental health services provided"),
            ("de-2", "Course performance (English and Math)"));

        var result = AnalysisProjectMigrator.MigrateIfNeeded(json);

        result.Notes.Should().Contain(n => n.Contains("EAP or mental health services provided") && n.Contains("Workforce"));
        result.Notes.Should().Contain(n => n.Contains("Course outcome") && n.Contains("Course subject area"));
    }

    [Fact]
    public void MigrateIfNeeded_AlreadyCurrentVersion_HasNoNotes()
    {
        var json = MakeMinimalProject(AnalysisProjectMigrator.CurrentSchemaVersion);
        var result = AnalysisProjectMigrator.MigrateIfNeeded(json);

        result.Notes.Should().BeEmpty();
    }

    // -------------------------------------------------------------------------
    // MigrateIfNeededAsync: step progress reporting
    // -------------------------------------------------------------------------

    [Fact]
    public async Task MigrateIfNeededAsync_ReportsEachStepInOrder()
    {
        var json = MakeMinimalProject(1);
        var reported = new List<(int Step, int Total, string Description)>();

        var result = await AnalysisProjectMigrator.MigrateIfNeededAsync(json, (step, total, description) =>
        {
            reported.Add((step, total, description));
            return Task.CompletedTask;
        });

        result.Success.Should().BeTrue();
        reported.Should().HaveCount(AnalysisProjectMigrator.CurrentSchemaVersion - 1);
        reported.Select(r => r.Step).Should().BeInAscendingOrder();
        reported.Should().OnlyContain(r => r.Total == AnalysisProjectMigrator.CurrentSchemaVersion - 1);
        reported.Should().OnlyContain(r => !string.IsNullOrWhiteSpace(r.Description));
    }

    [Fact]
    public async Task MigrateIfNeededAsync_AlreadyCurrentVersion_ReportsNoSteps()
    {
        var json = MakeMinimalProject(AnalysisProjectMigrator.CurrentSchemaVersion);
        var reportCount = 0;

        await AnalysisProjectMigrator.MigrateIfNeededAsync(json, (_, _, _) =>
        {
            reportCount++;
            return Task.CompletedTask;
        });

        reportCount.Should().Be(0);
    }

    [Fact]
    public async Task MigrateIfNeededAsync_MatchesSyncResult()
    {
        var json = MakeMinimalProject(1);

        var asyncResult = await AnalysisProjectMigrator.MigrateIfNeededAsync(json);
        var syncResult = AnalysisProjectMigrator.MigrateIfNeeded(json);

        JsonNode.Parse(asyncResult.Json!)!.ToJsonString().Should().Be(JsonNode.Parse(syncResult.Json!)!.ToJsonString());
    }
}
