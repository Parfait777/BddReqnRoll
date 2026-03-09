using Asp.Versioning;
using Coral.Clean.API.Gateway.Configurations;
using Coral.Clean.API.Gateway.Middleware;
using Coral.Clean.API.Gateway.Security.Abstractions;
using Coral.Clean.API.Gateway.Security.Gateway;
using Coral.Clean.API.Gateway.Security.Infrastructure;
using Microsoft.AspNetCore.HttpOverrides;

namespace Coral.Clean.API.Gateway
{
    /// <summary>
    /// Extension methods for configuring services in the CIMA PUBLIC API Gateways.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures the coral API gateway.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        public static IServiceCollection ConfigureCoralApiGateway(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddCoralApiGatewayInfra(configuration)
                .ConfigureSwagger()
                .AddSwaggerForOcelot(configuration);

            return services;
        }

        /// <summary>
        /// Adds the coral API gateway infra.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        private static IServiceCollection AddCoralApiGatewayInfra(this IServiceCollection services, IConfiguration configuration)
        {
           services
            .Configure<GatewayOptions>(configuration.GetSection(nameof(GatewayOptions)))
            .Configure<ApiKeyAuthOptions>(configuration.GetSection(ApiKeyAuthOptions.SectionName))
            .Configure<KeycloakOptions>(configuration.GetSection(KeycloakOptions.SectionName))
            .Configure<ApiKeyCatalogOptions>(configuration.GetSection(ApiKeyCatalogOptions.SectionName))
            .Configure<DownstreamHealthOptions>(configuration.GetSection(nameof(DownstreamHealthOptions)))
            .AddTransient<IpWhitelistMiddleware>()
            .AddTransient<RequestValidationMiddleware>();

            services.AddMemoryCache();

            // API Key -> Keycloak token exchange (gateway-managed auth)
            services.AddSingleton<IApiKeyStore, ConfigApiKeyStore>();

            services.AddHttpClient<KeycloakTokenService>((sp, http) =>
            {
                var opts = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<KeycloakOptions>>().Value;
                http.BaseAddress = new Uri(opts.BaseUrl);
                http.Timeout = TimeSpan.FromSeconds(Math.Max(1, opts.TokenRequestTimeoutSeconds));
            });
            services.AddSingleton<ITokenService>(sp => sp.GetRequiredService<KeycloakTokenService>());

            // Ocelot DelegatingHandler
            services.AddTransient<ApiKeyToTokenDelegatingHandler>();

            // Forwarded headers (if behind LB / reverse proxy)
           services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            return services;
        }

        private static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            return
                services
                    .AddApiVersioning(options =>
                    {
                        options.ReportApiVersions = true;
                        options.DefaultApiVersion = new ApiVersion(1, 0);
                        options.AssumeDefaultVersionWhenUnspecified = true;
                        options.ApiVersionReader = ApiVersionReader.Combine(
                            new UrlSegmentApiVersionReader(),
                            new HeaderApiVersionReader("X-Api-Version"),
                            new QueryStringApiVersionReader("api-version"));
                    })
                    .AddApiExplorer(options =>
                    {
                        options.GroupNameFormat = "'v'VVV";
                        options.SubstituteApiVersionInUrl = true;
                    })
                    .Services.AddSwaggerGen()
                    .ConfigureOptions<ConfigureSwaggerOptions>();
        }
    }
}
