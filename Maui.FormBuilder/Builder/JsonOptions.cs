using System.Text.Json;

namespace Maui.FormBuilder.Builder;

/// <summary>Shared <see cref="JsonSerializerOptions"/> for schema deserialization.</summary>
internal static class JsonOptions
{
    internal static readonly JsonSerializerOptions Default = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling         = JsonCommentHandling.Skip,
        AllowTrailingCommas         = true
    };
}
