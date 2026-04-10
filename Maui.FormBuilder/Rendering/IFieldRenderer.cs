using Maui.FormBuilder.Core.ViewModels;
using Microsoft.Maui.Controls;

namespace Maui.FormBuilder.Rendering;

/// <summary>
/// Contract for all field renderers.
/// Implement this to support a new field type or override a built-in one.
/// </summary>
public interface IFieldRenderer
{
    /// <summary>
    /// Produces a MAUI <see cref="View"/> for the given <paramref name="viewModel"/>.
    /// The view must set its <c>BindingContext</c> so that TwoWay bindings to
    /// <see cref="FieldViewModel.Value"/> function correctly.
    /// </summary>
    View Render(FieldViewModel viewModel);
}
