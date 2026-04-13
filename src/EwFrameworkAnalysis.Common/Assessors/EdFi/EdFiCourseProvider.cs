using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class EdFiCourseProvider
{
    private List<EdFiCourse>? _cachedData;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public void ClearCache()
    {
        _cachedData = null;
    }

    public async Task<List<EdFiCourse>> GetDataAsync(
        HttpClient httpClient, AssessorContext? context = null, CancellationToken ct = default)
    {
        if (_cachedData != null)
            return _cachedData;

        await _lock.WaitAsync(ct);
        try
        {
            if (_cachedData != null)
                return _cachedData;

            var data = new List<EdFiCourse>();

            await EdFiApiPatterns.PageAndProcessAsync<EdFiCourse>(
                httpClient,
                "ed-fi/courses",
                item => data.Add(item),
                context,
                ct: ct
            );

            _cachedData = data;
            return _cachedData;
        }
        finally
        {
            _lock.Release();
        }
    }
}
