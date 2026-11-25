using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;
using AngelSix.ThemeEngine;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AngelSix.Themes.Default;

/// <summary>
/// Displays a vector icon (from a theme icon name or <see cref="Geometry"/>) or an <see cref="IImage"/>.
/// Defaults to a square using <see cref="Size"/> while still allowing independent <see cref="Layoutable.Width"/> and <see cref="Layoutable.Height"/> overrides.
/// </summary>
public class Icon : Control
{
    #region Fields

    private static readonly ConcurrentDictionary<(Type GalleryType, string Name), StreamGeometry?> geometryCache = new();

    private ThemeContext? _subscribedContext;
    private Geometry? _resolvedGeometry;
    private IImage? _resolvedImage;

    #endregion Fields

    #region Properties

    /// <summary>Defines the <see cref="Content"/> property.</summary>
    public static readonly StyledProperty<object?> ContentProperty =
        AvaloniaProperty.Register<Icon, object?>(nameof(Content));

    /// <summary>Defines the <see cref="Size"/> property.</summary>
    public static readonly StyledProperty<double> SizeProperty =
        AvaloniaProperty.Register<Icon, double>(nameof(Size), double.NaN);

    /// <summary>Defines the <see cref="Foreground"/> property.</summary>
    public static readonly StyledProperty<IBrush?> ForegroundProperty =
        TextElement.ForegroundProperty.AddOwner<Icon>();

    /// <summary>
    /// The icon content: a theme icon name (<see cref="string"/>), a <see cref="Geometry"/> (e.g. <c>{icons:ArrowDown}</c>),
    /// or an <see cref="IImage"/>.
    /// </summary>
    [Content]
    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    /// <summary>
    /// Square icon size applied to both width and height unless <see cref="Layoutable.Width"/> or <see cref="Layoutable.Height"/> is set explicitly.
    /// </summary>
    public double Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    /// <summary>The brush used to fill vector icon geometry; inherits from the parent element by default.</summary>
    public IBrush? Foreground
    {
        get => GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    #endregion Properties

    #region Constructors

    static Icon()
    {
        AffectsRender<Icon>(ContentProperty, ForegroundProperty, SizeProperty, WidthProperty, HeightProperty);
        AffectsMeasure<Icon>(ContentProperty, SizeProperty, WidthProperty, HeightProperty);
        HorizontalAlignmentProperty.OverrideDefaultValue<Icon>(HorizontalAlignment.Center);
        VerticalAlignmentProperty.OverrideDefaultValue<Icon>(VerticalAlignment.Center);
    }

    #endregion Constructors

    #region Methods

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        SubscribeToTheme();
        ResolveContent();
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        UnsubscribeFromTheme();
        base.OnDetachedFromVisualTree(e);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ContentProperty)
            ResolveContent();
    }

    /// <inheritdoc />
    protected override Size MeasureOverride(Size availableSize)
    {
        if (_resolvedGeometry is null && _resolvedImage is null)
            return default;

        var defaultSize = double.IsNaN(Size) ? 0.0 : Size;
        var width = !double.IsNaN(Width) ? Width : defaultSize;
        var height = !double.IsNaN(Height) ? Height : defaultSize;

        if (!double.IsInfinity(availableSize.Width))
            width = Math.Min(width, availableSize.Width);
        if (!double.IsInfinity(availableSize.Height))
            height = Math.Min(height, availableSize.Height);

        return new Size(Math.Max(0.0, width), Math.Max(0.0, height));
    }

    /// <inheritdoc />
    public override void Render(DrawingContext context)
    {
        var bounds = Bounds;
        if (bounds.Width <= 0 || bounds.Height <= 0)
            return;

        if (_resolvedGeometry is { } geometry)
        {
            var foreground = Foreground;
            var geomBounds = geometry.Bounds;
            if (foreground is null || geomBounds.Width <= 0 || geomBounds.Height <= 0)
                return;

            var scale = Math.Min(bounds.Width / geomBounds.Width, bounds.Height / geomBounds.Height);
            var tx = (bounds.Width - geomBounds.Width * scale) * 0.5 - geomBounds.X * scale;
            var ty = (bounds.Height - geomBounds.Height * scale) * 0.5 - geomBounds.Y * scale;

            using (context.PushTransform(Matrix.CreateScale(scale, scale) * Matrix.CreateTranslation(tx, ty)))
                context.DrawGeometry(foreground, null, geometry);
        }
        else if (_resolvedImage is { } image)
        {
            var imgSize = image.Size;
            if (imgSize.Width <= 0 || imgSize.Height <= 0)
                return;

            var scale = Math.Min(bounds.Width / imgSize.Width, bounds.Height / imgSize.Height);
            var drawWidth = imgSize.Width * scale;
            var drawHeight = imgSize.Height * scale;
            var destRect = new Rect(
                (bounds.Width - drawWidth) * 0.5,
                (bounds.Height - drawHeight) * 0.5,
                drawWidth,
                drawHeight);

            context.DrawImage(image, destRect);
        }
    }

    private void SubscribeToTheme()
    {
        UnsubscribeFromTheme();
        var context = ThemeContext.Resolver?.Invoke();
        if (context is null)
            return;

        _subscribedContext = context;
        context.PropertyChanged += OnThemeContextPropertyChanged;
    }

    private void UnsubscribeFromTheme()
    {
        if (_subscribedContext is null)
            return;

        _subscribedContext.PropertyChanged -= OnThemeContextPropertyChanged;
        _subscribedContext = null;
    }

    private void OnThemeContextPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (Content is string)
        {
            ResolveContent();
            InvalidateMeasure();
            InvalidateVisual();
        }
    }

    private void ResolveContent()
    {
        switch (Content)
        {
            case Geometry geometry:
                _resolvedGeometry = geometry;
                _resolvedImage = null;
                break;

            case IImage image:
                _resolvedGeometry = null;
                _resolvedImage = image;
                break;

            case string name when !string.IsNullOrWhiteSpace(name):
                _resolvedGeometry = ResolveNamedIcon(name);
                _resolvedImage = null;
                break;

            default:
                _resolvedGeometry = null;
                _resolvedImage = null;
                break;
        }
    }

    private static StreamGeometry? ResolveNamedIcon(string name)
    {
        var activeTheme = ThemeContext.Resolver?.Invoke()?.Active as ThemeBase;
        var icons = activeTheme?.Icons;
        if (icons is null)
            return null;

        var key = (icons.GetType(), name);
        if (geometryCache.TryGetValue(key, out var cached))
            return cached;

        var property = key.Item1.GetProperty(
            name,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

        var path = property?.GetValue(icons) as string;
        if (string.IsNullOrWhiteSpace(path))
            return null;

        try
        {
            var parsed = StreamGeometry.Parse(path);
            geometryCache[key] = parsed;
            return parsed;
        }
        catch
        {
            return null;
        }
    }

    #endregion Methods
}
