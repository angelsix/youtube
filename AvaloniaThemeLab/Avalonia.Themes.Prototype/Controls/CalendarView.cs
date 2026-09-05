using System.Globalization;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace Avalonia.Themes.Prototype;

/// <summary>
/// A single-control calendar: an inline month view with month/year/decade zoom, single or range
/// selection with a hover preview, event dot markers, and an optional picker presentation where
/// the calendar opens from a text box in a popup.
/// </summary>
/// <remarks>
/// <para>
/// Replaces the five-control built-in calendar family. All day, month and year cells are
/// <see cref="CalendarViewCell"/> instances the control creates itself, so the whole look lives
/// in one ControlTheme file.
/// </para>
/// <para>
/// Pseudo-classes: <c>:picker</c> (picker presentation), <c>:open</c> (picker popup showing),
/// <c>:year</c> and <c>:decade</c> (zoomed-out views).
/// </para>
/// </remarks>
[TemplatePart(PartHeaderButton, typeof(Button))]
[TemplatePart(PartPreviousButton, typeof(Button))]
[TemplatePart(PartNextButton, typeof(Button))]
[TemplatePart(PartDayTitles, typeof(UniformGrid))]
[TemplatePart(PartMonthGrid, typeof(UniformGrid))]
[TemplatePart(PartZoomGrid, typeof(UniformGrid))]
[TemplatePart(PartTextBox, typeof(TextBox))]
[TemplatePart(PartPickerButton, typeof(Button))]
[TemplatePart(PartPopup, typeof(Popup))]
public class CalendarView : TemplatedControl
{
    private const string PartHeaderButton = "PART_HeaderButton";
    private const string PartPreviousButton = "PART_PreviousButton";
    private const string PartNextButton = "PART_NextButton";
    private const string PartDayTitles = "PART_DayTitles";
    private const string PartMonthGrid = "PART_MonthGrid";
    private const string PartZoomGrid = "PART_ZoomGrid";
    private const string PartTextBox = "PART_TextBox";
    private const string PartPickerButton = "PART_PickerButton";
    private const string PartPopup = "PART_Popup";

