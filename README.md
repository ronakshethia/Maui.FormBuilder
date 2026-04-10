# Maui.FormBuilder

> **A JSON-driven dynamic form engine for .NET MAUI.**  
> Convert JSON schemas into native MAUI UI at runtime — with full MVVM support, a pluggable renderer registry, and built-in validation.

[![NuGet](https://img.shields.io/nuget/v/Maui.FormBuilder.svg)](https://www.nuget.org/packages/Maui.FormBuilder)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
![Platforms](https://img.shields.io/badge/platforms-Android%20%7C%20iOS%20%7C%20macOS%20%7C%20Windows-blue)

---

## ✨ Features

| Feature | Description |
|---|---|
| 📄 JSON-driven | Define entire forms in JSON — no XAML, no code |
| 🧱 MVVM | Every field is an observable `FieldViewModel`; UI syncs automatically |
| 🔌 Pluggable renderers | Register any `IFieldRenderer` or a one-liner lambda |
| ✅ Validation | Built-in `required` validation; extensible via `IValidator` |
| 🎛️ 15+ built-in types | text, email, password, number, dropdown, radio, checkbox, switch, multiselect, date, time, label, heading, divider, multiline |
| 📦 NuGet-ready | Multi-targeted: Android, iOS, macOS Catalyst, Windows |

---

## 📦 Installation

```bash
dotnet add package Maui.FormBuilder
```

---

## 🚀 Quick Start

### 1. Define your JSON schema

```json
{
  "title": "Contact Us",
  "fields": [
    { "type": "text",  "key": "name",    "label": "Full Name",  "required": true },
    { "type": "email", "key": "email",   "label": "Email",      "required": true },
    { "type": "multiline", "key": "message", "label": "Message" }
  ],
  "actions": [
    { "type": "submit", "key": "send",  "label": "Send Message" },
    { "type": "reset",  "key": "clear", "label": "Clear" }
  ]
}
```

### 2. Build and display the form

```csharp
// In your ContentPage code-behind
string json = await LoadJsonAsync("contact.json");

var form = new MauiFormBuilder()
    .LoadFromJson(json)
    .OnSubmit(data =>
    {
        string name  = data["name"]?.ToString()  ?? "";
        string email = data["email"]?.ToString() ?? "";
        Console.WriteLine($"From: {name} <{email}>");
    })
    .Build();

this.Content = form;
```

---

## 🗂️ Pipeline Architecture

```
JSON string
  └─► JsonSerializer.Deserialize<FormSchema>
        └─► FormSchemaMapper.Map(schema)
              └─► FormViewModel  ←─ ObservableCollection<FieldViewModel>
                    └─► RendererRegistry.Resolve(field.Type)
                          └─► IFieldRenderer.Render(fieldVm)  ─►  MAUI View
```

Each layer is independently testable and has no dependency on the layer above it.

---

## 📋 Field Types Reference

### Text Input

| JSON type | Control | Notes |
|---|---|---|
| `text` | `Entry` | Default keyboard |
| `email` | `Entry` | Email keyboard |
| `password` | `Entry` | `IsPassword = true` |
| `number` | `Entry` | Numeric keyboard |
| `multiline` | `Editor` | Auto-size, min 100px height |

```json
{ "type": "email", "key": "userEmail", "label": "Email", "required": true, "placeholder": "you@example.com" }
```

### Selection

| JSON type | Control | Value type |
|---|---|---|
| `dropdown` | `Picker` | `string` |
| `radio` | `RadioButton` group | `string` |
| `checkbox` | `CheckBox` | `bool` |
| `switch` | `Switch` | `bool` |
| `multiselect` | `CheckBox` list | `string[]` |

```json
{
  "type": "dropdown",
  "key": "country",
  "label": "Country",
  "properties": { "options": ["USA", "UK", "Canada", "Australia"] }
}
```

```json
{
  "type": "multiselect",
  "key": "skills",
  "label": "Your Skills",
  "properties": { "options": ["C#", "MAUI", "Blazor", "Azure"] }
}
```

### Date & Time

| JSON type | Control | Value type |
|---|---|---|
| `date` | `DatePicker` | `DateTime` |
| `time` | `TimePicker` | `TimeSpan` |

### Display / Layout

| JSON type | Element | Properties |
|---|---|---|
| `label` | `Label` | `text`, `color` |
| `heading` | `Label` (bold) | `fontSize` (default 20), `color` |
| `divider` | `BoxView` | `color` (default `#E0E0E0`), `thickness` (default 1) |

```json
{ "type": "heading", "key": "h1", "label": "Personal Details", "properties": { "fontSize": 22, "color": "#6C63FF" } }
{ "type": "divider", "key": "d1", "label": "", "properties": { "color": "#D0D0D0", "thickness": 1.5 } }
```

---

## 📐 Full JSON Schema Contract

```json
{
  "title":       "string (displayed as form heading)",
  "description": "string (optional)",
  "fields": [
    {
      "type":         "string  (renderer key, e.g. 'text')",
      "key":          "string  (used as submit dictionary key)",
      "label":        "string  (displayed label)",
      "required":     "bool    (triggers RequiredValidator on submit)",
      "placeholder":  "string  (optional hint text)",
      "defaultValue": "any     (pre-fills the field)",
      "properties":   "object  (renderer-specific options)",
      "validators":   ["string (named validators — extensible)"]
    }
  ],
  "actions": [
    {
      "type":  "submit | reset | custom",
      "key":   "string",
      "label": "string"
    }
  ]
}
```

---

## 🔌 Custom Renderers

### Lambda renderer (one-liner)

```csharp
var form = new MauiFormBuilder()
    .LoadFromJson(json)
    .RegisterRenderer("rating", vm =>
    {
        var stack = new HorizontalStackLayout { Spacing = 4 };
        // build your star-picker UI here
        // set vm.Value = selectedRating;
        return stack;
    })
    .Build();
```

The lambda is automatically wrapped in a `LambdaRenderer` internally. No boilerplate needed.

### Typed renderer class

```csharp
// 1. Implement IFieldRenderer (or extend BaseFieldRenderer for the WrapWithLabel helper)
public sealed class SliderRenderer : BaseFieldRenderer
{
    public override View Render(FieldViewModel vm)
    {
        double min = vm.Properties.GetDouble("min", 0);
        double max = vm.Properties.GetDouble("max", 100);

        var slider = new Slider { Minimum = min, Maximum = max };

        if (vm.Value is double d) slider.Value = d;

        slider.ValueChanged += (_, e) => vm.Value = e.NewValue;

        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(FieldViewModel.Value) && vm.Value is double v)
                slider.Value = v;
        };

        return WrapWithLabel(vm, slider);
    }
}

// 2. Register it
builder.RegisterRenderer("slider", new SliderRenderer());
```

```json
{ "type": "slider", "key": "volume", "label": "Volume", "properties": { "min": 0, "max": 100 } }
```

### Override built-in renderer

```csharp
// Replace the default text renderer with your styled version
builder.RegisterRenderer("text", new MyStyledTextRenderer());
```

### Custom fallback renderer

```csharp
builder.SetFallbackRenderer(new MyFallbackRenderer());
```

---

## ✅ Validation

### Built-in: `required`

Set `"required": true` in the field schema.  
On submit, `RequiredValidator` checks:
- `null` → ❌
- empty/whitespace string → ❌
- empty array → ❌
- everything else → ✅

Error messages appear below the field automatically (bound to `FieldViewModel.ValidationMessage`).

### Custom validators

```csharp
public sealed class EmailFormatValidator : IValidator
{
    public bool Validate(FieldViewModel field)
        => field.Type != "email"
           || field.Value is string s && s.Contains('@');

    public string ErrorMessage(FieldViewModel field)
        => "Please enter a valid email address.";
}

// Register it on the builder
builder.AddValidator(new EmailFormatValidator());
```

---

## 🧩 Dynamic Field Control

```csharp
// Access individual fields after building
var formVm = /* obtained from your own FormViewModel instance */;

// Conditionally show/hide a field
formVm.Fields.First(f => f.Key == "company").IsVisible = false;

// Set a value programmatically
formVm.Fields.First(f => f.Key == "country").Value = "Australia";
```

---

## 📁 Project Structure

```
Maui.FormBuilder/
├── Core/
│   ├── Models/          FormSchema, FieldSchema, ActionSchema
│   └── ViewModels/      FormViewModel, FieldViewModel
├── Rendering/
│   ├── IFieldRenderer.cs
│   ├── LambdaRenderer.cs
│   ├── RendererRegistry.cs
│   └── Renderers/       15 built-in renderers + BaseFieldRenderer + FallbackRenderer
├── Builder/
│   ├── MauiFormBuilder.cs   (public fluent API)
│   ├── FormSchemaMapper.cs
│   └── JsonOptions.cs
├── Validation/
│   ├── IValidator.cs
│   └── RequiredValidator.cs
├── Converters/
│   └── InvertedBoolConverter.cs
└── Extensions/
    └── FieldPropertiesExtensions.cs
```

---

## 🔧 `MauiFormBuilder` API Reference

| Method | Description |
|---|---|
| `LoadFromJson(string json)` | Parse and load the form schema |
| `RegisterRenderer(string type, IFieldRenderer)` | Register a typed renderer |
| `RegisterRenderer(string type, Func<FieldViewModel, View>)` | Register a lambda renderer |
| `AddValidator(IValidator)` | Add a custom form-wide validator |
| `SetFallbackRenderer(IFieldRenderer)` | Override the unknown-type fallback |
| `OnSubmit(Action<Dictionary<string, object?>>)` | Callback on validated submit |
| `OnAction(Action<string, Dictionary<string, object?>>)` | Callback for custom action buttons |
| `Build()` | Render and return the form `View` (a `ScrollView`) |

---

## 📱 Supported Platforms

| Platform | Minimum Version |
|---|---|
| Android | API 21 (Android 5.0) |
| iOS | 15.0 |
| macOS Catalyst | 15.0 |
| Windows | 10.0.17763 (Fall Creators Update) |

Targets: `net9.0-android`, `net9.0-ios`, `net9.0-maccatalyst`, `net9.0-windows10.0.19041.0`

---

## 📄 License

MIT — see [LICENSE](LICENSE) for details.
