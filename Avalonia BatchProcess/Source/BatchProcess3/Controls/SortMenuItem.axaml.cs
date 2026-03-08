using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;

namespace BatchProcess3.Controls;

public partial class SortMenuItem : UserControl
{
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<SortMenuItem, string?>(nameof(Text));

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly StyledProperty<string?> SortArrowProperty =
        AvaloniaProperty.Register<SortMenuItem, string?>(nameof(SortArrow));

    public string? SortArrow
    {
        get => GetValue(SortArrowProperty);
        set => SetValue(SortArrowProperty, value);
    }

    public static readonly StyledProperty<bool> IsSortActiveProperty =
        AvaloniaProperty.Register<SortMenuItem, bool>(nameof(IsSortActive));

    public bool IsSortActive
    {
        get => GetValue(IsSortActiveProperty);
        set => SetValue(IsSortActiveProperty, value);
    }

    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<SortMenuItem, ICommand?>(nameof(Command));

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<SortMenuItem, object?>(nameof(CommandParameter));

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public SortMenuItem()
    {
        InitializeComponent();
    }
}
