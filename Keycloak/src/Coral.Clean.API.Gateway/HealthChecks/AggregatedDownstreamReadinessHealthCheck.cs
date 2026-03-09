using System.Diagnostics;
using Coral.Clean.API.Gateway.Configurations;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Coral.Clean.API.Gateway.HealthChecks
{
    public sealed class AggregatedDownstreamReadinessHealthCheck(IHttpClientFactory httpClientFactory, IOptionsMonitor<DownstreamHealthOptions> optionsMonitor)
        : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            DownstreamHealthOptions options = optionsMonitor.CurrentValue;

            if (options.Endpoints is null || options.Endpoints.Count == 0)
            {
                return HealthCheckResult.Degraded("No downstream endpoints configured for readiness checks.");
            }

            List<(string Name, bool Ok, string Message, int? StatusCode, double DurationMs)> results = new();

            foreach (EndpointModel endpoint in options.Endpoints)
            {
                (bool Ok, string Message, int? StatusCode, double DurationMs) result = await this.ProbeAsync(endpoint, cancellationToken);
                results.Add((endpoint.Name, result.Ok, result.Message, result.StatusCode, result.DurationMs));
            }

            int total = results.Count;
            int okCount = results.Count(r => r.Ok);
            int failCount = total - okCount;

            Dictionary<string, object> data = results.ToDictionary(
                r => r.Name,
                r => (object)new EndpointHealthResultModel(r.Ok, r.Message, r.StatusCode, r.DurationMs));

            if (failCount == 0)
            {
                return HealthCheckResult.Healthy("All downstream endpoints are ready.", data);
            }

            if (options.FailReadinessOnAnyFailure)
            {
                return HealthCheckResult.Unhealthy(
                    $"Downstream readiness failed for {failCount}/{total} endpoint(s).",
                    data: data);
            }

            // Partial failure => Degraded (still return details)
            return HealthCheckResult.Degraded(
                $"Downstream readiness degraded: {failCount}/{total} endpoint(s) failing.", data: data);
        }

        private async Task<(bool Ok, string Message, int? StatusCode, double DurationMs)> ProbeAsync(
            EndpointModel ep,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(ep.Name))
            {
                return (false, "Endpoint name is missing.", null, 0);
            }

            if (!Uri.TryCreate(ep.Url, UriKind.Absolute, out Uri? uri))
            {
                return (false, $"Invalid URL: '{ep.Url}'", null, 0);
            }

            int timeoutSeconds = Math.Max(1, ep.TimeoutInSeconds);

            HttpClient client = httpClientFactory.CreateClient(nameof(AggregatedDownstreamReadinessHealthCheck));
            client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

            Stopwatch sw = Stopwatch.StartNew();

            try
            {
                using HttpRequestMessage req = new(HttpMethod.Get, uri);
                using HttpResponseMessage res = await client.SendAsync(req, cancellationToken);

                sw.Stop();

                if (res.IsSuccessStatusCode)
                {
                    return (true, "Ready.", (int)res.StatusCode, sw.Elapsed.TotalMilliseconds);
                }

                return (false, $"Returned {(int)res.StatusCode} ({res.ReasonPhrase}).", (int)res.StatusCode, sw.Elapsed.TotalMilliseconds);
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                sw.Stop();
                return (false, $"Timed out after {timeoutSeconds}s. {ex.GetType().Name}", null, sw.Elapsed.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                sw.Stop();
                return (false, $"Exception: {ex.GetType().Name}", null, sw.Elapsed.TotalMilliseconds);
            }
        }
    }
}
