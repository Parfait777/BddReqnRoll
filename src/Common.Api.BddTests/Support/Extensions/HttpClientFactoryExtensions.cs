using Common.Api.BddTests.Support;

namespace Common.Api.BddTests.Support.Extensions
{
    public static class HttpClientFactoryExtensions
    {
        public static HttpClient CreateApiClient(this IHttpClientFactory httpClientFactory) => httpClientFactory.CreateClient(HttpConstants.ApiClientName);
        public static HttpClient CreateAuthClient(this IHttpClientFactory httpClientFactory) => httpClientFactory.CreateClient(HttpConstants.AuthClientName);
    }
}
