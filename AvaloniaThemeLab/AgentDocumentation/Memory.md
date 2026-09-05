# Memory — AvaloniaThemeLab

Durable, cross-session facts. Session-by-session detail lives in `Sessions/`.

## Architecture: themes own appearance, packages ship types only

The Prototype theme (`Avalonia.Themes.Prototype`) styles controls; it does **not** define them.
Most `Controls/*.axaml` files are ControlThemes for stock Avalonia types (Button, ComboBox…).
The calendar was the exception — it defined *new* C# types inside the theme. Those types now
live in the **`AngelSix.ThemeEngine.Controls`** NuGet package (repo: angelsix-consulting,
folder `Avalonia Themes/AngelSix.ThemeEngine.Controls`). Current contents: `CalendarView`,
`CalendarViewCell`, `CalendarViewMarker`, `CalendarViewMode`, `CalendarViewSelectionMode`,
and the pure-logic `CalendarSelectionEngine`.

Why types-only matters: it mirrors how first-party Avalonia controls are split (types in the
framework, look supplied by the theme package). Absorbing these controls into the framework
later is dropping a reference, not untangling duplicated styling. The theme keeps
`Controls/CalendarView.axaml` and references the package; the engine's dictionary-list generator
scans the theme's `.axaml` by content, so `PrototypeTheme` still auto-merges the calendar's
ControlThemes with no extra wiring. A smoke test
(`Avalonia.Themes.Prototype.Tests/CalendarViewThemingSmokeTests.cs`) guards against the calendar
silently losing its theme if that registration link ever breaks.

Dependency shape (no cycle): Controls → Avalonia only · Theme → Controls + Engine · Lab → Theme.

## Gotcha: Avalonia 12 moved core property types out of `Avalonia.Data`

In Avalonia 12.0.1, `StyledProperty`, `DirectProperty`, `AvaloniaPropertyChangedEventArgs` and
the `PseudoClasses` extensions live in the **root `Avalonia` namespace**, not `Avalonia.Data`
(where they sat in 11.x). Code needs both `using Avalonia;` and `using Avalonia.Data;` (the latter
still holds `BindingMode`). Incremental builds hide this: files compiled under an older Avalonia
stay green until something forces a fresh compile — moving/recreating those files surfaced it as
dozens of CS0246s. Verify type locations against the actual ref assembly (PortableExecutableReader
over `~/.nuget/packages/avalonia/<ver>/ref/net10.0/*.dll`) rather than trusting imports.

Also: `MergeResourceInclude` is in `Avalonia.Markup.Xaml.Styling`; `ResourceDictionary` is in
`Avalonia.Controls`. The 12.0.1 headless-testing API dropped the old synchronous `TopLevel` hook
— prefer asserting on merged-resource state over forcing template application.

## Repo topology note

This workspace (`…/youtube/AvaloniaThemeLab`) is a subfolder of the git repo rooted at
`…/youtube` (branch `develop`). `guard git commit-plan` emits repo-root-relative paths but
stages from the workspace dir — strip the leading `AvaloniaThemeLab/` prefix from the plan's
file list before `commit-apply`, or staging fails with "did not match any files".
