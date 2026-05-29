# Prototype Theme Tokens
Status: LOCKED

A minimal design token system for Avalonia.Themes.Prototype — a small set of meaningful, reusable tokens (colours, spacing, sizing) that define the prototype theme. Themes swap token values while keeping the same token names, enabling consistent theming across multiple palettes.

## The plan

### Scope
Only the theme class (`DefaultTheme.cs` and the ThemeEngine integration). No control files will be touched at this stage.

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
| `Thickness` | `Thickness` | `1` | `Thickness(1 × BaseSize)` |

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
