using Common.Api.BddTests.Support.Helpers;
using Common.Api.BddTests.Support.Services;
using FluentAssertions;
using Reqnroll;
namespace Common.Api.BddTests.Steps
{
    [Binding]
    public sealed class JsonSteps(ApiTestContext ctx)
    {
        [Then(@"the response error code ""(.*)"" should be ""(.*)""")]
        public void ThenTheResponseErrorCodeShouldBe(string jsonPathOrProperty, string expectedErrorCode)
        {
            ctx.ResponseBody.Should().NotBeNullOrWhiteSpace();

            using var doc = JsonHelpers.Parse(ctx.ResponseBody);

            // Your feature uses: the response error code "code" should be "FIL-VAL-E400"
            // Treat the first arg as a JSON path (works for "code" and also deeper paths if needed).
            var actual = JsonHelpers.GetString(doc.RootElement, jsonPathOrProperty);

            Assert.IsFalse(
                string.IsNullOrWhiteSpace(actual),
                $"Expected '{jsonPathOrProperty}' to exist. Body: {ctx.ResponseBody}"
            );

            Assert.AreEqual(expectedErrorCode, actual);
        }

        [Then(@"the response should include field ""(.*)""")]
        public void ThenTheResponseShouldIncludeField(string responseField)
        {
            Assert.IsFalse(
                string.IsNullOrWhiteSpace(ctx.ResponseBody),
                "Response body must not be null or whitespace"
            );

            using var doc = JsonHelpers.Parse(ctx.ResponseBody);

            var hasErrors = doc.RootElement.TryGetProperty("errors", out var errors);

            Assert.IsTrue(
                hasErrors,
                $"Expected ProblemDetails.errors to exist. Body: {ctx.ResponseBody}"
            );

            Assert.AreEqual(
                System.Text.Json.JsonValueKind.Object,
                errors.ValueKind,
                $"Expected ProblemDetails.errors to be an object. Body: {ctx.ResponseBody}"
            );

            var keys = errors.EnumerateObject().Select(p => p.Name).ToList();
            var hasKey = keys.Contains(responseField);

            Assert.IsTrue(
                hasKey,
                $"Expected ProblemDetails.errors to include key '{responseField}'. " +
                $"Available keys: {string.Join(", ", keys)}. " +
                $"Body: {ctx.ResponseBody}"
            );
        }
    }
}
