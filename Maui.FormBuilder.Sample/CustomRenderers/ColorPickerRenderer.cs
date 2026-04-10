using Maui.FormBuilder.Core.ViewModels;
using Maui.FormBuilder.Extensions;
using Maui.FormBuilder.Rendering;
using Maui.FormBuilder.Rendering.Renderers;
using Microsoft.Maui.Controls;

namespace Maui.FormBuilder.Sample.CustomRenderers;

/// <summary>
/// Bonus custom renderer: color swatch picker (type = "colorpicker").
/// Demonstrates how to implement <see cref="IFieldRenderer"/> as a typed class.
///
/// JSON schema usage:
/// <code>
/// {
///   "type": "colorpicker",
///   "key": "favouriteColor",
///   "label": "Favourite Color",
///   "properties": {
///     "colors": ["#FF6584", "#6C63FF", "#43D9AD", "#F4A261", "#2D6A4F"]
///   }
/// }
/// </code>
/// Value: the selected hex color string, e.g. "#6C63FF".
/// </summary>
public sealed class ColorPickerRenderer : BaseFieldRenderer
{
    public override View Render(FieldViewModel vm)
    {
        var colors = vm.Properties.GetStringList("colors");

        if (colors.Count == 0)
            colors = ["#FF6584", "#6C63FF", "#43D9AD", "#F4A261", "#E76F51"];

        var swatchRow = new HorizontalStackLayout { Spacing = 10 };
        Border? selectedBorder = null;

        foreach (var hex in colors)
        {
            var border = new Border
            {
                BackgroundColor   = Color.FromArgb(hex),
                StrokeShape       = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(24) },
                WidthRequest      = 40,
                HeightRequest     = 40,
                Padding           = new Thickness(0),
                Stroke            = Colors.Transparent
            };

            var tap = new TapGestureRecognizer();
            tap.Tapped += (_, _) =>
            {
                // Deselect all
                if (selectedBorder is not null)
                    selectedBorder.Stroke = Colors.Transparent;

                // Select tapped
                border.Stroke = Colors.Black;
                selectedBorder = border;
                vm.Value = hex;
            };

            border.GestureRecognizers.Add(tap);
            swatchRow.Add(border);
        }

        // Pre-select default value
        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName != nameof(FieldViewModel.Value)) return;
            // visual reset on external clear — simplified: just clear border
            if (vm.Value is null && selectedBorder is not null)
            {
                selectedBorder.Stroke = Colors.Transparent;
                selectedBorder        = null;
            }
        };

        return WrapWithLabel(vm, swatchRow);
    }
}
