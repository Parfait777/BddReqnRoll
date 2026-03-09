using Coral.Clean.API.Gateway.HealthChecks;

namespace Coral.Clean.API.Gateway.Configurations
{
    public sealed record DownstreamHealthOptions
    {
        /// <summary>
        /// Gets or sets the list of downstream API endpoints to monitor for health checks.
        /// </summary>
        /// <value>The list of health check endpoints.</value>
        public IList<EndpointModel> Endpoints { get; set; } = [];

        /// <summary>
        /// If true, any failed endpoint makes readiness Unhealthy. If false, partial failures return Degraded.
        /// </summary>
        public bool FailReadinessOnAnyFailure { get; init; } = true;
    }
}
