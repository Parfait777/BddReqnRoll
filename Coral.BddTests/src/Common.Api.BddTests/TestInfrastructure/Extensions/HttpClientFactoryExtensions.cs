using Common.Api.BddTests.TestInfrastructure;

namespace Common.Api.BddTests.TestInfrastructure.Extensions
{
    public static class HttpClientFactoryExtensions
    {
        public static HttpClient CreateApiClient(this IHttpClientFactory httpClientFactory) => httpClientFactory.CreateClient(HttpConstants.ApiClientName);
        public static HttpClient CreateAuthClient(this IHttpClientFactory httpClientFactory) => httpClientFactory.CreateClient(HttpConstants.AuthClientName);
    }
}
