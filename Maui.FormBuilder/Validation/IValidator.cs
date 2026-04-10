using Maui.FormBuilder.Core.ViewModels;

namespace Maui.FormBuilder.Validation;

/// <summary>
/// Contract for a field-level validator.
/// Implement this interface to create custom validation rules.
/// </summary>
public interface IValidator
{
    /// <summary>Returns <c>true</c> if <paramref name="field"/> passes this rule.</summary>
    bool Validate(FieldViewModel field);

    /// <summary>Human-readable error message shown when <see cref="Validate"/> returns <c>false</c>.</summary>
    string ErrorMessage(FieldViewModel field);
}
