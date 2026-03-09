using System.Security.Cryptography;
using System.Text;
using Coral.Clean.API.Gateway.Configurations;
using Coral.Clean.API.Gateway.Security.Abstractions;
using Microsoft.Extensions.Options;

namespace Coral.Clean.API.Gateway.Security.Infrastructure;

/// <summary>
/// Configuration-backed API key store.
/// Intended for development and small catalogs; replace with persistent backing store for production.
/// </summary>
public sealed class ConfigApiKeyStore(
    IOptions<ApiKeyAuthOptions> apiKeyAuthOptions,
    IOptions<ApiKeyCatalogOptions> apiKeyCatalogOptions) : IApiKeyStore
{
    public Task<ApiClientCredential?> TryResolveAsync(string externalClientId, string apiKey, CancellationToken ct)
    {
        ApiKeyClientEntry? entry = FindEntry(externalClientId, apiKey);
        if (entry is null || entry.Revoked)
        {
            return Task.FromResult<ApiClientCredential?>(null);
        }

        return Task.FromResult<ApiClientCredential?>(new ApiClientCredential(
            Realm: entry.Realm,
            KeycloakClientId: entry.ClientId,
            KeycloakClientSecret: entry.ClientSecret,
            Scopes: [.. entry.Scopes]));
    }

    public Task<bool> IsRevokedAsync(string externalClientId, string apiKey, CancellationToken ct)
    {
        ApiKeyClientEntry? entry = FindEntry(externalClientId, apiKey);
        return Task.FromResult(entry?.Revoked ?? true);
    }

    private ApiKeyClientEntry? FindEntry(string externalClientId, string apiKey)
    {
        if (string.IsNullOrWhiteSpace(externalClientId) || string.IsNullOrWhiteSpace(apiKey))
        {
            return null;
        }

        foreach (ApiKeyClientEntry entry in apiKeyCatalogOptions.Value.Clients)
        {
            if (!externalClientId.Equals(entry.ExternalClientId, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (apiKeyAuthOptions.Value.ApiKeysAreSha256Hex)
            {
                string presentedHash = Sha256Hex(apiKey);
                if (FixedTimeEqualsHex(presentedHash, entry.ApiKey))
                {
                    return entry;
                }
            }
            else
            {
                if (FixedTimeEquals(Encoding.UTF8.GetBytes(apiKey), Encoding.UTF8.GetBytes(entry.ApiKey)))
                {
                    return entry;
                }
            }
        }

        return null;
    }

    private static string Sha256Hex(string input)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        var sb = new StringBuilder(bytes.Length * 2);
        foreach (byte b in bytes)
        {
            _ = sb.Append(b.ToString("x2"));
        }
        return sb.ToString();
    }

    private static bool FixedTimeEqualsHex(string a, string b)
    {
        // Normalize to lowercase to avoid trivial mismatches.
        a = a.Trim().ToLowerInvariant();
        b = b.Trim().ToLowerInvariant();
        return FixedTimeEquals(Encoding.UTF8.GetBytes(a), Encoding.UTF8.GetBytes(b));
    }

    private static bool FixedTimeEquals(byte[] a, byte[] b)
    {
        if (a.Length != b.Length)
        {
            // Still do fixed-time comparison on same-length buffers to reduce leakiness.
            int min = Math.Min(a.Length, b.Length);
            Span<byte> aa = stackalloc byte[min];
            Span<byte> bb = stackalloc byte[min];
            a.AsSpan(0, min).CopyTo(aa);
            b.AsSpan(0, min).CopyTo(bb);
            _ = CryptographicOperations.FixedTimeEquals(aa, bb);
            return false;
        }

        return CryptographicOperations.FixedTimeEquals(a, b);
    }
}
