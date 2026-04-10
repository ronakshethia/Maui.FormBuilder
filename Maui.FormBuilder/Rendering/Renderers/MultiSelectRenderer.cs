using System.Collections.Generic;
using System.Linq;
using Maui.FormBuilder.Core.ViewModels;
using Maui.FormBuilder.Extensions;
using Microsoft.Maui.Controls;

namespace Maui.FormBuilder.Rendering.Renderers;

/// <summary>
/// Renders a multi-select list using individual checkboxes (type = "multiselect").
/// JSON schema example:
/// <code>
/// { "type": "multiselect", "key": "skills", "label": "Skills",
///   "properties": { "options": ["C#", "MAUI", "Blazor", "Azure"] } }
/// </code>
/// Value: <c>string[]</c> of selected option strings.
/// </summary>
public sealed class MultiSelectRenderer : BaseFieldRenderer
{
    public override View Render(FieldViewModel vm)
    {
        List<string> options = vm.Properties.GetStringList("options");
        var selected         = new HashSet<string>();

        // Initialise from default value
        if (vm.Value is string[] defaultArr)
        {
            foreach (var s in defaultArr)
                selected.Add(s);
        }

        var container = new VerticalStackLayout { Spacing = 6 };

        foreach (var option in options)
        {
            var row = new HorizontalStackLayout { Spacing = 8 };

            var check = new CheckBox { IsChecked = selected.Contains(option) };

            check.CheckedChanged += (_, e) =>
            {
                if (e.Value)
                    selected.Add(option);
                else
                    selected.Remove(option);

                vm.Value = selected.ToArray();
            };

            // Sync back on external reset
            vm.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(FieldViewModel.Value))
                {
                    selected.Clear();
                    if (vm.Value is string[] arr)
                        foreach (var s in arr) selected.Add(s);

                    check.IsChecked = selected.Contains(option);
                }
            };

            row.Add(check);
            row.Add(new Label
            {
                Text            = option,
                VerticalOptions = LayoutOptions.Center,
                FontSize        = 14
            });

            container.Add(row);
        }

        return WrapWithLabel(vm, container);
    }
}
