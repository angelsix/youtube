# Session memory — 2026-06-06 12:00

Task: Merge the 3 thickness resources in `Controls/AdornerLayer.axaml` into the ThemeEngine `DefaultTheme.cs`, then survey every other control file for similar stranded inline resources/hardcoded values worth promoting into the theme.

### 2026-06-06: Context gathered
Category: Context
- ThemeEngine (`AngelSix.ThemeEngine` 1.2.0) emits one `{theme:PropName}` markup extension per public property on `[Theme] DefaultTheme`. No parameters/multipliers — a value like "2px" needs its own token property.
- `DefaultTheme.cs` shape tokens: `RadiusSm`(3), `RadiusMd`(6), `Thickness`(=new Thickness(1×BaseSize)). Spacing scale Sm/Md/Lg/Xl/Xxl. No thickness *scale* (only one Thickness token).
- Plan `Plans/PrototypeThemeTokens.md` is **LOCKED**; original scope said control files untouched. We are now past that, migrating controls to tokens.
- AdornerLayer's 3 keys (`SystemControlFocusVisualMargin`=0, `...PrimaryThickness`=2, `...SecondaryThickness`=1) are used ONLY inside AdornerLayer.axaml.
- KEY: ThemeEngine ships a `{theme:Edges Left=.. Top=.. All=.. Horizontal=.. Vertical=..}` markup ext that builds a `Thickness` from spacing tokens (used 29× already). So thickness/margin/padding can be composed from spacing tokens — usually no new token needed.
- `{theme:Thickness}` (=1) used 33×. Renaming it would ripple — ADD tokens, don't rename.
- SpacingSm=2, SpacingMd=4, SpacingLg=6, ControlHeight=32, IconSize=20, FontSizeSm=12 — these absorb most stranded numbers.

### Survey buckets (stranded inline resources across Controls/)
- Bucket 1 (map to EXISTING tokens): MinHeight/ButtonSize 32 → ControlHeight (ComboBox, DropDownButton, RadioButton, SplitButton, CalendarDatePicker, MenuBar, Slider H/V, TextSelectHandle, ExpanderChevronButtonSize); FontSize 12 → FontSizeSm; IconSize 20 → IconSize; border/separator 1 → Thickness/SpacingNone; top-header bottom margin 4 → SpacingMd; ToggleSwitch 6 → SpacingLg.
- Bucket 2 (recurring, NO token → propose NEW): height 48 (Expander, NavBar, TabItem, TabStripItem, TabbedPage header, SplitView compact pane) → ControlHeightLg?; emphasis/pipe thickness 2 (TabItem/TabStripItem/TabbedPage pipe + AdornerLayer primary) → ThicknessLg/SpacingSm.
- Bucket 3 (genuine one-offs, LEAVE): DatePicker/TimePicker min/max widths, SplitView 320 pane, ToolTip 320, TreeView indent 16/chevron 12, CaptionButton 45/30, Slider thumb radius 10, MenuFlyout offset -4, Slider pre/post 15.
- Bucket 4 (colours): ManagedFileChooser icon artwork + WindowDrawnDecorations Windows-red close (#e81123) + box-shadow overlays — mostly leave; shadows could derive from SurfaceOverlay later.

### 2026-06-06: AdornerLayer merged + ThicknessLg added (Luke confirmed thickness scale)
Category: System change
- Added `ThicknessLg` (=2×BaseSize) to DefaultTheme.cs Source Properties (Thickness scale: Thickness=1, ThicknessLg=2). Updated LOCKED plan PrototypeThemeTokens.md sizing table.
- AdornerLayer.axaml: removed the 3 SystemControlFocusVisual* resources; outer border → {theme:ThicknessLg}, inner → {theme:Thickness}, dropped Margin (default 0). Builds clean (generator emits ThicknessLgExtension).
- Pending Luke's decision: full new-token set for folder-wide migration (ControlHeightLg=48, SpacingXxxl=16, optional ControlHeightMd=40) + off-scale padding policy (snap-to-scale vs literal).

### 2026-06-06: Luke's token + migration policy (DECIDED)
Category: Decision
- THICKNESS: expand into a full scale like spacing (proactive, "by default"). Rename bare `Thickness`(1)/`ThicknessLg`(2) → `ThicknessSm/Md/Lg/Xl/Xxl` = 1/2/3/4/6 ×BaseSize (spacing halved, same proportions). No bare base. Also rename in XAML now (or build breaks).
- SPACING: add the ONE extra `SpacingXxxl`=16. NO more spacing values beyond that — Luke is not a fan of bloating spacing.
- CONTROL HEIGHT: make it a scale too (currently bare `ControlHeight`=32). Need 32/40/48 now → `ControlHeightSm`=32, `ControlHeightMd`=40, `ControlHeightLg`=48 (partial ascending like Radius, no bare base). If it ever grows past ~5 values, extend to a centred 5-tier (smaller+larger than centre) like spacing.
- OFF-SCALE one-offs (5,7,9,11,3 paddings etc.): SNAP to nearest existing token value.
- DELIBERATE off-scale / exceptionally large (min/max widths 296/456/320, a header that wants 2× max spacing): DON'T add tokens and DON'T leave literals — multiply an existing theme token via a one-off converter/markup-ext INSIDE that specific control, so it still scales with BaseSize/zoom (else zoom goes disproportional). Pick the token whose base value is closest and multiply.
- Principle throughout: never bloat the theme for a single exception; controls multiply existing tokens for their unique needs.

### 2026-06-06: REVERTED SpacingXxxl — Luke never wanted ANY extra spacing tier
Category: Decision
- Misread "add the extra one spacing" — Luke meant do NOT add it. Spacing scale stays Sm..Xxl (2/4/6/8/12), final. NO SpacingXxxl ever.
- Removed SpacingXxxl from DefaultTheme.cs + plan; Expander's three 16/20px refs snapped to `SpacingXxl` (12) per his "snap one-offs to existing values". Builds green.
- LESSON: when Luke gives a value-set instruction, take the conservative/no-bloat reading; don't add scale tiers he hasn't clearly asked for.

### 2026-06-06: Foundation + infra DONE & green; Expander exemplar done
Category: System change
- DefaultTheme.cs final tokens: SpacingXxxl=16; ControlHeightSm/Md/Lg=32/40/48; ThicknessSm/Md/Lg/Xl/Xxl=1/2/3/4/6. Renamed all XAML usages (Thickness}→ThicknessSm}, ThicknessLg}→ThicknessMd}, ControlHeight}→ControlHeightSm}).
- New infra files: `MultiplyConverter.cs` + `ScaledExtension.cs` (`{prototype:Scaled By=N, Token=Tok}`, default Token=BaseSize) for the off-scale/large exception policy. Builds clean.
- Plan PrototypeThemeTokens.md updated with full scales + off-scale policy section.
- Migration PATTERNS proven on Expander.axaml (self-contained keys, builds green): 48→ControlHeightLg; 32→ControlHeightSm; 16 padding→`{theme:Edges All={theme:SpacingXxxl}}`; directional 1px borders→`{theme:Edges Left={theme:BaseSize}...}` (omitted side=0); margin 20→snap 16 (SpacingXxxl); 8→SpacingXl; 0→literal; unused def deleted.
- WATCH: an external linter/IDE keeps reverting .axaml edits (AdornerLayer, AutoCompleteBox, Expander). Use perl for usage rewrites and rebuild immediately to catch reverts.
- ~24 control files still to migrate. Exceptions needing Luke's call before rollout: 32px right-padding (dropdown button room), 24px menu gesture margin, slider thumb CornerRadius 10 (circle), TreeView chevron glyph 12, caption buttons 45/30 (Windows), and whether to also tokenise inline literals like StrokeThickness="1".

