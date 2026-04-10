using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Maui.FormBuilder.Converters;

/// <summary>
/// Logical NOT converter for <c>bool</c> bindings.
/// Used to show validation error labels when <c>IsValid == false</c>.
/// </summary>
public sealed class InvertedBoolConverter : IValueConverter
{
    /// <summary>Shared singleton instance — reuse to avoid allocations.</summary>
    public static readonly InvertedBoolConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b ? !b : value;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b ? !b : value;
}
