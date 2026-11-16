# AvaloniaThemeLab.Icons

The worked example for custom icons: the default theme, one gallery of your own, and the three ways a gallery gains icons — SVG files, hand-written paths, and overrides that redraw a default.

## What is here

| File | What it does |
|---|---|
| `Theme/IconsTheme.cs` | The whole feature: `IconsTheme : DefaultTheme` hands `Icons` to `AppIcons`, a gallery deriving from `ThemeIconGallery`. `Icons/Svg/*.svg` become tokens; `Heart` is a hand-written path; `CoffeeCup` aliases `Coffee`; `ChevronDown` is overridden, so every combo-box arrow in the app draws the new shape. |
| `Icons/Svg/` | Three path-only SVGs — `Label.svg`, `Coffee.svg`, `Rocket.svg` — each one file, one `{icons:Name}`. |
| `Views/MainWindow.axaml` | Every form side by side: default icons still alive, file icons, hand-written icons, and `theme:Icon.Data` on buttons. |

## Running it

```
dotnet run --project Examples/AvaloniaThemeLab.Icons
```

## The rules this example shows

- A gallery for a theme extending `DefaultTheme` derives from `ThemeIconGallery` — that keeps every default icon and adds yours to it. See *Icons* in the [guide](../../ThemeEngine.html).
- `SvgFolder` is read from the project folder, recursively; `.svg` is the format it reads today.
- The SVG reader takes `<path d="…">` only. A file drawn with `rect`/`circle`/`polygon` elements, or relying on a `transform`, builds with a warning and draws partly or not at all — flatten shapes to paths in your editor first (in Inkscape: Path → Object to Path).
- A hand-written property wins over a file of the same name — that is the alias and redraw mechanism.
