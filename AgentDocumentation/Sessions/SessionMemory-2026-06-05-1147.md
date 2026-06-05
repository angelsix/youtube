# Session memory — 2026-06-05 11:47

Project: AvaloniaThemeLab (Avalonia.Themes.Prototype) — migrate controls to AngelSix.ThemeEngine.

### 2026-06-05: Task kickoff — migrate Controls/ to theme engine
Category: Context
Goal: replace legacy style/brush DynamicResources in every Controls/*.axaml with `{theme:...}` markup
extensions from `DefaultTheme.cs`, and add the `prototype` + `theme` xmlns. Reference exemplar:
`Controls/Button.axaml` (already migrated — full reskin, references only `{theme:...}`).

### 2026-06-05: Theme token mapping (from DefaultTheme.cs + PrototypeControlResources.axaml)
Category: Decision
Legacy prototype tokens → theme tokens:
- ThemeColor1 (White) → SurfaceBrush; ThemeColor5 (#AA000000, used as fg/border in Button) → OnSurfaceBrush
- ThemeColor2 #22000000 → SurfaceOverlayWeakBrush; ThemeColor3 #44000000 → SurfaceOverlayMediumBrush;
  ThemeColor4 #66000000 → SurfaceOverlayStrongBrush (overlay levels are ~8/16/32%)
- Accent1 CornflowerBlue → AccentPrimary; Accent2 ForestGreen → AccentSuccess; Accent3 Yellow → AccentWarning;
  Accent4 Orange → AccentInfo; Accent5 Firebrick → AccentError (ButtonSpinner :error used Accent5 → AccentErrorBrush)
- ControlCornerRadius (3) → RadiusSm; TextControlBorderThemeThickness (1) → Thickness

### 2026-06-05: Scope ambiguity — surfaced to Luke, awaiting answer
Category: Context
1247 total resource refs across Controls/: only 22 are ThemeColor* (the example category), 107 System*, ~1118
per-control derived. Button.axaml is a full reskin. Asked Luke: like-for-like prototype-token swap vs full
reskin of every brush. Not guessing — blocks the bulk edit.

### 2026-06-05: Decisions + execution
Category: Decision
Luke chose FULL RESKIN (like Button.axaml) and opted into a parallel WORKFLOW for the remaining files.
ButtonSpinner.axaml migrated by hand first as the validated exemplar — builds clean (0/0). Then launched a
64-agent workflow (one per Controls/*.axaml with >=1 resource ref; the 9 zero-ref layout files skipped) using
the locked mapping table. Brush-vs-Color form rule enforced. Agents do NOT build (avoid parallel build races);
I build once at the end and review flagged ambiguities. First workflow attempt failed (args arrived as a
string not array) — fixed with a coercion in the script, relaunched (run wf_c4e0123f-df4).

### 2026-06-05: Workflow complete — all controls migrated, builds clean
Category: System change
65 files reskinned, 731 colour/size refs replaced, 0 colours left unmapped, 0 residual legacy colour refs.
Full solution (Avalonia.Themes.Prototype + AvaloniaThemeLab app) builds 0 warnings / 0 errors. 194 judgement
calls flagged. Systematic choices to keep in mind: borders (BaseMedium/ChromeMedium) -> AccentBorderBrush;
OverlayCornerRadius -> RadiusMd (6 vs legacy 5); on-accent text -> SurfaceBrush (white). PipsPager local
ThemeDictionaries block deleted + tokens inlined; FlyoutPresenter local Thickness=1 removed. NOT yet visually
verified by running the app — build success != visual correctness (runtime themes can override). Offered to run.

### 2026-06-05: Orphaned legacy resources deleted (Luke approved)
Category: System change
Luke approved all 194 judgement calls (no changes) and approved deleting the orphaned PCR resources.
Static analysis: 646 keys in PrototypeControlResources.axaml, only 45 still referenced (sizing/structural),
600 orphaned colours removed. File 1727 -> 54 lines; empty Dark theme-dict dropped. First removal attempt used
a hand-rolled tag scanner that mis-counted nesting and mangled the file -> restored from /tmp/PCR.backup.axaml
and instead reconstructed the file from the exact original single-line kept resources. Full solution builds 0/0;
static sweep confirms zero live references (incl. DynamicResource) to any deleted key. App not run/visually
verified yet. Note for git: AvaloniaThemeLab has ~80 modified control files + PCR shrink staged for commit.

### 2026-06-05: Phase 3 — strip to 3 core files (DefaultTheme.cs + PrototypeTheme.axaml + Controls/)
Category: Decision
Luke wants the theme reduced to DefaultTheme.cs, PrototypeTheme.axaml, Controls/ (+ backend cs, project/sln).
Deleting: Accents/ (BaseColorsPalette, BaseResources, PrototypeControlResources, SystemAccentColors.cs),
DensityStyles/Compact.axaml, Strings/InvariantResources.axaml, ColorPaletteResources*.cs (3).
Analysis: BaseColorsPalette 0 refs; Compact only overrides (every key locally defined or unref) -> deleting just
drops compact density; SystemAccentColors 0 refs once BaseResources gone; ColorPaletteResources used only by
PrototypeTheme.axaml.cs (Palettes). Spacing tokens 0 usage -> safe to renumber.
Theme growth (Luke-approved): +9 source props (FontSizeSm/Md/Lg/Xl/Xxl=12/14/16/20/24, ControlHeight=32,
ControlMinWidth=64, IconSize=20, FontFamily=Inter) + 5th spacing step (SpacingXl 12->8, new SpacingXxl=12).
Asymmetric paddings handled by NEW Inset markup extension (Inset.cs) composing Thickness from spacing tokens,
fed via nested {theme:Spacing*} (PROVEN to compile - piloted ToolTip.axaml). Inter not actually bundled (no
asset) -> token = "Inter, $Default", falls back to system default exactly as before.
Running Phase-3 rewiring as a 45-file workflow (run wf_b0146c86-f2a). After: edit PrototypeTheme.axaml (drop all
MergedDictionaries, keep only Controls StyleInclude), gut Palettes+Compact/DensityStyle from code-behind, delete
the 7 files, build + verify zero residual refs. Converters (CornerRadiusFilterConverter x4, scroll TransformOps x2)
moved local into Expander/SplitButton/ScrollBar. 20 culture strings inlined as literals.

### 2026-06-05: Phase 3 complete — lean theme, builds clean
Category: System change
Rewiring workflow: 45 files, 147 refs replaced (5 agents didn't emit structured output but DID edit; verified via
residual sweep). Deleted 9 files (Accents/ x4 incl SystemAccentColors.cs, DensityStyles/Compact, Strings/Invariant,
ColorPaletteResources x3) + 3 now-empty folders. PrototypeTheme.axaml slimmed to a single Controls StyleInclude;
code-behind gutted to just the DefaultTheme designer fallback + XamlLoader (removed Palettes + DensityStyle/Compact).
Non-Controls core now exactly: DefaultTheme.cs, Inset.cs, PrototypeTheme.axaml(.cs), Properties/AssemblyInfo.cs.
One build break fixed: a string-inline agent put <x:String> inside a MultiBinding in ManagedFileChooser -> replaced
with a single StringFormat binding. Full solution builds 0/0; zero dangling refs vs all 841 historical keys.
Theme grew by 9 source props + 1 spacing step, net huge deletion. NOT visually run yet — Inset paddings are rounded
approximations; visual pass recommended.

### 2026-06-05: Runtime fixes — "compiles but designer/app crashed" (Luke caught it)
Category: System change
LESSON: build success != runtime success. Declared done on a green build, but the app crashed on launch (twice).
Two bugs found by actually RUNNING it (must run, not just build, for Avalonia theme changes):
1. PrototypeControls.axaml uses MergeResourceInclude -> ALL control resources flatten into ONE dictionary, so
   resource keys must be globally unique. My per-control converter defs duplicated RightCornerRadiusFilterConverter
   + LeftCornerRadiusFilterConverter across Expander + SplitButton -> "item with same key already added" at load
   (this was Luke's reported designer error). Fix: prefixed SplitButton's two keys (SplitButton*). Added a cross-file
   dup-key check to the toolkit.
2. {theme:Spacing*} returns a live Avalonia.Data.Binding (dynamic), NOT a constant double. Nesting it into Edges'
   double properties compiled but threw InvalidCastException at runtime. Fix: Edges properties are now BindingBase,
   and ProvideValue returns a MultiBinding (converter -> Thickness) so paddings stay reactive. NOTE for this Avalonia
   (12.0.1): the binding base type is BindingBase, and MultiBinding.Bindings is IList<BindingBase> (NOT IBinding).
Also renamed the converter Inset -> Edges (per Luke: margins are outsets, "inset" misleads) across 29 usages + class.
App now launches clean (0 exceptions, theme merges, Edges paddings apply). Could NOT screenshot to visually verify:
macOS Screen Recording permission NOT granted to the capture process, so only the desktop is captured. Left app
running (Luke to eyeball) / offered to verify if he grants Screen Recording to FeedbackCLI.

### 2026-06-05: Edges promoted into the AngelSix.ThemeEngine package
Category: System change
Moved Edges from the prototype into the engine repo (/Users/lukemalpass/Documents/GitHub/angelsix-consulting/
Avalonia Themes/AngelSix.ThemeEngine/Edges.cs). Luke chose to reuse the `theme:` xmlns -> Edges namespace is
AngelSix.ThemeEngine.Generated (same ns the source-gen emits into; hand-written class shares it by design), so
consumers write {theme:Edges} next to {theme:SpacingMd}. Bumped package 1.1.1 -> 1.2.0, packed (nupkg in engine
bin/Release). Prototype: deleted its Edges.cs, swapped 29 {prototype:Edges} -> {theme:Edges}, bumped BOTH the
theme project AND the app project PackageReference to 1.2.0 (app pinned it directly too -> NU1605 downgrade until
both bumped). Consumed 1.2.0 via a TEMPORARY local feed (AvaloniaThemeLab/nuget.config pointing at engine
bin/Release) — REMOVE that nuget.config once 1.2.0 is on nuget.org. Builds + runs clean; Edges resolves from the
package. Prototype non-Controls source now exactly DefaultTheme.cs + PrototypeTheme.axaml(.cs) + AssemblyInfo.
DONE: added an Edges section to the engine README, repacked, and PUBLISHED 1.2.0 to nuget.org (Luke supplied a
push key for one-time use, used only in the dotnet nuget push command, never written to any file — Luke rotating
it afterwards). Removed the temporary AvaloniaThemeLab/nuget.config; prototype builds clean from the cached
package with only nuget.org configured. nuget.org indexing takes a few minutes (clean restore on another machine
needs it indexed). Git: engine repo (Edges.cs, version 1.2.0, README) and youtube repo changes are uncommitted —
left for Luke / a Git Sync (no commit requested).
