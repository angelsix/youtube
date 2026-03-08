using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;

namespace BatchProcess3.Controls;

public partial class SortColumnHeader : UserControl
{
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<SortColumnHeader, string?>(nameof(Text));

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly StyledProperty<string?> SortArrowProperty =
        AvaloniaProperty.Register<SortColumnHeader, string?>(nameof(SortArrow));

    public string? SortArrow
    {
        get => GetValue(SortArrowProperty);
        set => SetValue(SortArrowProperty, value);
    }

    public static readonly StyledProperty<bool> IsSortActiveProperty =
        AvaloniaProperty.Register<SortColumnHeader, bool>(nameof(IsSortActive));

    public bool IsSortActive
    {
        get => GetValue(IsSortActiveProperty);
        set => SetValue(IsSortActiveProperty, value);
    }

    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<SortColumnHeader, ICommand?>(nameof(Command));

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<SortColumnHeader, object?>(nameof(CommandParameter));

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public SortColumnHeader()
    {
        InitializeComponent();
    }
}
