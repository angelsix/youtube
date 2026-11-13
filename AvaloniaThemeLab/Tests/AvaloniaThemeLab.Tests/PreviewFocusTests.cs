using AngelSix.ThemeEngine.Controls;
using AngelSix.ThemeEngine.Generated;
using AngelSix.Themes.Default;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaThemeLab.ThemePlayground.ViewModels;
using AvaloniaThemeLab.ThemePlayground.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AvaloniaThemeLab.Tests;

/// <summary>A preview pinned to the focused state draws a focus ring, so it must say it is focused.</summary>
public sealed class PreviewFocusTests
{
    [AvaloniaFact]
    public void A_hue_variant_pinned_focused_says_so()
    {
        var lab = ((App)Application.Current!).Services.GetRequiredService<MainWindowViewModel>();
        var shown = lab.SelectedEntry;
        var window = new MainWindow { DataContext = lab, Width = 1600, Height = 1200 };
        TestTheme.With(null);
        var unlabelled = new List<string>();
        try
        {
            window.Show();
            foreach (var entry in lab.Catalogue.Entries)
            {
                lab.SelectedEntry = entry;
                Dispatcher.UIThread.RunJobs();
                window.UpdateLayout();
                var preview = window.GetVisualDescendants().OfType<LightDarkPreview>().FirstOrDefault();
                if (preview is null)
                    continue;
                var pane = preview.GetVisualDescendants().OfType<Border>().First(b => b.Name == "PART_LightPane");
                foreach (var control in pane.GetVisualDescendants().OfType<Control>().Where(c => Preview.GetState(c) == PreviewState.Focused && c.IsSet(ThemeAccent.HueProperty)).Distinct())
                {
                    if (!SaysFocused(control))
                        unlabelled.Add($"{entry.Name}: a {control.GetType().Name} labelled '{Label(control)}'");
                }
            }
        }
        finally
        {
            window.Close();
            lab.SelectedEntry = shown;
        }

        Assert.True(unlabelled.Count == 0, "Pinned focused with nothing saying so:\n" + string.Join("\n", unlabelled.Distinct()));
    }

    private static string Label(Control control) => control switch
    {
        AutoCompleteBox a => a.PlaceholderText ?? a.Text ?? "",
        TextBox t => (t.Text ?? "") + " " + (t.PlaceholderText ?? ""),
        ContentControl c => c.Content as string ?? "",
        _ => "",
    };

    private static bool SaysFocused(Control control)
    {
        if (Label(control).Contains("Focus", StringComparison.OrdinalIgnoreCase))
            return true;

        // The caption just above it: the nearest earlier TextBlock among its own or its parent's siblings
        for (Control? at = control; at?.Parent is Panel panel; at = panel)
        {
            var index = panel.Children.IndexOf(at);
            var caption = panel.Children.Take(index).Reverse().OfType<TextBlock>().FirstOrDefault();
            if (caption is not null)
                return caption.Text?.Contains("Focus", StringComparison.OrdinalIgnoreCase) == true;
        }
        return false;
    }
}
