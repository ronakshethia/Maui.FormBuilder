using System;
using Maui.FormBuilder.Core.ViewModels;
using Microsoft.Maui.Controls;

namespace Maui.FormBuilder.Rendering;

/// <summary>
/// Wraps a <c>Func&lt;FieldViewModel, View&gt;</c> lambda into the <see cref="IFieldRenderer"/> interface.
/// Created internally when callers use:
/// <code>
/// builder.RegisterRenderer("myType", vm => new MyView());
/// </code>
/// </summary>
internal sealed class LambdaRenderer : IFieldRenderer
{
    private readonly Func<FieldViewModel, View> _factory;

    internal LambdaRenderer(Func<FieldViewModel, View> factory)
        => _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    public View Render(FieldViewModel viewModel) => _factory(viewModel);
}
