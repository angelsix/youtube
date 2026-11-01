using System;
using System.Collections.Generic;
using System.Linq;
using AngelSix.ThemeEngine;
using Avalonia;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Themes.Default;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaThemeLab.ViewModels;

/// <summary>The settings window: the app's live theme instance, edited in place.</summary>
/// <remarks>
/// The view binds straight to <see cref="Theme"/>. Its declared values are observable, and the
/// theme context re-resolves every token binding when one changes, so an edit here restyles the
/// whole application with nothing in between. This class only adds what the theme itself cannot
/// offer a view: the lists a choice is made from, and a way back to the shipped values.
/// </remarks>
public sealed partial class ThemeSettingsViewModel : ObservableObject
{
    /// <summary>The theme every window is drawing with, edited in place.</summary>
    public DefaultTheme Theme { get; }

    /// <summary>
    /// The settings window's own copy of the theme, a fresh instance of the same type that no edit
    /// touches, so the editor keeps its size and colours while the app it is editing changes.
    /// </summary>
    public DefaultTheme EditorTheme { get; }

    public IReadOnlyList<FontFamily> Fonts { get; }

    public IReadOnlyList<HorizontalAlignment> HorizontalAlignments { get; } = Enum.GetValues<HorizontalAlignment>();

    public IReadOnlyList<VerticalAlignment> VerticalAlignments { get; } = Enum.GetValues<VerticalAlignment>();

    public ThemeSettingsViewModel(ThemeContext context)
    {
        Theme = (DefaultTheme)context.Active;
        EditorTheme = (DefaultTheme)Activator.CreateInstance(Theme.GetType())!;

        // The theme's own family first, so the current value is an item the list can select
        Fonts = [Theme.FontFamily, .. FontManager.Current.SystemFonts.OrderBy(font => font.Name)];
    }

    // Design-time constructor, for the previewer and headless tools that build the window with no
    // application container: the booted app's context when there is one, else a fresh theme.
    public ThemeSettingsViewModel()
        : this(Application.Current is App app
            ? app.Services.GetRequiredService<ThemeContext>()
            : new ThemeContext(new BrandTheme()))
    {
    }

    /// <summary>Puts every editable value back to what the theme ships with.</summary>
    /// <remarks>
    /// Read off a fresh instance of the same theme type rather than a list kept here, so a value
    /// added to the theme later resets too, and a subclass's own defaults are honoured.
    /// </remarks>
    [RelayCommand]
    private void Reset()
    {
        var shipped = Activator.CreateInstance(Theme.GetType())!;
        foreach (var property in typeof(DefaultTheme).GetProperties())
        {
            if (property.SetMethod?.IsPublic == true)
                property.SetValue(Theme, property.GetValue(shipped));
        }
    }
}
