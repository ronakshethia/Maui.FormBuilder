using Maui.FormBuilder.Core.ViewModels;
using Microsoft.Maui.Controls;

namespace Maui.FormBuilder.Rendering.Renderers;

/// <summary>Renders a single-line text input (type = "text").</summary>
public sealed class TextRenderer : BaseFieldRenderer
{
    public override View Render(FieldViewModel vm)
    {
        var entry = new Entry
        {
            Placeholder  = vm.Placeholder ?? vm.Label,
            BindingContext = vm
        };

        entry.SetBinding(Entry.TextProperty,
            new Binding(nameof(FieldViewModel.Value), BindingMode.TwoWay));

        return WrapWithLabel(vm, entry);
    }
}
