using Maui.FormBuilder.Core.ViewModels;
using Microsoft.Maui.Controls;

namespace Maui.FormBuilder.Rendering.Renderers;

/// <summary>
/// Renders a toggle switch (type = "switch").
/// Value: <c>true</c> / <c>false</c> (bool).
/// Label is rendered inline to the right of the toggle.
/// </summary>
public sealed class SwitchRenderer : BaseFieldRenderer
{
    public override View Render(FieldViewModel vm)
    {
        var toggle = new Switch
        {
            IsToggled      = vm.Value is bool b && b,
            BindingContext = vm
        };

        toggle.Toggled += (_, e) => vm.Value = e.Value;

        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(FieldViewModel.Value))
                toggle.IsToggled = vm.Value is bool bv && bv;
        };

        var row = new HorizontalStackLayout
        {
            Spacing = 10,
            BindingContext = vm
        };
        row.SetBinding(View.IsVisibleProperty, nameof(FieldViewModel.IsVisible));

        row.Add(toggle);
        row.Add(new Label
        {
            Text            = vm.Required ? $"{vm.Label} *" : vm.Label,
            VerticalOptions = LayoutOptions.Center,
            FontSize        = 14
        });

        return row;
    }
}
