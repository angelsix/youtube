# Avalonia.Themes.Skeleton

The smallest possible theme built on [AngelSix.ThemeEngine](https://www.nuget.org/packages/AngelSix.ThemeEngine): a fresh `dotnet new avalonia.mvvm` app with Fluent removed and just enough added to show how the engine works. Start here to write your own theme.

`Avalonia.Themes.Default`, next door, is one complete theme built the same way. It is an example, not a requirement: the engine needs none of it.

## What is here

| File | What it does |
|---|---|
| `Theme/SkeletonTheme.cs` | The tokens: one base size, two colours and one font, plus the few values derived from them. Marked `[Theme]`, so the engine turns every public property into a markup extension: `{size:ButtonPadding}`, `{color:TextBrush}`, `{theme:FontFamily}`. |
| `Theme/SkeletonThemeStyles.cs` | The entry point you put in `App.axaml`. It merges every `ControlTheme` dictionary in the project and registers a `SkeletonTheme` for the markup extensions to read. |
| `Controls/Button.axaml` | The smallest button: its text inside a border, every value read from the tokens. |
| `Controls/Window.axaml` | A bare window. With no theme at all a `Window` has no template and shows nothing, so this one is needed. |

## No Fluent

`Avalonia.Themes.Fluent` is not referenced. A control with no `ControlTheme` in `Controls/` has no template, so it renders nothing at all. That is deliberate: it shows exactly what your theme owns. To theme another control, add a `ControlTheme` for it in a new file under `Controls/`. There is no list to update: the engine finds every dictionary in the project at build time.

## Making it yours

- Change a value in `SkeletonTheme` and every control that reads it follows.
- Need a new value? Add a public property to `SkeletonTheme` and read it as `{size:Name}`, `{color:Name}` or `{theme:Name}`.
- Want a variant (dark, a brand)? Subclass `SkeletonTheme`, override the properties that change, and pass your class to `ThemeStyles<T>`.
