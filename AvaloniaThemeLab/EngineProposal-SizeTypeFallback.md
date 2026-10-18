# Proposal: type-directed token resolution in AngelSix.ThemeEngine (colours and sizes)

For the owner of `AngelSix.ThemeEngine` (current release referenced here: **1.20.0**). Goal: one recommended spelling for every token, with no type suffix: `{color:TextDefault}` whether the target is a `Color` or a brush, `{size:RadiusMd}` whether the target is a `CornerRadius` or a `double`. The engine picks the right typed sibling from the target property. The suffixed spellings (`…Brush`, `…Double`) stay valid as optional aliases.

## 1. Problem

A token that the engine emits in more than one CLR type puts each extra type on a suffixed twin:

- colours: `{color:TextDefault}` (`Color`) and `{color:TextDefaultBrush}` (`SolidColorBrush`);
- sizes: `{size:RadiusSm}` (`CornerRadius`) and `{size:RadiusSmDouble}` (`double`).

That has three costs:

- **Authors must know which twin a property wants.** `Foreground` wants the `Brush` twin, `Ellipse.Width` the `Double` twin, and nothing in the markup says so. The `Double` suffix also reads like "twice the value".
- **Each ladder has its own suffix table.** Colours use `""`/`"Brush"`, the Radius ladder `""`/`"Double"`, and another ladder might differ. Authors get no uniform mental model.
- **Using the wrong twin fails silently.** It compiles, it raises no error, and it paints nothing or measures zero (§2). A missing name, by contrast, fails loudly. So the mistake people actually make is the one nobody sees.

The colour case is not hypothetical. The Theme Lab already uses the unsuffixed colour spelling everywhere: **286 uses across 73 `.axaml` files** (excluding `Scratch/`). Most of them sit on brush properties: `Foreground` 110, `Background` 108, `BorderBrush` 40, `TextElement.Foreground` 15, `PlaceholderForeground` 4. The tokens involved are `TextDefault`, `BackgroundDefault`, `DimTextDefault`, `BorderDefault`, `HoverBackgroundDefault`, `PressedBackgroundDefault` and `HoverTextDefault`. Every one of these hands a `Color` to an `IBrush` property today.

## 2. Root cause

Each emitted extension is a flat property-path lookup with no notion of its typed siblings.

- The generators emit one sealed class per member, holding only the path:
  - `ThemeExtensionsGenerator` emits `RadiusSmExtension` → `RadiusSm` and `RadiusSmDoubleExtension` → `RadiusSmDouble`.
  - `AccentRoleGenerator` emits `TextDefaultExtension` → `Accent{Hue}Dark9` and `TextDefaultBrushExtension` → `Accent{Hue}Dark9Brush`.
- The two converters behind those extensions, `ScopedThemeTokenConverter` (theme tokens) and `AccentBrushResolver` (accent roles), read that one path. Then, on a type mismatch, they call `TypeUtilities.ConvertOrDefault(value, targetType)`. That call fails for both pairs, and the failure shows up differently for each:
  - **value types** (`CornerRadius`→`double`): `ConvertOrDefault` returns `default(double)`, which is `0`. That is non-null, so the converter publishes it, and the property gets a silent zero.
  - **reference types** (`Color`→`IBrush`): `ConvertOrDefault` returns `null`. The converter falls back to the raw `Color`, and Avalonia's binding layer rejects it without an error. The property keeps whatever else set it, or nothing.

An earlier draft of this proposal said colours escape the trap because "Avalonia coerces `Color`→`SolidColorBrush` on assignment". **That is false.** Measured on 18 Oct 2026 in throwaway headless apps:

| Check (Avalonia 12.1.2 unless noted) | Result |
|---|---|
| `TypeUtilities.TryConvert(IBrush, Color)` | fails |
| `DefaultValueConverter` `Color`→`IBrush` | `InvalidCastException` |
| `BrushConverter.CanConvertFrom(typeof(Color))` | `false` |
| MultiBinding returning raw `Color.Red` → `TextBlock.Foreground` | brush unchanged, no log at Warning |
| same → `Border.Background` (**12.0.1**, the version the Theme Lab references) | `null` |
| control: MultiBinding returning `SolidColorBrush(Red)` | Red (so the check can tell the difference) |
| MultiBinding returning `CornerRadius(4)` → `Width` | `NaN` |
| MultiBinding returning `4.0` → `Border.CornerRadius` | `0,0,0,0` |

The converter does receive the target type: `IBrush`, `Double` and `CornerRadius` respectively in the checks above. Everything the fix needs is therefore already available at the point of conversion.

Not yet confirmed: these measurements used direct `Bind` calls, not the compiled style-setter path, and they did not run the real Theme Lab. The first test in §5 closes that gap.

## 3. Proposed change (engine package)

Make conversion **type-directed** inside the two converters, using the `targetType` they already receive. No `IProvideValueTarget` plumbing is needed in `ProvideValue`.

