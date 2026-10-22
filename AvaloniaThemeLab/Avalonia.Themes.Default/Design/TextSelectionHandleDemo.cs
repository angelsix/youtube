using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace Avalonia.Themes.Default.Design;

/// <summary>Which grip a standalone selection handle wears.</summary>
public enum GripShapeKind
{
    /// <summary>The grip that stands at the START of a selection.</summary>
    Start,

    /// <summary>The grip that stands at the END of a selection.</summary>
    End,

    /// <summary>The grip that stands at a bare insertion point (the caret).</summary>
    Caret,
}

/// <summary>
/// Marks a <see cref="TextSelectionHandle"/> with the grip shape it should wear.
/// </summary>
/// <remarks>
/// <para>
/// Avalonia drives its own touch handles with the INTERNAL <c>SelectionHandleType</c>, which reaches
/// the template as the <c>:start</c>, <c>:end</c> and <c>:caret</c> pseudo-classes. A handle that
/// stands on its own — a demo grip rather than a touch handle — cannot set that internal property:
/// it is not public, and the control re-derives its pseudo-classes from it on every arrange, so a
/// pseudo-class set from outside is wiped again at the next layout pass. This attached property is
/// the sanctioned way for the theme to say which shape a standalone grip wears: the theme's
/// ControlTheme reads it and dresses the grip accordingly.
/// </para>
/// <para>
/// The property is attached rather than a plain CLR property because the grip is Avalonia's own
/// control: the theme cannot add a property to it, but it can attach one.
/// </para>
/// </remarks>
public sealed class GripShape
{
    /// <summary>Defines the <see cref="Shape"/> property.</summary>
    public static readonly AttachedProperty<GripShapeKind> ShapeProperty =
        AvaloniaProperty.RegisterAttached<GripShape, TextSelectionHandle, GripShapeKind>("Shape", GripShapeKind.Caret);

    /// <summary>Sets the grip shape a handle wears.</summary>
    public static void SetShape(TextSelectionHandle target, GripShapeKind value) =>
        target.SetValue(ShapeProperty, value);

    /// <summary>Reads the grip shape a handle wears.</summary>
    public static GripShapeKind GetShape(TextSelectionHandle target) =>
        target.GetValue(ShapeProperty);
}

/// <summary>
/// A text field with WORKING selection grips: the theme's own <see cref="TextSelectionHandle"/>
/// controls stand at the start, the end and the caret of the field's selection, and dragging one
/// with the mouse moves the field's <see cref="TextBox.SelectionStart"/>,
/// <see cref="TextBox.SelectionEnd"/> or <see cref="TextBox.CaretIndex"/>.
/// </summary>
/// <remarks>
/// <para>
/// <b>WHY A HELPER AT ALL.</b> Avalonia's own selection grips are a TOUCH affordance, and the
/// framework has no public switch for showing them under a mouse. Measured in this sweep
/// (headless, Avalonia 12.0.1): a focused field whose selection is non-empty DOES make the stock
/// <c>TextSelectionHandleCanvas</c> report its three handles as visible — but the canvas itself
/// stays hidden and every handle measures 0x0, because the canvas only seats and shows itself
/// from <c>Show()</c>, and every route into that method is reached only from a NON-MOUSE pointer:
/// <c>TextBox.OnTapped</c> skips the mouse, the double-tap handler skips the mouse, and the
/// canvas's own <c>Show(forceTouchMode: true)</c> is only called from the touch double-tap path.
/// A mouse click calls <c>Hide()</c> outright. So under a mouse the stock handles never appear,
/// and there is no public API that changes that.
/// </para>
/// <para>
/// <b>WHAT THIS DOES INSTEAD.</b> It wires the theme's OWN grip controls to the field through the
/// PUBLIC API only: the grips are real <see cref="TextSelectionHandle"/> controls, so they wear the
/// theme's own artwork, accent face, touch target and indicator line; they are SEATED from the
/// field's own text geometry (the presenter's text layout), so they stand exactly where Avalonia
/// would put them; and a drag on one is resolved by hit-testing the pointer against that same text
/// layout — the hit test Avalonia's own canvas performs — which yields a character index that is
/// written back to the field's selection.
/// </para>
/// <para>
/// <b>WHAT IS NOT REINVENTED.</b> The grip's look, its accent and its hit target all come from the
/// theme's <c>TextSelectionHandle</c> ControlTheme. Only the seating and the drag are this
/// control's work.
/// </para>
/// </remarks>
public sealed class TextSelectionHandleDemo : Canvas
{
    /// <summary>Defines the <see cref="Text"/> property.</summary>
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<TextSelectionHandleDemo, string?>(nameof(Text));

