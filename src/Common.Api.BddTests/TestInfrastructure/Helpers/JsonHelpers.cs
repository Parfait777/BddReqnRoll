using System.Text.Json;

namespace Common.Api.BddTests.TestInfrastructure.Helpers
{
    public static class JsonHelpers
    {
        public static JsonDocument Parse(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return JsonDocument.Parse("{}");
            }

            return JsonDocument.Parse(json);
        }

        public static string? GetString(JsonElement root, string jsonPath)
        {
            // Very small JSON-path-like helper: "data.id" or "id"
            string[]? parts = jsonPath.Split('.', StringSplitOptions.RemoveEmptyEntries);
            JsonElement current = root;

            foreach (var p in parts)
            {
                if (current.ValueKind != JsonValueKind.Object || !current.TryGetProperty(p, out var next))
                {
                    return null;
                }
                current = next;
            }

            return current.ValueKind switch
            {
                JsonValueKind.String => current.GetString(),
                JsonValueKind.Number => current.GetRawText(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                _ => current.GetRawText()
            };
        }
    }
}
