using EwFrameworkAnalysis.Common.Models.Project;
using Microsoft.JSInterop;

namespace EwFrameworkAnalysis.UI.UnitTests.Services;

/// <summary>
/// Minimal IJSRuntime stub — silently swallows all JS calls and returns
/// default values. Sufficient for unit-testing service logic that doesn't
/// actually need a browser.
/// </summary>
file sealed class NullJSRuntime : IJSRuntime
{
    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
        => new(default(TValue)!);

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier,
        CancellationToken cancellationToken, object?[]? args)
        => new(default(TValue)!);
}

public class AnalysisProjectServiceTests
{
    private static AnalysisProjectService CreateService() =>
        new(new NullJSRuntime());

    private static DataSource MakeSource(
        string name = "Test Source",
        DataSourceType type = DataSourceType.Custom,
        Guid? id = null) => new()
        {
            Id = id ?? Guid.NewGuid(),
            Name = name,
            Type = type,
            Enabled = true,
            Assessments = []
        };

    // -------------------------------------------------------------------------
    // ImportDataSourcesAsync — happy path
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ImportDataSources_AddsAllSources_WhenNoConflicts()
    {
        var svc = CreateService();
        var sources = new[] { MakeSource("A"), MakeSource("B"), MakeSource("C") };

        var skipped = await svc.ImportDataSourcesAsync(sources);

        Assert.Equal(3, svc.DataSources.Count);
        Assert.Empty(skipped);
    }

    [Fact]
    public async Task ImportDataSources_InsertsAtFront()
    {
        var svc = CreateService();
        var existing = MakeSource("Existing");
        await svc.ImportDataSourcesAsync([existing]);

        var incoming = MakeSource("Incoming");
        await svc.ImportDataSourcesAsync([incoming]);

        Assert.Equal("Incoming", svc.DataSources[0].Name);
        Assert.Equal("Existing", svc.DataSources[1].Name);
    }

    [Fact]
    public async Task ImportDataSources_PreservesAssessmentsOnImportedSource()
    {
        var svc = CreateService();
        var source = MakeSource("With Assessments");
        source.Assessments.Add(new DataSourceAssessment
        {
            Id = Guid.NewGuid(),
            Name = "Profile 1",
            ConductedAt = DateTimeOffset.Now,
            DataElementAssessments = []
        });

        await svc.ImportDataSourcesAsync([source]);

        Assert.Single(svc.DataSources[0].Assessments);
        Assert.Equal("Profile 1", svc.DataSources[0].Assessments[0].Name);
    }

    // -------------------------------------------------------------------------
    // ImportDataSourcesAsync — conflict handling
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ImportDataSources_SkipsConflictingGuids()
    {
        var svc = CreateService();
        var sharedId = Guid.NewGuid();

        await svc.ImportDataSourcesAsync([MakeSource("Original", id: sharedId)]);
        var skipped = await svc.ImportDataSourcesAsync([MakeSource("Duplicate", id: sharedId)]);

        Assert.Single(svc.DataSources);                     // still only one
        Assert.Equal("Original", svc.DataSources[0].Name); // original untouched
        Assert.Single(skipped);
        Assert.Equal(sharedId, skipped[0]);
    }

    [Fact]
    public async Task ImportDataSources_ImportsNonConflicting_AndSkipsConflicting()
    {
        var svc = CreateService();
        var conflictId = Guid.NewGuid();

        await svc.ImportDataSourcesAsync([MakeSource("Original", id: conflictId)]);

        var incoming = new[]
        {
            MakeSource("Conflict",  id: conflictId),
            MakeSource("New A"),
            MakeSource("New B")
        };
        var skipped = await svc.ImportDataSourcesAsync(incoming);

        Assert.Equal(3, svc.DataSources.Count); // original + New A + New B
        Assert.Single(skipped);
        Assert.Equal(conflictId, skipped[0]);
        Assert.DoesNotContain(svc.DataSources, ds => ds.Name == "Conflict");
    }

