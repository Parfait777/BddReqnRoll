namespace Coral.Clean.API.Gateway.Configurations
{
    /// <summary>
    /// Swagger for Ocelot Endpoitns Config Options
    /// </summary>
    public class SwaggerForOcelotEndpointsConfigOptions
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; } = default!;
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public string Version { get; set; } = default!;
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string Url { get; set; } = default!;
    }
}
