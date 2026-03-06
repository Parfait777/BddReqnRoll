using Common.TestHelpers.Enums;
using Common.TestHelpers.Interfaces;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Common.TestHelpers
{
    /// <summary>
    /// A helper class for working with JSON in the integration tests of the Coral Clean API. Provides methods for safely parsing JSON strings, retrieving values using simple JSON paths, and ensuring that certain paths exist in a JsonNode structure. This is particularly useful for handling dynamic JSON content in form submissions and responses without having to define rigid contract classes for every possible structure. The methods in this class allow for flexible manipulation of JSON data while avoiding common pitfalls like null reference exceptions when navigating nested structures.
    /// </summary>
    public static class JsonHelpers
    {
        /// <summary>
        /// Sets a value in a JSON string at the specified field path, creating any necessary intermediate objects or arrays along the way. The fieldPath is a dot-separated string that can include property names and array indices (e.g. "ReportingPeriod.ReportingPeriodFrom" or "Attachments[0].FileName"). The rawValueToken is a string representation of the value to set, which can be a JSON literal (null, true, false), a quoted string (e.g. "\"hello\""), or a number (e.g. "123" or "45.67"). This method will parse the rawValueToken into an appropriate JsonNode value and set it at the specified path in the JSON structure, returning the modified JSON as a string.
        /// </summary>
        /// <param name="jsonText">The original JSON string to modify. This should represent a JSON object (i.e. start with '{') for the fieldPath to be set correctly.</param>
        /// <param name="fieldPath">The dot-separated path to the field to set, which can include array indices in square brackets (e.g. "ReportingPeriod.ReportingPeriodFrom" or "Attachments[0].FileName"). This indicates where in the JSON structure the value should be set.</param>
        /// <param name="rawValueToken">The raw string token representing the value to set at the specified path. This can be a JSON literal (null, true, false), a quoted string (e.g. "\"hello\""), or a number (e.g. "123" or "45.67"). The method will parse this token into an appropriate JsonNode value before setting it in the JSON structure.</param>
        /// <returns>The modified JSON string with the new value set at the specified path. If the fieldPath does not exist in the original JSON, it will be created along with any necessary intermediate objects or arrays.</returns>
        public static string SetJsonField(string jsonText, string fieldPath, string rawValueToken)
        {
            JsonNode rootNode = JsonNode.Parse(jsonText)!;
            EnsurePathContainerExists(rootNode, fieldPath);

            JsonNode? valueNode = ParseValueToken(rawValueToken);
            SetJsonValue(rootNode, fieldPath, valueNode);

            return rootNode.ToJsonString();
        }

        /// <summary>
        /// Sets a value in a JSON string at the specified field path, creating any necessary intermediate objects or arrays along the way. The fieldPath is a dot-separated string that can include property names and array indices (e.g. "ReportingPeriod.ReportingPeriodFrom" or "Attachments[0].FileName"). The rawValueToken is a string representation of the value to set, which can be a JSON literal (null, true, false), a quoted string (e.g. "\"hello\""), or a number (e.g. "123" or "45.67"). This method will parse the rawValueToken into an appropriate JsonNode value and set it at the specified path in the JSON structure, returning the modified JSON as a string. The IDateTimeWrapper parameter is included to allow for any special handling of date/time values if needed during parsing, although in this implementation it is not utilized.
        /// </summary>
        /// <param name="jsonText"></param>
        /// <param name="fieldPath"></param>
        /// <param name="rawValueToken"></param>
        /// <param name="dateTimeWrapper"></param>
        /// <returns></returns>
        public static string SetJsonField(string jsonText, string fieldPath, string rawValueToken, IDateTimeWrapper dateTimeWrapper)
        {
            JsonNode rootNode = JsonNode.Parse(jsonText)!;
            EnsurePathContainerExists(rootNode, fieldPath);

            JsonNode? valueNode = ParseValueToken(rawValueToken, dateTimeWrapper);
            SetJsonValue(rootNode, fieldPath, valueNode);

            return rootNode.ToJsonString();
        }

        /// <summary>
        /// Parses a JSON string into a JsonDocument. If the input string is null, empty, or consists only of whitespace, an empty JSON object will be returned instead. This allows for safe parsing of optional JSON content without having to check for null or empty strings beforehand.
        /// </summary>
        /// <param name="json">The JSON string to parse, which may be null, empty, or whitespace. If so, an empty JSON object will be returned.</param>
        /// <returns>The parsed JsonDocument, or an empty JSON object if the input string is null, empty, or whitespace.</returns>
        public static JsonDocument Parse(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return JsonDocument.Parse("{}");
            }

            return JsonDocument.Parse(json);
        }

        /// <summary>
        /// Gets the string value at the specified JSON path from the given JsonElement. The jsonPath is a dot-separated string that indicates the path to the desired value within the JSON structure (e.g. "data.id" or "id"). This method will return null if any part of the path does not exist or if the final value is not a string, number, or boolean. For numbers and booleans, their string representation will be returned ("true", "false", or the numeric value as a string). For other JSON value kinds (objects, arrays), their raw JSON text will be returned.
        /// </summary>
        /// <param name="root">The JsonElement to search within.</param>
        /// <param name="jsonPath">The dot-separated JSON path to the desired value (e.g. "data.id" or "id").</param>
        /// <returns>The string value at the specified JSON path, or null if the path does not exist or if the value is not a string, number, or boolean.</returns>
        public static string? GetString(JsonElement root, string jsonPath)
        {
            // Very small JSON-path-like helper: "data.id" or "id"
            string[]? parts = jsonPath.Split('.', StringSplitOptions.RemoveEmptyEntries);
            JsonElement current = root;

            foreach (string p in parts)
            {
                if (current.ValueKind != JsonValueKind.Object || !current.TryGetProperty(p, out JsonElement next))
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

        /// <summary>
        /// Ensures that the path specified by fieldPath exists in the JSON document rooted at 'root', creating any necessary intermediate objects or arrays along the way. This method does not set any value at the target path; it only ensures that the structure is in place so that a value can be set there without encountering null reference errors. The fieldPath is a dot-separated string that can include property names and array indices (e.g. "ReportingPeriod.ReportingPeriodFrom" or "Attachments[0].FileName").
        /// </summary>
        /// <param name="root">The root JSON node to modify.</param>
        /// <param name="fieldPath">The dot-separated path to the field to ensure, which can include array indices in square brackets (e.g. "ReportingPeriod.ReportingPeriodFrom" or "Attachments[0].FileName").</param>
        private static void EnsurePathContainerExists(JsonNode root, string fieldPath)
        {
            List<PathSegment> segments = ParsePath(fieldPath);

            for (int i = 0; i < segments.Count - 1; i++)
            {
                PathSegment seg = segments[i];
                PathSegment next = segments[i + 1];

                if (seg.Kind == PathSegmentKind.Property)
                {
                    JsonObject obj = EnsureObject(root, segments.Take(i).ToList());
                    string prop = seg.PropertyName!;

                    JsonNode? currentChild = GetPropertyCaseInsensitive(obj, prop, out string actualPropName);

                    JsonNode desiredContainer = next.Kind == PathSegmentKind.ArrayIndex
                        ? new JsonArray()
                        : new JsonObject();

                    if (currentChild is null)
                    {
                        obj[actualPropName] = desiredContainer;
                    }
                    else
                    {
                        if (next.Kind == PathSegmentKind.ArrayIndex && currentChild is not JsonArray)
                        {
                            obj[actualPropName] = new JsonArray();
                        }

                        if (next.Kind != PathSegmentKind.ArrayIndex && currentChild is not JsonObject)
                        {
                            obj[actualPropName] = new JsonObject();
                        }
                    }
                }
                else
                {
                    JsonArray arr = EnsureArray(root, segments.Take(i).ToList());
                    EnsureArraySize(arr, seg.Index!.Value + 1);

                    JsonNode? currentChild = arr[seg.Index!.Value];

                    if (currentChild is null)
                    {
                        arr[seg.Index!.Value] = next.Kind == PathSegmentKind.ArrayIndex ? new JsonArray() : new JsonObject();
                    }
                    else
                    {
                        if (next.Kind == PathSegmentKind.ArrayIndex && currentChild is not JsonArray)
                        {
                            arr[seg.Index!.Value] = new JsonArray();
                        }

                        if (next.Kind != PathSegmentKind.ArrayIndex && currentChild is not JsonObject)
                        {
                            arr[seg.Index!.Value] = new JsonObject();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Parse a raw string token into an appropriate JsonNode value. Handles JSON literals (null, true, false), quoted strings, and numbers. If the token does not match any of these patterns, it will be treated as a string value.
        /// </summary>
        /// <param name="raw">The raw string token to parse, which may represent a JSON literal, a quoted string, or a number.</param>
        /// <returns>The parsed JsonNode value, or null if the token represents a JSON null literal.</returns>
        private static JsonNode? ParseValueToken(string raw, IDateTimeWrapper? dateTimeWrapper = null)
        {
            string token = raw.Trim();

            // Backward-compatible legacy tokens
            if (string.Equals(token, SpecialValueTokens.Null, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (string.Equals(token, SpecialValueTokens.True, StringComparison.OrdinalIgnoreCase))
            {
                return JsonValue.Create(true);
            }

            if (string.Equals(token, SpecialValueTokens.False, StringComparison.OrdinalIgnoreCase))
            {
                return JsonValue.Create(false);
            }

            // New typed token format: <type:value>
            if (TryParseTypedToken(token, dateTimeWrapper, out JsonNode? typedNode))
            {
                return typedNode;
            }

            // Existing behavior: quoted string
            if (token.Length >= 2 && token.StartsWith('"') && token.EndsWith('"'))
            {
                string s = token.Substring(1, token.Length - 2);
                return JsonValue.Create(s);
            }

            // Existing behavior: numbers
            if (decimal.TryParse(token, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal dec))
            {
                if (long.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out long lng))
                {
                    return JsonValue.Create(lng);
                }

                return JsonValue.Create(dec);
            }

            // Fallback: treat as string
            return JsonValue.Create(token);
        }

        private static bool TryParseTypedToken(string token, IDateTimeWrapper? dateTimeWrapper, out JsonNode? node)
        {
            node = null;

            // Must be wrapped like: <type:value>
            if (token.Length < 5 || token[0] != '<' || token[^1] != '>')
            {
                return false;
            }

            string inner = token.Substring(1, token.Length - 2); // remove < >
            int colonIndex = inner.IndexOf(':');
            if (colonIndex <= 0 || colonIndex == inner.Length - 1)
            {
                return false;
            }

            string type = inner[..colonIndex].Trim();
            string value = inner[(colonIndex + 1)..].Trim();

            switch (type.ToLowerInvariant())
            {
                case "boolean":
                    if (string.Equals(value, "true", StringComparison.OrdinalIgnoreCase))
                    {
                        node = JsonValue.Create(true);
                        return true;
                    }
                    if (string.Equals(value, "false", StringComparison.OrdinalIgnoreCase))
                    {
                        node = JsonValue.Create(false);
                        return true;
                    }
                    throw new InvalidOperationException($"Invalid boolean token '{token}'. Expected <boolean:true> or <boolean:false>.");

                case "string":
                    node = JsonValue.Create(value);
                    return true;

                case "integer":
                    if (!long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out long lng))
                    {
                        throw new InvalidOperationException($"Invalid integer token '{token}'. Example: <integer:100>.");
                    }
                    node = JsonValue.Create(lng);
                    return true;

                case "decimal":
                    if (!decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal dec))
                    {
                        throw new InvalidOperationException($"Invalid decimal token '{token}'. Example: <decimal:2.5>.");
                    }
                    node = JsonValue.Create(dec);
                    return true;

                case "date":
                    node = ParseDateToken(value, dateTimeWrapper);
                    return true;

                default:
                    return false; // Not a typed token we understand
            }
        }

        private static JsonNode ParseDateToken(string value, IDateTimeWrapper? dateTimeWrapper)
        {
            // <date:today>, <date:today:+1>, <date:today:-1>
            if (value.StartsWith("today", StringComparison.OrdinalIgnoreCase))
            {
                DateTime utcNow = dateTimeWrapper?.GetUtcNow() ?? DateTime.UtcNow;
                DateOnly baseDate = DateOnly.FromDateTime(utcNow);

                int offsetDays = 0;

                // value may be: "today" or "today:+1" or "today:-1"
                if (value.Length > "today".Length)
                {
                    // expecting "today:+N" or "today:-N"
                    if (value["today".Length] != ':')
                    {
                        throw new InvalidOperationException($"Invalid date token '<date:{value}>'. Expected <date:today> or <date:today:+N>/<date:today:-N>.");
                    }

                    string offsetPart = value[("today:".Length)..].Trim();
                    if (!int.TryParse(offsetPart, NumberStyles.Integer, CultureInfo.InvariantCulture, out offsetDays))
                    {
                        throw new InvalidOperationException($"Invalid date offset in token '<date:{value}>'. Example: <date:today:+1>.");
                    }
                }

                DateOnly result = baseDate.AddDays(offsetDays);

                // represent dates as ISO-8601 string in JSON
                return JsonValue.Create(result.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))!;
            }

            // <date:2025-01-01>
            if (!DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly date))
            {
                throw new InvalidOperationException($"Invalid date token '<date:{value}>'. Expected ISO format yyyy-MM-dd or 'today'.");
            }

            return JsonValue.Create(date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))!;
        }

        /// <summary>
        /// Sets the value at the specified field path in the JSON document, creating any necessary intermediate objects or arrays along the path.
        /// </summary>
        /// <param name="root">The root JSON node to modify.</param>
        /// <param name="fieldPath">The dot-separated path to the field to set, which can include array indices in square brackets (e.g. "ReportingPeriod.ReportingPeriodFrom" or "Attachments[0].FileName").</param>
        /// <param name="value">The value to set at the specified path, or null to set a JSON null.</param>
        private static void SetJsonValue(JsonNode root, string fieldPath, JsonNode? value)
        {
            List<PathSegment> segments = ParsePath(fieldPath);
            PathSegment leaf = segments.Last();

            List<PathSegment> parentSegments = segments.Take(segments.Count - 1).ToList();

            if (leaf.Kind == PathSegmentKind.Property)
            {
                JsonObject parentObj = EnsureObject(root, parentSegments);
                SetPropertyCaseInsensitive(parentObj, leaf.PropertyName!, value);

                return;
            }

            JsonArray parentArr = EnsureArray(root, parentSegments);
            EnsureArraySize(parentArr, leaf.Index!.Value + 1);
            parentArr[leaf.Index!.Value] = value;
        }

        private static JsonObject EnsureObject(JsonNode root, List<PathSegment> pathToObject)
        {
            JsonNode current = root;

            foreach (PathSegment seg in pathToObject)
            {
                if (seg.Kind == PathSegmentKind.Property)
                {
                    JsonObject obj = current as JsonObject
                        ?? throw new InvalidOperationException($"Expected object while navigating '{seg.PropertyName}'.");

                    JsonNode next = EnsurePropertyContainerCaseInsensitive(
                        obj,
                        seg.PropertyName!,
                        static () => new JsonObject());

                    current = next;
                }
                else
                {
                    JsonArray arr = current as JsonArray
                        ?? throw new InvalidOperationException($"Expected array while navigating index [{seg.Index}].");

                    EnsureArraySize(arr, seg.Index!.Value + 1);
                    arr[seg.Index!.Value] ??= new JsonObject();
                    current = arr[seg.Index!.Value]!;
                }
            }

            return current as JsonObject ?? throw new InvalidOperationException("Expected JsonObject at target.");
        }

        private static JsonArray EnsureArray(JsonNode root, List<PathSegment> pathToArray)
        {
            JsonNode current = root;

            foreach (PathSegment seg in pathToArray)
            {
                if (seg.Kind == PathSegmentKind.Property)
                {
                    JsonObject obj = current as JsonObject
                        ?? throw new InvalidOperationException($"Expected object while navigating '{seg.PropertyName}'.");

                    JsonNode next = EnsurePropertyContainerCaseInsensitive(
                        obj,
                        seg.PropertyName!,
                        static () => new JsonArray());

                    current = next;
                }
                else
                {
                    JsonArray arr = current as JsonArray
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

        private static List<PathSegment> ParsePath(string fieldPath)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(fieldPath);

            List<PathSegment> segments = new();
            string[] parts = fieldPath.Split(
                '.',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (string part in parts)
            {
                string remaining = part;

                while (true)
                {
                    int open = remaining.IndexOf('[', StringComparison.Ordinal);
                    if (open < 0)
                    {
                        if (!string.IsNullOrWhiteSpace(remaining))
                        {
                            segments.Add(PathSegment.Property(remaining));
                        }

                        break;
                    }

                    string prop = remaining[..open];
                    if (!string.IsNullOrWhiteSpace(prop))
                    {
                        segments.Add(PathSegment.Property(prop));
                    }

                    int close = remaining.IndexOf(']', open + 1);
                    if (close < 0)
                    {
                        throw new InvalidOperationException(
                            $"Invalid path segment '{part}': missing ']'. Path: {fieldPath}");
                    }

                    string indexText = remaining.Substring(open + 1, close - open - 1);
                    if (!int.TryParse(indexText, NumberStyles.Integer, CultureInfo.InvariantCulture, out int index))
                    {
                        throw new InvalidOperationException(
                            $"Invalid array index '{indexText}' in path '{fieldPath}'.");
                    }

                    segments.Add(PathSegment.ArrayIndex(index));

                    remaining = remaining[(close + 1)..];
                    if (string.IsNullOrWhiteSpace(remaining))
                    {
                        break;
                    }
                }
            }

            return segments;
        }

        private static bool TryGetPropertyNameCaseInsensitive(JsonObject obj, string desiredName, out string actualName)
        {
            // Exact match fast-path
            if (obj.ContainsKey(desiredName))
            {
                actualName = desiredName;
                return true;
            }

            // Case-insensitive scan
            foreach (KeyValuePair<string, JsonNode?> kvp in obj)
            {
                if (string.Equals(kvp.Key, desiredName, StringComparison.OrdinalIgnoreCase))
                {
                    actualName = kvp.Key;
                    return true;
                }
            }

            actualName = desiredName;
            return false;
        }

        private static JsonNode? GetPropertyCaseInsensitive(JsonObject obj, string desiredName, out string actualName)
        {
            TryGetPropertyNameCaseInsensitive(obj, desiredName, out actualName);
            return obj.TryGetPropertyValue(actualName, out JsonNode? node) ? node : null;
        }

        private static JsonNode EnsurePropertyContainerCaseInsensitive(JsonObject obj, string desiredName, Func<JsonNode> createIfMissing)
        {
            JsonNode? existing = GetPropertyCaseInsensitive(obj, desiredName, out string actualName);

            if (existing is null)
            {
                JsonNode created = createIfMissing();
                obj[actualName] = created; // actualName == desiredName if not found
                return created;
            }

            return existing;
        }

        private static void SetPropertyCaseInsensitive(JsonObject obj, string desiredName, JsonNode? value)
        {
            _ = GetPropertyCaseInsensitive(obj, desiredName, out string actualName);
            obj[actualName] = value;
        }

        private sealed class PathSegment
        {
            public PathSegmentKind Kind { get; private init; }

            public string? PropertyName { get; private init; }

            // Normalized version for case-insensitive lookup
            public string? NormalizedPropertyName { get; private init; }

            public int? Index { get; private init; }

            public static PathSegment Property(string name) => new()
            {
                Kind = PathSegmentKind.Property,
                PropertyName = name,
                NormalizedPropertyName = name.ToLowerInvariant()
            };

            public static PathSegment ArrayIndex(int index) => new()
            {
                Kind = PathSegmentKind.ArrayIndex,
                Index = index
            };
        }
    }


}
