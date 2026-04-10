using System;
using Maui.FormBuilder.Core.ViewModels;
using Microsoft.Maui.Controls;

namespace Maui.FormBuilder.Rendering.Renderers;

/// <summary>
/// Renders a time picker (type = "time").
/// Value: <see cref="TimeSpan"/>.
/// </summary>
public sealed class TimeRenderer : BaseFieldRenderer
{
    public override View Render(FieldViewModel vm)
    {
        var picker = new TimePicker
        {
            Time           = vm.Value is TimeSpan ts ? ts : DateTime.Now.TimeOfDay,
            Format         = "hh:mm tt",
            BindingContext = vm
        };

        // TimePicker exposes Time change via PropertyChanged (no dedicated event)
        picker.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(TimePicker.Time))
                vm.Value = picker.Time;
        };

        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(FieldViewModel.Value) && vm.Value is TimeSpan t)
                picker.Time = t;
        };

        return WrapWithLabel(vm, picker);
    }
}
