using System.Text.Json;

namespace Common;

public static class JsonEx
{
    public static JsonDocument Parse(string json) =>
        JsonDocument.Parse(json);

    public static bool HasProperty(JsonElement root, string name) =>
        root.TryGetProperty(name, out _);
}
