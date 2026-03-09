namespace Coral.Clean.API.Gateway.HealthChecks
{
    public sealed record EndpointModel
    {
        /// <summary>
        /// Gets or sets the name of the endpoint (for reporting purposes).
        /// </summary>
        /// <value>The name of the endpoint.</value>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the URL of the endpoint to probe.
        /// </summary>
        /// <value>The URL of the endpoint.</value>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the timeout in seconds for the health check probe to this endpoint. Default is 5 seconds.
        /// </summary>
        /// <value>The timeout in seconds.</value>
        public int TimeoutInSeconds { get; set; } = 5;
    }
}
