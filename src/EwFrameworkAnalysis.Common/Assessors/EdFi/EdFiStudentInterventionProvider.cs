using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class EdFiStudentInterventionProvider
{
    private List<EdFiStudentInterventionAssociation>? _cachedAssociations;
    private List<EdFiIntervention>? _cachedInterventions;
    private Dictionary<string, EdFiIntervention>? _cachedLookup;
    private readonly SemaphoreSlim _associationsLock = new(1, 1);
    private readonly SemaphoreSlim _interventionsLock = new(1, 1);
    private readonly SemaphoreSlim _lookupLock = new(1, 1);

    public void ClearCache()
    {
        _cachedAssociations = null;
        _cachedInterventions = null;
        _cachedLookup = null;
    }

    public async Task<List<EdFiStudentInterventionAssociation>> GetAssociationsAsync(
        HttpClient httpClient, AssessorContext? context = null, CancellationToken ct = default)
    {
        if (_cachedAssociations != null)
            return _cachedAssociations;

        await _associationsLock.WaitAsync(ct);
        try
        {
            if (_cachedAssociations != null)
                return _cachedAssociations;

            var data = new List<EdFiStudentInterventionAssociation>();

            await EdFiApiPatterns.PageAndProcessAsync<EdFiStudentInterventionAssociation>(
                httpClient,
                "ed-fi/studentInterventionAssociations",
                item => data.Add(item),
                context,
                ct: ct
            );

            _cachedAssociations = data;
            return _cachedAssociations;
        }
        finally
        {
            _associationsLock.Release();
        }
    }

    public async Task<List<EdFiIntervention>> GetInterventionsAsync(
        HttpClient httpClient, AssessorContext? context = null, CancellationToken ct = default)
    {
        if (_cachedInterventions != null)
            return _cachedInterventions;

        await _interventionsLock.WaitAsync(ct);
        try
        {
            if (_cachedInterventions != null)
                return _cachedInterventions;

            var data = new List<EdFiIntervention>();

            await EdFiApiPatterns.PageAndProcessAsync<EdFiIntervention>(
                httpClient,
                "ed-fi/interventions",
                item => data.Add(item),
                context,
                ct: ct
            );

            _cachedInterventions = data;
            return _cachedInterventions;
        }
        finally
        {
            _interventionsLock.Release();
        }
    }

    public async Task<Dictionary<string, EdFiIntervention>> GetInterventionLookupAsync(
        HttpClient httpClient, AssessorContext? context = null, CancellationToken ct = default)
    {
        if (_cachedLookup != null)
            return _cachedLookup;

        await _lookupLock.WaitAsync(ct);
        try
        {
            if (_cachedLookup != null)
                return _cachedLookup;

            var interventions = await GetInterventionsAsync(httpClient, context, ct);
            var lookup = new Dictionary<string, EdFiIntervention>();
            foreach (var intervention in interventions)
            {
                var key = BuildKey(
                    intervention.EducationOrganizationReference?.EducationOrganizationId,
                    intervention.InterventionIdentificationCode);
                lookup.TryAdd(key, intervention);
            }

            _cachedLookup = lookup;
            return _cachedLookup;
        }
        finally
        {
            _lookupLock.Release();
        }
    }

    public static string BuildKey(long? educationOrganizationId, string? interventionIdentificationCode)
    {
        return $"{educationOrganizationId}|{interventionIdentificationCode}";
    }
}
