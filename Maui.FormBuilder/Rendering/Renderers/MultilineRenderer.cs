using Maui.FormBuilder.Core.ViewModels;
using Microsoft.Maui.Controls;

namespace Maui.FormBuilder.Rendering.Renderers;

/// <summary>Renders a multi-line text editor (type = "multiline").</summary>
public sealed class MultilineRenderer : BaseFieldRenderer
{
    public override View Render(FieldViewModel vm)
    {
        var editor = new Editor
        {
            Placeholder    = vm.Placeholder ?? vm.Label,
            AutoSize       = EditorAutoSizeOption.TextChanges,
            MinimumHeightRequest = 100,
            BindingContext = vm
        };

        editor.SetBinding(Editor.TextProperty,
            new Binding(nameof(FieldViewModel.Value), BindingMode.TwoWay));

        return WrapWithLabel(vm, editor);
    }
}
