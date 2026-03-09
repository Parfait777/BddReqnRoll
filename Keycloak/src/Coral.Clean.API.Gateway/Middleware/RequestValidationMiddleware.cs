using Coral.Clean.API.Gateway.Configurations;
using Coral.Clean.API.Gateway.Extensions;
using Microsoft.Extensions.Options;

namespace Coral.Clean.API.Gateway.Middleware
{
    public sealed class RequestValidationMiddleware(
        IOptions<ApiKeyAuthOptions> apiKeyAuthOptions,
        IOptions<GatewayOptions> gatewayOptions,
        ILogger<RequestValidationMiddleware> logger) 
        : IMiddleware
    {

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            bool isExemptPath = 
                gatewayOptions.Value.AuthenticationExemptPaths.Any(path => context.Request.Path.StartsWithSegments(path, StringComparison.OrdinalIgnoreCase));
            if (isExemptPath)
            {
                string infoMessage = $"Request path '{context.Request.Path}' is exempt from API key authentication and validation.";
                logger.LogInformation(infoMessage);
                await next(context);
                return;
            }

            string clientHeader = apiKeyAuthOptions.Value.ClientIdHeader;
            string apiKeyHeader = apiKeyAuthOptions.Value.ApiKeyHeader;

            // 1) Basic header validation for client-based rate limiting identity
            if (gatewayOptions.Value.RequireClientIdHeader && context.Request.IsHeaderNullOrWhiteSpace(clientHeader))
            {
                string errorMessage = $"Missing required client ID header: {clientHeader}.";
                logger.LogError(errorMessage);
                await context.WriteProblemDetailsAsJsonAsync(StatusCodes.Status400BadRequest, errorMessage);
                return;
            }

            if (gatewayOptions.Value.RequiredApiKeyHeader && context.Request.IsHeaderNullOrWhiteSpace(apiKeyHeader))
            {
                string errorMessage = $"Missing required API key header: {apiKeyHeader}.";
                logger.LogError(errorMessage);
                await context.WriteProblemDetailsAsJsonAsync(StatusCodes.Status400BadRequest, errorMessage);
                return;
            }

            // 2) Content-Type allowlist (skip for GET/HEAD)
            string method = context.Request.Method;
            if (!HttpMethods.IsGet(method) && !HttpMethods.IsHead(method))
            {
                string contentType = context.Request.ContentType ?? string.Empty;
                bool ok = gatewayOptions.Value.AllowedContentTypes.Any(allowed =>
                    contentType.StartsWith(allowed, StringComparison.OrdinalIgnoreCase));

                if (!ok)
                {
                    string errorMessage = $"Unsupported Content-Type: {contentType}. Allowed: {string.Join(", ", gatewayOptions.Value.AllowedContentTypes)}.";
                    logger.LogError(errorMessage);
                    await context.WriteProblemDetailsAsJsonAsync(
                        StatusCodes.Status415UnsupportedMediaType,
                        errorMessage);
                    return;
                }
            }

            // 3) Max body size (protect downstream)
            if (context.Request.ContentLength.HasValue && context.Request.ContentLength.Value > gatewayOptions.Value.MaxBodyBytes)
            {
                string errorMessage = $"Request body too large. Content-Length: {context.Request.ContentLength.Value} bytes. Max allowed: {gatewayOptions.Value.MaxBodyBytes} bytes.";
                logger.LogError(errorMessage);
                await context.WriteProblemDetailsAsJsonAsync(StatusCodes.Status413PayloadTooLarge, errorMessage);
                return;
            }

            // 4) Optional: enforce HTTPS at gateway (if not handled by LB)
            if (!context.Request.IsHttps)
            {
                string errorMessage = "Insecure request. HTTPS is required.";
                logger.LogError(errorMessage);
                await context.WriteProblemDetailsAsJsonAsync(StatusCodes.Status400BadRequest, errorMessage);
                return;
            }

            await next(context);
        }
    }
}