### 2026-06-06: Full keyed-resource migration DONE (Luke: snap aggressively, close is fine)
Category: System change
- Applied deterministic value→token map across all 24 remaining control files (script migrate_tokens.py, since deleted). Heights→ControlHeight*, font/glyph 12→FontSizeSm, sizes 18/20→IconSize, border 1→ThicknessSm, hairline double 1→BaseSize, pipe 2→SpacingSm, paddings/margins→Edges+Spacing*(snapped), large widths→{prototype:Scaled By=N[,Token=]}, zeros→literal 0, thumb radius 10→RadiusMd.
- Edge cases: TreeViewItemIndent used via element `<DynamicResource ResourceKey=.../>` in a MultiBinding → replaced with `<theme:SpacingXxl/>` (element form works in MultiBinding). TreeViewItemIndent was missing from initial map — caught in post-check.
- GridLengths (Slider Pre/Post 15→12, ToggleSwitch 6) kept as local resources — GridLength can't bind to a double token; values snapped on-scale.
- Caption buttons (45/30) left literal per Luke (shouldn't scale with app zoom).
- Verified: no dangling DynamicResource/leftover defs; FULL solution builds green (0/0).
- STILL OPEN (broader sweep, not done): bare inline literals inside templates (e.g. StrokeThickness="1", MaxDropDownHeight="374", inline Margins, shadow colours) — only keyed resources were migrated this pass. Visual run-through recommended given snaps shift some pixels.

### 2026-06-06: Scaled redesigned to mirror Edges; new GridPixels ext; GridLengths now token-bound
Category: Decision
- Luke: Scaled must NOT hardcode ThemeContext and must NOT take the token as a string (broke IntelliSense). Mirror Edges: take a bindable `BindingBase Value` woven into a MultiBinding. ThemeEngine source is on disk at `~/Documents/GitHub/angelsix-consulting/Avalonia Themes/AngelSix.ThemeEngine/Edges.cs` — used as the template.
- Rewrote `Scaled.cs`: `Value` (BindingBase) + `By` (double) → MultiBinding with internal ScaleConverter (ConverterParameter=By). Deleted old ScaledExtension.cs + MultiplyConverter.cs. Usage: `{prototype:Scaled Value={theme:ControlMinWidth}, By=5}` (nests fine inside `{theme:Edges Left=...}`).
- New `GridPixels.cs` (same Edges pattern) → yields pixel GridLength from a token binding. Replaced the 4 GridLength resources (Slider 12→SpacingXxl, ToggleSwitch 6→SpacingLg) bound on RowDefinition.Height/ColumnDefinition.Width. NO GridLength literals remain.
- Full solution builds green. Only deliberate literals left: Windows caption buttons (45/30).
- RUNTIME NOT YET VERIFIED: MultiBinding on RowDefinition.Height + nested Scaled-in-Edges compile; a visual app run is the remaining confidence check.

### 2026-06-06: AutoCompleteBox bugs + inline-literal scope surfaced
Category: Context
- Fixed: MainWindow + AutoCompleteBox preview `generic:List` held plain text → parsed as ONE string (dropdown showed one row). Now separate `<x:String>` items.
- Fixed: theme-linked negative margins now use `{prototype:Scaled ... By=-1}` (AutoCompleteBox, ComboBoxItem, CalendarDatePicker, NumericUpDown; Slider focus uses IconSize×-0.5). ManagedFileChooser glyph margins left (bespoke).
- Fixed: AutoCompleteBox `MaxDropDownHeight="374"` → `{prototype:Scaled Value={theme:ControlHeightSm}, By=12}` (=384).
### 2026-06-06: Inline layout-literal sweep DONE (BorderThickness/CornerRadius/Margin/Padding/Spacing)
Category: System change
- New `Corners.cs` markup ext (mirrors Edges) → builds a CornerRadius applying a radius token to selected corners (Top/Bottom/Left/Right/All/individual). For directional radii like "0,0,2,2"→`{prototype:Corners Radius={theme:RadiusSm}, Bottom=True}`.
- Migrated 54 inline layout literals across 19 files + 18 in ManagedFileChooser (positives only; its negative icon-path margins left as geometry). 0.5→1 (ThicknessSm), uniform/directional borders→Thickness*/Edges, corner radii→Radius*/Corners, margins/paddings→Edges (snapped), spacing→Spacing*. Large one-offs (38,15)→Scaled. Skipped Design.PreviewWith (Luke: hardcoded preview is fine) and Width/Height/MinWidth/MinHeight + StrokeThickness (icon geometry / sizes, deferred).
- Full solution builds green; no non-zero layout literals remain outside preview.
- EXCEPTIONS flagged to Luke: (1) WindowDrawnDecorations chrome margins/spacing now scale while caption buttons stay fixed (possible inconsistency); (2) ToggleSwitch + SliderThumb radius 10→RadiusMd(6) loses perfect pill/circle — future "shape" token candidate; (3) container sizes (CalendarItem MinHeight 290/MinWidth 294, dropdown Width 30, etc.) + StrokeThickness left for the deferred sizes/geometry pass.

### 2026-06-06: Caption buttons now scale too — NO non-scaling exceptions remain
Category: Decision
- Luke: merge caption buttons into overall scaling so absolutely everything scales. CaptionButtonWidth/Height (45/30) keyed resources removed; usages → `{prototype:Scaled Value={theme:BaseSize}, By=45/30}`. Builds green.
- Luke approved all other default decisions (pill radii snapped, deferred sizes/geometry) — nothing else needs review for now.
- State: every layout/dimensional value in the theme either resolves to a token or scales via Scaled; only intentional 0s + icon geometry (StrokeThickness, glyph Width/Height, path coords) remain as literals (deferred resource-sharing pass).

### 2026-06-06: Moved Scaled/Corners/GridPixels into AngelSix.ThemeEngine; bumped to 1.2.1
Category: System change
- Moved the 3 markup extensions from the Prototype project INTO the ThemeEngine source (`~/Documents/GitHub/angelsix-consulting/Avalonia Themes/AngelSix.ThemeEngine/`), namespace `AngelSix.ThemeEngine.Generated` (same as Edges) → now used via `theme:` not `prototype:`. Deleted the Prototype copies; XAML usages renamed `prototype:`→`theme:` (PrototypeTheme still `prototype:`).
- ThemeEngine version bumped 1.2.0 → 1.2.1 (patch = lowest number, "minor revisions"). Both consuming csprojs (Avalonia.Themes.Prototype + AvaloniaThemeLab app) bumped to 1.2.1.
- Packed `AngelSix.ThemeEngine.1.2.1.nupkg` (bin/Release). Verified full solution builds green against it via local feed (RestoreAdditionalProjectSources).
- PENDING: push 1.2.1 to nuget.org — waiting on Luke's NuGet API key (push is irreversible; he asked to be prompted). Nothing committed in either repo yet.

### 2026-06-06: (superseded) BIG REMAINING: a folder-wide scan shows MANY inline dimensional literals still (BorderThickness 1/2, control Min/Width/Height, inline Margins/Paddings, FontSizes) PLUS preview-only scaffolding (Design.PreviewWith Padding/Spacing/Width) PLUS intrinsic icon geometry (dot Width=3, StrokeThickness=1.5, CornerRadius="0,0,2,2", checkmark Width=15). Luke wants random values gone — but preview scaffolding and true glyph geometry should likely stay. Proposed: sweep real-template themeable literals; leave preview + intrinsic geometry. Awaiting confirm on those exclusions before mass-edit (geometry changes can break icon rendering).
