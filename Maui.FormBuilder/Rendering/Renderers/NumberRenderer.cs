using Maui.FormBuilder.Core.ViewModels;
using Microsoft.Maui.Controls;

namespace Maui.FormBuilder.Rendering.Renderers;

/// <summary>Renders a numeric input with the numeric keyboard (type = "number").</summary>
public sealed class NumberRenderer : BaseFieldRenderer
{
    public override View Render(FieldViewModel vm)
    {
        var entry = new Entry
        {
            Placeholder    = vm.Placeholder ?? vm.Label,
            Keyboard       = Keyboard.Numeric,
            BindingContext = vm
        };

        entry.SetBinding(Entry.TextProperty,
            new Binding(nameof(FieldViewModel.Value), BindingMode.TwoWay));

        return WrapWithLabel(vm, entry);
    }
}
