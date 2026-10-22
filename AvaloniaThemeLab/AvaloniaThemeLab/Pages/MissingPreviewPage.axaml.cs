using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaloniaThemeLab.Pages;

/// <summary>
/// The page a FLAGGED gallery row shows: the themed control has no preview template, and its theme
/// file declares no reason for the omission. The page says so in words, so the mistake is visible
/// when the row is selected rather than only in the sidebar's name.
/// </summary>
/// <remarks>
/// Task 176. The gallery lists a control only when the theme keys a <c>{Name}Preview</c> template
/// for it. A control shown inside another entry has none, so it used to vanish from the gallery
/// SILENTLY — and a control whose preview was merely forgotten looked exactly like one that was
/// deliberately folded in. The catalogue now lists such a control as a flagged row, and this page
/// is what that row opens.
/// </remarks>
public partial class MissingPreviewPage : UserControl
{
    public MissingPreviewPage() => InitializeComponent();
}
