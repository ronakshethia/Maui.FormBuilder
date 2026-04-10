using Maui.FormBuilder.Core.ViewModels;
using Maui.FormBuilder.Extensions;
using Microsoft.Maui.Controls;

namespace Maui.FormBuilder.Rendering.Renderers;

/// <summary>
/// Renders a read-only descriptive label (type = "label").
/// The label text comes from the <c>label</c> field (or optionally <c>properties.text</c>).
/// </summary>
public sealed class LabelRenderer : BaseFieldRenderer
{
    public override View Render(FieldViewModel vm)
    {
        var text = vm.Properties.GetString("text", vm.Label);

        var label = new Label
        {
            Text           = text,
            FontSize       = 14,
            TextColor      = Colors.Gray,
            BindingContext = vm
        };

        label.SetBinding(View.IsVisibleProperty, nameof(FieldViewModel.IsVisible));

        return label;
    }
}