1. **Sibling rule.**
   - **Colours.** No new data is needed: the generator already guarantees a `{Name}Brush` sibling for every colour (`BrushAttribute`), and for every accent role and shade. When the target is assignable from `IBrush` and the resolved value is a `Color`, read `{path}Brush` instead. If the theme suppressed that brush (`[Brush(Generate = false)]`) or renamed it (`[Brush(Name = …)]`), the generator must emit that mapping too, so the rule never guesses.
   - **Sizes.** For each `[SizeScale]` that declares more than one `Types`, emit a sibling map, `{CLR type → property name}`, alongside the individual extensions. That data already fully determines the twins; it just isn't surfaced.
2. **Selection** (in both converters, cached per `(theme type, path, target type)` like the existing getter cache):
   - If the value already fits the target, it passes through unchanged. Every current correct usage takes this path.
   - Otherwise use the sibling whose type exactly matches the target, falling back to an assignable one.
   - If no sibling fits, **fail loudly**: return `UnsetValue` and log a binding error naming the token and the target type. Never publish `ConvertOrDefault`'s `default(T)`. A size that meets a mismatched target must never become `0` again.

### The recommended pattern

- **Write the bare token** for every themed value: `{color:TextDefault}`, `{color:BackgroundDefault}`, `{size:RadiusSm}`, on any property, whatever its type.
- **`…Brush` and `…Double` remain valid but optional.** Use one only when you need that exact type outside a typed property: a `Color`/`Brush` resource, a converter input, or code-behind.
- This is a non-breaking change. Every usage that resolves correctly today resolves identically afterwards, and every usage that silently fails today starts working.

## 4. Migration in the consuming repo

Once a package ≥ the change ships:

- **Colours: nothing to edit.** The 286 unsuffixed uses in §1 already follow the recommended pattern and start resolving correctly. Check the visual change before and after with `guard shot` on a light and a dark gallery pane, because those 286 setters currently paint nothing (dark-pane text and backgrounds will move). The suffixed uses (`{color:OverlayWeakBrush}`, `{color:AccentBrush Dark1}` and others) keep working. Flip them to the bare form when a file is next touched; a sweep is not required.
- **Sizes:** flip the three `…Double` sites to the bare form:
  - `Avalonia.Themes.Default/Controls/CalendarView.axaml` ~152–153: `<Ellipse Width="{size:RadiusSmDouble}" Height="{size:RadiusSmDouble}">`.
  - `Avalonia.Themes.Default/Controls/GroupBox.axaml` ~129: `<Setter Property="Margin" Value="{size:Edges Left={size:RadiusSmDouble}}" />`.
  - Then either drop `TypeSuffixes` from the Radius ladder in `Avalonia.Themes.Default/Theme/DefaultTheme.cs` (~218–221) or keep it for downstream consumers.
- **`ThemeRules.md`:** change the role table so its examples use the bare spelling, and add one rule: *write the bare token; the suffixed twin is an optional alias*.

## 5. Test plan

Engine-side, in `AngelSix.ThemeEngine.Tests`, through both converters and a real headless binding (not just a call to `Convert`):

- **Colour → brush property** (`{color:TextDefault}` on `Foreground`, `{color:BackgroundDefault}` on `Background`) resolves to the `…Brush` sibling. This test must fail on 1.20.0 first, for the reason in §2.
- **Size → double property** (`{size:RadiusSm}` on `Width`) resolves to the `Double` sibling, and the reverse direction resolves too.
- **Exact type** for each pair is unchanged (regression guard).
- **No matching sibling** returns `UnsetValue` plus a logged error, **not** `0`, `null` or the raw value.
- **Suppressed or renamed brush** (`[Brush(Generate = false)]`, `[Brush(Name = …)]`) follows the generated mapping rather than guessing `{path}Brush`.
- **Style-setter path**: the same colour and size cases applied through a `Style` `Setter`, which is the path the 286 Theme Lab uses take.

Consumer-side:

- A `guard shot` before-and-after pair of a light and a dark gallery pane, with the colour change expected and visible.
- The three flipped size sites render pixel-identically.

## 6. Alternatives considered and rejected

- **Rename the suffixes** (e.g. `"Double"` → something less misleading): cosmetic only. The per-ladder table remains and so does the silent failure.
- **Flip every colour use in the repo to `…Brush`:** fixes this repo's 286 sites today, but leaves the trap armed for the next author. It is the reverse of the recommended pattern.
- **Hand-written one-off `double` properties in `DefaultTheme.cs`:** token explosion; it defeats the scale attribute.
- **Doing the selection in `ProvideValue` via `IProvideValueTarget`:** unnecessary. The converter already receives `targetType`, and selecting there also covers bindings created in code.

None of these removes the root cause in §2. Only making the conversion type-directed does that.
