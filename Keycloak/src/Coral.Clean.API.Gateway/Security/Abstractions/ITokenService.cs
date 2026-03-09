namespace Coral.Clean.API.Gateway.Security.Abstractions;

public interface ITokenService
{
    Task<string> GetAccessTokenAsync(ApiClientCredential credential, CancellationToken ct);
}