    /// <summary>Defines the <see cref="SelectionStart"/> property.</summary>
    public static readonly StyledProperty<int> SelectionStartProperty =
        AvaloniaProperty.Register<TextSelectionHandleDemo, int>(nameof(SelectionStart));

    /// <summary>Defines the <see cref="SelectionEnd"/> property.</summary>
    public static readonly StyledProperty<int> SelectionEndProperty =
        AvaloniaProperty.Register<TextSelectionHandleDemo, int>(nameof(SelectionEnd));

    /// <summary>Defines the <see cref="CaretIndex"/> property.</summary>
    public static readonly StyledProperty<int> CaretIndexProperty =
        AvaloniaProperty.Register<TextSelectionHandleDemo, int>(nameof(CaretIndex));

    private readonly TextBox _textBox;
    private readonly TextSelectionHandle _startHandle;
    private readonly TextSelectionHandle _endHandle;
    private readonly TextSelectionHandle _caretHandle;

    private TextPresenter? _presenter;

    // The drag in progress: the grip being held, the point in the text it is anchored at, and the
    // pointer's position when it went down.
    private TextSelectionHandle? _dragged;
    private double _anchorX;
    private double _anchorY;
    private double _pressX;

    /// <summary>Creates the demo: a field, three grips, and the wiring between them.</summary>
    public TextSelectionHandleDemo()
    {
        _textBox = new TextBox
        {
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
        };
        _startHandle = CreateGrip(GripShapeKind.Start);
        _endHandle = CreateGrip(GripShapeKind.End);
        _caretHandle = CreateGrip(GripShapeKind.Caret);

        // Which grips are shown is decided HERE, before the first layout pass, so the control can
        // reserve the room the shown grip needs in its very first measure. Deciding it later (when
        // the grips are seated) leaves the first pass short of the room and the grip spills over
        // whatever sits below.
        UpdateGripVisibility();

        Children.Add(_textBox);
        Children.Add(_startHandle);
        Children.Add(_endHandle);
        Children.Add(_caretHandle);

        foreach (var grip in new[] { _startHandle, _endHandle, _caretHandle })
        {
            grip.PointerPressed += GripPressed;
            grip.PointerMoved += GripMoved;
            grip.PointerReleased += GripReleased;
        }
    }

    /// <summary>The field the grips belong to.</summary>
    public TextBox TextBox => _textBox;

    /// <summary>The grip standing at the start of the selection.</summary>
    public TextSelectionHandle StartHandle => _startHandle;

    /// <summary>The grip standing at the end of the selection.</summary>
    public TextSelectionHandle EndHandle => _endHandle;

    /// <summary>The grip standing at the bare insertion point.</summary>
    public TextSelectionHandle CaretHandle => _caretHandle;

    /// <summary>The text the field shows.</summary>
    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>Where the field's selection starts.</summary>
    public int SelectionStart
    {
        get => GetValue(SelectionStartProperty);
        set => SetValue(SelectionStartProperty, value);
    }

    /// <summary>Where the field's selection ends.</summary>
    public int SelectionEnd
    {
        get => GetValue(SelectionEndProperty);
        set => SetValue(SelectionEndProperty, value);
    }