    /// <summary>Defines the <see cref="DisplayDate"/> property.</summary>
    public static readonly StyledProperty<DateTime> DisplayDateProperty =
        AvaloniaProperty.Register<CalendarView, DateTime>(nameof(DisplayDate), DateTime.Today,
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>Defines the <see cref="DisplayMode"/> property.</summary>
    public static readonly StyledProperty<CalendarViewMode> DisplayModeProperty =
        AvaloniaProperty.Register<CalendarView, CalendarViewMode>(nameof(DisplayMode));

    /// <summary>Defines the <see cref="SelectionMode"/> property.</summary>
    public static readonly StyledProperty<CalendarViewSelectionMode> SelectionModeProperty =
        AvaloniaProperty.Register<CalendarView, CalendarViewSelectionMode>(nameof(SelectionMode),
            CalendarViewSelectionMode.Single);

    /// <summary>Defines the <see cref="SelectedDate"/> property.</summary>
    public static readonly StyledProperty<DateTime?> SelectedDateProperty =
        AvaloniaProperty.Register<CalendarView, DateTime?>(nameof(SelectedDate),
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>Defines the <see cref="RangeStart"/> property.</summary>
    public static readonly StyledProperty<DateTime?> RangeStartProperty =
        AvaloniaProperty.Register<CalendarView, DateTime?>(nameof(RangeStart),
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>Defines the <see cref="RangeEnd"/> property.</summary>
    public static readonly StyledProperty<DateTime?> RangeEndProperty =
        AvaloniaProperty.Register<CalendarView, DateTime?>(nameof(RangeEnd),
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>Defines the <see cref="MinDate"/> property.</summary>
    public static readonly StyledProperty<DateTime?> MinDateProperty =
        AvaloniaProperty.Register<CalendarView, DateTime?>(nameof(MinDate));

    /// <summary>Defines the <see cref="MaxDate"/> property.</summary>
    public static readonly StyledProperty<DateTime?> MaxDateProperty =
        AvaloniaProperty.Register<CalendarView, DateTime?>(nameof(MaxDate));

    /// <summary>Defines the <see cref="IsDateDisabled"/> property.</summary>
    public static readonly StyledProperty<Func<DateTime, bool>?> IsDateDisabledProperty =
        AvaloniaProperty.Register<CalendarView, Func<DateTime, bool>?>(nameof(IsDateDisabled));

    /// <summary>Defines the <see cref="FirstDayOfWeek"/> property.</summary>
    public static readonly StyledProperty<DayOfWeek> FirstDayOfWeekProperty =
        AvaloniaProperty.Register<CalendarView, DayOfWeek>(nameof(FirstDayOfWeek),
            CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);

    /// <summary>Defines the <see cref="IsTodayHighlighted"/> property.</summary>
    public static readonly StyledProperty<bool> IsTodayHighlightedProperty =
        AvaloniaProperty.Register<CalendarView, bool>(nameof(IsTodayHighlighted), true);

    /// <summary>Defines the <see cref="Markers"/> property.</summary>
    public static readonly StyledProperty<IEnumerable<CalendarViewMarker>?> MarkersProperty =
        AvaloniaProperty.Register<CalendarView, IEnumerable<CalendarViewMarker>?>(nameof(Markers));

    /// <summary>Defines the <see cref="MarkerBrush"/> property.</summary>
    public static readonly StyledProperty<IBrush?> MarkerBrushProperty =
        AvaloniaProperty.Register<CalendarView, IBrush?>(nameof(MarkerBrush));

    /// <summary>Defines the <see cref="DisplayAsPicker"/> property.</summary>
    public static readonly StyledProperty<bool> DisplayAsPickerProperty =
        AvaloniaProperty.Register<CalendarView, bool>(nameof(DisplayAsPicker));

    /// <summary>Defines the <see cref="HeaderBackground"/> property.</summary>
    public static readonly StyledProperty<IBrush?> HeaderBackgroundProperty =
        AvaloniaProperty.Register<CalendarView, IBrush?>(nameof(HeaderBackground));

    /// <summary>Defines the <see cref="HeaderText"/> property.</summary>
    public static readonly DirectProperty<CalendarView, string> HeaderTextProperty =
        AvaloniaProperty.RegisterDirect<CalendarView, string>(nameof(HeaderText), o => o.HeaderText);

    private readonly CalendarSelectionEngine _engine = new();
    private readonly List<CalendarViewCell> _dayCells = [];
    private readonly List<CalendarViewCell> _zoomCells = [];
    private readonly List<CalendarViewCell> _titleCells = [];

    private Button? _headerButton;
    private Button? _previousButton;
    private Button? _nextButton;
    private UniformGrid? _dayTitles;
    private UniformGrid? _monthGrid;
    private UniformGrid? _zoomGrid;
    private TextBox? _textBox;
    private Button? _pickerButton;
    private Popup? _popup;

    private string _headerText = string.Empty;
    private DateTime? _focusedDate;
    private bool _syncingSelection;

    /// <summary>The month, year or decade currently displayed.</summary>
    public DateTime DisplayDate
    {
        get => GetValue(DisplayDateProperty);
        set => SetValue(DisplayDateProperty, value);
    }

    /// <summary>The zoom level currently displayed. The header button zooms out.</summary>
    public CalendarViewMode DisplayMode
    {
        get => GetValue(DisplayModeProperty);
        set => SetValue(DisplayModeProperty, value);
    }

    /// <summary>How day clicks select: not at all, a single day, or a range.</summary>
    public CalendarViewSelectionMode SelectionMode
    {
        get => GetValue(SelectionModeProperty);
        set => SetValue(SelectionModeProperty, value);
    }

    /// <summary>The selected day in <see cref="CalendarViewSelectionMode.Single"/> mode.</summary>
    public DateTime? SelectedDate
    {
        get => GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    /// <summary>The start of the selected range in <see cref="CalendarViewSelectionMode.Range"/> mode.</summary>
    public DateTime? RangeStart
    {
        get => GetValue(RangeStartProperty);
        set => SetValue(RangeStartProperty, value);
    }

    /// <summary>The end of the selected range; null while a start awaits its end.</summary>
    public DateTime? RangeEnd
    {
        get => GetValue(RangeEndProperty);
        set => SetValue(RangeEndProperty, value);
    }

    /// <summary>The earliest selectable day, or null for no lower bound.</summary>
    public DateTime? MinDate
    {
        get => GetValue(MinDateProperty);
        set => SetValue(MinDateProperty, value);
    }

    /// <summary>The latest selectable day, or null for no upper bound.</summary>
    public DateTime? MaxDate
    {
        get => GetValue(MaxDateProperty);
        set => SetValue(MaxDateProperty, value);
    }

    /// <summary>A predicate marking individual days unselectable, e.g. weekends.</summary>
    public Func<DateTime, bool>? IsDateDisabled
    {
        get => GetValue(IsDateDisabledProperty);
        set => SetValue(IsDateDisabledProperty, value);
    }

    /// <summary>The day the week starts on. Defaults from the current culture.</summary>
    public DayOfWeek FirstDayOfWeek
    {
        get => GetValue(FirstDayOfWeekProperty);
        set => SetValue(FirstDayOfWeekProperty, value);
    }

    /// <summary>Whether today's cell is visually highlighted.</summary>
    public bool IsTodayHighlighted
    {
        get => GetValue(IsTodayHighlightedProperty);
        set => SetValue(IsTodayHighlightedProperty, value);
    }

    /// <summary>The event markers rendered as dots under their day numbers.</summary>
    public IEnumerable<CalendarViewMarker>? Markers
    {
        get => GetValue(MarkersProperty);
        set => SetValue(MarkersProperty, value);
    }

    /// <summary>The dot brush for markers that do not carry their own. Set by the theme.</summary>
    public IBrush? MarkerBrush
    {
        get => GetValue(MarkerBrushProperty);
        set => SetValue(MarkerBrushProperty, value);
    }

    /// <summary>
    /// When true the control renders as a text box with a button, and the calendar opens in a
    /// popup underneath — the date-picker presentation, from the same control.
    /// </summary>
    public bool DisplayAsPicker
    {
        get => GetValue(DisplayAsPickerProperty);
        set => SetValue(DisplayAsPickerProperty, value);
    }

    /// <summary>The background behind the header row.</summary>
    public IBrush? HeaderBackground
    {
        get => GetValue(HeaderBackgroundProperty);
        set => SetValue(HeaderBackgroundProperty, value);
    }

    /// <summary>The header label: month name, year, or decade span.</summary>
    public string HeaderText
    {
        get => _headerText;
        private set => SetAndRaise(HeaderTextProperty, ref _headerText, value);
    }

    /// <inheritdoc />
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_headerButton is not null)
            _headerButton.Click -= OnHeaderClick;
        if (_previousButton is not null)
            _previousButton.Click -= OnPreviousClick;
        if (_nextButton is not null)
            _nextButton.Click -= OnNextClick;
        if (_pickerButton is not null)
            _pickerButton.Click -= OnPickerButtonClick;
        if (_textBox is not null)
        {
            _textBox.LostFocus -= OnTextBoxLostFocus;
            _textBox.KeyDown -= OnTextBoxKeyDown;
        }
        if (_popup is not null)
        {
            _popup.Opened -= OnPopupOpened;
            _popup.Closed -= OnPopupClosed;
        }

        _headerButton = e.NameScope.Find<Button>(PartHeaderButton);
        _previousButton = e.NameScope.Find<Button>(PartPreviousButton);
        _nextButton = e.NameScope.Find<Button>(PartNextButton);
        _dayTitles = e.NameScope.Find<UniformGrid>(PartDayTitles);
        _monthGrid = e.NameScope.Find<UniformGrid>(PartMonthGrid);
        _zoomGrid = e.NameScope.Find<UniformGrid>(PartZoomGrid);
        _textBox = e.NameScope.Find<TextBox>(PartTextBox);
        _pickerButton = e.NameScope.Find<Button>(PartPickerButton);
        _popup = e.NameScope.Find<Popup>(PartPopup);

        if (_headerButton is not null)
            _headerButton.Click += OnHeaderClick;
        if (_previousButton is not null)
            _previousButton.Click += OnPreviousClick;
        if (_nextButton is not null)
            _nextButton.Click += OnNextClick;
        if (_pickerButton is not null)
            _pickerButton.Click += OnPickerButtonClick;
        if (_textBox is not null)
        {
            _textBox.LostFocus += OnTextBoxLostFocus;
            _textBox.KeyDown += OnTextBoxKeyDown;
        }
        if (_popup is not null)
        {
            _popup.Opened += OnPopupOpened;
            _popup.Closed += OnPopupClosed;
        }

        CreateCells();
        _engine.Load(RangeStart, RangeEnd);
        Rebuild();
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == DisplayAsPickerProperty)
            PseudoClasses.Set(":picker", DisplayAsPicker);

        if (change.Property == DisplayModeProperty)
        {
            PseudoClasses.Set(":year", DisplayMode == CalendarViewMode.Year);
            PseudoClasses.Set(":decade", DisplayMode == CalendarViewMode.Decade);
        }

        if (change.Property == RangeStartProperty || change.Property == RangeEndProperty)
        {
            if (!_syncingSelection)
                _engine.Load(RangeStart, RangeEnd);

            if (change.Property == RangeEndProperty && RangeEnd is not null)
                ClosePopupAfterSelection();
        }

        if (change.Property == SelectedDateProperty && SelectedDate is not null)
            ClosePopupAfterSelection();

        if (change.Property == DisplayDateProperty ||
            change.Property == DisplayModeProperty ||
            change.Property == SelectionModeProperty ||
            change.Property == SelectedDateProperty ||
            change.Property == RangeStartProperty ||
            change.Property == RangeEndProperty ||
            change.Property == MinDateProperty ||
            change.Property == MaxDateProperty ||
            change.Property == IsDateDisabledProperty ||
            change.Property == FirstDayOfWeekProperty ||
            change.Property == IsTodayHighlightedProperty ||
            change.Property == MarkersProperty ||
            change.Property == MarkerBrushProperty)
            Rebuild();
    }

    /// <inheritdoc />
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Handled || DisplayAsPicker || DisplayMode != CalendarViewMode.Month)
            return;

        var focused = _focusedDate ?? SelectedDate?.Date ?? DateTime.Today;

        var moved = e.Key switch
        {
            Key.Left => focused.AddDays(-1),
            Key.Right => focused.AddDays(1),
            Key.Up => focused.AddDays(-7),
            Key.Down => focused.AddDays(7),
            Key.PageUp => focused.AddMonths(-1),
            Key.PageDown => focused.AddMonths(1),
            _ => (DateTime?)null
        };

        if (moved is { } target)
        {
            _focusedDate = target;

            if (target.Year != DisplayDate.Year || target.Month != DisplayDate.Month)
                DisplayDate = target;

            Rebuild();
            e.Handled = true;
            return;
        }

        if (e.Key is Key.Enter or Key.Space && _focusedDate is { } date)
        {
            SelectDay(date);
            e.Handled = true;
        }
    }

