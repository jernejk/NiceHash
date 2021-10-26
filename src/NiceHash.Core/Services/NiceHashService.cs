﻿using Microsoft.Extensions.Caching.Memory;
using NiceHash.Core.Config;
using NiceHash.Core.Exceptions;
using NiceHash.Core.Models;
using NiceHash.Core.Utils;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace NiceHash.Core;

public interface INiceHashService
{
    Task<TResponse?> Delete<TResponse>(string url, Guid? requestId = null, CancellationToken ct = default);
    Task<TResponse?> Get<TResponse>(string url, CancellationToken ct = default);
    Task<TResponse?> GetAnnonymous<TResponse>(string url, CancellationToken ct);
    Task<TResponse?> Post<TRequest, TResponse>(string url, TRequest payload, Guid? requestId = null, CancellationToken ct = default);
}

internal class NiceHashService : INiceHashService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApiConfig _apiConfig;
    private readonly IMemoryCache _memoryCache;

    public NiceHashService(IHttpClientFactory httpClientFactory, ApiConfig apiConfig, IMemoryCache memoryCache)
    {
        _httpClientFactory = httpClientFactory;
        _apiConfig = apiConfig;
        _memoryCache = memoryCache;
    }

    public async Task<TResponse?> GetAnnonymous<TResponse>(string url, CancellationToken ct)
    {
        using HttpClient client = _httpClientFactory.CreateClient();
        client.BaseAddress = _apiConfig.BaseUri;

        var a = await client.GetStringAsync(url, ct);

        return await client.GetFromJsonAsync<TResponse>(url, ct);
    }

    public async Task<TResponse?> Get<TResponse>(string url, CancellationToken ct = default)
    {
        HttpClient client = await CreateClientWithAuth(url, "GET", null, null, ct);
        return await client.GetFromJsonAsync<TResponse>(url, ct);
    }

    public async Task<TResponse?> Post<TRequest, TResponse>(string url, TRequest payload, Guid? requestId = null, CancellationToken ct = default)
    {
        string json = System.Text.Json.JsonSerializer.Serialize(payload, options: new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });

        HttpClient client = await CreateClientWithAuth(url, "POST", requestId, json, ct);
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        var response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"), ct);
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: ct);
    }

    public async Task<TResponse?> Delete<TResponse>(string url, Guid? requestId = null, CancellationToken ct = default)
    {
        HttpClient client = await CreateClientWithAuth(url, "DELETE", requestId, null, ct);

        HttpResponseMessage? response = await client.DeleteAsync(url, ct);
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: ct);
    }

    /// <summary>
    /// Get and cache server time.
    /// </summary>
    internal async Task<string?> GetServerTime(CancellationToken ct)
        => await _memoryCache.GetOrCreateAsync("NiceHashServerTime", async entity =>
        {

            try
            {
                var response = await GetAnnonymous<ServerTimeResponse>("/api/v2/time", ct);

                // TODO: Check how often we need to update server time.
                entity.SetAbsoluteExpiration(TimeSpan.FromMinutes(1));
                return response?.ServerTime.ToString();
            }
            catch
            {
                return null;
            }
        });

    private async Task<HttpClient> CreateClientWithAuth(string url, string method, Guid? requestId = null, string? jsonPayload = null, CancellationToken ct = default)
    {
        HttpClient client = _httpClientFactory.CreateClient();
        client.BaseAddress = _apiConfig.BaseUri;

        await AddAuth(client.DefaultRequestHeaders, url, method, jsonPayload, ct);

        if (requestId.HasValue)
        {
            client.DefaultRequestHeaders.Add("X-Request-Id", requestId.ToString());
        }

        return client;
    }

    private async Task AddAuth(HttpRequestHeaders header, string url, string method, string payload = null, CancellationToken ct = default)
    {
        string serverTime = await GetServerTime(ct) ?? throw new ServerTimeException();

        string nonce = Guid.NewGuid().ToString();
        string digest = CryptoUtils.HashBySegments(_apiConfig.ApiSecret, _apiConfig.ApiKey, serverTime, nonce, _apiConfig.OrganizationId, method, url, payload);

        header.Add("X-Time", serverTime);
        header.Add("X-Nonce", nonce);
        header.Add("X-Auth", $"{_apiConfig.ApiKey}:{digest}");
        header.Add("X-Organization-Id", _apiConfig.OrganizationId);
    }
}