namespace Coral.Clean.API.Gateway.Extensions
{
    public static class HttpRequestMessageExtensions
    {
        /// <summary>
        /// Determines whether the specified header is null, empty, or consists only of white-space characters.
        /// </summary>
        /// <param name="request">The HttpRequestMessage to check.</param>
        /// <param name="headerName">The name of the header to check.</param>
        /// <returns>true if the header is null, empty, or consists only of white-space characters; otherwise, false.</returns>
        public static bool IsHeaderNullOrWhiteSpace(this HttpRequestMessage request, string headerName, out string? headerValue)
        {
            if (request.Headers.TryGetValues(headerName, out var headerValues))
            {
                headerValue = headerValues.FirstOrDefault();
                return string.IsNullOrWhiteSpace(headerValue);
            }
            headerValue = null;
            return true;
        }
    }
}
