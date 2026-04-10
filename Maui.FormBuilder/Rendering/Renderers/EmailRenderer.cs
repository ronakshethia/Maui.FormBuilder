using Maui.FormBuilder.Core.ViewModels;
using Microsoft.Maui.Controls;

namespace Maui.FormBuilder.Rendering.Renderers;

/// <summary>Renders an email address input with the email keyboard (type = "email").</summary>
public sealed class EmailRenderer : BaseFieldRenderer
{
    public override View Render(FieldViewModel vm)
    {
        var entry = new Entry
        {
            Placeholder    = vm.Placeholder ?? vm.Label,
            Keyboard       = Keyboard.Email,
            BindingContext = vm
        };

        entry.SetBinding(Entry.TextProperty,
            new Binding(nameof(FieldViewModel.Value), BindingMode.TwoWay));

        return WrapWithLabel(vm, entry);
    }
}
