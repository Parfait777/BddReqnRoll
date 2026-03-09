namespace Coral.Clean.API.Gateway.Configurations
{
    /// <summary>
    /// The configuration options for the API Gateway.
    /// This includes settings for IP whitelisting, forwarded headers,
    /// maximum request body size, client ID header requirement,
    /// allowed content types, and other gateway-level policies.
    /// </summary>
    public record GatewayOptions
    {
        /// <summary>
        /// Gets or sets the list of allowed IP addresses (whitelist).
        /// </summary>
        /// <value>The list of allowed IP addresses.</value>
        public List<string> IpWhitelist { get; init; } = [];

        /// <summary>
        /// Gets or sets a value indicating whether forwarded headers should be trusted.
        /// </summary>
        /// <value><c>true</c> if forwarded headers are trusted; otherwise, <c>false</c>.</value>
        public bool TrustForwardedHeaders { get; init; } = true;

        /// <summary>
        /// Gets or sets the maximum allowed size of the request body in bytes.
        /// </summary>
        /// <value>The maximum allowed size of the request body in bytes.</value>
        public long MaxBodyBytes { get; init; } = 10 * 1024 * 1024;

        /// <summary>
        /// Gets or sets a value indicating whether the Client ID header is required.
        /// </summary>
        /// <value><c>true</c> if the Client ID header is required; otherwise, <c>false</c>.</value>
        public bool RequireClientIdHeader { get; init; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether an API key header is required for authentication.
        /// </summary>
        /// <value><c>true</c> if an API key header is required; otherwise, <c>false</c>.</value>
        public bool RequiredApiKeyHeader { get; init; } = true;

        /// <summary>
        /// Gets or sets the list of allowed content types for incoming requests.
        /// </summary>
        /// <value>The list of allowed content types.</value>
        public List<string> AllowedContentTypes { get; init; } = ["application/json"];

        /// <summary>
        /// Gets or sets the list of URL path segments that are exempt from API key authentication.
        /// </summary>
        public IList<string> AuthenticationExemptPaths { get; init; } = ["/health", "/swagger"];
    }
}
