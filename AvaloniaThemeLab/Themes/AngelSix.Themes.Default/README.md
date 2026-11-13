# AngelSix.Themes.Default

A complete Avalonia 12 theme built on [AngelSix.ThemeEngine](https://www.nuget.org/packages/AngelSix.ThemeEngine). Every control reads its colours, sizes and fonts from one token class, `DefaultTheme`, so a new look is a subclass, not a rewrite.

## Use it

```bash
dotnet add package AngelSix.Themes.Default
```

In `App.axaml`, replace Fluent:

```xml
<Application.Styles>
    <DefaultTheme />
</Application.Styles>
```

No `xmlns` is needed: the theme maps onto Avalonia's own XML namespace. No code is needed either.

Keep `.WithInterFont()` in `Program.cs`; the theme's font is Inter.

## Accents

Give any control a hue:

```xml
<Window xmlns:theme="using:AngelSix.ThemeEngine.Generated">
    <Button Content="Save" theme:Accent.Kind="Primary" />
</Window>
```

Hues: `Primary`, `Success`, `Warning`, `Destructive`, `Error`, `Info`, `Favourite`, `Subtle`.

## Make it yours

Subclass `DefaultTheme`, change what you want, and include your class instead:

```csharp
[Theme(FallbackHue = "Neutral")]
public partial class BrandTheme : DefaultTheme
{
    [AccentHue, ColorRamp(HueAware = true)] public virtual Color AccentBrand => Color.Parse("#7C4DFF");
}
```

```xml
<Application.Styles>
    <local:BrandTheme />
</Application.Styles>
```

A subclass needs the generator, so reference `AngelSix.ThemeEngine` directly in that project. Styles files in that project (a `Styles` root, no `x:Class`) are loaded with it.

## Change it at runtime

The app has one theme. Get it with `Application.Current.Styles.OfType<DefaultTheme>().Single()`, set any token (`theme.BaseSize = 1.25`, `theme.IsDark = true`) and the app follows. `ResetToDefaults()` puts the tokens back.
