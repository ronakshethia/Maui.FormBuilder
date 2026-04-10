using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Maui.FormBuilder.Core.ViewModels;

/// <summary>
/// Observable view-model for a single form field.
/// Renderers bind directly to this — no manual UI syncing required.
/// </summary>
public class FieldViewModel : INotifyPropertyChanged
{
    // ── Identity ──────────────────────────────────────────────────────────────

    /// <summary>Unique key used as the data key on form submit.</summary>
    public string Key { get; init; } = string.Empty;

    /// <summary>Renderer type (e.g. "text", "dropdown"). Resolved via RendererRegistry.</summary>
    public string Type { get; init; } = string.Empty;

    /// <summary>Human-readable label displayed above the control.</summary>
    public string Label { get; init; } = string.Empty;

    /// <summary>Whether the field must be non-empty for the form to submit.</summary>
    public bool Required { get; init; }

    /// <summary>Optional placeholder hint (where applicable).</summary>
    public string? Placeholder { get; init; }

    /// <summary>Renderer-specific properties (options list, maxLength, min, max, etc.).</summary>
    public Dictionary<string, object> Properties { get; init; } = [];

    // ── Observable State ──────────────────────────────────────────────────────

    private object? _value;
    private bool _isVisible = true;
    private bool _isValid = true;
    private string _validationMessage = string.Empty;

    /// <summary>
    /// The current value of this field. Bind controls to this (TwoWay).
    /// Type depends on the field: string, bool, DateTime, TimeSpan, string[], etc.
    /// </summary>
    public object? Value
    {
        get => _value;
        set => SetProperty(ref _value, value);
    }

    /// <summary>Controls the visibility of the rendered field container.</summary>
    public bool IsVisible
    {
        get => _isVisible;
        set => SetProperty(ref _isVisible, value);
    }

    /// <summary>False when validation fails. Drives error label visibility.</summary>
    public bool IsValid
    {
        get => _isValid;
        set => SetProperty(ref _isValid, value);
    }

    /// <summary>Validation error message shown below the field when IsValid is false.</summary>
    public string ValidationMessage
    {
        get => _validationMessage;
        set => SetProperty(ref _validationMessage, value);
    }

    // ── INotifyPropertyChanged ────────────────────────────────────────────────

    public event PropertyChangedEventHandler? PropertyChanged;

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
