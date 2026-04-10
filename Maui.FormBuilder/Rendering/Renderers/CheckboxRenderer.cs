using Maui.FormBuilder.Core.ViewModels;
using Microsoft.Maui.Controls;

namespace Maui.FormBuilder.Rendering.Renderers;

/// <summary>
/// Renders a single checkbox (type = "checkbox").
/// Value: <c>true</c> / <c>false</c> (bool).
/// The label is rendered inline to the right of the checkbox.
/// </summary>
public sealed class CheckboxRenderer : BaseFieldRenderer
{
    public override View Render(FieldViewModel vm)
    {
        var check = new CheckBox
        {
            IsChecked      = vm.Value is bool b && b,
            BindingContext = vm
        };

        check.CheckedChanged += (_, e) => vm.Value = e.Value;

        // Sync back when Value is reset externally
        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(FieldViewModel.Value))
                check.IsChecked = vm.Value is bool bv && bv;
        };

        var row = new HorizontalStackLayout
        {
            Spacing        = 8,
            VerticalOptions = LayoutOptions.Center,
            BindingContext = vm
        };

        row.SetBinding(View.IsVisibleProperty, nameof(FieldViewModel.IsVisible));

        var inlineLabel = new Label
        {
            Text            = vm.Required ? $"{vm.Label} *" : vm.Label,
            VerticalOptions = LayoutOptions.Center,
            FontSize        = 14
        };

        var errorLabel = new Label
        {
            TextColor    = Colors.OrangeRed,
            FontSize     = 11,
            BindingContext = vm
        };
        errorLabel.SetBinding(Label.TextProperty, nameof(FieldViewModel.ValidationMessage));
        errorLabel.SetBinding(Label.IsVisibleProperty,
            new Binding(nameof(FieldViewModel.IsValid), BindingMode.OneWay,
                Converters.InvertedBoolConverter.Instance));

        row.Add(check);
        row.Add(inlineLabel);

        var outer = new VerticalStackLayout { Spacing = 2, BindingContext = vm };
        outer.SetBinding(View.IsVisibleProperty, nameof(FieldViewModel.IsVisible));
        outer.Add(row);
        outer.Add(errorLabel);

        return outer;
    }
}
