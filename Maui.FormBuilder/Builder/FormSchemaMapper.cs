using Maui.FormBuilder.Core.Models;
using Maui.FormBuilder.Core.ViewModels;

namespace Maui.FormBuilder.Builder;

/// <summary>
/// Converts a <see cref="FormSchema"/> (JSON model) into a <see cref="FormViewModel"/> (MVVM).
/// This is the only place where JSON-layer types touch ViewModel-layer types.
/// </summary>
internal static class FormSchemaMapper
{
    /// <summary>
    /// Maps a <see cref="FormSchema"/> to a <see cref="FormViewModel"/>.
    /// Each <see cref="FieldSchema"/> becomes a <see cref="FieldViewModel"/>
    /// preserving all properties and the default value.
    /// </summary>
    internal static FormViewModel Map(FormSchema schema)
    {
        var formVm = new FormViewModel
        {
            Title = schema.Title
        };

        foreach (var field in schema.Fields)
        {
            var fieldVm = new FieldViewModel
            {
                Key         = field.Key,
                Type        = field.Type,
                Label       = field.Label,
                Required    = field.Required,
                Placeholder = field.Placeholder,
                Properties  = field.Properties,
                Value       = field.DefaultValue  // may be JsonElement; renderers handle this
            };

            formVm.Fields.Add(fieldVm);
        }

        return formVm;
    }
}
