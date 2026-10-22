using System;

namespace Avalonia.Themes.Default;

/// <summary>
/// The gallery rule a control's theme file DECLARES: this control has no gallery entry of its own,
/// because the entry named by <see cref="ShownIn"/> already shows it.
/// </summary>
/// <remarks>
/// <para>
/// The gallery (<c>AvaloniaThemeLab/ViewModels/GalleryCatalogue.cs</c>) lists a control only when the
/// theme keys a <c>{Name}Preview</c> DataTemplate for it. A control shown INSIDE another entry — a
/// ListBoxItem inside a ListBox, a MenuItem inside a ContextMenu — simply has no preview, so it used
/// to vanish from the gallery SILENTLY: nothing recorded WHY, and a control whose preview was merely
/// forgotten looked exactly like one that was deliberately folded in.
/// </para>
/// <para>
/// This type is that record. A theme file declares, beside the control's ControlTheme:
/// </para>
/// <code>
/// &lt;default:PreviewShownIn x:Key="ListBoxItemPreviewShownIn" ShownIn="{x:Type ListBox}" Reason="..." /&gt;
/// </code>
/// <para>
/// and the gallery then treats the control as DELIBERATELY excluded. A control with NEITHER a preview
/// NOR a marker is a MISTAKE, and the gallery lists it flagged rather than dropping it silently.
/// </para>
/// </remarks>
public sealed class PreviewShownIn
{
    /// <summary>The gallery entry that shows this control — the entry a person clicks to meet it.</summary>
    public Type ShownIn { get; set; } = typeof(object);

    /// <summary>Why this control has no entry of its own, in one line.</summary>
    public string Reason { get; set; } = string.Empty;
}
