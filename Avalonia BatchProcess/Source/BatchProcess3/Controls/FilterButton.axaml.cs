using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace BatchProcess3.Controls;

public partial class FilterButton : UserControl
{
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<FilterButton, string?>(nameof(Text));

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly StyledProperty<IBrush?> AccentBrushProperty =
        AvaloniaProperty.Register<FilterButton, IBrush?>(nameof(AccentBrush));

    public IBrush? AccentBrush
    {
        get => GetValue(AccentBrushProperty);
        set => SetValue(AccentBrushProperty, value);
    }

    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<FilterButton, bool>(nameof(IsActive));

    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<FilterButton, ICommand?>(nameof(Command));

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<FilterButton, object?>(nameof(CommandParameter));

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public FilterButton()
    {
        InitializeComponent();
    }
}
