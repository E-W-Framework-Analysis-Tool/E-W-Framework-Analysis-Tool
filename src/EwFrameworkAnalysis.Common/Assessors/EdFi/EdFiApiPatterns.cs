using EwFrameworkAnalysis.Common.Services;
using Flurl;
using Newtonsoft.Json;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

/// <summary>
/// Provides common patterns for interacting with Ed-Fi API endpoints.
/// </summary>
public static class EdFiApiPatterns
{
    /// <summary>
    /// Pages through an Ed-Fi API endpoint and counts resources matching a predicate.
    /// </summary>
    /// <typeparam name="T">The type of resource returned by the API endpoint.</typeparam>
    /// <param name="client">The HTTP client configured with base address and authentication.</param>
    /// <param name="endpoint">The API endpoint path (e.g., "ed-fi/students").</param>
    /// <param name="predicate">A function to test each resource for a match condition.</param>
    /// <param name="context">Optional assessor context for progress reporting.</param>
    /// <param name="queryParams">Optional query parameters to include in each request.</param>
    /// <param name="pageSize">The number of records to retrieve per page. Default is 200.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <returns>The total count of resources matching the predicate across all pages.</returns>
    /// <exception cref="HttpRequestException">Thrown when an API request returns a non-success status code.</exception>
    /// <remarks>
    /// This method retrieves and deserializes all pages of results to apply the predicate.
    /// When possible to use query parameters to filter the results, consider using <see cref="CountFromHeaderAsync"/> as it is more efficient.
    /// </remarks>
    public static async Task<int> PageAndCountMatchesAsync<T>(
        HttpClient client,
        string endpoint,
        Func<T, bool> predicate,
        AssessorContext? context = null,
        Dictionary<string, string>? queryParams = null,
        int pageSize = 200,
        CancellationToken ct = default)
    {
        // First, try to get total count from header to provide better progress
        int? estimatedTotal = null;
        try
        {
            estimatedTotal = await CountFromHeaderAsync(client, endpoint, queryParams, ct);
        }
        catch
        {
            // Silently continue - we'll just page without progress percentage
        }

        var offset = 0;
        var totalMatches = 0;
        var recordsProcessed = 0;
        var hasMorePages = true;
        var pageNumber = 1;

        while (hasMorePages)
        {
            var url = endpoint
                .SetQueryParams(queryParams ?? [])
                .SetQueryParam("offset", offset)
                .SetQueryParam("limit", pageSize);

            var response = await client.GetAsync(url, ct);

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"API request failed: {response.StatusCode}");

            var json = await response.Content.ReadAsStringAsync(ct);
            var items = JsonConvert.DeserializeObject<List<T>>(json);

            if (items == null || items.Count == 0)
            {
                hasMorePages = false;
                break;
            }

            var pageMatches = items.Count(predicate);
            totalMatches += pageMatches;
            recordsProcessed += items.Count;

            // Only report progress if we have multiple pages
            if (estimatedTotal.HasValue && estimatedTotal.Value > pageSize)
            {
                var progressPercent = Math.Min(100, (int)((recordsProcessed * 100.0) / estimatedTotal.Value));
                context?.ReportProgress(
                    progressPercent,
                    $"Page {pageNumber} - {recordsProcessed:N0} / {estimatedTotal:N0} records - {totalMatches:N0} matches"
                );
            }

            if (items.Count < pageSize)
                hasMorePages = false;
            else
                offset += pageSize;

            pageNumber++;
        }

