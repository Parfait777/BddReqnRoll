namespace Coral.Clean.API.Gateway.Configurations;

/// <summary>
/// Keycloak connectivity options.
/// </summary>
public sealed class KeycloakOptions
{
    public const string SectionName = nameof(KeycloakOptions);

    /// <summary>
    /// Keycloak base URL, e.g. https://keycloak.example.com
    /// </summary>
    public required string BaseUrl { get; init; }

    /// <summary>
    /// Optional hard timeout for token requests.
    /// </summary>
    public int TokenRequestTimeoutSeconds { get; init; } = 10;

    /// <summary>
    /// Cache skew (seconds) to avoid using near-expiry tokens.
    /// Token is cached for (expires_in - skew).
    /// </summary>
    public int TokenExpirySkewSeconds { get; init; } = 30;
}
