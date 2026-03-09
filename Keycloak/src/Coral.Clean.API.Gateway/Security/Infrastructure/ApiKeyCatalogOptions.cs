namespace Coral.Clean.API.Gateway.Security.Infrastructure;

/// <summary>
/// Simple configuration-backed API key catalog.
/// Replace with a DB/Redis-backed store for production.
/// </summary>
public sealed class ApiKeyCatalogOptions
{
    public const string SectionName = nameof(ApiKeyCatalogOptions);

    public List<ApiKeyClientEntry> Clients { get; init; } = [];
}

public sealed class ApiKeyClientEntry
{
    /// <summary>External client identifier (sent by consumer as X-Client-Id).</summary>
    public required string ExternalClientId { get; init; }

    /// <summary>
    /// API key value (either plaintext or SHA256 hex depending on ApiKeyAuthOptions).
    /// </summary>
    public required string ApiKey { get; init; }

    public bool Revoked { get; init; }

    /// <summary>Client realm.</summary>
    public required string Realm { get; init; }

    /// <summary>Client id used for client_credentials token exchange.</summary>
    public required string ClientId { get; init; }

    /// <summary>Client secret used for client_credentials token exchange.</summary>
    public required string ClientSecret { get; init; }

    /// <summary>Optional scopes.</summary>
    public List<string> Scopes { get; init; } = [];
}
