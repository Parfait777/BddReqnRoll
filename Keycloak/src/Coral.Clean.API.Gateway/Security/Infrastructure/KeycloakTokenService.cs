using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Text.Json;
using Coral.Clean.API.Gateway.Configurations;
using Coral.Clean.API.Gateway.Security.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Coral.Clean.API.Gateway.Security.Infrastructure;

/// <summary>
/// Acquires Keycloak access tokens (client_credentials) and caches them to avoid per-request token calls.
/// </summary>
public sealed class KeycloakTokenService : ITokenService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly HttpClient http;
    private readonly IMemoryCache cache;
    private readonly KeycloakOptions options;
    private readonly ILogger<KeycloakTokenService> logger;

    // Prevent thundering herd: one token request per cache key at a time.
    private readonly ConcurrentDictionary<string, SemaphoreSlim> locks = new();

    public KeycloakTokenService(
        HttpClient http,
        IMemoryCache cache,
        IOptions<KeycloakOptions> options,
        ILogger<KeycloakTokenService> logger)
    {
        this.http = http;
        this.cache = cache;
        this.options = options.Value;
        this.logger = logger;
    }

    public async Task<string> GetAccessTokenAsync(ApiClientCredential credential, CancellationToken ct)
    {
        string cacheKey = BuildCacheKey(credential);
        if (this.cache.TryGetValue<string>(cacheKey, out string? token) && !string.IsNullOrWhiteSpace(token))
        {
            return token;
        }

        SemaphoreSlim gate = this.locks.GetOrAdd(cacheKey, _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync(ct);
        try
        {
            // Check again after acquiring lock.
            if (this.cache.TryGetValue<string>(cacheKey, out token) && !string.IsNullOrWhiteSpace(token))
            {
                return token;
            }

            KeycloakTokenResponse resp = await RequestTokenAsync(credential, ct);

            int skew = Math.Max(0, this.options.TokenExpirySkewSeconds);
            int ttl = Math.Max(5, resp.expires_in - skew);

            this.cache.Set(cacheKey, resp.access_token, TimeSpan.FromSeconds(ttl));
            return resp.access_token;
        }
        finally
        {
            _ = gate.Release();
        }
    }

    private async Task<KeycloakTokenResponse> RequestTokenAsync(ApiClientCredential credential, CancellationToken ct)
    {
        // POST /realms/{realm}/protocol/openid-connect/token
        string tokenPath = $"/realms/{Uri.EscapeDataString(credential.Realm)}/protocol/openid-connect/token";

        var kv = new List<KeyValuePair<string, string>>
        {
            new("grant_type", "client_credentials"),
            new("client_id", credential.KeycloakClientId),
            new("client_secret", credential.KeycloakClientSecret),
        };

        if (credential.Scopes is { Length: > 0 })
        {
            string scope = string.Join(' ', credential.Scopes.Where(s => !string.IsNullOrWhiteSpace(s)));
            if (!string.IsNullOrWhiteSpace(scope))
            {
                kv.Add(new("scope", scope));
            }
        }

        using var content = new FormUrlEncodedContent(kv);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

        using var req = new HttpRequestMessage(HttpMethod.Post, tokenPath) { Content = content };

        using var resp = await this.http.SendAsync(req, ct);
        string body = await resp.Content.ReadAsStringAsync(ct);

        if (!resp.IsSuccessStatusCode)
        {
            this.logger.LogWarning(
                "Keycloak token request failed. Status={StatusCode}. Body={Body}",
                (int)resp.StatusCode,
                body);

            throw new InvalidOperationException($"Keycloak token request failed: {(int)resp.StatusCode}");
        }

        KeycloakTokenResponse? token = JsonSerializer.Deserialize<KeycloakTokenResponse>(body, JsonOptions);
        return token ?? throw new InvalidOperationException("Keycloak token response could not be parsed.");
    }

    private static string BuildCacheKey(ApiClientCredential credential)
    {
        string scopes = credential.Scopes is { Length: > 0 }
            ? string.Join(' ', credential.Scopes).Trim()
            : string.Empty;

        return $"kc-token::{credential.Realm}::{credential.KeycloakClientId}::{scopes}";
    }
}