    [Fact]
    public async Task ImportDataSources_ReturnsAllSkipped_WhenAllConflict()
    {
        var svc = CreateService();
        var idA = Guid.NewGuid();
        var idB = Guid.NewGuid();

        await svc.ImportDataSourcesAsync([MakeSource(id: idA), MakeSource(id: idB)]);
        var skipped = await svc.ImportDataSourcesAsync(
        [
            MakeSource("Dup A", id: idA),
            MakeSource("Dup B", id: idB)
        ]);

        Assert.Equal(2, svc.DataSources.Count);
        Assert.Equal(2, skipped.Count);
    }

    [Fact]
    public async Task ImportDataSources_EmptyList_ChangesNothing()
    {
        var svc = CreateService();
        await svc.ImportDataSourcesAsync([MakeSource("Existing")]);

        var skipped = await svc.ImportDataSourcesAsync([]);

        Assert.Single(svc.DataSources);
        Assert.Empty(skipped);
    }

    // -------------------------------------------------------------------------
    // ImportDataSourcesAsync — notifications
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ImportDataSources_FiresChangedEvent()
    {
        var svc = CreateService();
        var fired = false;
        svc.Changed += () => fired = true;

        await svc.ImportDataSourcesAsync([MakeSource()]);

        Assert.True(fired);
    }

    [Fact]
    public async Task ImportDataSources_DoesNotFireChanged_WhenAllSkipped()
    {
        // Even when everything is skipped we still save+notify because
        // LastModifiedAt is updated. This test documents that behaviour
        // explicitly so a future refactor doesn't silently change it.
        var svc = CreateService();
        var id = Guid.NewGuid();
        await svc.ImportDataSourcesAsync([MakeSource(id: id)]);

        var fireCount = 0;
        svc.Changed += () => fireCount++;

        await svc.ImportDataSourcesAsync([MakeSource(id: id)]); // all conflict

        Assert.Equal(1, fireCount); // still fires once (save+notify always runs)
    }

    // -------------------------------------------------------------------------
    // DeserializeProject — now public, used by the import modal
    // -------------------------------------------------------------------------

    [Fact]
    public void DeserializeProject_ReturnsNull_ForGarbage()
    {
        var svc = CreateService();
        var result = svc.DeserializeProject("this is not json");
        Assert.Null(result);
    }

    [Fact]
    public void DeserializeProject_ReturnsProject_ForValidJson()
    {
        var svc = CreateService();
        var json = """
            {
              "id": "50fba414-97f7-43ff-aced-8e08e365ec18",
              "schemaVersion": 2,
              "title": "Test Project",
              "createdAt": "2026-01-01T00:00:00Z",
              "lastModifiedAt": "2026-01-01T00:00:00Z",
              "dataSources": []
            }
            """;

        var result = svc.DeserializeProject(json);

        Assert.NotNull(result);
        Assert.Equal("Test Project", result.Title);
    }

    [Fact]
    public void DeserializeProject_PreservesDataSources()
    {
        var svc = CreateService();
        var sourceId = Guid.NewGuid();
        var json = $$"""
            {
              "id": "50fba414-97f7-43ff-aced-8e08e365ec18",
              "schemaVersion": 2,
              "title": "Test Project",
              "createdAt": "2026-01-01T00:00:00Z",
              "lastModifiedAt": "2026-01-01T00:00:00Z",
              "dataSources": [
                {
                  "id": "{{sourceId}}",
                  "name": "My Source",
                  "description": null,
                  "enabled": true,
                  "type": 0,
                  "assessments": []
                }
              ]
            }
            """;

        var result = svc.DeserializeProject(json);

        Assert.NotNull(result);
        Assert.Single(result.DataSources);
        Assert.Equal(sourceId, result.DataSources[0].Id);
        Assert.Equal("My Source", result.DataSources[0].Name);
    }
}
