# Prototype Theme Tokens
Status: LOCKED

A minimal design token system for Avalonia.Themes.Prototype — a small set of meaningful, reusable tokens (colours, spacing, sizing) that define the prototype theme. Themes swap token values while keeping the same token names, enabling consistent theming across multiple palettes.

## The plan

### Scope
Originally the theme class only (`DefaultTheme.cs` + ThemeEngine integration). **Phase 2 (2026-06-06):** the control files in `Controls/` were migrated off their stranded inline resources *and* inline layout literals onto these tokens — heights, thicknesses, paddings/margins (via `{theme:Edges}`), fonts, radii (uniform via `{theme:Radius*}`, per-corner via `{prototype:Corners}`), and GridLength row/column sizes (via `{prototype:GridPixels}`) now resolve to tokens; large/off-scale one-offs (incl. the window caption buttons) use `{prototype:Scaled}`, so **everything scales with `BaseSize`**. The only remaining literals are intentional `0`s and icon-drawing geometry (path coordinates, `StrokeThickness`, small glyph `Width`/`Height`), deferred to a later resource-sharing pass.

### Colours — 13 base tokens

| Token | Purpose | Prototype Light | Prototype Dark | Derived variants |
|-------|---------|----------------|----------------|-----------------|
| `Surface` | Page/content background | `#FFFFFF` | `#1E1E2E` | — |
| `OnSurface` | Primary text, icons, essential content on surface | `#1E1E2E` | `#E0E0F0` | — |
| `OnSurfaceDim` | Secondary text, placeholders, disabled | `#7A7A8E` | `#8888A0` | — |
| `Overlay` | Overlay base colour (used with opacity) | `#000000` | `#FFFFFF` | 3 levels: OverlaySm (hover ~8%), OverlayMd (pressed ~16%), OverlayLg (disabled ~32%) — all derived from Surface |
| `AccentPrimary` | Main brand / primary interactive | `#87CEEB` (blue) | `#87CEEB` | Dark1/2/3, Light1/2/3 |
| `AccentSuccess` | Positive confirmation, completed | `#98D8AA` (green) | `#98D8AA` | Dark1/2/3, Light1/2/3 |
| `AccentWarning` | Cautions, non-critical alerts | `#F5D79E` (yellow) | `#F5D79E` | Dark1/2/3, Light1/2/3 |
| `AccentError` | Error states, validation failures | `#F2A0A0` (red) | `#F2A0A0` | Dark1/2/3, Light1/2/3 |
| `AccentInfo` | Informational elements, help | `#FFB366` (orange) | `#FFB366` | Dark1/2/3, Light1/2/3 |
| `AccentDestructive` | Irreversible actions | Darker red (derived from Error) | Darker red | Dark1/2/3, Light1/2/3 |
| `AccentSubtle` | Decorative, muted emphasis | `#C8A8E8` (purple) | `#6A5080` | Dark1/2/3, Light1/2/3 |
| `AccentNeutral` | Neutral tint, mild emphasis | `#D0D0D8` | `#606068` | Dark1/2/3, Light1/2/3 |
| `AccentBorder` | Dividers, subtle separation | `#D0C0E0` (lilac) | `#505060` | Dark1/2/3, Light1/2/3 |

Each accent auto-derives Dark1/2/3 and Light1/2/3 via HSL steps. Overlay levels derive from Surface with opacity applied.

### Sizing — 7 tokens