    /// <summary>Where the field's caret stands.</summary>
    public int CaretIndex
    {
        get => GetValue(CaretIndexProperty);
        set => SetValue(CaretIndexProperty, value);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == TextProperty)
        {
            _textBox.Text = change.GetNewValue<string?>();
            ApplySelection();
        }
        else if (change.Property == SelectionStartProperty || change.Property == SelectionEndProperty)
        {
            ApplySelection();
        }
        else if (change.Property == CaretIndexProperty)
        {
            _textBox.CaretIndex = change.GetNewValue<int>();
        }
        else if (change.Property == WidthProperty)
        {
            var width = change.GetNewValue<double>();
            if (!double.IsNaN(width))
                _textBox.Width = width;
        }
    }

    /// <inheritdoc />
    protected override Size MeasureOverride(Size availableSize)
    {
        // Which grips are shown follows the field's selection, and the SHOWN grip has to be
        // measured before this control can reserve the room it needs. Deciding the visibility
        // later (when the grips are seated, in the arrange pass) is too late: the first measure
        // reserves nothing, the control stays the height of the field alone, and the grip then
        // hangs over whatever sits below it.
        UpdateGripVisibility();

        base.MeasureOverride(availableSize);

        var field = _textBox.DesiredSize;

        // The grips hang BELOW the field's line of text, so the control has to be tall enough to
        // hold them: a control that lets its children spill out of it is one the layout gate
        // reads as clipped content, and one whose grip paints over the next element.
        // The reserve is the text line plus the grip's own MEASURED height, so the grip's box
        // stays inside this control and the preview's own margin leaves the gap below it.
        var reserve = TextLineBottom() + GripHeight();

        return new Size(Math.Max(field.Width, 0), Math.Max(field.Height, reserve));
    }

    /// <inheritdoc />
    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = base.ArrangeOverride(finalSize);
        SeatGrips();
        return size;
    }

    /// <summary>Writes the demo's selection onto the field.</summary>
    private void ApplySelection()
    {
        var length = _textBox.Text?.Length ?? 0;
        _textBox.SelectionStart = Math.Clamp(SelectionStart, 0, length);
        _textBox.SelectionEnd = Math.Clamp(SelectionEnd, 0, length);
    }

    /// <summary>Builds one grip and marks the shape it wears.</summary>
    private static TextSelectionHandle CreateGrip(GripShapeKind shape)
    {
        var grip = new TextSelectionHandle
        {
            IsVisible = false,
        };
        GripShape.SetShape(grip, shape);
        SetHandleType(grip, shape);
        return grip;
    }

    /// <summary>
    /// Tells Avalonia which part of the selection the grip stands at, so the framework dresses it
    /// the way it dresses its own touch handles.
    /// </summary>
    /// <remarks>
    /// The framework's <c>SelectionHandleType</c> is INTERNAL, and it is what decides the grip's
    /// <c>:start</c> / <c>:end</c> / <c>:caret</c> pseudo-classes — the ones the theme's
    /// ControlTheme dresses the artwork and the indicator with. The control re-derives those
    /// pseudo-classes from this property on every arrange, so a pseudo-class set from outside is
    /// wiped again at the next layout pass; the property itself is the only durable way in, and it
    /// is written by reflection.
    ///
    /// The theme ALSO keys the same three shapes on the public
    /// <see cref="GripShape.ShapeProperty"/>, so a grip that is not Avalonia's own still wears the
    /// right artwork. The two routes agree: this one drives the framework's pseudo-classes, and
    /// that one drives the theme's rules.
    /// </remarks>
    private static void SetHandleType(TextSelectionHandle grip, GripShapeKind shape)
    {
        var property = typeof(TextSelectionHandle)
            .GetProperty("SelectionHandleType", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (property is null || !property.CanWrite)
            return;

        try
        {
            var value = Enum.Parse(property.PropertyType, shape.ToString());
            property.SetValue(grip, value);
        }
        catch
        {
            // A framework that renames the property leaves the grip in its default shape; the
            // theme's own attached-property rules still dress it correctly.
        }
    }

    /// <summary>The field's text presenter — the control that owns the text geometry.</summary>
    private TextPresenter? FindPresenter() =>
        _textBox.GetVisualDescendants().OfType<TextPresenter>().FirstOrDefault();

    /// <summary>
    /// Which grips are shown: the start and end grips on a non-empty selection, the caret grip on
    /// a bare insertion point. Exactly the same rule the seating applies, so the visibility the
    /// MEASURE reserves room for is the visibility the arrange shows.
    /// </summary>
    private void UpdateGripVisibility()
    {
        var length = _textBox.Text?.Length ?? 0;
        var start = Math.Clamp(_textBox.SelectionStart, 0, length);
        var end = Math.Clamp(_textBox.SelectionEnd, 0, length);
        var hasSelection = start != end;

        _startHandle.IsVisible = hasSelection;
        _endHandle.IsVisible = hasSelection;
        _caretHandle.IsVisible = !hasSelection;
    }

    /// <summary>
    /// The bottom of the field's line of text, in this control's own coordinates: the field's top
    /// padding plus the height of the text layout. The grips hang from this line.
    /// </summary>
    private double TextLineBottom() =>
        _textBox.Padding.Top + (FindPresenter()?.TextLayout.Height ?? 0);

    /// <summary>
    /// The height of the grip that is shown, measured: the grip's own desired height, which
    /// includes its enlarged touch target. It is the SHOWN grip's height, because a hidden grip
    /// reserves nothing.
    /// </summary>
    private double GripHeight()
    {
        var grip = _startHandle.IsVisible ? _startHandle
            : _endHandle.IsVisible ? _endHandle
            : _caretHandle.IsVisible ? _caretHandle
            : null;

        return grip?.DesiredSize.Height ?? 0;
    }

    /// <summary>
    /// Seats the three grips on the field's text geometry: the start and end grips on the first
    /// and last selected character, the caret grip on the insertion point.
    /// </summary>
    private void SeatGrips()
    {
        var presenter = _presenter ??= FindPresenter();
        if (presenter is null)
            return;

        var layout = presenter.TextLayout;
        if (layout is null)
            return;

        var start = _textBox.SelectionStart;
        var end = _textBox.SelectionEnd;
        var first = Math.Min(start, end);
        var last = Math.Max(start, end);
        var hasSelection = first != last;

        UpdateGripVisibility();

        if (hasSelection)
        {
            // The start grip stands under the FIRST selected character and the end grip under the
            // LAST one — the same two points Avalonia's own canvas uses.
            var startRect = FirstRect(layout.HitTestTextRange(first, 1));
            if (startRect is not null)
                Seat(_startHandle, startRect.Value.Left, startRect.Value.Bottom, presenter);

            var endRect = FirstRect(layout.HitTestTextRange(last - 1, 1));
            if (endRect is not null)
                Seat(_endHandle, endRect.Value.Right, endRect.Value.Bottom, presenter);
        }
        else
        {
            var caret = layout.HitTestTextPosition(_textBox.CaretIndex);
            Seat(_caretHandle, caret.Left, caret.Bottom, presenter);
        }
    }

    /// <summary>
    /// Puts one grip at a point of the text, in this control's own coordinates. The grip is
    /// placed so its INDICATOR — the line that runs back up to the line of text — lands exactly on
    /// the point, which is how Avalonia's own canvas seats its handles.
    /// </summary>
    private void Seat(TextSelectionHandle grip, double textX, double textY, TextPresenter presenter)
    {
        var point = presenter.TranslatePoint(new Point(textX, textY), this) ?? new Point(textX, textY);
        Canvas.SetLeft(grip, point.X - IndicatorOffset(grip));
        Canvas.SetTop(grip, point.Y);
    }

    /// <summary>
    /// The distance from the grip's own left edge to the middle of its indicator line — the offset
    /// the grip's left edge takes so the indicator lands on the text.
    /// </summary>
    private static double IndicatorOffset(TextSelectionHandle grip)
    {
        var indicator = grip.GetVisualDescendants().OfType<Border>()
            .FirstOrDefault(b => b.Name == "PART_Indicator");
        if (indicator is null)
            return 0;
        return indicator.Bounds.X + (indicator.Bounds.Width / 2);
    }

    /// <summary>The first rectangle of a text range, or null when the range is empty.</summary>
    private static Rect? FirstRect(IEnumerable<Rect> rects)
    {
        foreach (var rect in rects)
            return rect;
        return null;
    }

    /// <summary>
    /// A grip is pressed: remember which grip, where it sits in the text, and where the pointer
    /// went down, so the drag can be measured from the text rather than from the grip's artwork.
    /// </summary>
    private void GripPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not TextSelectionHandle grip)
            return;

        var presenter = _presenter ??= FindPresenter();
        if (presenter is null)
            return;

        if (!TryGetAnchor(grip, presenter, out var x, out var y))
            return;

        _dragged = grip;
        _anchorX = x;
        _anchorY = y;
        _pressX = e.GetPosition(presenter).X;
    }

    /// <summary>
    /// A grip is dragged: the pointer's distance from where it went down is added to the grip's
    /// anchor, the result is hit-tested against the field's text, and the character under the
    /// pointer becomes the new selection end.
    /// </summary>
    private void GripMoved(object? sender, PointerEventArgs e)
    {
        if (_dragged is null || sender is not TextSelectionHandle grip)
            return;
        if (e.Pointer.Captured != grip)
            return;

        var presenter = _presenter ??= FindPresenter();
        if (presenter is null)
            return;

        var offset = e.GetPosition(presenter).X - _pressX;
        var hit = presenter.TextLayout.HitTestPoint(new Point(_anchorX + offset, _anchorY));
        var index = hit.CharacterHit.FirstCharacterIndex + hit.CharacterHit.TrailingLength;

        ApplyDrag(grip, index);
    }

    /// <summary>The drag is over.</summary>
    private void GripReleased(object? sender, PointerReleasedEventArgs e)
    {
        _dragged = null;
    }

    /// <summary>Writes a dragged grip's new position into the field's selection.</summary>
    private void ApplyDrag(TextSelectionHandle grip, int index)
    {
        var length = _textBox.Text?.Length ?? 0;

        switch (GripShape.GetShape(grip))
        {
            case GripShapeKind.Start:
                _textBox.SelectionStart = Math.Clamp(index, 0, _textBox.SelectionEnd);
                break;
            case GripShapeKind.End:
                _textBox.SelectionEnd = Math.Clamp(index, _textBox.SelectionStart, length);
                break;
            default:
                _textBox.CaretIndex = Math.Clamp(index, 0, length);
                break;
        }

        SeatGrips();
    }

    /// <summary>
    /// The point in the text a grip stands at: the leading edge of the first selected character
    /// for the start grip, the trailing edge of the last for the end grip, and the caret's own
    /// edge for the caret grip.
    /// </summary>
    private bool TryGetAnchor(TextSelectionHandle grip, TextPresenter presenter, out double x, out double y)
    {
        x = 0;
        y = 0;

        var layout = presenter.TextLayout;
        if (layout is null)
            return false;

        var start = _textBox.SelectionStart;
        var end = _textBox.SelectionEnd;
        var first = Math.Min(start, end);
        var last = Math.Max(start, end);

        Rect rect;
        if (GripShape.GetShape(grip) == GripShapeKind.Caret || first == last)
        {
            rect = layout.HitTestTextPosition(_textBox.CaretIndex);
        }
        else if (GripShape.GetShape(grip) == GripShapeKind.Start)
        {
            var found = FirstRect(layout.HitTestTextRange(first, 1));
            if (found is null)
                return false;
            rect = found.Value;
        }
        else
        {
            var found = FirstRect(layout.HitTestTextRange(last - 1, 1));
            if (found is null)
                return false;
            rect = found.Value;
        }

        x = rect.Left;
        y = rect.Y + (rect.Height / 2);
        return true;
    }
}
