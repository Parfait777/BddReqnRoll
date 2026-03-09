using Microsoft.AspNetCore.Mvc;

namespace Coral.Clean.API.Gateway.Extensions
{
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Writes a ProblemDetails response in JSON format based on the provided HTTP status code.
        /// </summary>
        /// <param name="context">The HttpContext to write the response to.</param>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="detail">The detailed error message to include in the response.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static async Task WriteProblemDetailsAsJsonAsync(
            this HttpContext context,
            int statusCode,
            string detail,
            CancellationToken cancellationToken = default)
        {
            
            ProblemDetails problemDetails = new()
            {
                Type = $"https://example.com/errors/{statusCode.ToString().ToLower()}", // TODO: Use more specific types based on status code
                Status = statusCode,
                Title = GetReasonPhrase(statusCode),
                Detail = detail,
                Instance = context.Request.Path
            };
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        }

        private static string GetReasonPhrase(int statusCode)
        {
            return statusCode switch
            {
                StatusCodes.Status400BadRequest => "Bad Request",
                StatusCodes.Status401Unauthorized => "Unauthorized",
                StatusCodes.Status403Forbidden => "Forbidden",
                StatusCodes.Status404NotFound => "Not Found",
                StatusCodes.Status500InternalServerError => "Internal Server Error",
                _ => "Error"
            };
        }
    }
}
