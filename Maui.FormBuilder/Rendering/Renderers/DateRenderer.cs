using System;
using Maui.FormBuilder.Core.ViewModels;
using Microsoft.Maui.Controls;

namespace Maui.FormBuilder.Rendering.Renderers;

/// <summary>
/// Renders a date picker (type = "date").
/// Value: <see cref="DateTime"/> (date portion only).
/// </summary>
public sealed class DateRenderer : BaseFieldRenderer
{
    public override View Render(FieldViewModel vm)
    {
        var picker = new DatePicker
        {
            Date           = vm.Value is DateTime dt ? dt : DateTime.Today,
            Format         = "dd MMM yyyy",
            BindingContext = vm
        };

        picker.DateSelected += (_, e) => vm.Value = e.NewDate;

        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(FieldViewModel.Value) && vm.Value is DateTime d)
                picker.Date = d;
        };

        return WrapWithLabel(vm, picker);
    }
}
