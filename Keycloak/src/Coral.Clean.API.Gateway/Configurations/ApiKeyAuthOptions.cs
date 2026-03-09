namespace Coral.Clean.API.Gateway.Configurations;

/// <summary>
/// Options for API-key based client authentication at the gateway.
/// Clients authenticate with (X-Client-Id, X-Api-Key). The gateway exchanges those
/// credentials for a Keycloak access token (client_credentials) and injects it downstream.
/// </summary>
public sealed class ApiKeyAuthOptions
{
    public const string SectionName = nameof(ApiKeyAuthOptions);

    /// <summary>
    /// If true, protected routes require both X-Client-Id and X-Api-Key.
    /// </summary>
    public bool Enabled { get; init; } = true;

    /// <summary>
    /// Header name for the client identifier.
    /// </summary>
    public string ClientIdHeader { get; init; } = "X-Client-Id";

    /// <summary>
    /// Header name for the API key.
    /// </summary>
    public string ApiKeyHeader { get; init; } = "X-Api-Key";

    /// <summary>
    /// If true, the gateway will remove API key headers before forwarding downstream.
    /// </summary>
    public bool StripCredentialsBeforeForwarding { get; init; } = true;

    /// <summary>
    /// If true, API keys stored in configuration are SHA256 hashes (hex) instead of plaintext.
    /// </summary>
    public bool ApiKeysAreSha256Hex { get; init; } = true;
}
