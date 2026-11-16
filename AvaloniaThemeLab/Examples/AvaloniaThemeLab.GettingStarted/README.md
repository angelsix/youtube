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

`AngelSix.Themes.Default` is on nuget.org, and the guide's `dotnet add package` step installs it. This project builds it from the repo with a `ProjectReference` instead, so a lab change to the theme shows up here at once; swap it for the `PackageReference` and nothing else in the app changes.

## Where to go next

- Want your own colours, hues or sizes on top of this? Subclass `DefaultTheme` — see *Creating your own theme* in the guide, and `AvaloniaThemeLab.ThemePlayground` for a worked example.
- Want a theme that owes nothing to this one? See *A theme from scratch* in the guide, and `AvaloniaThemeLab.Minimal`.
