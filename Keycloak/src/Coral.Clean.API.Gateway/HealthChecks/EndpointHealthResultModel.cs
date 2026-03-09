namespace Coral.Clean.API.Gateway.HealthChecks
{
    public sealed record EndpointHealthResultModel(
        bool Ok,
        string Message,
        int? StatusCode,
        double DurationMs);
}