    /// <inheritdoc />
    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        _engine.ClearHover();
        Rebuild();
    }

    // Creates the fixed cell pools once per template application.
    private void CreateCells()
    {
        _titleCells.Clear();
        _dayCells.Clear();
        _zoomCells.Clear();

        if (_dayTitles is not null)
        {
            _dayTitles.Children.Clear();
            for (var i = 0; i < 7; i++)
            {
                var cell = new CalendarViewCell { Focusable = false };
                cell.Classes.Add("title");
                _titleCells.Add(cell);
                _dayTitles.Children.Add(cell);
            }
        }

        if (_monthGrid is not null)
        {
            _monthGrid.Children.Clear();
            for (var i = 0; i < CalendarSelectionEngine.MonthCellCount; i++)
            {
                var cell = new CalendarViewCell { Focusable = false };
                cell.PointerPressed += OnDayCellPressed;
                cell.PointerEntered += OnDayCellEntered;
                _dayCells.Add(cell);
                _monthGrid.Children.Add(cell);
            }
        }

        if (_zoomGrid is not null)
        {
            _zoomGrid.Children.Clear();
            for (var i = 0; i < CalendarSelectionEngine.ZoomCellCount; i++)
            {
                var cell = new CalendarViewCell { Focusable = false };
                cell.Classes.Add("zoom");
                cell.PointerPressed += OnZoomCellPressed;
                _zoomCells.Add(cell);
                _zoomGrid.Children.Add(cell);
            }
        }
    }

    // Refreshes header, titles and every cell from current state. Cheap at 61 cells.
    private void Rebuild()
    {
        HeaderText = CalendarSelectionEngine.HeaderText(DisplayDate, DisplayMode, CultureInfo.CurrentCulture);
        UpdateTextBox();

        var titles = CalendarSelectionEngine.DayTitles(FirstDayOfWeek, CultureInfo.CurrentCulture);
        for (var i = 0; i < _titleCells.Count; i++)
            _titleCells[i].Content = titles[i];

        RebuildDays();
        RebuildZoom();
    }

    private void RebuildDays()
    {
        if (_dayCells.Count == 0)
            return;

        var markerLookup = BuildMarkerLookup();
        var dates = CalendarSelectionEngine.MonthCells(DisplayDate.Year, DisplayDate.Month, FirstDayOfWeek);
        var today = DateTime.Today;
        var isRange = SelectionMode == CalendarViewSelectionMode.Range;

        for (var i = 0; i < _dayCells.Count; i++)
        {
            var cell = _dayCells[i];
            var date = dates[i];
            cell.Date = date;
            cell.Content = date.Day.ToString(CultureInfo.CurrentCulture);
            cell.Markers = markerLookup?.GetValueOrDefault(date);

            var selectable = CalendarSelectionEngine.IsSelectable(date, MinDate, MaxDate, IsDateDisabled);
            var isStart = isRange && _engine.RangeStart == date;
            var isEnd = isRange && _engine.RangeEnd == date;

            cell.SetState(":inactive", date.Month != DisplayDate.Month);
            cell.SetState(":today", IsTodayHighlighted && date == today);
            cell.SetState(":blackout", !selectable);
            cell.SetState(":selected",
                (SelectionMode == CalendarViewSelectionMode.Single && SelectedDate?.Date == date) ||
                isStart || isEnd);
            cell.SetState(":range-start", isStart);
            cell.SetState(":range-end", isEnd);
            cell.SetState(":in-range", isRange && _engine.IsInRange(date));
            cell.SetState(":preview", isRange && _engine.IsInPreview(date));
            cell.SetState(":day-focused", _focusedDate == date && IsKeyboardFocusWithin);
        }
    }

    private void RebuildZoom()
    {
        if (_zoomCells.Count == 0 || DisplayMode == CalendarViewMode.Month)
            return;

        var culture = CultureInfo.CurrentCulture;

        if (DisplayMode == CalendarViewMode.Year)
        {
            for (var i = 0; i < _zoomCells.Count; i++)
            {
                var cell = _zoomCells[i];
                var date = new DateTime(DisplayDate.Year, i + 1, 1);
                cell.Date = date;
                cell.Content = culture.DateTimeFormat.GetAbbreviatedMonthName(i + 1);
                cell.SetState(":inactive", false);
                cell.SetState(":selected", DisplayDate.Month == i + 1);
                cell.SetState(":today", false);
                cell.SetState(":blackout", false);
            }
        }
        else
        {
            var decadeStart = CalendarSelectionEngine.DecadeStart(DisplayDate.Year);
            for (var i = 0; i < _zoomCells.Count; i++)
            {
                var cell = _zoomCells[i];
                var year = decadeStart - 1 + i;
                cell.Date = new DateTime(year, 1, 1);
                cell.Content = year.ToString(culture);
                cell.SetState(":inactive", year < decadeStart || year > decadeStart + 9);
                cell.SetState(":selected", DisplayDate.Year == year);
                cell.SetState(":today", false);
                cell.SetState(":blackout", false);
            }
        }
    }

    private Dictionary<DateTime, IReadOnlyList<IBrush>>? BuildMarkerLookup()
    {
        if (Markers is null)
            return null;

        Dictionary<DateTime, IReadOnlyList<IBrush>>? lookup = null;

        foreach (var marker in Markers)
        {
            var brush = marker.Brush ?? MarkerBrush;
            if (brush is null)
                continue;

            lookup ??= [];
            var date = marker.Date.Date;
            if (lookup.TryGetValue(date, out var existing))
                ((List<IBrush>)existing).Add(brush);
            else
                lookup[date] = new List<IBrush> { brush };
        }

        return lookup;
    }

    private void SelectDay(DateTime date)
    {
        date = date.Date;

        if (SelectionMode == CalendarViewSelectionMode.None ||
            !CalendarSelectionEngine.IsSelectable(date, MinDate, MaxDate, IsDateDisabled))
            return;

        _focusedDate = date;

        if (SelectionMode == CalendarViewSelectionMode.Single)
        {
            SelectedDate = date;
            Rebuild();
            return;
        }

        _engine.Click(date);
        _syncingSelection = true;
        try
        {
            RangeStart = _engine.RangeStart;
            RangeEnd = _engine.RangeEnd;
        }
        finally
        {
            _syncingSelection = false;
        }

        Rebuild();
    }

    private void OnDayCellPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is CalendarViewCell cell)
        {
            SelectDay(cell.Date);
            e.Handled = true;
        }
    }

    private void OnDayCellEntered(object? sender, PointerEventArgs e)
    {
        if (SelectionMode != CalendarViewSelectionMode.Range || !_engine.IsSelecting)
            return;

        if (sender is CalendarViewCell cell)
        {
            _engine.Hover(cell.Date);
            Rebuild();
        }
    }

    private void OnZoomCellPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not CalendarViewCell cell)
            return;

        if (DisplayMode == CalendarViewMode.Year)
        {
            DisplayDate = new DateTime(cell.Date.Year, cell.Date.Month, 1);
            DisplayMode = CalendarViewMode.Month;
        }
        else
        {
            DisplayDate = new DateTime(cell.Date.Year, DisplayDate.Month, 1);
            DisplayMode = CalendarViewMode.Year;
        }

        e.Handled = true;
    }

    private void OnHeaderClick(object? sender, RoutedEventArgs e)
    {
        DisplayMode = DisplayMode switch
        {
            CalendarViewMode.Month => CalendarViewMode.Year,
            _ => CalendarViewMode.Decade
        };
    }

    private void OnPreviousClick(object? sender, RoutedEventArgs e) => MoveDisplay(-1);

    private void OnNextClick(object? sender, RoutedEventArgs e) => MoveDisplay(1);

    private void MoveDisplay(int direction)
    {
        DisplayDate = DisplayMode switch
        {
            CalendarViewMode.Month => DisplayDate.AddMonths(direction),
            CalendarViewMode.Year => DisplayDate.AddYears(direction),
            _ => DisplayDate.AddYears(direction * 10)
        };
    }

    private void OnPickerButtonClick(object? sender, RoutedEventArgs e)
    {
        if (_popup is not null)
            _popup.IsOpen = !_popup.IsOpen;
    }

    private void OnPopupOpened(object? sender, EventArgs e) => PseudoClasses.Set(":open", true);

    private void OnPopupClosed(object? sender, EventArgs e) => PseudoClasses.Set(":open", false);

    private void ClosePopupAfterSelection()
    {
        if (DisplayAsPicker && _popup is { IsOpen: true })
            _popup.IsOpen = false;
    }

    private void UpdateTextBox()
    {
        if (_textBox is null || !DisplayAsPicker)
            return;

        var culture = CultureInfo.CurrentCulture;
        _textBox.Text = SelectionMode switch
        {
            CalendarViewSelectionMode.Range when RangeStart is { } start && RangeEnd is { } end =>
                $"{start.ToString("d", culture)} – {end.ToString("d", culture)}",
            CalendarViewSelectionMode.Range when RangeStart is { } start =>
                start.ToString("d", culture),
            _ => SelectedDate?.ToString("d", culture) ?? string.Empty
        };
    }

    private void OnTextBoxLostFocus(object? sender, RoutedEventArgs e) => CommitTextBox();

    private void OnTextBoxKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            CommitTextBox();
            e.Handled = true;
        }
    }

    // Parses typed input in single mode; anything unparseable reverts to the current selection.
    private void CommitTextBox()
    {
        if (_textBox is null || SelectionMode != CalendarViewSelectionMode.Single)
            return;

        if (DateTime.TryParse(_textBox.Text, CultureInfo.CurrentCulture, DateTimeStyles.None, out var parsed) &&
            CalendarSelectionEngine.IsSelectable(parsed, MinDate, MaxDate, IsDateDisabled))
        {
            SelectedDate = parsed.Date;
            DisplayDate = parsed.Date;
        }
        else
        {
            UpdateTextBox();
        }
    }
}
