using Common.Api.BddTests.TestInfrastructure.Extensions;
using Common.Api.BddTests.TestInfrastructure.Services;
using FluentAssertions;
using Reqnroll;
using System.Text;
using System.Text.Json;

namespace Common.Api.BddTests.Specs.Steps
{
    [Binding]
    public sealed class HttpSteps(IHttpClientFactory httpClientFactory, ApiTestContext ctx)
    {
        [When(@"I GET ""(.*)""")]
        public async Task WhenIGetAsync(string path) =>
            await SendAsync(HttpMethod.Get, path, jsonBody: null).ConfigureAwait(false);

        [When(@"I DELETE ""(.*)""")]
        public async Task WhenIDeleteAsync(string path) =>
            await SendAsync(HttpMethod.Delete, path, jsonBody: null).ConfigureAwait(false);

        [When(@"I POST ""(.*)"" with json")]
        public async Task WhenIPostWithJsonAsync(string path, string multilineJson) =>
            await SendAsync(HttpMethod.Post, path, multilineJson).ConfigureAwait(false);

        [When(@"I PUT ""(.*)"" with json")]
        public async Task WhenIPutWithJsonAsync(string path, string multilineJson) =>
            await SendAsync(HttpMethod.Put, path, multilineJson).ConfigureAwait(false);

        private async Task SendAsync(HttpMethod method, string path, string? jsonBody)
        {
            ctx.BaseUrl.Should().NotBeNullOrWhiteSpace("base url must be set");
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
