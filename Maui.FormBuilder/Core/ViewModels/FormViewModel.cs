using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Maui.FormBuilder.Validation;

namespace Maui.FormBuilder.Core.ViewModels;

/// <summary>
/// Observable view-model for the entire form.
/// Holds the ordered collection of <see cref="FieldViewModel"/> instances
/// and orchestrates validation &amp; value collection.
/// </summary>
public sealed class FormViewModel : INotifyPropertyChanged
{
    private string _title = string.Empty;

    /// <summary>Form title sourced from <c>FormSchema.Title</c>.</summary>
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    /// <summary>Ordered list of field view-models to render.</summary>
    public ObservableCollection<FieldViewModel> Fields { get; } = [];

    // ── Value Collection ──────────────────────────────────────────────────────

    /// <summary>
    /// Returns a snapshot of all field values keyed by <c>FieldViewModel.Key</c>.
    /// Called by the builder on submit after validation passes.
    /// </summary>
    public Dictionary<string, object?> GetValues()
        => Fields.ToDictionary(f => f.Key, f => f.Value);

    // ── Validation ────────────────────────────────────────────────────────────

    /// <summary>
    /// Runs all <paramref name="validators"/> against every field.
    /// Updates <see cref="FieldViewModel.IsValid"/> and <see cref="FieldViewModel.ValidationMessage"/> reactively.
    /// </summary>
    /// <returns><c>true</c> when every field passes all validators.</returns>
    public bool Validate(IEnumerable<IValidator> validators)
    {
        bool allValid = true;

        foreach (var field in Fields)
        {
            // Reset previous state
            field.IsValid = true;
            field.ValidationMessage = string.Empty;

            foreach (var validator in validators)
            {
                if (!validator.Validate(field))
                {
                    field.IsValid = false;
                    field.ValidationMessage = validator.ErrorMessage(field);
                    allValid = false;
                    break; // show first failure per field
                }
            }
        }

        return allValid;
    }

    // ── Reset ─────────────────────────────────────────────────────────────────

    /// <summary>Clears all field values and resets validation state.</summary>
    public void Reset()
    {
        foreach (var field in Fields)
        {
            field.Value = null;
            field.IsValid = true;
            field.ValidationMessage = string.Empty;
        }
    }

    // ── INotifyPropertyChanged ────────────────────────────────────────────────

    public event PropertyChangedEventHandler? PropertyChanged;

    private bool SetProperty<T>(ref T backingField, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingField, value))
            return false;

        backingField = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}
