namespace Coral.Clean.API.Gateway.Configurations
{
    /// <summary>
    /// Swagger for Ocelot Endpoints Options
    /// </summary>
    public class SwaggerForOcelotEndpointsOptions
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key { get; set; } = default!;
        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public List<SwaggerForOcelotEndpointsConfigOptions> Config { get; set; } = new();
    }
}
