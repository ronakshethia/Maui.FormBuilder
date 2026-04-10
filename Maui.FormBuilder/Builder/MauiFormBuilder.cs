using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Maui.FormBuilder.Core.Models;
using Maui.FormBuilder.Core.ViewModels;
using Maui.FormBuilder.Rendering;
using Maui.FormBuilder.Rendering.Renderers;
using Maui.FormBuilder.Validation;
using Microsoft.Maui.Controls;

namespace Maui.FormBuilder.Builder;

/// <summary>
/// Fluent builder that converts a JSON form schema into a ready-to-use MAUI <see cref="View"/>.
/// </summary>
/// <example>
/// <code>
/// var form = new MauiFormBuilder()
///     .LoadFromJson(json)
///     .RegisterRenderer("rating", vm => new RatingView { BindingContext = vm })
///     .OnSubmit(data => Console.WriteLine(string.Join(", ", data.Keys)))
///     .Build();
///
/// this.Content = form;
/// </code>
/// </example>
public sealed class MauiFormBuilder
{
    // ── Internal State ────────────────────────────────────────────────────────

    private readonly RendererRegistry                    _registry   = new();
    private readonly List<IValidator>                    _validators = [new RequiredValidator()];
    private          FormSchema?                         _schema;
    private          Action<Dictionary<string, object?>>? _onSubmit;
    private          Action<string, Dictionary<string, object?>>? _onAction;

    // ── Constructor ───────────────────────────────────────────────────────────

    public MauiFormBuilder()
    {
        RegisterDefaultRenderers();
    }

    // ── Public Fluent API ─────────────────────────────────────────────────────

    /// <summary>
    /// Parses a JSON string conforming to <see cref="FormSchema"/> into the builder.
    /// Call before <see cref="Build"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the JSON is invalid or empty.</exception>
    public MauiFormBuilder LoadFromJson(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);

        _schema = JsonSerializer.Deserialize<FormSchema>(json, JsonOptions.Default)
            ?? throw new InvalidOperationException(
                "Failed to deserialize the form schema. Ensure the JSON matches the FormSchema contract.");

