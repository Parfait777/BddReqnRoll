namespace Coral.Clean.API.Gateway.Security.Abstractions
{
    /// <summary>
    /// Resolves incoming (clientId, apiKey) credentials to a downstream identity (Keycloak client)
    /// and supports revocation checks.
    /// </summary>
    public interface IApiKeyStore
    {
        Task<ApiClientCredential?> TryResolveAsync(string externalClientId, string apiKey, CancellationToken ct);

        Task<bool> IsRevokedAsync(string externalClientId, string apiKey, CancellationToken ct);
    }
}
