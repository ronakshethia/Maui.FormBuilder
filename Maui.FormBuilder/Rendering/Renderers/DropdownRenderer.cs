using System.Collections.Generic;
using Maui.FormBuilder.Core.ViewModels;
using Maui.FormBuilder.Extensions;
using Microsoft.Maui.Controls;

namespace Maui.FormBuilder.Rendering.Renderers;

/// <summary>
/// Renders a drop-down picker (type = "dropdown").
/// JSON schema example:
/// <code>
/// { "type": "dropdown", "key": "country", "label": "Country",
///   "properties": { "options": ["USA", "UK", "Canada"] } }
/// </code>
/// Value: the selected string, or <c>null</c> when nothing is selected.
/// </summary>
public sealed class DropdownRenderer : BaseFieldRenderer
{
    public override View Render(FieldViewModel vm)
    {
        List<string> options = vm.Properties.GetStringList("options");

        var picker = new Picker
        {
            Title        = vm.Placeholder ?? $"Select {vm.Label}",
            ItemsSource  = options,
            BindingContext = vm
        };

        // Pre-select if default value exists
        if (vm.Value is string selected && options.Contains(selected))
            picker.SelectedItem = selected;

        // Picker does not directly bind object-typed SelectedItem easily via SetBinding
        // Use event-driven sync to keep logic transparent
        picker.SelectedIndexChanged += (_, _) =>
            vm.Value = picker.SelectedItem?.ToString();

        // Sync back when Value is reset externally
        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(FieldViewModel.Value))
                picker.SelectedItem = vm.Value?.ToString();
        };

        return WrapWithLabel(vm, picker);
    }
}
