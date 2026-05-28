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

AvaloniaThemeLab (a lab under this repo) uses the `AngelSix.ThemeEngine` package; its bundled analyzer regenerates the theme markup extensions at compile time. Don't commit a `Generated/ThemeExtensions.g.cs` (or any `Generated/` copy) — a stale committed copy broke the build here when the engine's `ThemeContext` API changed after the move from in-repo source to NuGet. The engine's source lives in the separate `angelsix-consulting` repo.
