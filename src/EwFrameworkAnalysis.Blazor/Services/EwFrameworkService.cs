using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using EwFrameworkAnalysis.Common.Models;
using Microsoft.Extensions.Caching.Memory;

namespace EwFrameworkAnalysis.Blazor.Services;

public class EwFrameworkService
{
    private readonly IMemoryCache _cache;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public EwFrameworkService(HttpClient httpClient, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _cache = cache;
        _jsonSerializerOptions = new JsonSerializerOptions();
        _jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        _jsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
    }

    public async Task<List<Indicator>> GetIndicatorsAsync()
    {
        return await _cache.GetOrCreateAsync("indicators", async entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromHours(1);
            var response = await _httpClient.GetFromJsonAsync<List<Indicator>>("data/indicators.jsonc?v=1", _jsonSerializerOptions) ?? throw new ApplicationException("Unable to load indicators");
            return response;
        }) ?? throw new ApplicationException("Unable to load indicators");
    }

    public async Task<List<EssentialQuestion>> GetEssentialQuestionsAsync()
    {
        return await _cache.GetOrCreateAsync("essential_questions", async entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromHours(1);
            var response = await _httpClient.GetFromJsonAsync<List<EssentialQuestion>>("data/essential_questions.jsonc?v=1", _jsonSerializerOptions) ?? throw new ApplicationException("Unable to load essential questions");
            return response;
        }) ?? throw new ApplicationException("Unable to load essential questions");
    }

    public async Task<List<EssentialQuestionWithIndicators>> GetEssentialQuestionWithIndicatorsAsync()
    {
        var questions = await GetEssentialQuestionsAsync();
        var indicators = await GetIndicatorsAsync();
        var joinedQuestions = questions.Select(q => new EssentialQuestionWithIndicators
        {
            Question = q.Question,
            Sectors = q.Sectors,
            RelatedIndicators = [.. q.RelatedIndicators.Select(ri => indicators.Single(i => ri == i.Name))]
        }).ToList();

        return joinedQuestions;
    }

    public async Task<List<DataElement>> GetDataElementsAsync()
    {
        return await _cache.GetOrCreateAsync("data_elements", async entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromHours(1);
            var response = await _httpClient.GetFromJsonAsync<List<DataElement>>("data/data_elements.jsonc?v=1", _jsonSerializerOptions) ?? throw new ApplicationException("Unable to load data elements");
            return response;
        }) ?? throw new ApplicationException("Unable to load data elements");
    }
}
