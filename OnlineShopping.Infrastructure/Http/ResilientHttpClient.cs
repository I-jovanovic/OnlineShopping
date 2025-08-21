using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using OnlineShopping.Infrastructure.Resilience;
using Polly;

namespace OnlineShopping.Infrastructure.Http;

public interface IResilientHttpClient
{
    Task<T?> GetAsync<T>(string url);
    Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data);
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
}

public class ResilientHttpClient : IResilientHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ResilientHttpClient> _logger;
    private readonly IAsyncPolicy _resiliencePolicy;
    private readonly JsonSerializerOptions _jsonOptions;

    public ResilientHttpClient(
        HttpClient httpClient,
        ILogger<ResilientHttpClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _resiliencePolicy = ResiliencePolicies.GetCombinedPolicy(logger, nameof(ResilientHttpClient));
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<T?> GetAsync<T>(string url)
    {
        return await _resiliencePolicy.ExecuteAsync(async () =>
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, _jsonOptions);
        });
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data)
    {
        return await _resiliencePolicy.ExecuteAsync(async () =>
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(responseContent, _jsonOptions);
        });
    }

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    {
        return await _resiliencePolicy.ExecuteAsync(async () =>
        {
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return response;
        });
    }
}