        return totalMatches;
    }

    /// <summary>
    /// Retrieves the total count of resources from the Ed-Fi API response header.
    /// </summary>
    /// <param name="client">The HTTP client configured with base address and authentication.</param>
    /// <param name="endpoint">The API endpoint path (e.g., "/ed-fi/students").</param>
    /// <param name="queryParams">Optional query parameters to filter the count.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <returns>The total count of resources as reported by the API's total-count header.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request returns a non-success status code.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the total-count header is missing or cannot be parsed.</exception>
    /// <remarks>
    /// This method automatically adds "totalCount=true" to the query parameters to request the count header.
    /// Ed-Fi APIs return the total count in the "total-count" response header when this parameter is present.
    /// This is more efficient than paging through results when you only need the total count.
    /// </remarks>
    public static async Task<int> CountFromHeaderAsync(
        HttpClient client,
        string endpoint,
        Dictionary<string, string>? queryParams = null,
        CancellationToken ct = default)
    {
        queryParams ??= [];
        queryParams["totalCount"] = "true";
        queryParams["limit"] = "0";

        var url = endpoint.SetQueryParams(queryParams);

        var response = await client.GetAsync(url, ct);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"API request failed: {response.StatusCode}");

        // Ed-Fi returns total count in X-Total-Count header when totalCount=true
        if (response.Headers.TryGetValues("total-count", out var values))
        {
            var countStr = values.FirstOrDefault();
            if (int.TryParse(countStr, out var count))
                return count;
        }

        throw new InvalidOperationException("total header not found in response. Ensure totalCount=true is in query params.");
    }

    /// <summary>
    /// Pages through an Ed-Fi API endpoint and processes each item with a custom action.
    /// </summary>
    /// <typeparam name="T">The type of resource returned by the API endpoint.</typeparam>
    /// <param name="client">The HTTP client configured with base address and authentication.</param>
    /// <param name="endpoint">The API endpoint path (e.g., "ed-fi/students").</param>
    /// <param name="processItem">An action to process each item. Use this to accumulate state, filter, etc.</param>
    /// <param name="context">Optional assessor context for progress reporting.</param>
    /// <param name="queryParams">Optional query parameters to include in each request.</param>
    /// <param name="pageSize">The number of records to retrieve per page. Default is 200.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <returns>The total number of records processed.</returns>
    /// <exception cref="HttpRequestException">Thrown when an API request returns a non-success status code.</exception>
    /// <remarks>
    /// This method retrieves and deserializes all pages of results, calling processItem for each record.
    /// The processItem action can modify external state (e.g., min/max tracking, custom filtering).
    /// </remarks>
    public static async Task<int> PageAndProcessAsync<T>(
        HttpClient client,
        string endpoint,
        Action<T> processItem,
        AssessorContext? context = null,
        Dictionary<string, string>? queryParams = null,
        int pageSize = 200,
        CancellationToken ct = default)
    {
        // Try to get total count for progress reporting
        int? estimatedTotal = null;
        try
        {
            estimatedTotal = await CountFromHeaderAsync(client, endpoint, queryParams, ct);
        }
        catch
        {
            // Silently continue - we'll just page without progress percentage
        }

        var offset = 0;
        var recordsProcessed = 0;
        var hasMorePages = true;
        var pageNumber = 1;

        while (hasMorePages)
        {
            var url = endpoint
                .SetQueryParams(queryParams ?? [])
                .SetQueryParam("offset", offset)
                .SetQueryParam("limit", pageSize);

            var response = await client.GetAsync(url, ct);

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"API request failed: {response.StatusCode}");

            var json = await response.Content.ReadAsStringAsync(ct);
            var items = JsonConvert.DeserializeObject<List<T>>(json);

            if (items == null || items.Count == 0)
            {
                hasMorePages = false;
                break;
            }

            // Process each item
            foreach (var item in items)
            {
                processItem(item);
            }

            recordsProcessed += items.Count;

            // Report progress if we have multiple pages
            if (estimatedTotal.HasValue && estimatedTotal.Value > pageSize)
            {
                var progressPercent = Math.Min(100, (int)((recordsProcessed * 100.0) / estimatedTotal.Value));
                context?.ReportProgress(
                    progressPercent,
                    $"Page {pageNumber} - {recordsProcessed:N0} / {estimatedTotal:N0} records"
                );
            }

            if (items.Count < pageSize)
                hasMorePages = false;
            else
                offset += pageSize;

            pageNumber++;
        }

        return recordsProcessed;
    }
}
