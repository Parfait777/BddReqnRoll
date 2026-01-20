using Common.Api.BddTests.Support;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System.Net;

namespace Common.Api.BddTests.Support.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers HttpClients used by the test suite.
        /// Adds a Polly retry policy (transient only) to the "api" client.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="retryCount">Number of retries for transient failures (e.g., 5).</param>
        /// <param name="baseDelay">Base delay used for exponential backoff (default 200ms).</param>
        /// <param name="maxJitter">Max jitter added per retry (default 150ms).</param>
        public static IServiceCollection AddHttpClients(
            this IServiceCollection services,
            int retryCount = 5,
            TimeSpan? baseDelay = null,
            int maxJitter = 150)
        {
            ArgumentNullException.ThrowIfNull(services);

            ArgumentOutOfRangeException.ThrowIfNegative(retryCount);

            ArgumentOutOfRangeException.ThrowIfNegative(maxJitter);

            var baseDelayValue = baseDelay ?? TimeSpan.FromMilliseconds(200);

            // Retries only for transient failures: network errors, 5xx/408, timeouts, and 429.
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError() // HttpRequestException, 5xx, 408
                .OrResult(r => r.StatusCode == HttpStatusCode.TooManyRequests) // 429
                .Or<TaskCanceledException>() // typical HttpClient timeout/cancel surface
                .WaitAndRetryAsync(
                    retryCount,
                    retryAttempt =>
                    {
                        // exponential backoff: baseDelay * 2^retryAttempt (+ jitter)
                        var exp = Math.Pow(2, retryAttempt);
                        var delayMs = baseDelayValue.TotalMilliseconds * exp;

                        var jitterMs = maxJitter == 0 ? 0 : Random.Shared.Next(0, maxJitter + 1);

                        return TimeSpan.FromMilliseconds(delayMs + jitterMs);
                    });

            services
                .AddHttpClient(HttpConstants.ApiClientName)
                .AddPolicyHandler(retryPolicy);

            // Token calls: usually keep simple, unless you see transient failures from your IdP.
            services.AddHttpClient(HttpConstants.AuthClientName);

            return services;
        }
    }
}
