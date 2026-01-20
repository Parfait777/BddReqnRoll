using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Common.Api.BddTests.Support.Helpers
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

        public static void EnsurePathContainerExists(JsonNode root, string fieldPath)
        {
            var segments = ParsePath(fieldPath);

            for (int i = 0; i < segments.Count - 1; i++)
            {
                var seg = segments[i];
                var next = segments[i + 1];

                if (seg.Kind == PathSegmentKind.Property)
                {
                    var obj = EnsureObject(root, segments.Take(i).ToList());
                    var prop = seg.PropertyName!;

                    if (obj[prop] is null)
                    {
                        obj[prop] = next.Kind == PathSegmentKind.ArrayIndex ? new JsonArray() : new JsonObject();
                    }
                    else
                    {
                        if (next.Kind == PathSegmentKind.ArrayIndex && obj[prop] is not JsonArray)
                            obj[prop] = new JsonArray();
                        if (next.Kind != PathSegmentKind.ArrayIndex && obj[prop] is not JsonObject)
                            obj[prop] = new JsonObject();
                    }
                }
                else
                {
                    var arr = EnsureArray(root, segments.Take(i).ToList());
                    EnsureArraySize(arr, seg.Index!.Value + 1);

                    if (arr[seg.Index!.Value] is null)
                    {
                        arr[seg.Index!.Value] = next.Kind == PathSegmentKind.ArrayIndex ? new JsonArray() : new JsonObject();
                    }
                    else
                    {
                        if (next.Kind == PathSegmentKind.ArrayIndex && arr[seg.Index!.Value] is not JsonArray)
                            arr[seg.Index!.Value] = new JsonArray();
                        if (next.Kind != PathSegmentKind.ArrayIndex && arr[seg.Index!.Value] is not JsonObject)
                            arr[seg.Index!.Value] = new JsonObject();
                    }
                }
            }
        }

        public static JsonNode? ParseValueToken(string raw)
        {
            var token = raw.Trim();

            if (string.Equals(token, "null", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (string.Equals(token, "TODAY+1", StringComparison.OrdinalIgnoreCase))
            {
                var future = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);
                return JsonValue.Create(future.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            }

            if (string.Equals(token, "true", StringComparison.OrdinalIgnoreCase))
            {
                return JsonValue.Create(true);
            }

            if (string.Equals(token, "false", StringComparison.OrdinalIgnoreCase))
            {
                return JsonValue.Create(false);
            }

            if (token.Length >= 2 && token.StartsWith("\"", StringComparison.Ordinal) && token.EndsWith("\"", StringComparison.Ordinal))
            {
                var s = token.Substring(1, token.Length - 2);
                return JsonValue.Create(s);
            }

            if (decimal.TryParse(token, NumberStyles.Number, CultureInfo.InvariantCulture, out var dec))
            {
                if (long.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out var lng))
                    return JsonValue.Create(lng);

                return JsonValue.Create(dec);
            }

            return JsonValue.Create(token);
        }

        public static void SetJsonValue(JsonNode root, string fieldPath, JsonNode? value)
        {
            var segments = ParsePath(fieldPath);
            var leaf = segments.Last();

            var parentSegments = segments.Take(segments.Count - 1).ToList();

            if (leaf.Kind == PathSegmentKind.Property)
            {
                var parentObj = EnsureObject(root, parentSegments);
                parentObj[leaf.PropertyName!] = value;
                return;
            }

            var parentArr = EnsureArray(root, parentSegments);
            EnsureArraySize(parentArr, leaf.Index!.Value + 1);
            parentArr[leaf.Index!.Value] = value;
        }

        private static JsonObject EnsureObject(JsonNode root, System.Collections.Generic.List<PathSegment> pathToObject)
        {
            JsonNode current = root;

            foreach (var seg in pathToObject)
            {
                if (seg.Kind == PathSegmentKind.Property)
                {
                    var obj = current as JsonObject
                              ?? throw new InvalidOperationException($"Expected object while navigating '{seg.PropertyName}'.");

                    obj[seg.PropertyName!] ??= new JsonObject();
                    current = obj[seg.PropertyName!]!;
                }
                else
                {
                    var arr = current as JsonArray
                              ?? throw new InvalidOperationException($"Expected array while navigating index [{seg.Index}].");

                    EnsureArraySize(arr, seg.Index!.Value + 1);
                    arr[seg.Index!.Value] ??= new JsonObject();
                    current = arr[seg.Index!.Value]!;
                }
            }

            return current as JsonObject ?? throw new InvalidOperationException("Expected JsonObject at target.");
        }

        private static JsonArray EnsureArray(JsonNode root, System.Collections.Generic.List<PathSegment> pathToArray)
        {
            JsonNode current = root;

            foreach (var seg in pathToArray)
            {
                if (seg.Kind == PathSegmentKind.Property)
                {
                    var obj = current as JsonObject
                              ?? throw new InvalidOperationException($"Expected object while navigating '{seg.PropertyName}'.");

                    obj[seg.PropertyName!] ??= new JsonArray();
                    current = obj[seg.PropertyName!]!;
                }
                else
                {
                    var arr = current as JsonArray
                              ?? throw new InvalidOperationException($"Expected array while navigating index [{seg.Index}].");

                    EnsureArraySize(arr, seg.Index!.Value + 1);
                    arr[seg.Index!.Value] ??= new JsonArray();
                    current = arr[seg.Index!.Value]!;
                }
            }

            return current as JsonArray ?? throw new InvalidOperationException("Expected JsonArray at target.");
        }

        private static void EnsureArraySize(JsonArray arr, int size)
        {
            while (arr.Count < size)
            {
                arr.Add(null);
            }
        }

        

        private static System.Collections.Generic.List<PathSegment> ParsePath(string fieldPath)
        {
            var segments = new System.Collections.Generic.List<PathSegment>();
            var parts = fieldPath.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (var part in parts)
            {
                var remaining = part;

                while (true)
                {
                    var open = remaining.IndexOf('[', StringComparison.Ordinal);
                    if (open < 0)
                    {
                        segments.Add(PathSegment.Property(remaining));
                        break;
                    }

                    var prop = remaining.Substring(0, open);
                    if (!string.IsNullOrWhiteSpace(prop))
                        segments.Add(PathSegment.Property(prop));

                    var close = remaining.IndexOf(']', open + 1);
                    if (close < 0)
                        throw new InvalidOperationException($"Invalid path segment '{part}': missing ']'. Path: {fieldPath}");

                    var indexText = remaining.Substring(open + 1, close - open - 1);
                    if (!int.TryParse(indexText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var index))
                        throw new InvalidOperationException($"Invalid array index '{indexText}' in path '{fieldPath}'.");

                    segments.Add(PathSegment.ArrayIndex(index));

                    remaining = remaining.Substring(close + 1);
                    if (string.IsNullOrWhiteSpace(remaining))
                        break;
                }
            }

            return segments;
        }

        private enum PathSegmentKind { Property, ArrayIndex }

        private sealed class PathSegment
        {
            public PathSegmentKind Kind { get; private init; }
            public string? PropertyName { get; private init; }
            public int? Index { get; private init; }

            public static PathSegment Property(string name) => new() { Kind = PathSegmentKind.Property, PropertyName = name };
            public static PathSegment ArrayIndex(int index) => new() { Kind = PathSegmentKind.ArrayIndex, Index = index };
        }
    }
}
