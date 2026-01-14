using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace BatchProcess3.Controls;

public partial class HeaderBanner : UserControl
{
    public static readonly StyledProperty<IImage?> BackgroundImageProperty = AvaloniaProperty.Register<HeaderBanner, IImage?>(
        nameof(BackgroundImage));

    public IImage? BackgroundImage
    {
        get => GetValue(BackgroundImageProperty);
        set => SetValue(BackgroundImageProperty, value);
    }
    
    public static readonly StyledProperty<string?> HeaderProperty = AvaloniaProperty.Register<HeaderBanner, string?>(
        nameof(Header));

    public string? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly StyledProperty<string?> SubHeaderAProperty = AvaloniaProperty.Register<HeaderBanner, string?>(
        nameof(SubHeaderA));

    public string? SubHeaderA
    {
        get => GetValue(SubHeaderAProperty);
        set => SetValue(SubHeaderAProperty, value);
    }

    public static readonly StyledProperty<string?> SubHeaderBProperty = AvaloniaProperty.Register<HeaderBanner, string?>(
        nameof(SubHeaderB));

    public string? SubHeaderB
    {
        get => GetValue(SubHeaderBProperty);
        set => SetValue(SubHeaderBProperty, value);
    }
    
    public HeaderBanner()
    {
        InitializeComponent();
        
        // Detect design time
        if (Avalonia.Controls.Design.IsDesignMode)
        {
            if (Application.Current != null)
            {
                if (Application.Current.TryFindResource("PrimaryForeground", out var foreground))
                    SetValue(ForegroundProperty, foreground);
                if (Application.Current.TryFindResource("PrimaryBackground", out var background))
                    SetValue(BackgroundProperty, background);
            }
            
            Header = "Big Header";
            SubHeaderA = "Sub Header A";
            SubHeaderB = "Less Important Sub Header";
;       }
        
        DataContext = this;
    }
}