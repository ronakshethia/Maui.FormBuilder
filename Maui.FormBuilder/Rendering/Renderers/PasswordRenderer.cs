using Maui.FormBuilder.Core.ViewModels;
using Microsoft.Maui.Controls;

namespace Maui.FormBuilder.Rendering.Renderers;

/// <summary>Renders a masked password input (type = "password").</summary>
public sealed class PasswordRenderer : BaseFieldRenderer
{
    public override View Render(FieldViewModel vm)
    {
        var entry = new Entry
        {
            Placeholder    = vm.Placeholder ?? vm.Label,
            IsPassword     = true,
            BindingContext = vm
        };

        entry.SetBinding(Entry.TextProperty,
            new Binding(nameof(FieldViewModel.Value), BindingMode.TwoWay));

        return WrapWithLabel(vm, entry);
    }
}
