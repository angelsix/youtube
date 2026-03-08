using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace BatchProcess3.Controls;

public partial class SummaryCard : UserControl
{
    public static readonly StyledProperty<string?> HeaderProperty =
        AvaloniaProperty.Register<SummaryCard, string?>(nameof(Header));

    public string? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly StyledProperty<string?> ValueProperty =
        AvaloniaProperty.Register<SummaryCard, string?>(nameof(Value));

    public string? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly StyledProperty<string?> SubtitleProperty =
        AvaloniaProperty.Register<SummaryCard, string?>(nameof(Subtitle));

    public string? Subtitle
    {
        get => GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public static readonly StyledProperty<Geometry?> IconDataProperty =
        AvaloniaProperty.Register<SummaryCard, Geometry?>(nameof(IconData));

    public Geometry? IconData
    {
        get => GetValue(IconDataProperty);
        set => SetValue(IconDataProperty, value);
    }

    public static readonly StyledProperty<IBrush?> AccentBrushProperty =
        AvaloniaProperty.Register<SummaryCard, IBrush?>(nameof(AccentBrush));

    public IBrush? AccentBrush
    {
        get => GetValue(AccentBrushProperty);
        set => SetValue(AccentBrushProperty, value);
    }

    public static readonly StyledProperty<IBrush?> CardGradientProperty =
        AvaloniaProperty.Register<SummaryCard, IBrush?>(nameof(CardGradient));

    public IBrush? CardGradient
    {
        get => GetValue(CardGradientProperty);
        set => SetValue(CardGradientProperty, value);
    }

    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<SummaryCard, bool>(nameof(IsActive));

    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<SummaryCard, ICommand?>(nameof(Command));

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<SummaryCard, object?>(nameof(CommandParameter));

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public SummaryCard()
    {
        InitializeComponent();
    }
}
