using System;
using System.Collections.Generic;
using Maui.FormBuilder.Core.ViewModels;
using Maui.FormBuilder.Rendering.Renderers;
using Microsoft.Maui.Controls;

namespace Maui.FormBuilder.Rendering;

/// <summary>
/// Central registry mapping field type strings to <see cref="IFieldRenderer"/> implementations.
/// Uses a case-insensitive dictionary so "Text", "text", and "TEXT" resolve to the same renderer.
/// Falls back to <see cref="FallbackRenderer"/> for unknown types unless overridden.
/// </summary>
public sealed class RendererRegistry
{
    private readonly Dictionary<string, IFieldRenderer> _renderers =
        new(StringComparer.OrdinalIgnoreCase);

    private IFieldRenderer _fallback = new FallbackRenderer();

    // ── Registration ──────────────────────────────────────────────────────────

    /// <summary>Registers a typed renderer implementation for <paramref name="type"/>.</summary>
    /// <exception cref="ArgumentException">Thrown when type is null or whitespace.</exception>
    public void Register(string type, IFieldRenderer renderer)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(type);
        ArgumentNullException.ThrowIfNull(renderer);
        _renderers[type] = renderer;
    }

    /// <summary>
    /// Registers a lambda as a renderer for <paramref name="type"/>.
    /// The lambda is wrapped in a <see cref="LambdaRenderer"/> internally.
    /// </summary>
    /// <example>
    /// <code>registry.Register("rating", vm => new RatingView { BindingContext = vm });</code>
    /// </example>
    public void Register(string type, Func<FieldViewModel, View> lambdaRenderer)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(type);
        ArgumentNullException.ThrowIfNull(lambdaRenderer);
        _renderers[type] = new LambdaRenderer(lambdaRenderer);
    }

    /// <summary>
    /// Overrides the default fallback renderer shown for unrecognised field types.
    /// The default fallback shows a styled warning label.
    /// </summary>
    public void SetFallbackRenderer(IFieldRenderer fallback)
    {
        ArgumentNullException.ThrowIfNull(fallback);
        _fallback = fallback;
    }

    // ── Resolution ────────────────────────────────────────────────────────────

    /// <summary>
    /// Resolves the renderer for <paramref name="type"/>.
    /// Returns the fallback renderer when no match is found.
    /// </summary>
    public IFieldRenderer Resolve(string type)
        => _renderers.TryGetValue(type, out var renderer) ? renderer : _fallback;

    /// <summary>Returns <c>true</c> if an explicit renderer is registered for <paramref name="type"/>.</summary>
    public bool IsRegistered(string type) => _renderers.ContainsKey(type);
}
