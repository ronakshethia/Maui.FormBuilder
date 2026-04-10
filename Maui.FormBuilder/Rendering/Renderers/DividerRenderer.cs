using Maui.FormBuilder.Core.ViewModels;
using Maui.FormBuilder.Extensions;
using Microsoft.Maui.Controls;

namespace Maui.FormBuilder.Rendering.Renderers;

/// <summary>
/// Renders a horizontal divider line (type = "divider").
/// Supports optional <c>properties.color</c> (default "#E0E0E0") and <c>properties.thickness</c> (default 1).
/// </summary>
public sealed class DividerRenderer : BaseFieldRenderer
{
    public override View Render(FieldViewModel vm)
    {
        string colorHex = vm.Properties.GetString("color", "#E0E0E0");
        double thickness = vm.Properties.GetDouble("thickness", 1);

        var box = new BoxView
        {
            HeightRequest  = thickness,
            Color          = Color.FromArgb(colorHex),
            HorizontalOptions = LayoutOptions.Fill,
            Margin         = new Thickness(0, 8),
            BindingContext = vm
        };

        box.SetBinding(View.IsVisibleProperty, nameof(FieldViewModel.IsVisible));

        return box;
    }
}
