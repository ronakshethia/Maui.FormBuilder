using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Maui.FormBuilder.Core.Models;

/// <summary>
/// Describes a form action (button) at the bottom of the form.
/// Built-in types: <c>"submit"</c> and <c>"reset"</c>.
/// Custom types are dispatched via the <c>OnAction</c> callback on <c>MauiFormBuilder</c>.
/// </summary>
public sealed class ActionSchema
{
    /// <summary>Action type: "submit", "reset", or a custom identifier.</summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>Unique key for this action button.</summary>
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    /// <summary>Text displayed on the button.</summary>
    [JsonPropertyName("label")]
    public string Label { get; set; } = "Submit";

    /// <summary>Additional styling or behavior hints passed to the button builder.</summary>
    [JsonPropertyName("properties")]
    public Dictionary<string, object> Properties { get; set; } = [];
}
