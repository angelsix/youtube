using AngelSix.ThemeEngine.Generated;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.Templates;

namespace Avalonia.Themes.Default;

/// <summary>
/// Renders one piece of design-preview content twice — under the light palette and under a dark
/// region — so a control's states can be compared side by side in a single glance.
/// </summary>
/// <remarks>
/// <para>
/// Used by <c>Design.PreviewWith</c> blocks and by any page that shows a control's preview template. Supply the shared content once as a
/// <see cref="DataTemplate"/> — either written INLINE between the tags (a <see cref="DataTemplate"/>
/// assigned to <see cref="ContentControl.Content"/> is adopted as the template), or passed via
/// <see cref="ContentControl.ContentTemplate"/> (typically a keyed resource). Whichever form is
/// used, the two panes instantiate INDEPENDENT copies of the template, so per-placement state
/// (hover flags, disabled instances, validation errors) belongs to each pane separately.
/// </para>
/// <para>
/// An ELEMENT TREE cannot be supplied: an element has exactly one visual parent, and two panes
/// cannot share one instance — the template IS the sharing mechanism. A bare element child is
/// therefore ignored rather than shown once and blanked elsewhere.
/// </para>
/// <para>
/// The dark pane flips <c>ThemeRegion.IsDark</c> on its own border, mirroring the scoping the
/// hand-written previews used before this existed. Its appearance is defined entirely in
/// Controls/LightDarkPreview.axaml.
/// </para>
/// <para>
/// <see cref="IsAccentPickerVisible"/> opts a preview into an accent picker: a row of swatches in each
/// pane's header, one per hue in <see cref="AccentChoices"/>. Picking one sets <see cref="SelectedAccent"/>,
/// which both panes' content carries as its <c>Accent.Kind</c>, so light and dark always show the same hue.
/// The headers and the swatches keep their own colours.
/// </para>
/// </remarks>
public class LightDarkPreview : ContentControl
{
    /// <summary>The caption shown above each pane; the panes append "- Light" / "- Dark".</summary>
    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>Defines the <see cref="Title"/> property.</summary>
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<LightDarkPreview, string?>(nameof(Title));

    /// <summary>Whether the two panes sit side by side (<see cref="Orientation.Horizontal"/>, the
    /// default) or stacked (<see cref="Orientation.Vertical"/>).</summary>
    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>Defines the <see cref="Orientation"/> property.</summary>
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<LightDarkPreview, Orientation>(nameof(Orientation), defaultValue: Orientation.Horizontal);

    /// <summary>Whether each pane's header shows the accent swatches. Off by default; a page opts in.</summary>
    public bool IsAccentPickerVisible
    {
        get => GetValue(IsAccentPickerVisibleProperty);
        set => SetValue(IsAccentPickerVisibleProperty, value);
    }

    /// <summary>Defines the <see cref="IsAccentPickerVisible"/> property.</summary>
    public static readonly StyledProperty<bool> IsAccentPickerVisibleProperty =
        AvaloniaProperty.Register<LightDarkPreview, bool>(nameof(IsAccentPickerVisible));

    /// <summary>The hue both panes' content renders in. <see cref="AccentKind.Neutral"/>, the theme's
    /// fallback hue, by default, so a preview nobody picks for looks exactly as it would without one.</summary>
    public AccentKind SelectedAccent
    {
        get => GetValue(SelectedAccentProperty);
        set => SetValue(SelectedAccentProperty, value);
    }

    /// <summary>Defines the <see cref="SelectedAccent"/> property.</summary>
    public static readonly StyledProperty<AccentKind> SelectedAccentProperty =
        AvaloniaProperty.Register<LightDarkPreview, AccentKind>(nameof(SelectedAccent));

    /// <summary>The hues the picker offers, in the order DefaultTheme declares them. Surface is left out:
    /// it is the canvas's own hue, so content drawn in it disappears into the pane.</summary>
    public static IReadOnlyList<AccentKind> AccentChoices { get; } =
    [
        AccentKind.Neutral, AccentKind.Primary, AccentKind.Success, AccentKind.Warning,
        AccentKind.Error, AccentKind.Info, AccentKind.Destructive, AccentKind.Subtle,
    ];

    /// <summary>The template both panes present. Kept in sync by <see cref="OnPropertyChanged"/>
    /// from whichever input carries the template; bound to by the panes in the template.</summary>
    public DataTemplate? PaneContentTemplate
    {
        get => GetValue(PaneContentTemplateProperty);
        set => SetValue(PaneContentTemplateProperty, value);
    }

    /// <summary>Defines the <see cref="PaneContentTemplate"/> property.</summary>
    public static readonly StyledProperty<DataTemplate?> PaneContentTemplateProperty =
        AvaloniaProperty.Register<LightDarkPreview, DataTemplate?>(nameof(PaneContentTemplate));

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ContentProperty)
        {
            if (change.NewValue is DataTemplate dataTemplate)
            {
                // Inline template: adopt it as the panes' template. It stays harmlessly in Content —
                // the outer template has no ContentPresenter, so nothing ever presents it.
                PaneContentTemplate = dataTemplate;
            }
            else if (ChangeWithdrawsInlineTemplate(change))
            {
                // Input withdrawn: fall back to whatever ContentTemplate holds, if anything.
                PaneContentTemplate = ContentTemplate as DataTemplate;
            }
        }
        else if (change.Property == ContentTemplateProperty
                 && change.OldValue is null
                 && change.NewValue is DataTemplate explicitTemplate)
        {
            // Explicit template: forward it to the panes.
            PaneContentTemplate = explicitTemplate;
        }
    }

    private static bool ChangeWithdrawsInlineTemplate(AvaloniaPropertyChangedEventArgs change) =>
        change.NewValue is not DataTemplate && change.OldValue is DataTemplate;
}
