using Common.Api.BddTests.Support.Helpers;
using Common.Api.BddTests.Support.Services.Interfaces;
using FluentAssertions;
using Reqnroll;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Common.Api.BddTests.Steps
{
    [Binding]
    public sealed class HttpSteps(ScenarioContext scenarioContext, IApiSender apiSender)
    {
        private const string RequestJsonKey = nameof(RequestJsonKey);

        [Given(@"I load request template ""(.*)""")]
        public void GivenILoadRequestTemplate(string templatePath)
        {
            var fullPath = ResolveTemplatePath(templatePath);

            var jsonText = File.ReadAllText(fullPath);
            var node = JsonNode.Parse(jsonText);

            Assert.IsNotNull(node, $"template '{templatePath}' must parse to JSON");

            ctxItems()[RequestJsonKey] = node!;
        }

        [Given(@"I ensure any required parent objects/arrays exist for ""(.*)""")]
        public void GivenIEnsureParentsExist(string fieldPath)
        {
            var root = GetRequestJson();
            JsonHelpers.EnsurePathContainerExists(root, fieldPath);
        }

        [Given(@"I set json field ""(.*)"" to (.*)")]
        public void GivenISetJsonField(string fieldPath, string rawValueToken)
        {
            var root = GetRequestJson();
            JsonHelpers.EnsurePathContainerExists(root, fieldPath);

            var valueNode = JsonHelpers.ParseValueToken(rawValueToken);
            JsonHelpers.SetJsonValue(root, fieldPath, valueNode);
        }

        [When(@"I POST ""(.*)"" using the current request json")]
        public async Task WhenIPostUsingCurrentRequestJson(string path)
        {
            var root = GetRequestJson();
            var json = root.ToJsonString(new JsonSerializerOptions { WriteIndented = false });
            await apiSender.SendAsync(HttpMethod.Post, path, json).ConfigureAwait(false);
        }

        private JsonNode GetRequestJson()
        {
            ctxItems().Should().ContainKey(RequestJsonKey, "request template must be loaded before patching");
            return (JsonNode)ctxItems()[RequestJsonKey];
        }

        private IDictionary<string, object> ctxItems()
        {
            Assert.IsNotNull(scenarioContext, "ScenarioContext must be available for request payload storage");
            return scenarioContext!;
        }

        private static string ResolveTemplatePath(string templatePath)
        {
            // Expecting paths like "/api/v1/filings.json"
            var trimmed = templatePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);

            // 1) Output directory (Copy to Output Directory)
            var inOutput = Path.Combine(AppContext.BaseDirectory, trimmed);
            if (File.Exists(inOutput))
            {
                return inOutput;
            }

            // 2) Working directory
            if (File.Exists(trimmed))
            {
                return trimmed;
            }

            // 3) Walk up to repo root (best-effort)
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            for (var i = 0; i < 6 && dir != null; i++)
            {
                var candidate = Path.Combine(dir.FullName, trimmed);
                if (File.Exists(candidate))
                {
                    return candidate;
                }

                dir = dir.Parent;
            }

            throw new FileNotFoundException($"Could not find template file '{templatePath}'. Tried '{inOutput}' and repo-root guesses.");
        }

        [When(@"I GET ""(.*)""")]
        public async Task WhenIGetAsync(string path) =>
            await apiSender.SendAsync(HttpMethod.Get, path, jsonBody: null).ConfigureAwait(false);

        [When(@"I DELETE ""(.*)""")]
        public async Task WhenIDeleteAsync(string path) =>
            await apiSender.SendAsync(HttpMethod.Delete, path, jsonBody: null).ConfigureAwait(false);

        [When(@"I POST ""(.*)"" with json")]
        public async Task WhenIPostWithJsonAsync(string path, string multilineJson) =>
            await apiSender.SendAsync(HttpMethod.Post, path, multilineJson).ConfigureAwait(false);

        [When(@"I PUT ""(.*)"" with json")]
        public async Task WhenIPutWithJsonAsync(string path, string multilineJson) =>
            await apiSender.SendAsync(HttpMethod.Put, path, multilineJson).ConfigureAwait(false);

    }
}
