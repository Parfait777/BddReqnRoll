using Common.Api.BddTests.Support.Models;
using Common.Api.BddTests.Support.Services.Impl;
using Common.Api.BddTests.Support.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Api.BddTests.Support.Services
{
    public static class Setup

    {
        public static IServiceCollection CreateServices()
        {
            var services = new ServiceCollection();

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.tests.json", optional: false)
                .AddEnvironmentVariables() // allow overrides in pipeline
                .Build();

            services
                .AddSingleton(config)
                .AddSingleton<ApiTestContext>();

            // Bind options
            var api = config.GetSection(nameof(ApiOptions)).Get<ApiOptions>() ?? new ApiOptions();
            var auth = config.GetSection(nameof(AuthOptions)).Get<AuthOptions>() ?? new AuthOptions();
            var apiKey = config.GetSection(nameof(ApiKeyOptions)).Get<ApiKeyOptions>() ?? new ApiKeyOptions();

            services
                .AddSingleton(api)
                .AddSingleton(auth)
                .AddSingleton(apiKey);

            services.AddHttpClient(); // for token + API calls
            services.AddScoped<IApiSender, ApiSender>();

            return services;
        }
    }
}
