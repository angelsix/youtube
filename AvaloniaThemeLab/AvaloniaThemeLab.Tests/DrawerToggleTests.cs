using AngelSix.ThemeEngine.Generated;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Themes.Default;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaThemeLab.ViewModels;
using AvaloniaThemeLab.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AvaloniaThemeLab.Tests;

/// <summary>
/// The compact drawer's hamburger lives in a pane of fixed width. However large the icon size goes, the button stays
/// inside the pane, stays square, and its icon stays inside its face: a confined button shrinks its icon rather than
/// letting it spill out and stretch the face.
/// </summary>
public sealed class DrawerToggleTests
{
    public static TheoryData<double> IconScales() => [1, 2, 3];

    [AvaloniaTheory]
    [MemberData(nameof(IconScales))]
    public void The_compact_hamburger_stays_inside_its_pane_and_its_icon_inside_it(double iconScale)
    {
        var lab = ((App)Application.Current!).Services.GetRequiredService<MainWindowViewModel>();
        var shown = lab.SelectedEntry;
        var window = new MainWindow { DataContext = lab, Width = 1600, Height = 1400 };
        ThemeRegion.SetTheme(window, new DefaultTheme { IconSizeScale = iconScale });
        try
        {
            lab.SelectedEntry = lab.Catalogue.Entries.Single(e => e.Name == "DrawerPage");
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();

            var toggles = window.GetVisualDescendants().OfType<ToggleButton>().Where(b => b.Name == "PART_CompactPaneToggle" && b.IsEffectivelyVisible).ToList();
            Assert.NotEmpty(toggles);
            var problems = new List<string>();
            foreach (var toggle in toggles)
            {
                var split = toggle.GetVisualAncestors().OfType<SplitView>().First();
                var right = toggle.TranslatePoint(new Point(toggle.Bounds.Width, 0), split)!.Value.X;
                if (right > split.CompactPaneLength + 0.5)
                    problems.Add($"the toggle ends at {right:0.#}, past the {split.CompactPaneLength:0.#}px compact pane");
                if (Math.Abs(toggle.Bounds.Width - toggle.Bounds.Height) > 1)
                    problems.Add($"the toggle is {toggle.Bounds.Width:0.#}x{toggle.Bounds.Height:0.#}, not square");

                var icon = toggle.GetVisualDescendants().OfType<PathIcon>().First();
                var topLeft = icon.TranslatePoint(default, toggle)!.Value;
                var bottomRight = icon.TranslatePoint(new Point(icon.Bounds.Width, icon.Bounds.Height), toggle)!.Value;
                if (topLeft.X < -0.5 || topLeft.Y < -0.5 || bottomRight.X > toggle.Bounds.Width + 0.5 || bottomRight.Y > toggle.Bounds.Height + 0.5)
                    problems.Add($"the icon spans {topLeft.X:0.#},{topLeft.Y:0.#} to {bottomRight.X:0.#},{bottomRight.Y:0.#} on a {toggle.Bounds.Width:0.#}x{toggle.Bounds.Height:0.#} face");
            }

            Assert.True(problems.Count == 0, $"Icon size {iconScale}: " + string.Join("; ", problems.Distinct()));
        }
        finally
        {
            window.Close();
            lab.SelectedEntry = shown;
        }
    }
}
