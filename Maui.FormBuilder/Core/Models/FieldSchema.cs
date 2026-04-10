using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Maui.FormBuilder.Core.Models;

/// <summary>
/// Describes a single form field parsed from JSON.
/// <para>
/// <b>type</b> — maps to a registered renderer (e.g. "text", "dropdown", "date").
/// </para>
/// </summary>
public sealed class FieldSchema
{
    /// <summary>Renderer type key (e.g. "text", "email", "dropdown").</summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>Unique identifier used as the form data key on submit.</summary>
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    /// <summary>Human-readable field label displayed above the control.</summary>
    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;

    /// <summary>Whether the field is required (triggers RequiredValidator on submit).</summary>
    [JsonPropertyName("required")]
    public bool Required { get; set; }

    /// <summary>Placeholder hint text shown inside the control (where applicable).</summary>
    [JsonPropertyName("placeholder")]
    public string? Placeholder { get; set; }

    /// <summary>Pre-filled value. System.Text.Json will deserialize this as a <c>JsonElement</c> at runtime.</summary>
    [JsonPropertyName("defaultValue")]
    public object? DefaultValue { get; set; }

    /// <summary>
    /// Arbitrary key/value pairs passed through to the renderer.
    /// Values are <c>JsonElement</c> at runtime — use <c>FieldPropertiesExtensions</c> for typed access.
    /// </summary>
    [JsonPropertyName("properties")]
    public Dictionary<string, object> Properties { get; set; } = [];

    /// <summary>Named validators to apply (e.g. "required"). Extensible.</summary>
    [JsonPropertyName("validators")]
    public List<string> Validators { get; set; } = [];
}
