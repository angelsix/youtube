# youtube — Context map

Orientation map for agents working in the `youtube` repo (`github.com/angelsix/youtube`). This is **not one product** — it is a grab-bag of self-contained code Samples that accompany AngelSix YouTube videos. There is no shared build, no shared domain, and no single entry point. To get the language straight first, read `AgentDocumentation/Glossary.md` (Sample / Video / Series / Lab).

For change-impact on a method **inside one of the .NET Samples**, run the live tool against that Sample's own solution, e.g.:

```sh
contextmap impact --member "MainViewModel.SomeMethod" --solution "AvaloniaThemeLab/AvaloniaThemeLab.sln"
```

## What this is

A monorepo of ~22 solutions / 32 projects where **each top-level folder is an independent Sample** for a video or a Series. Samples do not reference each other and are not built together. They span years of tech: classic WPF, Xamarin, ASP.NET Core, plain C# console exercises, and recent Avalonia (net10.0) work. Plus non-code folders (wallpapers, dotfiles, web/Jekyll guides).

## Layout — where each Sample lives

Active / substantial .NET Samples (the ones you are most likely asked to change):

- **`AvaloniaThemeLab/`** — `AvaloniaThemeLab.sln` (net10.0). A **Lab**: ongoing Avalonia theming playground. Has its own `AgentDocumentation/` and `Plans/` — treat it as the most "live" Sample. Pairs with `Avalonia.Themes.Prototype` project.
- **`Avalonia BatchProcess/Source/`** — `BatchProcess3.sln` (net10.0). Multi-project Avalonia app: `BatchProcess3` (UI), `.Core`, `.Desktop`, `BatchProcess3Host`. Uses central package management (`Directory.Packages.props`) and has its own `Agents.md`.
- **`AvaloniaLoudnessMeter/`** — `AvaloniaLoudnessMeter.sln` (net7.0): `AvaloniaLoudnessMeter` (UI) + `.Desktop` head. Has `Directory.Build.props`.
- **`StreamElectronics/Software/`** — `StreamElectronics.csproj`, an Avalonia app (note: **project with no `.sln`** — one of the reasons proj count > sln count; also a `.parcel`/Parcel-style layout with `Multimeter/`, `Controls/`, `ViewLocator.cs`).
- **`PrototypeTheme/`** — empty/abandoned shell: contains **no source**, only stale build artifacts under `PrototypeTheme/Avalonia.Themes.Prototype/obj/`. The live `Avalonia.Themes.Prototype` project actually lives inside `AvaloniaThemeLab/Avalonia.Themes.Prototype/` (with `DefaultTheme.cs`, `Controls/`, etc.).

Teaching Samples (small, single-purpose, mostly legacy):

- **`WPF/`** — `01-WpfBasics`, `02-TreeViewsAndValueConverters`, `03-TreeViewsSimpleViewModel` (each its own `.sln`).
- **`C# Beginners/`** — `BasicCalculator`, `BitwiseOperators`, `ConsoleApplication1`, `TicTacToe`.
- **`C# General/`** — `DelegatesMethodsLambdas`, `Windows Installer Wix DotNet Core`.
- **`ASP.Net Core/`** — `EntityFrameworkBasics`, `MVCBasics` (`test.sln`), `SqlConnector`.
- **`DependencyInjectionExample/`** (multi-project DI sample) and **`DotNetCoreDependencyInjection/`**.
- **`Tasks/`** — `TasksInConsole`, `TasksInWPF`.
- **`FileFormats/BinaryViewer/`**, **`Email HTML Template/EmailSendTester/`**, **`Xamarin Android/Android Lifecycle/`**.

Non-.NET / asset folders (no build): **`WebDevelopment/`** (HTML/CSS/Sass/responsive lessons, numbered), **`Github Pages Jekyll Guide/`**, **`Windows 10 Dark Theme/`** (`.reg`/`.ahk`/dotfiles), **`Visual Studio Shortcuts/`** (`Shortcuts.md`), **`AngelSix/Wallpapers/`**, `StreamElectronics/Resources/`.

## Agent docs

- `AgentDocumentation/` (repo root): `Project.md` (stub — overview is TODO), `Glossary.md` (the agreed language — read first), `Memory.md`, `Sessions/` (per-session reflections). This `ContextMap.md`.
- Some Samples carry their **own** agent docs/plans: `AvaloniaThemeLab/AgentDocumentation/`, `AvaloniaThemeLab/Plans/`, `Avalonia BatchProcess/Source/Agents.md`. Repo-level `Plans/` also exists.

## Key flows

There is no cross-cutting runtime flow. The flow is per Sample: open that Sample's `.sln`, build/run it in isolation. The Avalonia desktop Samples run from their `*.Desktop` (or root) head project. A change in one Sample has **zero blast radius into any other** — they share no code.

## Invariants / conventions

- **Stay inside one Sample.** Never refactor across folder boundaries or try to unify Samples — they are deliberately independent and pinned to whatever tech/TFM the video used (do not "upgrade" a net7.0 or WPF/Xamarin Sample just because newer ones are net10.0).
- **Match the Sample's own setup.** If it has `Directory.Packages.props` (BatchProcess3) use central package versions; if it has `Directory.Build.props` respect it. Don't introduce repo-wide build props.
- **Use the Glossary's words** — Sample, Video, Series, Lab — not "demo/example/project".
- A Sample's own `AgentDocumentation/` / `Agents.md` / `Plans/` override this map for that Sample.
- `youtube` has a public GitHub remote (`origin`); it accompanies videos, so commits are visible to viewers — keep them clean.

## To change X

Identify which **top-level Sample** X lives in, open that Sample's solution, and (for the .NET Samples) sanity-check blast radius with `contextmap impact --member "Type.Method" --solution <that-sample.sln>` before editing. Don't reach outside the Sample.
