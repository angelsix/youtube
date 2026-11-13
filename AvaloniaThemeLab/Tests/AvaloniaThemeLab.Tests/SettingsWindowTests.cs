using AngelSix.ThemeEngine;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaThemeLab.ThemePlayground.ViewModels;
using AvaloniaThemeLab.ThemePlayground.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AvaloniaThemeLab.Tests;

/// <summary>The settings window binds straight to the theme, both ways: a picker edits the theme, and a change made anywhere else shows in the picker.</summary>
public sealed class SettingsWindowTests
{
    [AvaloniaFact]
    public void Reset_puts_every_control_back_to_the_theme_defaults()
    {
        var theme = TestTheme.Current;
        var window = Open();
        try
        {
            theme.BaseSize = 1.5;
            theme.StandardAlignment = new Alignment(HorizontalAlignment.Right, VerticalAlignment.Bottom);
            Render();

            Assert.Equal(1.5m, Named<NumericUpDown>(window, "BaseSizeBox").Value);
            Assert.Equal(HorizontalAlignment.Right, Named<ComboBox>(window, "StandardHorizontalPicker").SelectedItem);

            Click(window, window.GetVisualDescendants().OfType<Button>().Single(b => (b.Content as string) == "Reset to theme defaults"));

            Assert.Equal(1.0m, Named<NumericUpDown>(window, "BaseSizeBox").Value);
            Assert.Equal(HorizontalAlignment.Left, Named<ComboBox>(window, "StandardHorizontalPicker").SelectedItem);
            Assert.Equal(VerticalAlignment.Center, Named<ComboBox>(window, "StandardVerticalPicker").SelectedItem);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void A_picker_edits_its_half_of_the_alignment()
    {
        var theme = TestTheme.Current;
        var window = Open();
        try
        {
            var picker = Named<ComboBox>(window, "ButtonVerticalPicker");
            picker.Focus();
            window.KeyPress(Key.Down, RawInputModifiers.None, PhysicalKey.ArrowDown, null);
            Render();

            Assert.Equal(VerticalAlignment.Bottom, theme.ButtonAlignment.Vertical);
            Assert.Equal(HorizontalAlignment.Center, theme.ButtonAlignment.Horizontal);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void A_closed_window_no_longer_clamps_the_theme_to_its_ranges()
    {
        var theme = TestTheme.Current;
        var window = Open();
        window.Close();

        // Below the BaseSize box's minimum of 0.5, which a still-bound box would write back
        theme.BaseSize = 0.25;
        Render();

        Assert.Equal(0.25, theme.BaseSize);
        GC.KeepAlive(window);
    }

    private static ThemeSettingsWindow Open()
    {
        var app = (App)Application.Current!;
        var window = new ThemeSettingsWindow { DataContext = app.Services.GetRequiredService<ThemeSettingsViewModel>(), Width = 560, Height = 4000 };
        window.Show();
        Render();
        return window;
    }

    private static T Named<T>(Window window, string name) where T : Control =>
        window.GetVisualDescendants().OfType<T>().Single(c => c.Name == name);

    private static void Click(Window window, Control control)
    {
        var centre = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;
        window.MouseMove(centre);
        window.MouseDown(centre, MouseButton.Left);
        window.MouseUp(centre, MouseButton.Left);
        Render();
    }

    private static void Render()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }
}
