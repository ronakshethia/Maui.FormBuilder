using Maui.FormBuilder.Core.ViewModels;

namespace Maui.FormBuilder.Validation;

/// <summary>
/// Validates that a required field has a non-null, non-empty value.
/// Non-required fields always pass.
/// </summary>
public sealed class RequiredValidator : IValidator
{
    public bool Validate(FieldViewModel field)
    {
        if (!field.Required)
            return true;

        return field.Value switch
        {
            null            => false,
            string s        => !string.IsNullOrWhiteSpace(s),
            string[] arr    => arr.Length > 0,
            _               => true
        };
    }

    public string ErrorMessage(FieldViewModel field)
        => $"{field.Label} is required.";
}
