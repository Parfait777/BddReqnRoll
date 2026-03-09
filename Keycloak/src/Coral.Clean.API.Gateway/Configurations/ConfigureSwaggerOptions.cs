using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Coral.Clean.API.Gateway.Configurations
{
    public class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        : IConfigureOptions<SwaggerGenOptions>
    {
        public void Configure(SwaggerGenOptions options)
        {
            foreach (ApiVersionDescription description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, new OpenApiInfo
                {
                    Title = "CIMA Public API Gateway",
                    Version = description.GroupName,
                    Description = "CIMA Public API Gateway endpoints (health, diagnostics, etc.)"
                });
            }

            // Optional: surface correlation/client headers in Swagger UI
            options.AddSecurityDefinition("ClientId", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                Name = "X-Client-Id",
                In = ParameterLocation.Header,
                Description = "Client identifier used for rate limiting."
            });


            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ClientId" }
                    },
                    Array.Empty<string>()
                }
            });

            options.EnableAnnotations();
        }
    }
}
