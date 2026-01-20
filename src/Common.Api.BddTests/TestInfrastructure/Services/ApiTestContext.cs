using Common.Api.BddTests.TestInfrastructure.Enums;
using System.Net.Http.Headers;

namespace Common.Api.BddTests.TestInfrastructure.Services
{
    public sealed class ApiTestContext
    {
        public string BaseUrl { get; set; } = string.Empty;

        public string? AccessToken { get; set; }

        public Dictionary<string, string> DefaultHeaders { get; } = [];

        public HttpResponseMessage? Response { get; set; }
        public string? ResponseBody { get; set; }

        public string? LastCreatedId { get; set; } // optional helper for CRUD flows

        public void ApplyAuth(HttpRequestMessage request)
        {
            if (!string.IsNullOrWhiteSpace(AccessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(AuthMode.Bearer.ToString(), AccessToken);
            }

            foreach (var kv in DefaultHeaders)
            {
                request.Headers.Remove(kv.Key);
                request.Headers.Add(kv.Key, kv.Value);
            }
        }
    }
}
