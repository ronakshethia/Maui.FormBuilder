using Maui.FormBuilder.Core.ViewModels;
using Maui.FormBuilder.Extensions;
using Microsoft.Maui.Controls;

namespace Maui.FormBuilder.Rendering.Renderers;

/// <summary>
/// Renders a bold section heading (type = "heading").
/// Supports optional <c>properties.fontSize</c> (default 20) and <c>properties.color</c>.
/// </summary>
public sealed class HeadingRenderer : BaseFieldRenderer
{
    public override View Render(FieldViewModel vm)
    {
        double fontSize = vm.Properties.GetDouble("fontSize", 20);
        string colorHex = vm.Properties.GetString("color", "#1A1A2E");

        var label = new Label
        {
            Text            = vm.Label,
            FontSize        = fontSize,
            FontAttributes  = FontAttributes.Bold,
            TextColor       = Color.FromArgb(colorHex),
            Margin          = new Thickness(0, 8, 0, 4),
            BindingContext  = vm
        };

        label.SetBinding(View.IsVisibleProperty, nameof(FieldViewModel.IsVisible));

        return label;
    }
}
