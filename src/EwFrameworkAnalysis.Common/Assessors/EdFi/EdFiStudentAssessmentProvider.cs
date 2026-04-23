using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class EdFiStudentAssessmentProvider
{
    private List<EdFiStudentAssessment>? _cachedData;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public void ClearCache()
    {
        _cachedData = null;
    }

    public async Task<List<EdFiStudentAssessment>> GetDataAsync(
        HttpClient httpClient, AssessorContext? context = null, CancellationToken ct = default)
    {
        if (_cachedData != null)
            return _cachedData;

        await _lock.WaitAsync(ct);
        try
        {
            if (_cachedData != null)
                return _cachedData;

            var data = new List<EdFiStudentAssessment>();

            await EdFiApiPatterns.PageAndProcessAsync<EdFiStudentAssessment>(
                httpClient,
                "ed-fi/studentAssessments",
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
