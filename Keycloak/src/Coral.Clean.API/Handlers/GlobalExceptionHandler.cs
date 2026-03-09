using Microsoft.AspNetCore.Diagnostics;

namespace Coral.Clean.API.Handlers
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            // Return problem details response for unhandled exceptions, and log the error.
            logger.LogError(exception, "An unhandled exception occurred while processing the request.");

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/problem+json";

            var problemDetails = new
            {
                type = "https://example.com/probs/internal-server-error",
                title = "An unexpected error occurred.",
                status = 500,
                detail = "An unexpected error occurred while processing your request. Please try again later."
            };

            return await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken)
                .ContinueWith(_ => true, cancellationToken);
        }
    }
}
