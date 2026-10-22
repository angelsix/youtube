namespace Avalonia.Themes.Default;

/// <summary>
/// The gallery rule a control's theme file DECLARES for a control that can have no gallery entry at
/// all: it is a piece of runtime infrastructure (a TopLevel, a popup host, an adorner layer) that
/// renders nothing on its own, and no other entry shows it either.
/// </summary>
/// <remarks>
/// <para>
/// This is the sibling of <see cref="PreviewShownIn"/>: one says "shown inside that entry", the other
/// says "shown nowhere, and that is deliberate". Both exist so the gallery never has to guess why a
/// themed control is absent from it.
/// </para>
/// <para>
/// A theme file declares it beside the control's ControlTheme:
/// </para>
/// <code>
/// &lt;default:PreviewExcluded x:Key="WindowPreviewExcluded" Reason="A window is its own OS surface." /&gt;
/// </code>
/// </remarks>
public sealed class PreviewExcluded
{
    /// <summary>Why this control can have no gallery entry, in one line.</summary>
    public string Reason { get; set; } = string.Empty;
}
