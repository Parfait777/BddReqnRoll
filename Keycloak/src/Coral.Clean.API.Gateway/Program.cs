using Coral.Clean.API.Gateway;
using Coral.Clean.API.Gateway.Configurations;
using Coral.Clean.API.Gateway.Conventions;
using Coral.Clean.API.Gateway.HealthChecks;
using Coral.Clean.API.Gateway.Middleware;
using Coral.Clean.API.Gateway.Security.Gateway;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Load Ocelot configuration
string ocelotConfigName = builder.Configuration["OcelotConfigName"]
    ?? throw new Exception("Failed to find ocelot config for current environemnt");

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile(ocelotConfigName, optional: false, reloadOnChange: true);

List<SwaggerForOcelotEndpointsOptions> swaggerForOcelotEndpointsOptions = new();
builder.Configuration
    .GetSection("SwaggerEndPoints")
    .Bind(swaggerForOcelotEndpointsOptions);

List<string> apiVersions = [.. swaggerForOcelotEndpointsOptions
    .SelectMany(x => x.Config)
    .Select(y => y.Version)
    .Distinct()];

builder.Services.ConfigureCoralApiGateway(builder.Configuration);

// Controllers
builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new ExcludeInfrastructureEndpointsConvention());
});
builder.Services.AddHttpClient();

// Health checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("Gateway is running"))
    .AddCheck<AggregatedDownstreamReadinessHealthCheck>(
        name: "downstream:ready",
        failureStatus: HealthStatus.Unhealthy,
        tags: ["ready"]);

// Ocelot
builder.Services
    .AddOcelot(builder.Configuration)
    .AddDelegatingHandler<ApiKeyToTokenDelegatingHandler>();

WebApplication app = builder.Build();

// Forwarded headers early
GatewayOptions gwOpts = app.Services.GetRequiredService<IOptions<GatewayOptions>>().Value;
if (gwOpts.TrustForwardedHeaders)
{
    app.UseForwardedHeaders();
}

// Swagger MUST be mapped before Ocelot (so it is not routed downstream)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.DefaultModelsExpandDepth(-1);
    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);

    foreach (string version in apiVersions)
    {
        c.SwaggerEndpoint($"/swagger/docs/{version}/cima-public-api-{version}/swagger.json", $"CIMA Public API Gateway {version}");

    }
    c.RoutePrefix = "swagger";
});

app.UseSwaggerForOcelotUI(opt =>
{
    opt.PathToSwaggerGenerator = "/swagger/docs";
});

// Correlation ID
app.Use(async (ctx, next) =>
{
    const string header = "X-Correlation-Id";
    if (!ctx.Request.Headers.TryGetValue(header, out StringValues cid) || string.IsNullOrWhiteSpace(cid))
    {
        cid = Guid.NewGuid().ToString("N");
        ctx.Request.Headers[header] = cid;
    }

    ctx.Response.Headers[header] = cid.ToString();
    await next();
});

// Routing must be before MapControllers in middleware-heavy apps
app.UseRouting();

// Gateway policies
app.UseMiddleware<IpWhitelistMiddleware>();
app.UseMiddleware<RequestValidationMiddleware>();

// Map controllers BEFORE Ocelot (e.g., /health/*)
app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers();
});

// Ocelot pipeline
await app.UseOcelot();
await app.RunAsync();
