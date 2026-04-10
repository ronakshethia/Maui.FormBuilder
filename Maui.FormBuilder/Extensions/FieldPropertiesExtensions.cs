using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Maui.FormBuilder.Extensions;

/// <summary>
/// Typed accessors for <c>FieldViewModel.Properties</c>.
/// Handles both native .NET types and <see cref="JsonElement"/> values
/// that System.Text.Json produces when deserializing <c>Dictionary&lt;string, object&gt;</c>.
/// </summary>
public static class FieldPropertiesExtensions
{
    // ── Generic ───────────────────────────────────────────────────────────────

    /// <summary>Gets a typed value from properties, with a fallback default.</summary>
    public static T? Get<T>(
        this Dictionary<string, object> props,
        string key,
        T? defaultValue = default)
    {
        if (!props.TryGetValue(key, out var raw))
            return defaultValue;

        if (raw is T direct)
            return direct;

        if (raw is JsonElement element)
        {
            try { return element.Deserialize<T>(); }
            catch { return defaultValue; }
        }

        try { return (T)System.Convert.ChangeType(raw, typeof(T)); }
        catch { return defaultValue; }
    }

    // ── Convenience overloads ─────────────────────────────────────────────────

    /// <summary>Gets a string value.</summary>
    public static string GetString(
        this Dictionary<string, object> props,
        string key,
        string defaultValue = "")
    {
        if (!props.TryGetValue(key, out var raw))
            return defaultValue;

        return raw switch
        {
            string s                                             => s,
            JsonElement { ValueKind: JsonValueKind.String } el  => el.GetString() ?? defaultValue,
            _                                                    => raw?.ToString() ?? defaultValue
        };
    }

    /// <summary>Gets an integer value.</summary>
    public static int GetInt(
        this Dictionary<string, object> props,
        string key,
        int defaultValue = 0)
    {
        if (!props.TryGetValue(key, out var raw))
            return defaultValue;

        if (raw is int i) return i;

        if (raw is JsonElement { ValueKind: JsonValueKind.Number } el)
            return el.GetInt32();

        return int.TryParse(raw?.ToString(), out var parsed) ? parsed : defaultValue;
    }

    /// <summary>Gets a double value.</summary>
    public static double GetDouble(
        this Dictionary<string, object> props,
        string key,
        double defaultValue = 0.0)
    {
        if (!props.TryGetValue(key, out var raw))
            return defaultValue;

        if (raw is double d) return d;

        if (raw is JsonElement { ValueKind: JsonValueKind.Number } el)
            return el.GetDouble();

        return double.TryParse(raw?.ToString(), out var parsed) ? parsed : defaultValue;
    }

    /// <summary>Gets a boolean value.</summary>
    public static bool GetBool(
        this Dictionary<string, object> props,
        string key,
        bool defaultValue = false)
    {
        if (!props.TryGetValue(key, out var raw))
            return defaultValue;

        return raw switch
        {
            bool b                                              => b,
            JsonElement { ValueKind: JsonValueKind.True }       => true,
            JsonElement { ValueKind: JsonValueKind.False }      => false,
            _                                                   =>
                bool.TryParse(raw?.ToString(), out var p) ? p : defaultValue
        };
    }

    /// <summary>Gets a list of strings from a JSON array property (e.g. "options").</summary>
    public static List<string> GetStringList(
        this Dictionary<string, object> props,
        string key)
    {
        if (!props.TryGetValue(key, out var raw))
            return [];

        if (raw is JsonElement { ValueKind: JsonValueKind.Array } el)
            return [.. el.EnumerateArray().Select(e => e.GetString() ?? string.Empty)];

        if (raw is IEnumerable<string> list)
            return [.. list];

        return [];
    }
}
