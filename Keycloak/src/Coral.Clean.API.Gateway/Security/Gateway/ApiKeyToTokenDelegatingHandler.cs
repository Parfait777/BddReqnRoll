using Coral.Clean.API.Gateway.Configurations;
using Coral.Clean.API.Gateway.Extensions;
using Coral.Clean.API.Gateway.Security.Abstractions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Coral.Clean.API.Gateway.Security.Gateway;

/// <summary>
/// Ocelot delegating handler:
/// - Reads (X-Client-Id, X-Api-Key)
/// - Resolves to Keycloak client credentials
/// - Acquires (cached) access token via client_credentials
/// - Injects Authorization: Bearer {token}
/// - Optionally strips the credential headers before forwarding
/// </summary>
public sealed class ApiKeyToTokenDelegatingHandler(
    IOptions<ApiKeyAuthOptions> apiKeyAuthOptions,
    IApiKeyStore apiKeyStore,
    ITokenService tokenService,
    ILogger<ApiKeyToTokenDelegatingHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        if (!apiKeyAuthOptions.Value.Enabled)
        {
            return await base.SendAsync(request, ct);
        }

        string clientHeader = apiKeyAuthOptions.Value.ClientIdHeader;
        string apiKeyHeader = apiKeyAuthOptions.Value.ApiKeyHeader;

        // 1) Basic header validation for client-based rate limiting identity
        if (request.IsHeaderNullOrWhiteSpace(clientHeader, out string? externalClientId))
        {
            string errorMessage = $"Missing required client ID header: {clientHeader}.";
            logger.LogError(errorMessage);
            return BadRequest(errorMessage);
        }

        if (request.IsHeaderNullOrWhiteSpace(apiKeyHeader, out string? apiKey))
        {
            string errorMessage = $"Missing required API key header: {apiKeyHeader}.";
            logger.LogError(errorMessage);
            return BadRequest(errorMessage);
        }

        if (await apiKeyStore.IsRevokedAsync(externalClientId!, apiKey!, ct))
        {
            return Unauthorized("API key revoked.");
        }

        ApiClientCredential? cred = await apiKeyStore.TryResolveAsync(externalClientId!, apiKey!, ct);
        if (cred is null)
        {
            return Unauthorized("Unknown client id / API key.");
        }

        string token = await tokenService.GetAccessTokenAsync(cred, ct);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        if (apiKeyAuthOptions.Value.StripCredentialsBeforeForwarding)
        {
            request.Headers.Remove(clientHeader);
            request.Headers.Remove(apiKeyHeader);
        }

        return await base.SendAsync(request, ct);
    }

    private static HttpResponseMessage Unauthorized(string message) => Problem(message, System.Net.HttpStatusCode.Unauthorized);

    private static HttpResponseMessage BadRequest(string message) => Problem(message, System.Net.HttpStatusCode.BadRequest);
    private static HttpResponseMessage Problem(string message, System.Net.HttpStatusCode statusCode)
    {
        ProblemDetails problemDetails = new()
        {
            Type = $"https://example.com/errors/{statusCode.ToString().ToLower()}",
            Status = (int)statusCode,
            Title = statusCode.ToString(),
            Detail = message
        };
        HttpResponseMessage response = new(statusCode)
        {
            Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(problemDetails), System.Text.Encoding.UTF8, "application/problem+json")
        };
        return response;
    }
}
