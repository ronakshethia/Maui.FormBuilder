using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Maui.FormBuilder.Builder;
using Maui.FormBuilder.Sample.CustomRenderers;
using Microsoft.Maui.Controls;

namespace Maui.FormBuilder.Sample.Pages;

/// <summary>
/// Demonstrates two extensibility patterns:
/// <list type="number">
///   <item><b>Lambda renderer</b> — inline star-rating field ("rating")</item>
///   <item><b>Typed renderer</b> — <see cref="ColorPickerRenderer"/> class ("colorpicker")</item>
/// </list>
/// </summary>
public partial class CustomRendererPage : ContentPage
{
    public CustomRendererPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (Content is not null && Content is not Microsoft.Maui.Controls.ScrollView)
            return;

        this.Content = await BuildCustomFormAsync();
    }

    private async Task<View> BuildCustomFormAsync()
    {
        string json = await LoadSchemaAsync("custom_demo.json");

        return new MauiFormBuilder()
            .LoadFromJson(json)

            // ── Lambda renderer: "rating" ──────────────────────────────────
            // Inline star picker. Value = int (1–5).
            .RegisterRenderer("rating", vm =>
            {
                int currentRating = vm.Value is int r ? r : 0;
                var stars         = new List<Label>();

                var row = new HorizontalStackLayout { Spacing = 6 };

                void RefreshStars(int selected)
                {
                    for (int i = 0; i < stars.Count; i++)
                    {
                        stars[i].Text      = i < selected ? "★" : "☆";
                        stars[i].TextColor = i < selected ? Colors.Gold : Colors.LightGray;
                    }
                }

                for (int i = 1; i <= 5; i++)
                {
                    int starIndex = i;
                    var star = new Label
                    {
                        Text     = "☆",
                        FontSize = 36,
                        TextColor = Colors.LightGray
                    };

                    var tap = new TapGestureRecognizer();
                    tap.Tapped += (_, _) =>
                    {
                        currentRating = starIndex;
                        vm.Value      = currentRating;
                        RefreshStars(currentRating);
                    };

                    star.GestureRecognizers.Add(tap);
                    stars.Add(star);
                    row.Add(star);
                }

                // Initialise display
                RefreshStars(currentRating);

                // Sync on external reset
                vm.PropertyChanged += (_, e) =>
                {
                    if (e.PropertyName == nameof(Maui.FormBuilder.Core.ViewModels.FieldViewModel.Value))
                        RefreshStars(vm.Value is int v ? v : 0);
                };

                var label = new Label
                {
                    Text           = vm.Required ? $"{vm.Label} *" : vm.Label,
                    FontAttributes = FontAttributes.Bold,
                    FontSize       = 14,
                    Margin         = new Thickness(0, 0, 0, 4)
                };

                var stack = new VerticalStackLayout { Spacing = 4 };
                stack.Add(label);
                stack.Add(row);
                return stack;
            })

            // ── Typed renderer: "colorpicker" ──────────────────────────────
            .RegisterRenderer("colorpicker", new ColorPickerRenderer())

            .OnSubmit(async data =>
            {
                var msg = $"Rating: {data.GetValueOrDefault("productRating")}\n" +
                          $"Favourite color: {data.GetValueOrDefault("favouriteColor")}";

                await MainThread.InvokeOnMainThreadAsync(() =>
                    DisplayAlert("Submitted", msg, "OK"));
            })
            .Build();
    }

    private static async Task<string> LoadSchemaAsync(string fileName)
    {
        await using Stream stream = await FileSystem.OpenAppPackageFileAsync(fileName);
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
}
