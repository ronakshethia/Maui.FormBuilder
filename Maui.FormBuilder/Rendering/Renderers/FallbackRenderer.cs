using Maui.FormBuilder.Core.ViewModels;
using Microsoft.Maui.Controls;

namespace Maui.FormBuilder.Rendering.Renderers;

/// <summary>
/// Shown when no renderer is registered for a field type.
/// Displays a styled warning so developers immediately notice missing renderers.
/// </summary>
internal sealed class FallbackRenderer : IFieldRenderer
{
    public View Render(FieldViewModel viewModel)
    {
        return new Border
        {
            BackgroundColor = Color.FromArgb("#FFF3CD"),
            Stroke          = Color.FromArgb("#FFC107"),
            StrokeThickness = 1,
            StrokeShape     = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(6) },
            Padding         = new Thickness(10),
            Content         = new Label
            {
                Text           = $"⚠️  No renderer registered for type \"{viewModel.Type}\"  (key: {viewModel.Key})",
                TextColor      = Color.FromArgb("#856404"),
                FontSize       = 12,
                FontAttributes = FontAttributes.Italic
            }
        };
    }
}
