namespace Coral.Clean.API.Gateway.Extensions
{
    /// <summary>
    /// Extension methods for HttpRequest to simplify common header checks and manipulations in the API Gateway.
    /// </summary>
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Determines whether the specified header is null, empty, or consists only of white-space characters.
        /// </summary>
        /// <param name="request">The HttpRequest to check.</param>
        /// <param name="headerName">The name of the header to check.</param>
        /// <returns>true if the header is null, empty, or consists only of white-space characters; otherwise, false.</returns>
        public static bool IsHeaderNullOrWhiteSpace(this HttpRequest request, string headerName)
        {
            if (request.Headers.TryGetValue(headerName, out var headerValues))
            {
                return string.IsNullOrWhiteSpace(headerValues.FirstOrDefault());
            }
            return true;
        }
    }
}
