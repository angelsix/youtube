using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;

namespace Avalonia.Themes.Default;

/// <summary>
/// One colour shared by every colour control in a design preview, so the controls follow each other.
/// The preview's root panel carries it, seeded from the pane's accent:
/// <c>&lt;StackPanel x:Name="Sample" default:ColorSample.Seed="{default:AccentColor}"&gt;</c>, and each control
/// binds <c>HsvColor="{Binding #Sample.(default:ColorSample.HsvColor), Mode=TwoWay}"</c>.
/// </summary>
/// <remarks>
/// The controls share an <see cref="Media.HsvColor"/>, not a <see cref="Color"/>: a colour at zero saturation or
/// brightness has no hue in RGB, so a shared RGB colour would throw the hue strip back to red the moment either
/// drag reached zero. A new seed (another accent picked, or the pane's palette) replaces whatever was dragged.
/// </remarks>
public static class ColorSample
{
    /// <summary>The colour the sample starts from and returns to whenever it changes.</summary>
    public static readonly AttachedProperty<Color> SeedProperty =
        AvaloniaProperty.RegisterAttached<Control, Color>("Seed", typeof(ColorSample));

    /// <summary>The sample's current colour, which every control in the preview binds two-way.</summary>
    public static readonly AttachedProperty<HsvColor> HsvColorProperty =
        AvaloniaProperty.RegisterAttached<Control, HsvColor>("HsvColor", typeof(ColorSample), defaultBindingMode: BindingMode.TwoWay);

    static ColorSample()
    {
        // SetCurrentValue, so the seed never displaces the controls' two-way bindings
        SeedProperty.Changed.AddClassHandler<Control>((sample, e) =>
            sample.SetCurrentValue(HsvColorProperty, ((Color)e.NewValue!).ToHsv()));
    }

    public static Color GetSeed(Control element) => element.GetValue(SeedProperty);

    public static void SetSeed(Control element, Color value) => element.SetValue(SeedProperty, value);

    public static HsvColor GetHsvColor(Control element) => element.GetValue(HsvColorProperty);

    public static void SetHsvColor(Control element, HsvColor value) => element.SetValue(HsvColorProperty, value);
}
