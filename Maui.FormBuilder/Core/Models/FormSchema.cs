using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Maui.FormBuilder.Core.Models;

/// <summary>Top-level form schema deserialized from JSON.</summary>
public sealed class FormSchema
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("fields")]
    public List<FieldSchema> Fields { get; set; } = [];

    [JsonPropertyName("actions")]
    public List<ActionSchema> Actions { get; set; } = [];
}
