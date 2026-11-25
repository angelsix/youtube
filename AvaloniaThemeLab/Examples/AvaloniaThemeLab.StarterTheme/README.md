# AvaloniaThemeLab.StarterTheme

Starter template for building an Avalonia app with a custom theme extending `DefaultTheme`, dependency injection (`Microsoft.Extensions.DependencyInjection` + `IServiceHost`), `AngelSix.Mvvm` inline commands, and a custom `[IconGallery]`.

## What it demonstrates

- **`Theme/AppTheme.cs`** — subclasses `DefaultTheme` and attaches `AppIconGallery`.
- **`Theme/AppIconGallery.cs`** — extends `ThemeIconGallery` with a custom `{icons:Sparkle}` glyph while inheriting every default icon.
- **`App.axaml.cs`** — implements `IServiceHost`, registers `AppTheme.Current` as a singleton, `ExampleContentViewModel` as transient, and `MainWindowViewModel` as singleton.
- **`ViewModels/ExampleContentViewModel.cs`** — constructor-injects `AppTheme` and exposes inline `=> Command(...)` properties (`ToggleModeCommand`, `BiggerCommand`, `SmallerCommand`).
- **`Views/ExampleContent.axaml`** — centered layout-only view composed strictly from theme tokens and `{icons:Sparkle}`.
