using System;
using System.Collections.Generic;
using Maui.FormBuilder.Core.ViewModels;
using Maui.FormBuilder.Extensions;
using Microsoft.Maui.Controls;

namespace Maui.FormBuilder.Rendering.Renderers;

/// <summary>
/// Renders a mutually exclusive radio button group (type = "radio").
/// JSON schema example:
/// <code>
/// { "type": "radio", "key": "gender", "label": "Gender",
///   "properties": { "options": ["Male", "Female", "Non-binary"] } }
/// </code>
/// Value: the selected option string, or <c>null</c>.
/// </summary>
public sealed class RadioRenderer : BaseFieldRenderer
{
    public override View Render(FieldViewModel vm)
    {
        List<string> options = vm.Properties.GetStringList("options");

        // Unique group name ensures isolation when multiple radio fields exist on same form
        string groupName = $"radio_group_{vm.Key}_{Guid.NewGuid():N}";

        var radioStack = new VerticalStackLayout { Spacing = 6 };

        foreach (var option in options)
        {
            var radio = new RadioButton
            {
                Content    = option,
                GroupName  = groupName,
                Value      = option,
                IsChecked  = vm.Value?.ToString() == option
            };

            radio.CheckedChanged += (_, e) =>
            {
                if (e.Value)
                    vm.Value = option;
            };

            // Sync back when Value is reset externally
            vm.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(FieldViewModel.Value))
                    radio.IsChecked = vm.Value?.ToString() == option;
            };

            radioStack.Add(radio);
        }

        return WrapWithLabel(vm, radioStack);
    }
}
