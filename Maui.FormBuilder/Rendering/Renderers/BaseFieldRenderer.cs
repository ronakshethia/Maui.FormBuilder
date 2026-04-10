using Maui.FormBuilder.Converters;
using Maui.FormBuilder.Core.ViewModels;
using Microsoft.Maui.Controls;

namespace Maui.FormBuilder.Rendering.Renderers;

/// <summary>
/// Base class for all built-in renderers.
/// Provides the shared <see cref="WrapWithLabel"/> helper that:
/// <list type="bullet">
///   <item>Binds container visibility to <see cref="FieldViewModel.IsVisible"/>.</item>
///   <item>Renders the field label (with asterisk when required).</item>
///   <item>Slots in the control view.</item>
///   <item>Appends a reactive validation error label.</item>
/// </list>
/// </summary>
public abstract class BaseFieldRenderer : IFieldRenderer
{
    /// <inheritdoc />
    public abstract View Render(FieldViewModel viewModel);

    // ── Shared Layout Builder ─────────────────────────────────────────────────

    /// <summary>
    /// Wraps <paramref name="control"/> in a labelled, validatable container
    /// with its <c>BindingContext</c> set to <paramref name="vm"/>.
    /// </summary>
    protected static View WrapWithLabel(FieldViewModel vm, View control)
    {
        var container = new VerticalStackLayout
        {
            Spacing = 4,
            BindingContext = vm
        };

        // Container visibility
        container.SetBinding(View.IsVisibleProperty, nameof(FieldViewModel.IsVisible));

        // Field label
        if (!string.IsNullOrWhiteSpace(vm.Label))
        {
            container.Add(new Label
            {
                Text          = vm.Required ? $"{vm.Label} *" : vm.Label,
                FontAttributes = FontAttributes.Bold,
                FontSize      = 14,
                Margin        = new Thickness(0, 0, 0, 2)
            });
        }

        // The rendered control
        container.Add(control);

        // Validation error label (visible only when IsValid == false)
        var errorLabel = new Label
        {
            TextColor    = Colors.OrangeRed,
            FontSize     = 11,
            Margin       = new Thickness(0, 2, 0, 0),
            BindingContext = vm
        };

        errorLabel.SetBinding(Label.TextProperty, nameof(FieldViewModel.ValidationMessage));
        errorLabel.SetBinding(
            Label.IsVisibleProperty,
            new Binding(
                nameof(FieldViewModel.IsValid),
                BindingMode.OneWay,
                InvertedBoolConverter.Instance));

        container.Add(errorLabel);

        return container;
    }
}
