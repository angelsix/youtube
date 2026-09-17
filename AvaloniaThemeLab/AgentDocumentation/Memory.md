# Memory — AvaloniaThemeLab

Durable, cross-session facts. Session-by-session detail lives in `Sessions/`.

## Architecture: themes own appearance, packages ship types only

The Prototype theme (`Avalonia.Themes.Prototype`) styles controls; it does **not** define them.
Most `Controls/*.axaml` files are ControlThemes for stock Avalonia types (Button, ComboBox…).
The calendar was the exception — it defined *new* C# types inside the theme. Those types now
live in the **`AngelSix.ThemeEngine.Controls`** NuGet package (repo: angelsix-consulting,
folder `Avalonia Themes/AngelSix.ThemeEngine.Controls`). Contents at 1.1.0: `CalendarView`,
`CalendarViewCell`, `CalendarViewMarker`, `CalendarViewMode`, `CalendarViewSelectionMode`,
the extensible event model `CalendarEvent` / `CalendarEvent<TData>` (Date, Time, Title,
Description, TData payload), and the pure-logic `CalendarSelectionEngine` (selection state
machine, month-grid maths, day titles, and week-number math incl. the ISO-8601 fallback that
fixes .NET's spurious week-53 bug). 1.1.0 added `CalendarView.Events` (dots + hover tooltip)
and `IsWeekNumberVisible` / `WeekNumberRule` (week-number gutter beside the month grid —
mirrors upstream Avalonia PR #21981). The month body is ONE 8-column × 7-row Grid
(`PART_MonthBody`; the old `PART_DayTitles`/`PART_MonthGrid` pair is gone): the control places
titles, six `.weeknumber` TextBlocks and the 42 day cells into it, so the gutter aligns with
the drawn weeks by construction. Pseudo-class `:hasweeknumbers` toggles the gutter width.

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

## Mandatory: `guard xaml analyze` after ANY AXAML change

A green `dotnet build` proves AXAML *parses*, never that it *runs* — resource-dictionary values
build lazily on first lookup, so illegal style selectors and broken preview blocks only explode
when the previewer/renderer touches them. Learned the hard way: a bare `TextBlock.weeknumber`
selector inside a CalendarView ControlTheme crashed the designer with "Child styles must have a
nesting selector" (every style in a ControlTheme must nest under `^`) while every build stayed
green. From now on: after touching any `.axaml`, run `guard xaml analyze <theme-project>` and
report its output — especially the AVLR001 runtime gate. Related invariant: **nodes created in
code cannot be reliably reached by `/template/` descendant selectors** — declare such elements
statically in the template (tagged with a class in markup) and let code only populate them;
don't inject CSS classes onto programmatically-built nodes expecting the styler to chase them.

## A retuned seed can silently invalidate a ramp test's premise

`DarkSeedTests.Dark_ramp_recentres_on_the_dark_seed` originally proved re-centring by asking
whether the dark ramp was *coloured*: the paper seed is grey, so a ramp mirroring it stays grey,
while one re-centred on a coloured `DarkSeed` does not. Re-seeding the dark ramp on `#222222`
made the DarkSeed grey too, and that premise died with it — the test's own guard assertion fired
and it sat red at HEAD. An earlier session read the red as an environment fault (the cached
nuget.org `AngelSix.ThemeEngine` 1.17.1 nupkg hashes differently from the locally-packed one);
that was a red herring — nuget.org *signs* packages, so the hash always differs and the contents
are the same. The test now measures distance instead: every dark stage must land nearer the
`DarkSeed` than the paper seed, and every light stage the other way round. That holds whatever
colour either seed is retuned to.

## Repo topology note

This workspace (`…/youtube/AvaloniaThemeLab`) is a subfolder of the git repo rooted at
`…/youtube` (branch `develop`). `guard git commit-plan` emits repo-root-relative paths but
stages from the workspace dir — strip the leading `AvaloniaThemeLab/` prefix from the plan's
file list before `commit-apply`, or staging fails with "did not match any files".