| Token | Type | Prototype value | How derived |
|-------|------|----------------|-------------|
| `BaseSize` | `x:Double` | `1.0` | User-adjustable (0.75–2.0) |
| `Spacing` | `x:Double` | `4` | `4 × BaseSize` |
| `SpacingSm` | `x:Double` | `2` | `2 × BaseSize` |
| `SpacingMd` | `x:Double` | `4` | `4 × BaseSize` |
| `SpacingLg` | `x:Double` | `6` | `6 × BaseSize` |
| `SpacingXl` | `x:Double` | `12` | `12 × BaseSize` |
| `RadiusSm` | `CornerRadius` | `3` | `CornerRadius(3 × BaseSize)` |
| `RadiusMd` | `CornerRadius` | `6` | `CornerRadius(6 × BaseSize)` |
| `ControlHeightSm` | `x:Double` | `32` | `32 × BaseSize` — standard control height |
| `ControlHeightMd` | `x:Double` | `40` | `40 × BaseSize` — picker/flyout rows |
| `ControlHeightLg` | `x:Double` | `48` | `48 × BaseSize` — nav bars, tab headers, tall controls |
| `ThicknessSm` | `Thickness` | `1` | `Thickness(1 × BaseSize)` — standard borders |
| `ThicknessMd` | `Thickness` | `2` | `Thickness(2 × BaseSize)` — emphasis/focus/selection |
| `ThicknessLg` | `Thickness` | `3` | `Thickness(3 × BaseSize)` |
| `ThicknessXl` | `Thickness` | `4` | `Thickness(4 × BaseSize)` |
| `ThicknessXxl` | `Thickness` | `6` | `Thickness(6 × BaseSize)` |

Thickness mirrors the spacing scale (spacing values halved), all `BaseSize`-driven. Control height is a partial ascending scale (extend to a centred 5-tier if it ever needs more than these).

### Off-scale policy

- **Near a scale value** → snap to the nearest existing token (don't invent a token for a one-off).
- **Deliberately off-scale / exceptionally large** (e.g. picker min/max widths, a header that wants 2× the largest spacing) → multiply the closest existing token *inside that control* with the `{prototype:Scaled Value={theme:Token}, By=N}` markup extension, never a hard-coded literal — so the value still scales with `BaseSize`/zoom. Never bloat the theme for a single exception.
- **GridLength row/column sizes** can't bind to a double token directly, so use `{prototype:GridPixels Value={theme:Token}}` to turn a spacing token into a pixel `GridLength`.

Both `Scaled` and `GridPixels` mirror the theme engine's `{theme:Edges}` design: each takes its input as a bindable `BindingBase` woven into a `MultiBinding`, so they're decoupled from the theme system and stay reactive to `BaseSize`/theme changes.

Each getter in `DefaultTheme.cs` returns a value that incorporates `BaseSize` multiplication. XAML has no idea the value is derived — it just receives a number.

### Total theme authoring effort
**20 values per theme** (13 colours + 7 sizing). All Dark1/2/3, Light1/2/3, and overlay levels auto-calculate.

### Implementation approach

1. The `[Theme]` attribute on `DefaultTheme` generates markup extensions (`{theme:Property}`) via the AngelSix.ThemeEngine source generator
2. `BaseSize` is a property on the theme class (default: `1.0`)
3. Every sizing property getter returns `rawValue × BaseSize`
4. Dark mode via existing `ColorPaletteResources` per-theme-variant system (same token names, different values)
5. Remove redundant legacy tokens: `ThemeColor1-5`, gradient brushes, 100+ `SystemControl*` brushes

### What was discarded

- **RenderTransform/ScaleTransform for BaseSize** — leaves some UI elements unscaled, causes blurry text, feels dirty, leaks abstraction. Not needed because the ThemeEngine already handles derived properties transparently in XAML.
- **Background/Foreground naming** — too positional, doesn't describe what the token is for. Replaced with `Surface` / `OnSurface` / `OnSurfaceDim`.
- **3-level spacing (Sm, Md, Lg)** — too coarse for comfortable UI work. Kept 4 levels (Sm, Md, Lg, Xl).

## Pinned terms

- **Design token** — a named, reusable value in the theme system (colour, spacing, radius, thickness) that can be swapped between themes
- **BaseSize** — a UI zoom multiplier (0.75–2.0) that scales every derived property proportionally, like Chrome's zoom
- **Overlay** — a base overlay colour applied with opacity to create hover (Sm ~8%), pressed (Md ~16%), and disabled (Lg ~32%) states
- **HSL derivation** — calculating Dark1/2/3 and Light1/2/3 shades from a single base colour using fixed HSL luminance steps
