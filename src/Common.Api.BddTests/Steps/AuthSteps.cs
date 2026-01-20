using Common.Api.BddTests.Support.Helpers;
using Common.Api.BddTests.Support.Models;
using Common.Api.BddTests.Support.Services;
using FluentAssertions;
using Reqnroll;

namespace Common.Api.BddTests.Steps
{
    [Binding]
    public sealed class AuthSteps(
        IHttpClientFactory httpClientFactory,
        ApiTestContext ctx,
        ApiOptions apiOptions,
        AuthOptions authOptions,
        ApiKeyOptions apiKeyOptions)
    {

        [Given(@"the API base url is configured")]
        public void GivenTheApiBaseUrlIsConfigured()
        {
            apiOptions.BaseUrl.Should().NotBeNullOrWhiteSpace();
            ctx.BaseUrl = apiOptions.BaseUrl.TrimEnd('/');
        }

        [Given(@"I authenticate")]
        public async Task GivenIAuthenticate()
        {
            if (string.Equals(authOptions.Mode, "ApiKey", StringComparison.OrdinalIgnoreCase))
            {
                apiKeyOptions.Value.Should().NotBeNullOrWhiteSpace("API key mode requires ApiKey:Value");
                ctx.DefaultHeaders[apiKeyOptions.HeaderName] = apiKeyOptions.Value;
                return;
            }

            // Bearer token: client_credentials (application/x-www-form-urlencoded)
            authOptions.TokenUrl.Should().NotBeNullOrWhiteSpace();
            authOptions.ClientId.Should().NotBeNullOrWhiteSpace();
            authOptions.ClientSecret.Should().NotBeNullOrWhiteSpace();

            var client = httpClientFactory.CreateClient();

            var form = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = authOptions.ClientId,
                ["client_secret"] = authOptions.ClientSecret
            };

            if (!string.IsNullOrWhiteSpace(authOptions.Scope))
            {
                form["scope"] = authOptions.Scope;
            }

            if (!string.IsNullOrWhiteSpace(authOptions.Audience))
            {
                form["audience"] = authOptions.Audience;
            }

            using var req = new HttpRequestMessage(HttpMethod.Post, authOptions.TokenUrl)
            {
                Content = new FormUrlEncodedContent(form)
            };

            using var resp = await client.SendAsync(req);
            var body = await resp.Content.ReadAsStringAsync();

            resp.IsSuccessStatusCode.Should().BeTrue($"token request failed: {(int)resp.StatusCode} {body}");

            // Expect { "access_token": "...", "token_type": "Bearer", ... }
            using var json = JsonHelpers.Parse(body);
            var token = JsonHelpers.GetString(json.RootElement, "access_token");
            token.Should().NotBeNullOrWhiteSpace("token response must contain access_token");

            ctx.AccessToken = token;
        }
    }
}
