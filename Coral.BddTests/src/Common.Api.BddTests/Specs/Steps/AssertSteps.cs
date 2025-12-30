using Common.Api.BddTests.TestInfrastructure.Helpers;
using Common.Api.BddTests.TestInfrastructure.Services;
using FluentAssertions;
using Reqnroll;
using System.Net;

namespace Common.Api.BddTests.Specs.Steps
{
    [Binding]
    public sealed class AssertSteps(ApiTestContext ctx)
    {

        [Then(@"the response status should be (\d+)")]
        public void ThenStatusShouldBe(int statusCode)
        {
            ctx.Response.Should().NotBeNull();
            ctx.Response!.StatusCode.Should().Be((HttpStatusCode)statusCode, ctx.ResponseBody);
        }

        [Then(@"the response json should contain ""(.*)""")]
        public void ThenJsonShouldContain(string jsonPath)
        {
            ctx.ResponseBody.Should().NotBeNullOrWhiteSpace();
            using var doc = JsonHelpers.Parse(ctx.ResponseBody);
            var value = JsonHelpers.GetString(doc.RootElement, jsonPath);
            value.Should().NotBeNull($"Expected JSON path '{jsonPath}' to exist. Body: {ctx.ResponseBody}");
        }

        [Then(@"I store ""(.*)"" as LastCreatedId")]
        public void ThenIStoreAsLastCreatedId(string jsonPath)
        {
            ctx.ResponseBody.Should().NotBeNullOrWhiteSpace();
            using var doc = JsonHelpers.Parse(ctx.ResponseBody);
            var value = JsonHelpers.GetString(doc.RootElement, jsonPath);
            value.Should().NotBeNullOrWhiteSpace($"Cannot store id from '{jsonPath}'. Body: {ctx.ResponseBody}");
            ctx.LastCreatedId = value;
        }
    }
}
