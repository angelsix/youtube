using AngelSix.ThemeEngine;
using AngelSix.ThemeEngine.Generated;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaThemeLab.ViewModels;
using AvaloniaThemeLab.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AvaloniaThemeLab.Tests;

/// <summary>
/// The cross Luke clicked in the theme settings window was the clear button the theme put on every NumericUpDown. It
/// emptied the field, the field's value went null, and the binding could not write null into the theme's double, so
/// the field showed "Could not convert '(null)' to 'System.Double'". A numeric field clears only when asked to, with
/// the clearButton class, the same as TextBox.
/// </summary>
public sealed class NumericClearButtonTests
{
    [AvaloniaFact]
    public void Clicking_every_cross_in_the_settings_window_leaves_no_error()
    {
        var app = (App)Application.Current!;
        var theme = app.Services.GetRequiredService<ThemeContext>().Active;
        var window = new ThemeSettingsWindow { DataContext = app.Services.GetRequiredService<ThemeSettingsViewModel>(), Width = 560, Height = 4000 };
        var before = Snapshot(theme);
        try
        {
            window.Show();
            Render();

            var boxes = window.GetVisualDescendants().OfType<NumericUpDown>().ToList();
            Assert.NotEmpty(boxes);
            foreach (var box in boxes)
            {
                var cross = box.GetVisualDescendants().OfType<Button>().SingleOrDefault(b => b.Name == "PART_ClearButton");
                if (cross is null || !cross.IsEffectivelyVisible || cross.Bounds.Width <= 0)
                    continue;

                // Real input at the cross's centre, as the pointer would land
                var centre = cross.TranslatePoint(new Point(cross.Bounds.Width / 2, cross.Bounds.Height / 2), window)!.Value;
                window.MouseMove(centre);
                window.MouseDown(centre, MouseButton.Left);
                window.MouseUp(centre, MouseButton.Left);
                Render();
            }

            var errors = boxes
                .Where(DataValidationErrors.GetHasErrors)
                .Select(b => $"{b.Name}: {string.Join(", ", DataValidationErrors.GetErrors(b) ?? [])}")
                .ToList();
            Assert.True(errors.Count == 0, "Fields in error after their cross was clicked:\n" + string.Join("\n", errors));
            Assert.Equal(before, Snapshot(theme));
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void A_numeric_field_offers_a_cross_only_with_the_clear_button_class()
    {
        var plain = new NumericUpDown { Value = 5 };
        var clearable = new NumericUpDown { Value = 5, Classes = { "clearButton" } };
        var window = new Window { Content = new StackPanel { Children = { plain, clearable } }, Width = 400, Height = 200 };
        ThemeRegion.SetTheme(window, new Avalonia.Themes.Default.DefaultTheme());
        try
        {
            window.Show();
            Render();

            Assert.False(CrossOf(plain).IsEffectivelyVisible);
            Assert.True(CrossOf(clearable).IsEffectivelyVisible);

            // The class's cross still clears
            var cross = CrossOf(clearable);
            var centre = cross.TranslatePoint(new Point(cross.Bounds.Width / 2, cross.Bounds.Height / 2), window)!.Value;
            window.MouseMove(centre);
            window.MouseDown(centre, MouseButton.Left);
            window.MouseUp(centre, MouseButton.Left);
            Render();
            Assert.Null(clearable.Value);
        }
        finally
        {
            window.Close();
        }
    }

    private static Button CrossOf(NumericUpDown box) =>
        box.GetVisualDescendants().OfType<Button>().Single(b => b.Name == "PART_ClearButton");

    private static void Render()
    {
        for (var i = 0; i < 3; i++)
        {
            Dispatcher.UIThread.RunJobs();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        }
        Dispatcher.UIThread.RunJobs();
    }

    private static string Snapshot(object theme) =>
        string.Join(";", theme.GetType().GetProperties()
            .Where(p => p.PropertyType == typeof(double) && p.CanRead && p.GetIndexParameters().Length == 0)
            .Select(p => $"{p.Name}={p.GetValue(theme)}"));
}
