# AvaloniaThemeLab.GettingStarted

The app the [Getting started](../../ThemeEngine.html) guide walks you through: a new Avalonia app, Fluent removed, and the default theme in its place. Same app as `AvaloniaThemeLab.Minimal`, but it uses `AngelSix.Themes.Default` instead of writing a theme.

There is no theme class and no `Controls/` folder here. That is the point: the default theme brings every control theme with it.

## What is here

| File | What it does |
|---|---|
| `App.axaml` | The one line that matters: `<DefaultTheme />` in `Application.Styles`, where `<FluentTheme />` used to be. No `xmlns` is needed — the theme registers itself in Avalonia's own namespace. |
| `App.axaml.cs` | Builds the dependency injection container: registers the app's one theme as `DefaultTheme.Current`, and the window's view model. It implements `IServiceHost`, so `Design.DataContext="{DesignContext}"` gives the designer the view model a view's `x:DataType` names, from the same container. |
| `ViewModels/MainWindowViewModel.cs` | Takes the theme through its constructor. Its `[Command]` methods switch dark mode and change `BaseSize`, and every control follows live. |
| `Views/MainWindow.axaml` | Buttons for those commands, and two buttons with an accent hue (`theme:Accent.Kind`). Every control here is styled by the theme, not by this app. |

## Running it

```
dotnet run --project Examples/AvaloniaThemeLab.GettingStarted
```

Keep `.WithInterFont()` in `Program.cs`: the theme's font is Inter.

## The theme reference

`AngelSix.Themes.Default` is not published to nuget.org yet, so the project builds it from this repo with a `ProjectReference`. Once the package is published that becomes a `PackageReference` and nothing else in the app changes — which is exactly the `dotnet add package` step the guide describes.

## Where to go next

- Want your own colours, hues or sizes on top of this? Subclass `DefaultTheme` — see *Creating your own theme* in the guide, and `AvaloniaThemeLab.ThemePlayground` for a worked example.
- Want a theme that owes nothing to this one? See *A theme from scratch* in the guide, and `AvaloniaThemeLab.Minimal`.
