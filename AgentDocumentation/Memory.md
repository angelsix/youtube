# youtube project memory

Long-term, curated memory for the youtube project: decisions, context, and progress that must survive across sessions. Keep it tight — it's read every session. Append entries to the Log; update in place when something changes. Per-session reflections live in `Sessions/`, not here. The universal cross-project log lives in the global `AgentDocumentationGlobal/Memory.md`.

## Format

```
### YYYY-MM-DD: Short title

Category: Decision | Context | System change

[1 to 3 sentences.]
```

## Log

### 2026-05-28: AgentDocumentation layout adopted

Category: System change

This project had no agent docs at the root. Created the standard `AgentDocumentation/` folder (`Project.md` + `Memory.md` + `Sessions/`) as part of a repo-wide cleanup. `Project.md` is a stub to be fleshed out.

### 2026-05-28: AvaloniaThemeLab consumes AngelSix.ThemeEngine via NuGet

Category: Context

AvaloniaThemeLab (a lab under this repo) uses the `AngelSix.ThemeEngine` package; its bundled analyzer regenerates the theme markup extensions at compile time. Don't commit a `Generated/ThemeExtensions.g.cs` (or any `Generated/` copy) — a stale committed copy broke the build here when the engine's `ThemeContext` API changed after the move from in-repo source to NuGet. The engine's source lives in a separate private repo.

### 2026-08-22: AvaloniaThemeLab pins NuGet to nuget.org; accents resolve per-control

Category: Context

The machine-wide NuGet config carries folder feeds pointing into the `AngelSix.ThemeEngine`
source tree in the engine's private repo, used to test the package before release. `AvaloniaThemeLab/NuGet.config`
now clears them, so the lab always builds against the published package and restores on a clean
machine. Iterating on the engine locally means publishing, or adding the folder feed back into
that file while doing it — a local engine change will *not* show up in the lab otherwise.

Accent styling uses `{theme:AccentBrush}`, which resolves against the hue the target control
carries rather than a named accent, so one style block serves every hue. Rules and gotchas are in
`AvaloniaThemeLab/Avalonia.Themes.Prototype/ThemeRules.md` Rule 16. The non-obvious part: the hue
property is deliberately *not* generated. `Accent.Kind` is a per-assembly typed façade that
forwards onto `ThemeAccent.HueProperty` in the runtime, because a generated enum-typed property
would be a different `AvaloniaProperty` in each assembly and a shipped theme's styles would never
see a hue added downstream.

### 2026-05-29: Glossary added

Category: System change

Built `AgentDocumentation/Glossary.md` via the `project-glossary` skill, closing the only gap in the wellness plan. Kept deliberately minimal: this repo is a grab-bag of code samples for YouTube videos with no shared domain. Locked "Sample" as the canonical word for a top-level project folder, plus Channel, Video, Series, and Lab.