        return this;
    }

    /// <summary>Registers a typed renderer for <paramref name="fieldType"/>.</summary>
    /// <remarks>Overwrites any existing registration for the same type.</remarks>
    public MauiFormBuilder RegisterRenderer(string fieldType, IFieldRenderer renderer)
    {
        _registry.Register(fieldType, renderer);
        return this;
    }

    /// <summary>
    /// Registers a lambda renderer for <paramref name="fieldType"/>.
    /// The lambda is wrapped in a <see cref="LambdaRenderer"/> internally.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.RegisterRenderer("rating", vm => new RatingView { BindingContext = vm });
    /// </code>
    /// </example>
    public MauiFormBuilder RegisterRenderer(string fieldType, Func<FieldViewModel, View> lambdaRenderer)
    {
        _registry.Register(fieldType, lambdaRenderer);
        return this;
    }

    /// <summary>
    /// Adds a custom validator applied to every field on submit.
    /// The built-in <see cref="RequiredValidator"/> is always active unless removed.
    /// </summary>
    public MauiFormBuilder AddValidator(IValidator validator)
    {
        _validators.Add(validator);
        return this;
    }

    /// <summary>
    /// Replaces the fallback renderer shown for unregistered field types.
    /// The default shows a styled amber warning label.
    /// </summary>
    public MauiFormBuilder SetFallbackRenderer(IFieldRenderer renderer)
    {
        _registry.SetFallbackRenderer(renderer);
        return this;
    }

    /// <summary>
    /// Registers a callback invoked when the form passes validation and the submit action fires.
    /// The dictionary contains all field values keyed by <c>FieldSchema.Key</c>.
    /// </summary>
    public MauiFormBuilder OnSubmit(Action<Dictionary<string, object?>> callback)
    {
        _onSubmit = callback;
        return this;
    }

    /// <summary>
    /// Registers a callback for custom action types not handled by built-in logic.
    /// Parameters: (actionKey, currentFormValues).
    /// </summary>
    public MauiFormBuilder OnAction(Action<string, Dictionary<string, object?>> callback)
    {
        _onAction = callback;
        return this;
    }

    /// <summary>
    /// Builds and returns the fully rendered form <see cref="View"/>.
    /// The returned view is a <see cref="ScrollView"/> containing all fields and action buttons.
    /// Set it directly as a <see cref="ContentPage.Content"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="LoadFromJson"/> has not been called.</exception>
    public View Build()
    {
        if (_schema is null)
            throw new InvalidOperationException(
                "Call LoadFromJson before Build.");

        var formVm = FormSchemaMapper.Map(_schema);
        return BuildFormView(formVm, _schema);
    }

    // ── Private Builder Logic ─────────────────────────────────────────────────

    private View BuildFormView(FormViewModel formVm, FormSchema schema)
    {
        var root = new VerticalStackLayout
        {
            Spacing = 18,
            Padding = new Thickness(20, 24)
        };

        // Form title
        if (!string.IsNullOrWhiteSpace(formVm.Title))
        {
            root.Add(new Label
            {
                Text           = formVm.Title,
                FontSize       = 26,
                FontAttributes = FontAttributes.Bold,
                Margin         = new Thickness(0, 0, 0, 6)
            });
        }

        // Fields
        foreach (var fieldVm in formVm.Fields)
        {
            var renderer = _registry.Resolve(fieldVm.Type);
            root.Add(renderer.Render(fieldVm));
        }

        // Action buttons
        if (schema.Actions.Count > 0)
        {
            var actionsRow = new FlexLayout
            {
                Wrap            = Microsoft.Maui.Layouts.FlexWrap.Wrap,
                JustifyContent  = Microsoft.Maui.Layouts.FlexJustify.Start,
                AlignItems      = Microsoft.Maui.Layouts.FlexAlignItems.Center
            };

            foreach (var action in schema.Actions)
            {
                var button = BuildActionButton(action, formVm);
                actionsRow.Add(button);
            }

            root.Add(actionsRow);
        }

        return new ScrollView { Content = root };
    }

    private Button BuildActionButton(ActionSchema action, FormViewModel formVm)
    {
        bool isPrimary = action.Type is "submit";

        var button = new Button
        {
            Text              = action.Label,
            Margin            = new Thickness(0, 0, 8, 0),
            Padding           = new Thickness(20, 10),
            BackgroundColor   = isPrimary ? Color.FromArgb("#6C63FF") : Color.FromArgb("#E0E0E0"),
            TextColor         = isPrimary ? Colors.White : Colors.Black,
            CornerRadius      = 8,
            FontAttributes    = FontAttributes.Bold
        };

        switch (action.Type)
        {
            case "submit":
                button.Clicked += (_, _) =>
                {
                    if (formVm.Validate(_validators))
                        _onSubmit?.Invoke(formVm.GetValues());
                };
                break;

            case "reset":
                button.Clicked += (_, _) => formVm.Reset();
                break;

            default:
                button.Clicked += (_, _) =>
                    _onAction?.Invoke(action.Key, formVm.GetValues());
                break;
        }

        return button;
    }

    // ── Default Renderer Registration ─────────────────────────────────────────

    private void RegisterDefaultRenderers()
    {
        // Text inputs
        _registry.Register("text",      new TextRenderer());
        _registry.Register("multiline", new MultilineRenderer());
        _registry.Register("number",    new NumberRenderer());
        _registry.Register("email",     new EmailRenderer());
        _registry.Register("password",  new PasswordRenderer());

        // Selection
        _registry.Register("dropdown",    new DropdownRenderer());
        _registry.Register("radio",       new RadioRenderer());
        _registry.Register("checkbox",    new CheckboxRenderer());
        _registry.Register("switch",      new SwitchRenderer());
        _registry.Register("multiselect", new MultiSelectRenderer());

        // Date & time
        _registry.Register("date", new DateRenderer());
        _registry.Register("time", new TimeRenderer());

        // Display
        _registry.Register("label",   new LabelRenderer());
        _registry.Register("heading", new HeadingRenderer());
        _registry.Register("divider", new DividerRenderer());
    }
}
