namespace Coral.Clean.API.Gateway.Security.Abstractions;

public sealed record ApiClientCredential(
    string Realm,
    string KeycloakClientId,
    string KeycloakClientSecret,
    string[] Scopes);

internal sealed record KeycloakTokenResponse(
    string access_token,
    int expires_in,
    int? refresh_expires_in,
    string token_type,
    string? scope);
