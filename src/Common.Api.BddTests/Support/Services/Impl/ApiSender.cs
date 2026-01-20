using Common.Api.BddTests.Support.Services.Interfaces;
using Common.Api.BddTests.Support.Extensions;
using System.Text;
using System.Text.Json;

namespace Common.Api.BddTests.Support.Services.Impl
{
    internal sealed class ApiSender(IHttpClientFactory httpClientFactory, ApiTestContext ctx) : IApiSender
    {
        public async Task SendAsync(HttpMethod method, string path, string? jsonBody)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(ctx.BaseUrl), "base url must be set");

            var url = $"{ctx.BaseUrl}{ResolvePath(path)}";

            var client = httpClientFactory.CreateApiClient();

            using var req = new HttpRequestMessage(method, url);

            ctx.ApplyAuth(req);

            if (!string.IsNullOrWhiteSpace(jsonBody))
            {
                // validate JSON early so failures are obvious
                JsonDocument.Parse(jsonBody);

                req.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            }

            ctx.Response = await client.SendAsync(req);
            ctx.ResponseBody = ctx.Response.Content is null ? null : await ctx.Response.Content.ReadAsStringAsync();
        }

        private string ResolvePath(string path)
        {
            // simple variable substitution: {LastCreatedId}
            if (ctx.LastCreatedId is not null)
            {
                path = path.Replace("{LastCreatedId}", ctx.LastCreatedId, StringComparison.OrdinalIgnoreCase);
            }

            return path.StartsWith('/') ? path : "/" + path;
        }
    }
}
