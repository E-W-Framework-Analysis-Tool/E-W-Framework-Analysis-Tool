using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Services;

public record EcsStateDataRecord(string Sector, string Indicator, string ElementName, AvailabilityJudgment? Reported);

public class EcsStateDataProvider
{
    private const string RESOURCE_PREFIX = "EwFrameworkAnalysis.Common.FrameworkReferenceData.EcsStateData.";
    private static readonly Assembly _assembly = typeof(EcsStateDataProvider).Assembly;

    public List<string> GetAvailableStates()
    {
        return [.. _assembly
            .GetManifestResourceNames()
            .Where(name => name.StartsWith(RESOURCE_PREFIX, StringComparison.Ordinal) && name.EndsWith(".json", StringComparison.Ordinal))
            .Select(name => name[RESOURCE_PREFIX.Length..^".json".Length])
            .OrderBy(name => name)];
    }

    public List<EcsStateDataRecord> GetStateData(string stateName)
    {
        var resourceName = $"{RESOURCE_PREFIX}{stateName}.json";
        using var stream = _assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"No embedded data found for state '{stateName}'");

        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();

        var records = JsonSerializer.Deserialize<List<EcsStateDataRecord>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        });

        return records ?? [];
    }
}
