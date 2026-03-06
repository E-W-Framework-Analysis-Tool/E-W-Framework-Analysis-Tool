using EwFrameworkAnalysis.Common.Services;
using Newtonsoft.Json;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public static class EdFiDescriptorHelper
{
    public static string ParseDescriptorValue(string? descriptor)
    {
        if (string.IsNullOrWhiteSpace(descriptor))
            return "Unknown";

        return descriptor.Split('#').LastOrDefault() ?? descriptor;
    }

    /// <summary>
    /// Fetches descriptors from an Ed-Fi API endpoint and builds a lookup from
    /// descriptor URI (namespace#codeValue) to the human-readable description.
    /// </summary>
    public static async Task<Dictionary<string, string>> BuildDescriptorLookupAsync(
        HttpClient httpClient, string endpoint, AssessorContext? context = null)
    {
        var lookup = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        try
        {
            await EdFiApiPatterns.PageAndProcessAsync<DescriptorRecord>(
                httpClient,
                endpoint,
                item =>
                {
                    var uri = $"{item.Namespace}#{item.CodeValue}";
                    if (!string.IsNullOrWhiteSpace(item.Description))
                        lookup[uri] = item.Description;
                },
                context
            );
        }
        catch (HttpRequestException)
        {
            // Fall back to empty lookup — ResolveDescriptorLabel will use the code value
        }

        return lookup;
    }

    /// <summary>
    /// Resolves a descriptor URI to its human-readable description using a lookup,
    /// falling back to the parsed code value if not found.
    /// </summary>
    public static string ResolveDescriptorLabel(
        string? descriptorUri, Dictionary<string, string> lookup)
    {
        if (string.IsNullOrWhiteSpace(descriptorUri))
            return "Unknown";

        if (lookup.TryGetValue(descriptorUri, out var description))
            return description;

        return ParseDescriptorValue(descriptorUri);
    }

    private sealed class DescriptorRecord
    {
        [JsonProperty("namespace")]
        public string Namespace { get; set; } = string.Empty;

        [JsonProperty("codeValue")]
        public string CodeValue { get; set; } = string.Empty;

        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;
    }
}
