# Theme Lab visual bug sweep
Status: READY
Priority: 4/5
Plan type: code
Plan style: solo
Fix every visual bug Luke found walking the Theme Lab gallery, one control file per worker, four local Coder workers at a time, each fix proved by a before and after screenshot.
## The plan
Luke walked every page of the Avalonia Theme Lab, the gallery app that shows each control in the Default theme with a light pane on the left and a dark pane on the right, and listed what is wrong. Most controls fall short of the theme's own standard: padding that does not match, colours that ignore the accent ramp, states that look nothing like Button or ButtonSpinner, preview layouts that clip or stagger. A handful are broken outright: EmbeddableControlRoot crashes the app, NavigationPage, Separator and RefreshVisualizer show nothing, and the DatePicker and TimePicker pop-ups fill the screen.

The work is done one control at a time. Each control's theme and its gallery preview live in one file, `Avalonia.Themes.Default/Controls/<Name>.axaml` (the gallery shows that file's `<Name>Preview` template), so one worker owns one file and four workers can run at once without touching each other. Where a parent and its item are inseparable (ListBox and ListBoxItem, the tab family, the menu family) the item goes first and the parent waits for it.

Four shared pieces come first because every later fix leans on them: a way to screenshot a single gallery entry headless; the preview header (title above the accent swatches, and dark-pane text that is actually light); a theme switch so the magenta focus ring only appears on neutral controls; and a fix for the theme's thickness sizes, which are generated only as border thicknesses, so every place that uses one as a plain number (a track height, a dot size, a stroke) silently gets 0. That one cause is behind the empty Separator, the dot-only Slider, the vanishing PipsPager dots and several invisible rings and chevrons.

Before writing the tasks, every gallery entry was rendered headless and read alongside its XAML, so each task carries a brief naming the likely cause of each defect and the traps around it (the Task briefs section). Workers start from that rather than rediscovering it.

The yardstick is Button and ButtonSpinner. Luke calls ButtonSpinner the perfect example of the theme. A control is right when its normal, hover, pressed, disabled and accent states match those two in hue stage, contrast, padding, corner radius and type, and when every value comes from a theme token, as `Avalonia.Themes.Default/ThemeRules.md` and `ThemePhilosophy.md` require. A preview is right when its examples are stacked vertically at one fixed width, nothing clips at the default window size (1200x800), and each example shows a state a person can name.

Left alone, half the gallery argues against the theme instead of for it, and nobody can judge the controls that are fine because the broken ones sit beside them.

The full list of Luke's findings, control by control, is in the Defect list section below. That list is the brief each worker receives.

### How to run this plan

This section is for the chat that is told "implement the plan". That chat is the ORCHESTRATOR. It writes no control XAML itself; it dispatches, reviews screenshots and redirects.

1. First action: `guard plan set-status` this plan to IN PROGRESS. Prime avalonia-coder. Read the Worker primer below, and `AvaloniaThemeLab/Avalonia.Themes.Default/ThemeRules.md` and `ThemePhilosophy.md`, so you can judge the work.
2. Workers are dispatched with `dispatch_subagent` (tool-search it; never the harness's built-in Agent or Task tool) with `provider: "Coder"`, the free local Rexxy Qwen 27B. No other provider for any task: not Coder Fast, not Mimo, not Claude. If Coder refuses because Rexxy is full, wait for a slot rather than switching provider.
3. At most FOUR workers running at once. Before each dispatch run `list_dispatches` and count. When one lands and passes review, dispatch the next task whose `(needs N)` tasks are ticked. Task order is the default queue.
4. Tasks 1 to 4 are foundations and run one at a time, in order, before any control task. Tasks 3 and 4 touch many control files, so no control task starts until task 4 is ticked.
5. Each dispatch gets: the task text and its `Owns:` files; that control's Defect list entry AND its Task brief (the starting points and traps found by reading the code up front) pasted verbatim; the Worker rules AND Worker primer sections pasted verbatim; the absolute working tree `/Users/lukemalpass/Documents/GitHub/youtube/AvaloniaThemeLab` (`workingFolder`) with the instruction to edit there and nowhere else; the absolute path of this plan. Task 1 instead works in `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs` plus the one DemoRun.md section. A worker sees nothing of your chat, so leave nothing out. If a worker is refused a write it needs (the shared proof folder, ai-mecca's build output), raise its access with `set_dispatch_permissions` to your own level rather than working round it.
6. When a worker lands, review it yourself before ticking. Open its `before/frame.png` and `after/frame.png` with the Read tool and check every bullet of the Defect list entry against the after image, light and dark pane. Run `guard xaml verify` on its verification.json. Run `git diff --stat` and confirm it changed only its owned files plus its proof folder. A child's own "all fixed" is a claim, not evidence.
7. Anything still wrong: `steer_dispatch` the worker if it is still alive, otherwise dispatch a fresh Coder worker for the same task with the remaining defects named one by one and the path of the last after image. Repeat until the after shot is right. Only then `guard plan mark-done` with evidence naming the after image and the verify result.
8. After every review run `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-proof-page.sh` so `/Users/lukemalpass/Documents/GitHub/youtube/AvaloniaThemeLab/Scratch/ThemeLabProof/index.html` shows every before and after pair so far with its status (fixed, redirected, open). Give Luke that path in each report and show the latest pair inline as markdown images.
9. ThemeRules.md: control workers never edit it. When a worker reports a rule change (tasks 12, 30, 53, 60 will), apply it yourself after review, so the file has one writer.
10. Report to Luke briefly as each task lands, then go idle. Do not commit or push; Git maintenance owns that. Never revert or clean files you did not dispatch.
11. When every control task is ticked, run task 65 yourself, then hand Luke task 66.

### Worker rules

Paste this section verbatim into every worker dispatch.

- Prime the avalonia-coder skill before editing any .axaml, and follow the avalonia-verify skill to prove the result. The Worker primer (pasted with these rules) already holds the theme rules, tokens, Avalonia traps and exact tool commands: start from it instead of rediscovering them.
- Read `AvaloniaThemeLab/Avalonia.Themes.Default/ThemeRules.md` and `ThemePhilosophy.md` once. Every colour, size, padding and radius comes from a theme token. Fix the control theme, never a page. Preview changes go in the `<Name>Preview` template in the same file.
- The reference look is Button and ButtonSpinner (`Controls/Button.axaml`, `Controls/ButtonSpinner.axaml`). Read them, do not edit them unless they are your task. Normal, hover, pressed, disabled and accent states must match their hue stages, contrast, padding and corner radius.
- Preview layout standard: examples in a vertical StackPanel, one fixed width wide enough for the longest example, nothing clipped at the default 1200x800 window, every example showing a nameable state (default, hover, pressed, focused, disabled, each accent) pinned with `theme:Preview.State` where needed.
- You own ONLY the files your task names. Other agents are editing other control files in this same checkout right now. Never edit, revert, reformat, stash or "fix" any other file. Never run git checkout, reset, stash, clean or commit.
- Keep your file compiling after every save: the other agents build the same project. Build only through the shot script (it serialises builds). A build or XAML error in a file you did not change is another agent's edit in flight; wait a minute and retry, and never touch it.
- Edit with the file edit tools, not shell rewrite scripts.
- Before your first edit run the shot script with `<Entry> before`; when finished, with `<Entry> after` (command in the primer). The crawler opens popups, flyouts and drop-downs, so their states land in the capture too.
- Never add a test, harness, script or tooling note inside `AvaloniaThemeLab/`: the lab is public and ships only the theme and its demo app. Proof output goes only under the gitignored `AvaloniaThemeLab/Scratch/ThemeLabProof/`.
- Fill `AvaloniaThemeLab/Scratch/ThemeLabProof/<Entry>/verification.json` with one row per bullet in your Defect list entry, each with a measured number or crawler finding, and run `guard xaml verify` until it passes.
- If the theme engine genuinely cannot express a fix (a missing token, a missing property), stop and say so in your result. Do not invent a local value.
- Your final reply lists every defect bullet as fixed or not fixed with one line of evidence each, plus the before and after PNG paths.

### Worker primer

Paste this section verbatim into every worker dispatch, along with Worker rules. Everything here was worked out once, from `ThemeRules.md`, `ThemePhilosophy.md`, the avalonia-coder knowledge and earlier Theme Lab sessions, so no worker has to work it out again. Read `ThemeRules.md` and `ThemePhilosophy.md` (in `AvaloniaThemeLab/Avalonia.Themes.Default/`) once as well; this primer is the short form, and those two files win if they disagree.

#### Where things live

- One file per control: `AvaloniaThemeLab/Avalonia.Themes.Default/Controls/<Name>.axaml`. It holds, in order: the `<DataTemplate x:Key="<Name>Preview">` the gallery shows, a `<Design.PreviewWith>` block (IDE previewer only), then the ControlTheme and any named sub-themes.
- The gallery (`AvaloniaThemeLab/AvaloniaThemeLab/Views/MainWindow.axaml`) wraps that DataTemplate in a `LightDarkPreview` it declares itself. Attributes you set on the LightDarkPreview inside `Design.PreviewWith` never reach the gallery. What reaches it is the `<Name>Preview` template, plus whatever per-preview options task 2 adds (stacked panes).
- Copy the xmlns block from `Controls/ButtonSpinner.axaml` (prefixes `theme`, `color`, `size`, `icons`, `default`, `sys`). Spelling is American: `color`, never `colour`.
- Tokens are defined in `Avalonia.Themes.Default/Theme/DefaultTheme.cs`. Task 3 owns that file: no other worker edits it. If you need a token that does not exist, stop and say so in your result. The engine (`AngelSix.ThemeEngine` 1.20.0) is a NuGet package, so no worker can add markup extensions.
- Theme icons are `{icons:Name}` from the engine; the theme's own SVGs are in `Avalonia.Themes.Default/Icons/`.

#### Token cheat sheet

- Colour follows the control's hue: `{color:AccentBrush <Stage>}`, with stages `Light1..10` (fills, receding to the canvas) and `Dark1..10` (ink). `theme:Accent.Kind` picks the hue and inherits down the tree. Hues: Neutral, Primary, Success, Warning, Error, Info, Destructive, Subtle.
- Prefer role tokens: `{color:TextDefault}` (Dark9), `{color:BorderDefault}` (Light1), `{color:BackgroundDefault}` (Light10), `{color:HoverBackgroundDefault}` (Light8), `{color:HoverTextDefault}` (Dark7), `{color:PressedBackgroundDefault}` (Light6), `{color:DimTextDefault}` (Dark3; placeholders, captions, disabled text).
- Outlined control (the normal case, ButtonSpinner): fill Light10, hover Light8, pressed Light6, text Dark9, border Light1, hover text Dark7, dim text Dark3.
- Filled control (selected or checked): fill Dark3, hover Dark4, pressed Dark5, text and glyphs Light10. Selected or checked and also disabled: drop the fill and show `DimTextDefault` on the canvas. Never mix the two columns; never use a seed (`{color:AccentPrimaryBrush}`) as a fill.
- Neutral regardless of hue (dividers, grooves, carets): `{color:NeutralDark1Brush}` and friends. `{color:AccentBrush X}` is NOT the same thing even though it looks identical on a neutral control.
- Focus: `{color:AccentFocusBrush}` with `{size:ThicknessMd}`. After task 3, follow the `FocusColorOnAccents` mechanism task 3 documents in `ThemeRules.md`.
- Canvas: `{color:SurfaceDefaultBrush}`. Overlays (`OverlayWeakBrush` 8%, `OverlayMediumBrush` 16%, `OverlayStrongBrush` 32%) are for shadows and scrims drawn as their own Border, never as a state's Background.
- Brush suffix: role tokens are written bare (`{color:TextDefault}`). Plain tokens need the `Brush` suffix in a paint slot (`{color:AccentNeutralBrush}`; the bare form throws InvalidCastException), and are bare in a `Color` slot.
- Sizes: Spacing Sm 2, Md 4, Lg 8, Xl 12, Xxl 16. FontSize Sm 12, Md 14, Lg 16, Xl 20, Xxl 24. ControlHeight Sm 32, Md 40, Lg 48. `ControlMinWidth` 64, `IconSize` 16. Radius Sm 6, Md 12. Thickness Sm 1, Md 2, Lg 3, Xl 4, Xxl 6. `ContentPadding` 8 (all controls), `ContentPaddingWide` 12 horizontal (button faces). `DisabledOpacity` 0.3, `PressedScale` 0.98. `AnimationFastMs` 75, `AnimationNormalMs` 150, `AnimationSlowMs` 300.
- A token of the wrong type silently becomes 0 with no error. `RadiusSm` is a CornerRadius; a double slot (`Shape.RadiusX`) needs `{size:RadiusSmDouble}`. The Thickness ladder (`ThicknessSm`..`Xxl`) is a Thickness: in a `Width`, `Height` or `StrokeThickness` it is 0. Task 4 adds a double twin (`{size:ThicknessSmDouble}` and so on); use that for any plain number.
- Alignment tokens: `{theme:ControlHorizontalAlignment}` (Left), `{theme:ControlVerticalAlignment}` (Center), `{theme:ControlHorizontalContentAlignment}` (Left), `{theme:ControlVerticalContentAlignment}` (Center), `{theme:ButtonHorizontalContentAlignment}` (Center, button-like controls only), `{theme:ContainerHorizontalAlignment}` and `{theme:ContainerVerticalAlignment}` (Stretch).
- Composing extensions: `{size:Edges Left=..., Top=...}`, `{size:Scaled Value=..., By=1.4}`, `{size:Corners Radius=..., Top=True}`, `{theme:TransformScale Value={size:PressedScale}, By=1}`, `{default:ConcentricCorners Radius={TemplateBinding CornerRadius}, Thickness={TemplateBinding BorderThickness}, Left=True}`.

#### The rules that bite

1. No literal colour, size, font or radius anywhere, previews included. Previews may hold literal sample data (strings, numbers, list items, and a fixed `Width` on a preview instance).
2. Every visual property is a ControlTheme setter from a token. Inside a ControlTemplate use only `{TemplateBinding}`. A themed value that varies by state on an inner part goes in a `<Style Selector="^ /template/ Border#PART_X">` on the ControlTheme, never in the template markup.
3. Alignment setters use the alignment tokens. A literal is allowed only under a `<!-- Theme Exception: reason -->` comment. Never change a line that already carries `Theme Exception:`.
4. Every themed control sets `Foreground` explicitly (it inherits as a resolved value, so `Accent.Kind` would never reach it otherwise). Do not restate `FontFamily`, `FontSize` or `FontWeight` unless the value genuinely differs from the root's. Glyphs and icons set no Foreground: they take their container's.
5. Bind colours to the hue (`AccentBrush`), never to a named hue. Button, ToggleButton, DropDownButton, RepeatButton and CheckBox still carry old per-hue blocks; replacing them with the ButtonSpinner pattern is part of those tasks, and is why they do not match today.
6. Disabled: `Opacity` = `{size:DisabledOpacity}`; on an accented control `{size:Scaled Value={size:DisabledOpacity}, By=1.4}`.
7. Press: `RenderTransform` = `{theme:TransformScale Value={size:PressedScale}, By=1}`, with the `Transitions` setter on the base state, not inside the pressed style.
8. State style order: template, part styles, hover, pressed, disabled, focus-visible, hue overrides, classes.
9. Every templated ControlTheme carries `<Setter Property="ClipToBounds" Value="{theme:ControlClipToBounds}" />`. A part that paints and touches a rounded corner takes `ConcentricCorners`; never clip a rounded frame to hide children.
10. Padding: `{size:ContentPadding}`; button faces widen horizontally with `{size:ContentPaddingWide}`.
11. Comments: short role labels on sections ("Hover state"), never the node kind or type name. Every template part gets a label of at most nine words saying what it is in UI terms.
12. `^.name` class selectors are declared automatically in `StyleClasses.axaml` at build. Never edit its generated region.
13. Previews show states with `theme:Preview.State="Hover"`, `"Pressed"` or `"Focused"`, accents with `theme:Accent.Kind="Warning"`, disabled with `IsEnabled="False"`, and an error by copying ButtonSpinner's `DataValidationErrors.Error` block. A preview never hand-rolls a template. One vertical StackPanel with `Spacing="{size:SpacingXl}"`, one fixed Width on every instance.

#### Avalonia mechanics already learned

- `Opacity="0"` neither hides a part nor stops it taking clicks, and any broader state style that sets Opacity brings it back. To keep something measured but invisible, also set `IsHitTestVisible="False"` and keep state styles off it.
- Content alignment centres the presenter's one child only. In a horizontal StackPanel set `VerticalAlignment` on each child, or text sits at the top.
- Padding and Margin paint nothing: put the space on the element that paints.
- A ScrollViewer's inset is its content's Margin, never its Padding.
- A focus ring is clipped by every scroll area above it, so items in lists (ListBoxItem, ComboBoxItem, TreeViewItem, MenuItem) show focus with their own inset border and `FocusAdorner` set to `{x:Null}`.
- A popup detaches its content from inheritance, so `Accent.Kind` and Foreground must be forwarded into popup content. CalendarView already does this; copy its approach.
- `/template/` reaches only the control's own template; `>` matches a direct child only.
- A render transform takes no layout space; a hover grow needs `{size:HoverZIndex}` to paint over neighbours.
- A Style beats a ControlTheme setter for the same property, and an app-level Style beats both.
- `AVLN2000 ... cannot be resolved` on a setter means the property does not exist on that control (AutoCompleteBox has no `HorizontalContentAlignment`); drop it or push it to a `/template/` style.
- `GradientStop` has no Opacity. `BoxShadow` cannot take a token: draw a shadow Border with an overlay brush behind the target (NotificationCard and ManagedFileChooser still have hex shadows to replace).
- A preview has no themed window above it, so text falls back to black 12px unless a root sets it. `LightDarkPreview` is a declared root (Rule 13): that is where task 2 fixes black text on the dark pane.

#### Patterns found across the theme (check your file for each)

These came from reading every control file against its render. Most broken controls have one or more of them; `grep` your file for each before you start.

- **Wrong-type size token** (above): a `{size:Thickness...}` in a double slot draws nothing.
- **Pinned seed brushes**: `{color:AccentPrimaryBrush}`, `{color:AccentErrorBrush}` and the other `Accent<Hue>Brush` names are fixed hues at seed strength. As a fill or text they ignore `Accent.Kind` (a Warning control stays blue) and break contrast in dark. Replace with `{color:AccentBrush <Stage>}`. Only the error state of a control's frame and the focus ring legitimately pin a hue.
- **Undefined stock resource keys**: `{DynamicResource Xxx}` where Xxx is a Fluent theme key (`NavigationControlNavBarBackground`, `DrawerPaneButtonIcon`, `TabbedPage...`, `NavigationPageBackIcon`) resolves to nothing, silently: an invisible icon or an unpainted bar. Keys defined in your own file are fine.
- **Overlays used as fills or borders**: `OverlayWeakBrush`, `OverlayMediumBrush` and `OverlayStrongBrush` as a Background, BorderBrush or state colour (MenuItem selected, SplitButton rest, Expander and RadioButton borders, ScrollViewer cells). They are for shadows and scrims only.
- **Asymmetric text padding**: Top 4 and Bottom 8 (`SpacingMd`/`SpacingLg`) on text parts puts text below centre (MenuItem, the date and time pickers). Vertical edges should match.
- **Padding applied outside the face**: `Margin="{TemplateBinding Padding}"` on the template root (DatePicker, TimePicker) insets the frame instead of the text.
- **No Foreground setter**: Rule 13 requires one; without it the control shows the pane's inherited colour (black on dark until task 2, and never the hue).
- **Content alignment overriding the child**: a presenter given `ControlHorizontalContentAlignment` (Left) places its child left even when the child asks for Center (ContentPage, CarouselPage, TabbedPage).
- **Row controls left-aligned**: list rows (ComboBoxItem, MenuItem, TreeViewItem) set `ControlHorizontalAlignment` (Left) instead of `ContainerHorizontalAlignment` (Stretch).
- **Background equal to the canvas**: `{color:BackgroundDefault}` (Light10) on a surface that should stand out (Carousel, ContentPage, SplitView pane, GridSplitter panes) is invisible on the canvas.
- **Previews that cannot show the thing**: horizontal rows wider than the ~440px pane, unsized Grids whose star columns collapse to 0, controls with no data (NavigationPage, ManagedFileChooser), hidden states never pinned, and entrance animations starting at Opacity 0 (NotificationCard) that a still capture catches invisible.

#### Tools, with the exact commands

- Shot: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh <Entry> before|after` (built by task 1). It renders one gallery entry exactly as the lab shows it at the default window size, with the crawler on, into `AvaloniaThemeLab/Scratch/ThemeLabProof/<Entry>/<tag>/` (frame.png, inspection.json, crawl and state files). It holds a lock so builds never overlap; never run `dotnet build` on the lab yourself, use `theme-lab-shot.sh build` instead.
- Before editing a control, read its measured reference: `guard xaml control <ControlType>` (padding, parts, what each pseudo-class changes).
- Geometry gate: `guard xaml geometry <capture>/inspection.json`, and again on each state file. It catches clipping, zero size and text overflow.
- State gate: `guard xaml states <capture>`.
- Checklist rows: `guard xaml verify --catalogue`. Then fill `<Entry>/verification.json` and run `guard xaml verify <Entry>/verification.json --capture <Entry>/after` until it passes.
- What a finding means: `guard xaml rule AVL0nn`.
- Screenshots are not evidence for a worker: prove each fix with a number from inspection.json. The orchestrator looks at the images.

### Task briefs

Starting points and traps for each task, found by rendering every gallery entry headless and reading each control file against its render on 18 Oct 2026. They point at the likely cause; the worker still proves it by measurement before and after. Paste the matching entry into the dispatch with the Defect list entry.

**Shot harness (task 1)**
- Starting point: the throwaway renderer used for this review, which rendered all 65 gallery entries in one process in about 9 seconds. It builds the lab's own `App` headless, takes `MainWindowViewModel` from `App.Services`, sets `SelectedEntry` to each entry on a 1200-wide MainWindow and saves `CaptureRenderedFrame()`. Its project references the lab exe and adds `Avalonia.Headless`, `Avalonia.Skia` and `Avalonia.Fonts.Inter` 12.0.1. Rebuild the same idea inside ThemeLabProofs (which already references the lab projects), never in the lab.
- Two outputs are needed per entry: the PNG for the orchestrator, and a `guard xaml inspect --crawl true` capture (inspection.json plus state files) for `guard xaml verify`. Either make the proof view selectable by `guard xaml inspect --view ThemeLabProofs.GalleryEntryView` (an environment variable naming the entry, if guard passes the environment through; check), or have the script run both.
- Window height: 1200x800 is Luke's default window; also offer a taller capture (the renderer used 900) so long previews are fully visible, but judge clipping at 800.
- EmbeddableControlRoot throws when hosted (it is a TopLevel). The script must report that entry as failed and carry on with the rest.
- The lock: four workers shoot at once and all builds touch the lab's obj folders. Serialise build plus capture with a lock (on macOS `mkdir` of a lock directory works where `flock` does not exist), and print the wait when held.

```csharp
using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Headless;
using Avalonia.Threading;
using AvaloniaThemeLab;
using AvaloniaThemeLab.ViewModels;
using AvaloniaThemeLab.Views;
using Microsoft.Extensions.DependencyInjection;

var outDir = args.Length > 0 ? args[0] : "/tmp/tlshots/out";
var only = args.Length > 1 ? args[1].Split(',') : null;
int w = 1200, h = int.Parse(Environment.GetEnvironmentVariable("TL_H") ?? "800");
Directory.CreateDirectory(outDir);
AppBuilder.Configure<App>().UseSkia().UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false }).WithInterFont().SetupWithoutStarting();
var app = (App)Application.Current!;
var vm = app.Services.GetRequiredService<MainWindowViewModel>();
foreach (var entry in vm.Catalogue.Entries.ToList())
{
    if (only != null && !only.Contains(entry.Name)) continue;
    try
    {
        var win = new MainWindow { DataContext = vm, Width = w, Height = h };
        vm.SelectedEntry = entry;
        win.Show();
        for (int i = 0; i < 5; i++) { Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); }
        var frame = win.CaptureRenderedFrame();
        frame?.Save(Path.Combine(outDir, entry.Name + ".png"));
        win.Close();
        Console.WriteLine("ok " + entry.Name);
    }
    catch (Exception ex) { Console.WriteLine("FAIL " + entry.Name + ": " + ex.GetType().Name + ": " + ex.Message.Split('
')[0]); }
}
```

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <EnforceCodeStyleInBuild>false</EnforceCodeStyleInBuild>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Avalonia.Headless" Version="12.0.1" />
    <PackageReference Include="Avalonia.Skia" Version="12.0.1" />
    <PackageReference Include="Avalonia.Fonts.Inter" Version="12.0.1" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="/Users/lukemalpass/Documents/GitHub/youtube/AvaloniaThemeLab/AvaloniaThemeLab/AvaloniaThemeLab.csproj" />
  </ItemGroup>
</Project>
```

**LightDarkPreview (task 2)**
- Black text in the dark pane, root cause found: the LightDarkPreview ControlTheme sets `Foreground` to `{color:AccentBrush Dark8}` once, on the control. The control sits in the light region, so the brush resolves to the light palette's Dark8 (near black). Foreground inherits as an already-resolved value (Rule 13), so the dark pane's content inherits that near-black. The pane captions look right only because `Label` sets its own Foreground inside the dark region. Fix: set Foreground (and keep the font properties) per pane, from a `^ /template/ Border` style so each pane resolves the brush in its own region. Prove it with a TextBlock in the dark pane measuring a light colour, and a TextBlock with an explicit Foreground keeping it (the refusal case in the Done-when). This one fix should clear the black text in ContentControl, GroupBox, Menu, DatePicker and every other control that sets no Foreground.
- Title above the swatches: each pane's header is `<Grid ColumnDefinitions="Auto,Auto">` with the Label in column 0 and the swatch ListBox in column 1. Make it two rows (title, then swatches).
- Literals in the template: `Padding="20"` and `StackPanel Spacing="10"` on both panes. Use `{size:SpacingXxl}` and `{size:SpacingXl}` or the nearest tokens, as `Setter`s or `^ /template/` styles (Rule 2).
- Stacked panes per control: `Orientation` already exists (`Vertical` stacks the panes), but the gallery's `MainWindow.axaml` hard-codes `Orientation="Horizontal"` for every entry, and a control file's `Design.PreviewWith` never reaches the gallery. Give control files a way to ask. The lightest is a keyed resource beside the preview template, `<Orientation x:Key="CarouselPreviewOrientation">Vertical</Orientation>`, looked up by the gallery the same way `PreviewTemplateConverter` (AvaloniaThemeLab/ValueConverters) finds `<Name>Preview`, defaulting to Horizontal. Document the key in ThemeRules.md Rule 20 (task 2 owns ThemeRules.md while it runs) so tasks 8 and later can use it.
- Its own preview: one TextBlock ("Hello, theme"). The comment says nesting a LightDarkPreview would recurse; it doesn't need to: show a Button, a CheckBox, a TextBox and a line of text.

**Button (task 5)**
- Cause: Button's template has no `DataValidationErrors` host and the theme has no `^:error` style, so setting `DataValidationErrors.Error` on a Button does nothing visible.
- Start: copy ButtonSpinner's shape. Its template's outermost element is a `<DataValidationErrors>` wrapping the frame, and it has `<Style Selector="^:error">` setting BorderBrush to `{color:AccentErrorBrush}` (a pinned hue is right here: an error is always Error).
- Trap: once wrapped, the error message sits under the face inside the control, so a `MinHeight` on the control is shared with the message and the face shrinks. Button has no MinHeight today; do not add one to the control.
- Trap: Button is the reference look for everything else. Change nothing but the error state, and re-shoot to prove the other twelve examples are pixel-identical.

**ButtonSpinner (task 6)**
- Start: the preview is one StackPanel with `HorizontalAlignment="Left"` and no widths. Give every ButtonSpinner the same `Width` (about 200; measure "Error · Destructive" in inspection.json and add a little).
- Trap: do not touch the ControlTheme; ButtonSpinner is the reference and is correct.

**CalendarView (task 7)**
- Width change: month view and the zoom grid are overlaid in one Panel and swapped by Opacity (styles on `UniformGrid#PART_ZoomGrid` and `Grid#PART_MonthBody`), so height holds. Width does not, because the zoom grid's month and year cells are created by the control only while zoomed. In month view the grid is empty and adds no width; zoomed, four columns of month names are wider than seven day columns, and the header text changes length too ("October 2026", "2026", "2020-2029"). Measure `Border#InlineRoot` width in month, year and decade (pin each mode in the preview if the control exposes a display-mode property; otherwise click `PART_HeaderButton` in the crawl) before changing anything.
- Fix direction: make the month body define the width, so zoom cells must fit inside it (their sub-theme is `DefaultCalendarViewDayButton` with the `.zoom` class: size the cells off the day cell width, or shorten the labels), and keep the header button from growing the row (it is in a `*` column, so check its MinWidth). A user-set Width must still win.
- The drop-down example is the third calendar (`DisplayAsPicker="True"`). Its focus ring `Border#PickerFocusRing` has `Margin="{size:ThicknessSm}"`, which insets the ring so 1px of the grey frame shows outside it. The ring should centre on the frame's stroke: drop the margin and get the radius from `ConcentricCorners` with the matching Inset (Rule 19), then measure that no frame pixel shows outside the ring.
- No left padding: `TextBox#PART_TextBox` is padded `Horizontal={size:SpacingMd}` (4); other fields use `{size:ContentPadding}` (8).
- Text sits low: measure the text's centre against `Border#PickerRoot`'s centre in inspection.json. The TextBox has vertical Padding 4 inside a 32 MinHeight frame, and `DefaultFramelessTextBox` (TextBox.axaml) brings its own MinHeight and inner ScrollViewer. Fix by padding, not by nudging with a margin.
- Literals: weeknumber `FontWeight="Bold"`, `HorizontalAlignment="Center"`, `VerticalAlignment="Center"`, and the TextBox's `HorizontalAlignment` `Stretch`.
- Trap: do not move the weeknumber font to italic. The comment explains a macOS font poisoning bug.

**Carousel (task 8)**
- "Slide 1" twice: the preview holds TWO carousels side by side (the second is the disabled one), each showing its first slide. Label them so it is obvious (an enabled one on slide 1, a second with `SelectedIndex="1"` showing "Slide 2", and the disabled one), stacked vertically.
- Overhang: the preview is a horizontal StackPanel of two 340px carousels (about 700px) in a pane about 440px wide, so the second spills into the dark pane. Stacking vertically fixes it; light-over-dark panes need task 2's option.
- Too pale: the frame is `Background={color:BackgroundDefault}` (Light10) on a canvas of nearly the same colour, and the border never draws because the template's only element is a ScrollViewer, whose template ignores BorderBrush and CornerRadius. Wrap the ScrollViewer in a Border that carries Background, BorderBrush, BorderThickness and CornerRadius, and move the fill to a stage that reads on the canvas in both variants (try `{color:AccentBrush Light6}` and measure contrast against `SurfaceDefaultBrush`). Record the stage you pick: ContentPage (task 9) must use the same one.
- Literals in the preview: `Margin="8"` (use `{size:SpacingLg}`). Width and Height on preview instances are allowed sample data.

**ContentPage (task 9)** and **CarouselPage (task 10)**
- One root cause for both: ContentPage sets `HorizontalContentAlignment` to `{theme:ControlHorizontalContentAlignment}` (Left), and its `PART_ContentPresenter` places the child by that, so a child's own `HorizontalAlignment="Center"` is ignored. CarouselPage's pages ARE ContentPages, so "First page" is left-aligned for the same reason. Fix it once in ContentPage: a page's content alignment should let the child decide (Stretch, through `{theme:ContainerHorizontalAlignment}` and `{theme:ContainerVerticalAlignment}`). CarouselPage then needs no alignment change of its own: that is the "comes from the theme" Luke asked for.
- Background: `{color:BackgroundDefault}` (Light10) is invisible on the canvas. Use the stage task 8 picked for Carousel.
- CarouselPage's template hard-codes `HorizontalAlignment="Stretch"` and `VerticalAlignment="Stretch"` on its inner Carousel: use the container tokens.
- Luke's "bound to a zoom on the theme control" is unclear. Check whether picking an accent swatch in the gallery recolours CarouselPage like other controls (the hue must reach through Carousel into each ContentPage), fix it if not, and say in your result what you found.

**CheckBox (task 11)**
- Cause of the wrong tick colour: `Path#CheckGlyph` Fill is `{color:AccentPrimaryBrush}`, a pinned hue's seed, so it ignores `Accent.Kind`. The checked fill is `{color:AccentBrush Light6}` with a border of `{color:OverlayStrongBrush}` (an overlay used as a state colour, which the rules forbid).
- Fix direction: checked is a filled control. Box Background `{color:AccentBrush Dark1}` (ThemeRules says glyphs sit on Dark1), glyph Fill `{color:AccentBrush Light10}`, no overlay border. Disabled then follows automatically through Opacity.
- Thin tick: the glyph is a filled outline (`{icons:Checkmark}`) in a Viewbox `Width={size:IconSizeMd}` inside `TickBorder` padded `SpacingMd`. Make it bolder by also stroking the Path in the same brush (`Stroke` plus `StrokeThickness="{size:ThicknessSm}"`), and give it more inset by shrinking the Viewbox (`{size:Scaled Value={size:IconSizeMd}, By=0.75}`) rather than growing the box.
- Press moves the label: there are two separate `^:pressed` styles (one for RenderTransform, one for Background); merge them. Then capture the pressed state and compare the label's centre Y with the rest state in inspection.json. If it moves, check `RenderTransformOrigin` and the Viewbox's `Height` binding to its own Width (it re-measures as it scales).
- Literal to fix: `IndeterminateBox` Width and Height `10`.

**ComboBoxItem (task 12)**
- Items do not stretch because the theme sets `HorizontalAlignment` to `{theme:ControlHorizontalAlignment}` (Left). A row in a list is a container: use `{theme:ContainerHorizontalAlignment}` (Stretch). ListBoxItem, TreeViewItem and MenuItem have the same question.
- The uneven inset is mostly ComboBox's popup (task 13): `Border#PopupBorder` pads 2 all round and `ItemsPresenter` adds 4 top and bottom only, so a selected pill sits 2px from the left edge and 6px from the top. Tell task 13, or fix it there if task 13 has already landed.
- Item padding is Horizontal 16, Vertical 8, with `MinHeight` 32 and `CornerRadius` `RadiusMd` (a pill). A pill inside a `RadiusMd` popup frame should take `ConcentricCorners`, or drop to `RadiusSm`.
- The styles only answer `:pointerover` and `:pressed`, never `theme:Preview.State`, so the preview cannot show hover or pressed. Add `^[(theme|Preview.State)=Hover]` and `=Pressed` beside them, as Button does, then add those examples.
- Disabled sets only a dim Foreground; other controls also take `{size:DisabledOpacity}`. Pick one and match ButtonSpinner's disabled look.
- `FocusAdorner` is not nulled: an item inside the popup's scroller shows focus with its own inset border (it already sets BorderBrush on focus), so set `FocusAdorner` to `{x:Null}`.
- Preview: every item gets one fixed Width.
- Selected states: Luke decided (18 Oct 2026): selected rows match ButtonSpinner's outlined look, not the filled column. Selected is `{color:AccentBrush Light6}` fill with `{color:TextDefault}` text (ButtonSpinner's pressed depth, so it reads as chosen without going dark); selected and hovered `Light5`, selected and pressed `Light4`; disabled, selected or not, is the whole row at `{size:DisabledOpacity}` like ButtonSpinner. Measure contrast of the text on the fill in both variants and nudge the stage one step if it falls below the outlined text figures in ThemeRules. This task sets the stages first; tasks 33 and 63 copy them. Report the final stages so the orchestrator can update ThemeRules.md's filled-column paragraph (you do not edit ThemeRules.md).

**ComboBox (task 13)**
- The blue boxes are not a custom background demo. They are the FOCUS state: `Border#HighlightBackground` is filled with `{color:AccentPrimaryBrush}` whenever the box is focused (examples 6, 7, 8 are Focused, Focused Warning, Focused Success). Delete that wash. Show focus like every other control: BorderBrush `{color:AccentFocusBrush}`, BorderThickness `{size:ThicknessMd}` on `Border#Background`, following task 3's FocusColorOnAccents rule.
- Error example is short: the template's root is `DataValidationErrors`, and `MinHeight` is set on the ComboBox itself, so the 32px is shared between the box and the error message below it. Move MinHeight onto `Border#Background` (a `^ /template/` style), or onto the layout Grid.
- The open example (`IsDropDownOpen="True"`) is third, so its popup covers the Hover and Pressed examples in the gallery. Move it last.
- Preview: replace the eleven boxes with one per nameable state: placeholder, selected, hover, pressed, focused, a Warning accent, disabled, error, editable, open.
- Literals: `Padding` is Left 16, Top 4, Bottom 4, Right 0 (use `{size:ContentPadding}`), `MaxDropDownHeight` 504, `ColumnDefinitions="*,32"` (structural, allowed).
- The popup's inset is yours: `Border#PopupBorder` pads 2 all round and the ItemsPresenter adds 4 top and bottom only, so items sit 2px from the sides and 6px from the top. Make the inset even, and use the same value as MenuFlyoutPresenter (task 17).
- The items' own stretch and padding belong to ComboBoxItem (task 12), which lands first.

**ToggleButton (task 14)**
- It doesn't match Button because its border is Transparent (Button's is `{color:BorderDefault}`), and ThemeRules notes it still carries old per-hue blocks. Make rest, hover, pressed, disabled and focus identical to Button, and keep the checked column (Dark3 fill, Dark4 hover, Dark5 pressed, Light10 text) from the rules.
- Disabled uses `PressedBackgroundDefault` with dim text (it reads as a pressed button); use Button's disabled look.
- Layout: three horizontal rows clip at the pane edge. One vertical StackPanel, one fixed width.
- CommandBar (task 15) will reuse this look for its toggle, so keep the checked styles reachable (a sub-theme or class CommandBar can apply).

**CommandBar (task 15)**
- Content is left-aligned because `CommandBarButton` and `CommandBarToggleButton` set `HorizontalContentAlignment` to `{theme:ControlHorizontalContentAlignment}` (Left) on a fixed `Width` of 64. These are button faces: use `{theme:ButtonHorizontalContentAlignment}` (Rule 12).
- The hover "random padding": neither button theme sets `CornerRadius`, so the hover fill is a square covering the whole 64x40 cell. Give the faces `{size:RadiusSm}` and an inset that matches the bar's own padding, so the hover pill sits evenly inside the bar.
- The checked toggle is `{color:AccentPrimaryBrush}` (a pinned seed), so it stays blue in every hue and theme. Make it ToggleButton's checked look (task 14 lands first): filled stage of the item's own hue with `Light10` foreground, and hover and pressed from the same family. Unchecked hover must be the outlined hover stage, not a hue.
- Neither button theme sets `Foreground` (Rule 13 says every themed control must). Set `{color:TextDefault}` and give the disabled state `{size:DisabledOpacity}`.
- Bars 2 and 3 show a pale rectangle to the left of the bordered frame. Find which element paints it in inspection.json (the bar's Background versus the frame Border) and make the fill and the frame the same box.
- Reload icon: `Avalonia.Themes.Default/Icons/Svg/Refresh.svg` is a three-quarter ring plus a triangle (`M 10 0 L 17 3.5 L 10 7 Z`) that overlaps the ring's open end. The generator joins every `<path>` into one geometry with the nonzero fill rule, so an overlap with the opposite winding cancels: that is the missing arrowhead pixels. The triangle also touches y=0, the edge of the viewBox. Redraw the arrowhead so it does not overlap the ring with opposite winding and sits inside the box. Shooting the bar proves it; there is no other icon test.
- The bar frame's radius should match the other framed controls (`{size:RadiusSm}`, or `RadiusMd` like other containers); measure what it is now.
- Preview: name the examples by state (rest, hover, pressed, checked, disabled). The comment above the preview says hover and pressed need live input because these themes lack `Preview.State` hooks; add `^[(theme|Preview.State)=Hover]` and `=Pressed` selectors as Button does.

**ContentControl (task 16)**
- Black text on the dark pane is task 2's root cause, not this file: ContentControl's theme carries only the ClipToBounds setter (its template comes from Avalonia code), so its TextBlock inherits from the preview pane, and the dark pane does not set Foreground. Do not add a Foreground setter to ContentControl to hide that. Confirm task 2 fixed it in your before shot.
- The work is the preview only: a ContentControl given a visible frame. The theme must not draw it, so give the sample content a themed frame, for example a `Border` with `Classes`, or a control that already draws one (a `GroupBox` or `HeaderedContentControl`), with the text centred both ways through `HorizontalContentAlignment` and `VerticalContentAlignment` on the ContentControl. Rule 20 still bans literal colours, so the frame's brushes must be tokens.

**MenuItem and MenuFlyoutPresenter (task 17)**
- White on white: the preview is a bare StackPanel of MenuItems with no menu surface. Host them in a real `MenuFlyoutPresenter` (or `ContextMenu`), which also gives them one shared width. A submenu only opens inside a real menu, because Avalonia's menu interaction handler lives on the root menu, so the bare StackPanel is also why "Submenu does nothing".
- Text sits off centre vertically: MenuItem `Padding` is Left 16, Top 4, Right 16, Bottom 8. Make the vertical edges equal (`{size:ContentPadding}`).
- Items are not stretched: `HorizontalAlignment` is `{theme:ControlHorizontalAlignment}` (Left). In a menu a row is a container: `{theme:ContainerHorizontalAlignment}`.
- Huge gap right of the chevron: `Path#PART_ChevronPath` has `Margin` Left 16 on top of the item's 16 right padding, and the chevron's column is a SharedSizeGroup. Icon to text gap: `PART_IconPresenter` and `PART_ToggleIconPresenter` have `Margin` Right 16 (`SpacingXxl`); use the same gap as Button's icon (`{size:SpacingLg}` or the padding).
- Selected uses `{color:OverlayWeakBrush}` as the row Background, which the rules forbid for states. Use the outlined hover stage (`{color:HoverBackgroundDefault}`), pressed `{color:PressedBackgroundDefault}`, and match Button's disabled look.
- The toggle support already exists (`^:checked:toggle` and `^:checked:radio` styles with `PART_ToggleIconPresenter`). ContextMenu (task 18) will rely on it, so make sure a `ToggleType="CheckBox" IsChecked="True"` item renders a theme-coloured tick and include one in the preview.
- MenuFlyoutPresenter: its frame's `Padding` is `0` in the template and the ItemsPresenter adds 4 top and bottom only, so items touch the left and right edges. Give one even inset (the same value ComboBox's popup ends up with), and literal `MaxWidth` 456 and `Padding="0"` become tokens or go.

**ContextMenu (task 18)**
- "With Checkbox" puts a whole `CheckBox` control inside the 16px icon slot, so all that shows is a squashed grey dot. Use the menu's own toggle: `<MenuItem Header="With checkbox" ToggleType="CheckBox" IsChecked="True" />` (task 17 makes it render).
- The clipped emoji: the icon is a `TextBlock` holding an emoji, which is wider than the 16px icon slot. Use a theme icon: `<MenuItem.Icon><PathIcon Data="{icons:Star}" /></MenuItem.Icon>`.
- The `<Separator />` leaves a gap but draws no line: check whether Separator (task 49) or MenuItem's `:separator` template is the cause before touching either; you own only ContextMenu.axaml.
- Uneven inset: the frame `Padding` is `0` and the ItemsPresenter's `Margin` is vertical only (2). Match MenuFlyoutPresenter's inset from task 17. `MaxWidth` 456 is a literal.

**Menu (task 19)**
- The top-level item theme `DefaultTopLevelMenuItem` sets no `Foreground` (Rule 13 says every themed control must), so "Standard" is black on the dark pane. Set `{color:TextDefault}`.
- Padding is horizontal 16 only, inside a Menu of fixed `Height` 32. Give the items vertical padding and let the bar size from them (MinHeight, not Height).
- Selected is `{color:OverlayWeakBrush}` (forbidden as a state Background). Use the outlined hover and pressed stages, as in task 17.
- Clicking does nothing because none of the preview's items has children. Give "File" and "Edit" child items so clicking opens a submenu (the crawler opens one level).
- The bar itself has no frame or fill, so it is invisible on the canvas. Give the Menu a themed background or border so it reads as a menu bar.

**DatePicker and DateTimePickerShared (task 20)**
- Black text on the dark pane: the DatePicker theme sets no `Foreground` (Rule 13), so the warning, error and default boxes inherit the pane's. Set `{color:TextDefault}`.
- Wrong padding: `Padding` (4 all round) is applied as the `Margin` of `Grid#LayoutRoot`, which is OUTSIDE the button face, so the frame is inset from the control and the text inside has no padding at all. Move the padding inside the face: `PART_FlyoutButton` should carry `{size:ContentPadding}`, and LayoutRoot no margin.
- Text off centre: the day, month and year TextBlocks are padded Top 4, Bottom 8. Equal vertical edges, or none, centred by `VerticalAlignment`. The same Top 4 / Bottom 8 pattern is in `DefaultDateTimePickerItem` in DateTimePickerShared.axaml: fix both.
- "September2026" with no gap: the content grid is `78*,Auto,132*,Auto,78*` and the divider Rectangles in the Auto columns have no width, so the columns collapse to their text. Give the dividers a width (`{size:ThicknessSm}`) and a gap (`{size:SpacingLg}` margin), or give the control a sensible MinWidth.
- The full-height pop-up: `DatePickerPresenter` has `Width` 296 but nothing limits its height, and each column is a looping `DateTimePickerPanel` (`ShouldLoop="True"`), which is infinitely tall when measured unconstrained. Give the presenter a MaxHeight built from tokens (for example `{size:Scaled Value={size:ControlHeightMd}, By=7}` for seven rows plus the accept row). Width 296 is a literal too.
- The selected value vanishing on hover: `DefaultDateTimePickerItem` (DateTimePickerShared.axaml) is `BasedOn` the ListBoxItem theme. ListBoxItem's selected sub-states (`^:selected:not(:focus)`, `^:selected:pointerover` and others) are more specific than this theme's plain `^:selected` and repaint the text `Light10` (near white) over the highlight. It also targets `Rectangle#PressedBackground`, a part ListBoxItem's template does not have. Give the picker item its own template and stop basing it on ListBoxItem, so ListBoxItem's restyle (task 33) cannot break it again.
- `Rectangle#HighlightRect` is filled with `{color:AccentPrimaryBrush}` (a pinned seed) under `TextDefault` text: use a stage of the control's hue, with text from the matching column.
- The `^ /template/ Panel` / `^:pointerover RepeatButton` style flips the up and down buttons visible on hover; check they don't cover the centre value.
- Preview: name each example in a label beside it or order them so they read (default, empty, hover, focused, Warning, disabled, error). The current fifth one is "Success and disabled", which is why it looks wrong.

**TimePicker (task 21)**
- Same family as DatePicker; read what task 20 did first and reuse it. Expect the same causes: no Foreground, padding applied outside the face, asymmetric text padding, a presenter with no height limit over looping panels, and selected items based on ListBoxItem.

**DrawerPage (task 22)**
- The invisible nav button and the wrong bar colours come from stock Fluent resource keys this theme never defines: `{DynamicResource DrawerPaneButtonIcon}` (the hamburger icon, used three times) and `{DynamicResource NavigationControlNavBarBackground}` (both bars). An undefined DynamicResource resolves to nothing, silently. Use `{icons:Menu}` (Icons/Svg/Menu.svg exists) and theme tokens for the bar.
- The toggle buttons get `Background="Transparent"` written locally in the template (Rule 2) and an inline `<DockPanel.Styles>` block. Put their look in a named sub-theme (Rule 7) with theme hover and pressed stages.
- Several literal `HorizontalAlignment="Center"`, `"Left"` and `"Stretch"` in the template: alignment tokens.
- The pane's background is `DrawerBackground`, which nothing sets: give it a setter from a token that contrasts with the page, so the open drawer reads as an overlay.
- Preview: the captions use `Opacity="0.7"` (a literal). Use `{color:DimTextDefault}` as their Foreground instead.

**DropDownButton (task 23)**
- No padding right of the arrow: `Padding` is applied to `PART_ContentPresenter` only, and the arrow `PathIcon#DropDown` sits in the Auto column with no margin, so it touches the frame. Give the arrow a right margin equal to the face's horizontal padding (or move the padding to the frame).
- Nothing drops down because no example has a `Flyout`. Give the examples a `<DropDownButton.Flyout><MenuFlyout>...` so clicking opens one (the crawler opens flyouts).
- Layout: the preview is two horizontal rows, so "Disabled" runs off the pane. One vertical StackPanel, every button one fixed width, as in Button.
- The arrow has a literal `HorizontalAlignment="Right"` and `VerticalAlignment="Center"` in the template.

**EmbeddableControlRoot (task 24)**
- The crash is structural: EmbeddableControlRoot is a TopLevel, and a TopLevel can never be a child inside another window's visual tree, so the preview template that instantiates one throws as soon as the gallery shows it (in the headless renderer it throws `PlatformNotSupportedException` instead). No styling fixes this.
- Fix: treat it like Window, PopupRoot and the other roots, which carry a "No design preview" comment and no `<Name>Preview` template. Remove the preview template, and make sure the gallery still opens (GalleryCatalogue.cs falls back to a page or drops the entry; if it still lists it, make it skip types deriving from `TopLevel`).
- Keep the ControlTheme: it sets the root text properties (Rule 13).

**Expander (task 25)**
- The "rectangle on the right" on hover: the chevron's `Path#ExpandCollapseChevron` never shows (it is a `Path` with `Stretch="None"`, drawn by `Stroke` only, from icon geometry made to be filled), and on hover `Border#ExpandCollapseChevronBorder` fills with `{color:AccentBrush Dark1}`: a dark square with nothing in it. Replace the Path with a `PathIcon` (`Data` from the direction sub-themes, sized `{size:IconSize}`, coloured by Foreground), and make the hover fill the outlined hover stage.
- Hover, pressed and disabled look the same because the header's hover and pressed both set `Background` to `{color:BackgroundDefault}`, the rest value. Use `HoverBackgroundDefault` and `PressedBackgroundDefault`, and disabled `{size:DisabledOpacity}`.
- Borders use `{color:OverlayWeakBrush}` (forbidden for anything but shadows): use `{color:BorderDefault}`.
- The Expander restates `FontFamily`, `FontSize` and `FontWeight` (Rule 13: delete them). The chevron animation `Duration="0:0:0.0625"` is a literal: use `{theme:AnimationFastMs}`.
- Preview: put each direction (Up, Left, Right) in a fixed-size frame so expanding does not move everything else, and stack the examples vertically at one width.

**FlyoutPresenter (task 26)**
- The clipped corners: inside the rounded `Border#LayoutRoot` sits a square `ScrollViewer`, which paints over the frame's corners (Rule 19: a square child inside a rounded frame covers its stroke). ComboBox solved the same thing by giving its scroller `Background="Transparent"` in a `^ /template/ ScrollViewer` style. Do the same, then measure that no corner pixel of the stroke is covered.
- Literals: `MinWidth` 96, `MaxWidth` 456, `MinHeight` 40, `MaxHeight` 758, and `Padding="0"` in the template.
- Preview: two plain TextBlocks. Show what a flyout really holds: a heading (`FontWeight` from `{theme:FontWeightSemiBold}`), body text, and a Button, padded by the presenter's own Padding (`SpacingXxl`, which is right for a surface).

**GridSplitter (task 27)**
- Only a dot shows because the preview Grid has `ColumnDefinitions="*, Auto, *"` but no size, inside a StackPanel with `HorizontalAlignment="Left"`. The star columns collapse to 0, and the panes are empty Borders. Give the Grid a fixed Width and Height (for example 360 by 160), put a label in each pane, and give the panes a fill that reads on the canvas (`{color:BackgroundDefault}` is invisible). Then the crawler's 20px drag can move it.
- Theme: `MinWidth` and `MinHeight` `6` are literals (use `{size:ThicknessXxl}`), and `PreviewContent` fills with `{color:AccentPrimaryBrush}`, a pinned seed. Use `{color:AccentBrush Dark3}`. Add a hover and pressed stage so the grip shows it can be dragged.

**GroupBox (task 28)**
- Text colour, decided: content text stays `{color:TextDefault}` (Dark9), the role every control and ButtonSpinner use, so GroupBox reads like the rest of the theme. Near-black in light is correct. Give the header label the hue instead, through the outlined hover-text stage `{color:HoverTextDefault}` (Dark7) on `ContentPresenter#PART_HeaderPresenter`, so an accented GroupBox is recognisable in both variants while the body text matches every other control. Do not change `TextDefault` itself.
- Real defects here: the "Warning · Disabled" example is not dimmed because the theme has no `^:disabled` style at all. Add `{size:Scaled Value={size:DisabledOpacity}, By=1.4}` for accented, or `{size:DisabledOpacity}`.
- The frame is `{color:AccentBrush Dark1}`, far stronger than every other control's `{color:BorderDefault}` (Light1).
- `FontSize` `FontSizeLg` on the GroupBox inherits into the content too, so the body text is 16px. If only the header should be larger, set it on `ContentPresenter#PART_HeaderPresenter` instead.
- Keep the `Theme Exception:` converter for the header notch as it is.

**HeaderedContentControl (task 29)**
- The template is a Border holding a Grid of header and content with no gap, and the padding is `{size:SpacingMd}` (4). Use `{size:ContentPadding}`, add a gap between header and content (row spacing or a header margin from tokens), and make the header read as a header: `{theme:FontWeightSemiBold}` on `ContentPresenter#PART_HeaderPresenter`, optionally a divider below it.
- The border is `{color:AccentBrush Dark1}`; other frames use `{color:BorderDefault}`.
- `RowDefinitions="auto *"` is fine (structural).

**HyperlinkButton (task 30)**
- Luke wants it to look like text: no fill, no border, no underline, at rest or on hover or pressed. Today it is a button: `Padding` ContentPaddingWide, `MinHeight` 32, `MinWidth` 64, hover fills `HoverBackgroundDefault` and grows to 1.07 scale, pressed fills `PressedBackgroundDefault`, and `TextBlock.TextDecorations` is `Underline`. Remove all of it. Keep the focus ring as the only frame (follow task 3's rule).
- Colour: `Foreground` is `{color:AccentPrimaryBrush}`, a pinned seed, so accents never change it. Rule 13 lists this as deliberate, and the orchestrator updates that line of ThemeRules.md from your report. Give the theme a default hue instead of a pinned brush: `<Setter Property="theme:Accent.Kind" Value="Primary" />` (an example's own `Accent.Kind` then overrides it), and colour the text by stage: rest `{color:AccentBrush Dark3}`, hover and pressed stepping further along the ink ramp (try Dark5 and Dark7). Check contrast on the canvas in both variants. Success then turns the text green, which is what Luke asked for.
- Warning "too saturated" is the hover fill (Warning `HoverBackgroundDefault`); it goes with the fill.
- `:visited` sets the text to `TextDefault` (black): visited should be a quieter stage of the same hue.
- Preview: vertical StackPanel; the horizontal row clips "Disabled" off the pane.

**ItemsControl (task 31)**
- An ItemsControl has no selection by design (that is ListBox). Say so in the preview by what it shows: a short list of items with an `ItemTemplate`, spaced by `ItemsPanel` spacing, so it reads as a list rather than two loose TextBlocks.
- Padding is 4 (`SpacingMd`); use `{size:ContentPadding}` like every other framed container.

**Label (task 32)**
- The theme has no state styles at all: no disabled, no hover. Add `^:disabled` (`{color:DimTextDefault}` or `{size:DisabledOpacity}`, matching Button).
- Preview: show Label in Button's states: normal, each accent (`theme:Accent.Kind`), disabled. Show what makes it a Label: one with `Target` pointing at a TextBox and an access key (`_File`), so the underline on the access key is visible.
- `Padding` is `{size:SpacingMd}`: compare with TextBlock-adjacent controls and use `{size:ContentPadding}` if it sits beside fields.

**ListBoxItem (task 33)**
- Today's states follow ThemeRules.md's filled column (selected `Dark3` with `Light10` text, hover `Dark4`, pressed `Dark5`, disabled dim text at full opacity). Luke decided (18 Oct 2026): selected rows match ButtonSpinner's outlined look, not the filled column. Selected is `{color:AccentBrush Light6}` fill with `{color:TextDefault}` text (ButtonSpinner's pressed depth, so it reads as chosen without going dark); selected and hovered `Light5`, selected and pressed `Light4`; disabled, selected or not, is the whole row at `{size:DisabledOpacity}` like ButtonSpinner. Measure contrast of the text on the fill in both variants and nudge the stage one step if it falls below the outlined text figures in ThemeRules. Use exactly the stages ComboBoxItem (task 12) settled on, so all three row controls match.
- Padding is Left and Right 16, Top and Bottom 12 with MinHeight 32: rows are tall next to every other control. Use `{size:ContentPadding}` vertical.
- `HorizontalAlignment` is already Container (Stretch), and `FocusAdorner` is already null with an inset focus border: the pattern ComboBoxItem and TreeViewItem should copy.
- Add `theme:Preview.State` selectors (`Hover`, `Pressed`) so the preview can show those states, as Button does.

**ListBox (task 34)**
- Off screen: the preview is a horizontal StackPanel of three 180px ListBoxes inside a pane about 440px wide. Stack them vertically.
- The three boxes demo SelectionMode values (Multiple, Toggle) that look identical in a still frame. Label each box (a caption, or the item text) with what it shows: states, a Warning hue, multi-select.

**ManagedFileChooser (task 35)**
- The biggest task in the plan: it is a whole file dialog (sidebar of quick links, a toolbar with up, location and refresh, a file list with a Name header, a file-name field, a filter box, "Show hidden files", plus a separate overwrite prompt). Work part by part, re-shooting after each, rather than in one rewrite.
- Too narrow: the preview places it in a ~440px pane with only `MinHeight="240"`. A dialog needs room: give it a fixed Width and Height (about 640 by 400) and stacked light-over-dark panes (task 2's option).
- Empty: with no DataContext the dialog has no files, no location, no filter, which is why the list and boxes are blank and the toolbar's location box collapses to a stub. Find whether Avalonia.Dialogs offers a view model you can construct for a design-time sample (`ManagedFileChooserViewModel`); if it can't be built without a real file system, keep it empty but make every empty part read correctly (sized boxes, a visible empty list frame).
- 31 literal hex colours: most are icon artwork (exempt by ThemeRules); check each is inside a `DrawingGroup` icon. The hex `BoxShadow` (`inset -6 0 3 -3 #20000000`) is a known violation: replace with a shadow Border using an overlay brush.
- 19 literal sizes: replace with tokens one by one.
- The quick-links sub-theme targets `PART_ShadowBorder` through a dedicated ListBox theme; keep that structure.
- If the theme engine cannot express something here, stop and report it rather than inventing values.

**NavigationPage (task 36)**
- Empty because the preview is a bare `<NavigationPage />` with nothing pushed: no page, no title, no back button. Give it a root page in markup (a `ContentPage` with a Header and some content as the NavigationPage's content), and if a static preview can show a second level (back button visible), add one.
- Its bar colours come from `SolidColorBrush` resources keyed `NavigationBarBackground` and `NavigationBarForeground` built from `{color:NeutralLight10}` and `{color:NeutralDark9}`, read with `{DynamicResource}`. A brush resource built this way is pinned to the neutral ramp and resolved once, so it ignores the dark region and the control's hue (Rule 16 and the hidden DynamicResource trap). Set the bar's Background and Foreground with setters from tokens (`{color:BackgroundDefault}` and `{color:TextDefault}` or the right stage) on the parts, and delete the brush resources.
- `{DynamicResource NavigationPageBackIcon}` is a stock key the theme never defines: use `{icons:ChevronLeft}` or `{icons:ArrowLeft}`.
- The bottom divider uses `{color:OverlayWeakBrush}` as a Background (forbidden): use `{color:BorderDefault}`.

**NotificationCard (task 37)**
- Shooting trap first: the card's ControlTheme has an entrance animation starting at `Opacity` 0. A still capture taken before the animation has run shows nothing (the throwaway renderer used to write this brief caught all four cards invisible). Capture with the clock advanced (`guard xaml inspect ... --clock 0,150,300,450,600`) or check the after shot is not blank before trusting it.
- All four look the same because the kind only recolours `Panel#PART_HeaderBar`, and with pinned seeds (`{color:AccentInfoBrush}` and so on). Make each kind set the card's hue instead: in `^:information`, `^:success`, `^:warning` and `^:error`, set `theme:Accent.Kind` (Info, Success, Warning, Error). Then colour the bar, border and text with `{color:AccentBrush ...}` stages once, so all four follow from one block (Rule 16).
- Top-left plain text: `ContentPresenter#PART_ContentPresenter` has no Padding and no content alignment. Give it `{size:ContentPadding}` (or `SpacingXl` for a surface) and vertical centring. Consider a title and message layout like a real notification.
- The shadow is already a Border with `{color:OverlayMediumBrush}`, as ThemeRules wants; ThemeRules' note about a hex BoxShadow here is out of date.
- Animation `Duration="0:0:0.45"` could be `{theme:AnimationSlowMs}`; keyframe values (20, 0.85) are exempt trajectory values.

**NumericUpDown (task 38)**
- Its template is a ButtonSpinner holding a full themed `TextBox`, so the TextBox draws its own frame inside the spinner's frame, patched with a `-1` negative margin (`^ /template/ TextBox#PART_TextBox`). Use the frameless sub-theme instead: `Theme="{StaticResource DefaultFramelessTextBox}"` (TextBox.axaml), the one ComboBox and CalendarView already use, and drop the negative margin.
- The ButtonSpinner gets `Padding="0"` and `MinWidth="0"` written in the template, which is why its buttons are cramped (about 17px wide where ButtonSpinner's are 41). Let the ButtonSpinner use its own theme values; ButtonSpinner is the reference for the look, and NumericUpDown should be pixel-identical to it except for the content.
- Padding is Left 16, Top 8, Right 8, Bottom 4: asymmetric, so the value sits off-centre. Use ButtonSpinner's padding.
- The "X" buttons in InnerLeftContent and InnerRightContent are full Buttons with frames; a small frameless icon button reads better. That is a preview choice.

**PathIcon (task 39)**
- The theme itself is right: it sets no Foreground (Rule 13: icons take their container's colour) and its default size is `{size:IconSizeMd}`. The preview is what is wrong.
- The three accent examples each set `Foreground="{color:AccentPrimaryBrush}"` locally, a pinned seed, so Warning and Success stay blue. Remove the local Foreground and put each icon in a container that carries `theme:Accent.Kind` and a Foreground from `{color:AccentBrush ...}` (a Button, or a TextBlock-coloured StackPanel), so the icon visibly follows its container.
- The "random sizes" are actually `IconSizeSm`, `Md`, `Lg` and `Xl`, but nothing says so. Label each size and lay them out on one baseline so the scale reads as a scale.

**PipsPager (task 40)**
- The vanishing dots, root cause: hover and selected set the pip's `Width` and `Height` to `{size:ThicknessXxl}`. That token is a Thickness, and a Thickness token in a double slot silently becomes 0 (the wrong-type trap in the primer). ai-mecca's own measurements record the selected pip at 0x0 for exactly this reason. Use a double token: the rest pip is `{size:SpacingMd}` (4), so the selected and hover pip could be `{size:SpacingLg}` (8).
- Background: the faint panel sits behind the dots only. Draw one rounded frame (a Border with `{size:RadiusMd}` and a stage that reads on the canvas) around the arrows and the dots together, in the PipsPager template.
- Literal sizes: the arrow buttons are `Width` and `Height` 24, the pip items 12 by 24 (24 by 12 vertical). Use tokens.
- The first and last page hide an arrow with `Opacity` 0, which still takes clicks and is undone by any broader Opacity style. Use `IsVisible` or `IsHitTestVisible="False"` with it. The uneven arrow padding: measure the gap from each arrow to the nearest pip in inspection.json and make them equal.

**ProgressBar (task 41)**
- The indicator fill and the progress text both use `Foreground` `{color:AccentPrimaryBrush}`: a pinned seed (so a Warning bar has a blue fill on a yellow track), and the "50%" text is the same colour as the fill under it, so it disappears where the two overlap. Fill with `{color:AccentBrush Dark3}` (the filled column) over a `{color:AccentBrush Light8}` track, and draw the text in a colour that reads on both (or place it beside the bar).
- Heights: `MinHeight` is `{size:SpacingMd}` (4) for the plain bar, while bars with text grow to fit it, so the column mixes 4px and 16px bars. Pick one height token for the bar (consider a ThicknessXl/Xxl bar and a taller variant only when text shows).
- Four thin bars that look identical: values 5 of 10, 50, 100, and 25 to 75 at 50. Three of them read as half full. Pick values that differ visibly (0, 25, 75, 100) and put a short label beside each.
- Indeterminate bars render empty in a still frame; ai-mecca's notes say the indeterminate sweep does not move under the headless render timer, so judge them with `--clock` or accept that row as unprovable in a still capture and say so.
- Only one of the four vertical bars shows in the gallery: measure the other three in inspection.json (zero width is likely, from the `:vertical` MinWidth `{size:SpacingMd}` with no Width).
- Keep both `Theme Exception:` lines (indicator anchoring) as they are.

**RadioButton (task 42)**
- Selected shows nothing, root cause: `Ellipse#CheckGlyph` (the dot) is the same size as the rings (all three are `{size:IconSizeMd}`), so the Light10 dot covers the whole filled ring. Make the dot smaller (`{size:Scaled Value={size:IconSizeMd}, By=0.4}`).
- The ring is invisible at rest: `OuterEllipse` is Light10 fill with a Light6 stroke, on a Light10 control background. Use the CheckBox's box colours (task 11 lands first), with the stroke at `{color:BorderDefault}` or stronger.
- The circle hugs the top-left corner: `Grid#PART_CheckBoxGrid` has a literal `VerticalAlignment="Top"` and no margin from the frame. Centre it vertically with the label and give it the same inset as CheckBox's box.
- `CheckOuterEllipse` is `{color:AccentPrimaryBrush}`: use the filled stage `{color:AccentBrush Dark1}` (glyph on Dark1, per ThemeRules), like CheckBox.
- Frame: RadioButton draws a bordered frame like a button (border `{color:OverlayMediumBrush}`, an overlay used as a border), while CheckBox has none. Match CheckBox: no frame at rest, same hover and pressed treatment.

**RefreshVisualizer (task 43)**
- The preview is a single 16x16 `RefreshVisualizer` and nothing else; in its idle state the visualizer draws nothing, so the pane looks empty. Run `guard xaml control RefreshVisualizer` to see its parts and whether its state can be pinned from markup. If the state is read-only, show it as it appears mid-pull by hosting it in a RefreshContainer (task 44), or render its arrow content at a readable size with a caption, and say in your result what could not be shown statically.
- Check the `Background` and `Foreground` setters resolve to tokens that read on the canvas (a white page suggests a Background stage equal to the canvas).

**RefreshContainer (task 44)**
- "Nothing happens when you pull": Avalonia's RefreshContainer is driven by a pull gesture (touch or pen), not a mouse drag. Check whether the mouse can trigger it at all here (the crawler's drag is mouse); if not, the gallery cannot demonstrate a live pull, and the preview should say so in a caption and show the visualizer in view instead of an empty strip.
- The preview has no size: a one-line TextBlock with no ScrollViewer. Give it a fixed size with a short scrollable list inside, which is what pull-to-refresh wraps in real use.
- `Padding` is 4 (`SpacingMd`); use `{size:ContentPadding}`. The border is `{color:AccentBrush Dark1}`; other frames use `{color:BorderDefault}`.

**RepeatButton (task 45)**
- It should look exactly like Button and doesn't: its border is `{color:AccentBrush Dark1}` (Button uses `{color:BorderDefault}`), its content alignment is `{theme:ControlHorizontalContentAlignment}` (Button uses `{theme:ButtonHorizontalContentAlignment}`), and ThemeRules notes it still carries old per-hue blocks. Copy Button's setters and states; ideally base its theme on Button's look rather than duplicating it.
- What makes it a RepeatButton is behaviour (it fires repeatedly while held), which a static preview with no view model cannot show by counting. Say it in the content ("Hold to repeat") and show `Delay` and `Interval` set on an example; if you can wire a visible repeat without code-behind, do, but do not add a view model to the theme.
- Layout: two horizontal rows overflow the pane. One vertical StackPanel, one fixed width, like ButtonSpinner.

**ScrollBar (task 46)**
- Off screen: the preview is one horizontal StackPanel of five 100px horizontal bars plus a column of vertical ones, far wider than the pane. Stack the horizontal bars vertically at one width, with the vertical ones in a row beneath.
- The "squashed dot" is the first bar: `AllowAutoHide="True"` collapses it to a thin resting strip whose thumb shrinks to a pill a few pixels high. Decide what the resting look should be and make the collapsed thumb a clean pill (its radius from a token, concentric with the track) rather than a squashed ellipse.
- Several parts are hidden with `Opacity` 0 (the line-up and line-down buttons, the track), which still take clicks and come back under any broader Opacity style. Use `IsVisible` or Transparent backgrounds (the primer's Opacity trap).
- The track is `{color:AccentBrush Light2}` and the scroller background `{color:BackgroundDefault}`; an overlay (`OverlayWeakBrush`) is used as a hover Background, which the rules forbid. Bring the track, thumb, hover and pressed onto the outlined column's stages.
- `CornerRadiusTransition Duration="0:0:0.1"` is a literal: use `{theme:AnimationFastMs}`.

**ScrollViewer (task 47)**
- The template already puts the bars beside the content (Grid `*,Auto`) and `AllowAutoHide` defaults to False, so first measure in inspection.json whether the vertical bar's bounds overlap `PART_ContentPresenter`'s in each example. Expect most of "content covered" to be the preview: each viewer's Grid is 300px wide in a 180px viewer, but `HorizontalScrollBarVisibility` defaults to Disabled in Avalonia, so the third column is cut off under the right edge with no horizontal bar to reach it. Set `HorizontalScrollBarVisibility="Auto"` on the examples.
- The second example sets `AllowAutoHide="True"`, which makes the bars overlay the content by design; label it as such, or drop it.
- Preview literals: `Margin="0,0,0,4"` and the cell `Margin="4"` (use tokens), and the cells are filled with `{color:OverlayWeakBrush}` (overlays are for shadows only; use a fill stage). Stack the examples vertically; three 180px viewers side by side overflow the pane.
- `PART_ScrollBarsSeparator` is shown and hidden by Opacity with a literal `0:0:0.1` transition.

**SelectableTextBlock (task 48)**
- The selection colour is `SelectionBrush` `{color:AccentPrimaryBrush}`, a pinned seed, with no matching selection foreground, so it ignores the hue and the text on it may not read. Use the filled column: `SelectionBrush` `{color:AccentBrush Dark3}` and `SelectionForegroundBrush` `{color:AccentBrush Light10}` (check the property exists on SelectableTextBlock in Avalonia 12 with `guard xaml control SelectableTextBlock`).
- `Background` is `{color:BackgroundDefault}`: a text block should not paint a fill; use Transparent so it sits on whatever it is placed on, like TextBlock. `Padding` `{size:SpacingMd}`: text blocks normally have none; match TextBlock unless there is a reason.
- Preview: nothing is selected, so the selection colour is never seen. Pin a selection in markup (`SelectionStart` and `SelectionEnd`) on one example, and show a Warning-hued one.

**Separator (task 49)**
- Shows nothing, root cause: `Height` is `{size:ThicknessSm}`, a Thickness token in a double slot, which is 0 (the wrong-type trap, fixed by task 4's double ladder). Use the double token task 4 adds.
- The preview puts one Separator in a StackPanel with `HorizontalAlignment="Left"`, so even at a real height it would be 0 wide. Give the preview a fixed width, and add a vertical separator between two items in a horizontal row.
- Then check its colour reads on the canvas in both variants (it is `{color:AccentBrush Light6}`; frames use `{color:BorderDefault}`), and that it shows inside a menu (ContextMenu, task 18, puts one between items).

**Slider (task 50)**
- "Random dots": the track is invisible because `Border#TrackBackground` gets `Height` (horizontal) and `Width` (vertical) from `{size:ThicknessSm}`, a Thickness token in a double slot, which is 0. Likewise the fill bars at `{size:ThicknessXxl}`. Only the thumbs survive. Use task 4's double thickness tokens.
- The thumb and the filled part are `{color:AccentPrimaryBrush}` (pinned seed), and the error state pins `AccentErrorBrush` on the fill. Fill with `{color:AccentBrush Dark3}`, track `{color:AccentBrush Light6}` or the BorderDefault stage, thumb in the filled column, so Warning and Success sliders recolour.
- Give the horizontal sliders a fixed width in the preview (they stretch to the DockPanel today, which is fine) and the vertical ones a fixed height, and label each example.

**SplitButton (task 51)**
- Why its style differs from ButtonSpinner: the rest `Background` is `{color:OverlayWeakBrush}` (an overlay used as a fill, which the rules forbid), with a Transparent border, and disabled is `OverlayWeakBrush` plus dim text. ButtonSpinner rests on `{color:BackgroundDefault}` with a `{color:BorderDefault}` border and `TextDefault`. Use exactly ButtonSpinner's outlined column for rest, hover, pressed and disabled.
- The checked states (`Dark3`, `Dark4`, `Dark5` with Light10 text) follow the rule's filled column, which still applies to toggles (Luke's lighter-selection decision covers list rows only). Checked and disabled ("Checked Off") drops the fill per the rule, so it reads as bare text: give it ButtonSpinner's disabled look (whole control at `DisabledOpacity`) instead.
- Preview: stack vertically with one fixed width; label which examples are toggled.

**SplitView (task 52)**
- The pane is invisible because `PaneBackground` is `{color:BackgroundDefault}`, about the canvas colour. Give the pane a stage that reads against the content (and a divider on its inner edge with `{color:BorderDefault}`).
- "Main conte" is clipped because no example sets `OpenPaneLength`, so the pane takes Avalonia's default width, most of a 400px example. Set `OpenPaneLength` (and `CompactPaneLength` where used) to a small value on each example.
- The light-dismiss overlay is a flat grey that does not change between light and dark: find what paints it and use `{color:OverlayMediumBrush}` (a scrim is exactly what overlays are for).
- Preview literals: caption `Opacity="0.7"` (use `{color:DimTextDefault}`), `FontWeight="Bold"` (use `{theme:FontWeightSemiBold}`), `Margin="10,10,0,0"` (tokens), `Height="96"` is allowed sample size.
- "NAV" renders black on the dark pane until task 2 lands; re-check after.

**TabItem (task 53)**
- The "random lines through the headers" are the selected indicator: for Top and Bottom placement, `Border#PART_SelectedPipe` gets a fixed `Height` (`SpacingSm`) with `VerticalAlignment` `{theme:ContainerVerticalAlignment}` (Stretch). A fixed height with Stretch centres it, so the pipe is drawn through the middle of the text. It must sit at the bottom edge for Top placement and the top edge for Bottom placement: an intrinsic alignment, so a literal with a `Theme Exception:` comment giving that reason (Rule 14). Left and Right placement have the same question horizontally.
- The pipe is `{color:AccentPrimaryBrush}`: use `{color:AccentBrush Dark3}` so a Warning tab's indicator is Warning.
- Hover, pressed and disabled all set Background Transparent and nothing else, so states don't show. Give hover and pressed the outlined text or background stages, and disabled `{size:DisabledOpacity}`.
- `FontSize` is `{size:FontSizeXxl}` (24); ThemeRules Rule 13 lists that as deliberate, but Luke says the tabs don't match the theme's sizes. Bring it down (FontSizeMd or Lg, with `MinHeight` from a control height token) and report the change so the orchestrator updates that line of the Rule 13 table.
- Literal margins on the pipe and presenter: `"0,0,2,0"`, `"8,0,0,0"`, `"0,0,0,2"`, `"2,0,0,0"`, `"0,0,8,0"`.
- Preview: the horizontal row of seven tabs is cropped at the pane edge. Stack them vertically, one per state, at one width.

**TabControl (task 54)**
- The stray line through "Home" is TabItem's pipe (task 53 fixes it); re-shoot after 52 before changing anything here.
- Selected content: the sample page is a Border filled with `{color:OverlayWeakBrush}` (an overlay as a fill, forbidden) with a literal Height, and the TabControl's `Padding` is horizontal only (16 left and right, 0 top and bottom), so content touches the bottom. Give the content area even padding from `{size:ContentPadding}` or `SpacingXl`, and the sample page a real fill stage.
- "Page A" and "Page C" are the Left and Right `TabStripPlacement` demos: tabs "A, B" down the left with "Page A" as the content, and "C, D" down the right. They read as nonsense because the headers are 24px letters with the pipe drawn wrongly and the content a stray TextBlock. Make them clearly labelled ("Left tabs" and "Right tabs", real header text, content that says which tab is open) and stacked rather than side by side.

**TabStripItem (task 55)**
- Each item draws its own frame (`BorderBrush` `{color:AccentBrush Dark1}`, `BorderThickness` 1, radius `RadiusSm`), inside TabStrip's frame: that is the double border. A tab strip item should not have a frame; its selection is the indicator, and hover and pressed are a fill stage.
- `PART_SelectedPipe` is `{color:AccentPrimaryBrush}` with literal-free but arbitrary Height and margins: make it identical to TabItem's indicator after task 53 (same stage, thickness, position), so the two tab families match.
- `FontSize` `{size:FontSizeXxl}`: match TabItem's new size.
- Hover, pressed and disabled are all Transparent: give them the same treatment as TabItem.

**TabStrip (task 56)**
- Its own frame (`BorderBrush` `{color:AccentBrush Dark1}`, padding `SpacingSm`) plus every item's frame (task 55) is the double border. Keep at most one frame: either a subtle container with no item frames, or none. Use `{color:BorderDefault}`, not Dark1.
- The preview wraps onto two rows because the 24px items are wide; after task 55 they shrink.

**TabbedPage (task 57)**
- Its colours come from stock Fluent keys this theme never defines: `{DynamicResource TabbedPageTabStripBackground}`, `TabbedPageTabItemHeaderBackgroundUnselected` and `TabbedPageTabItemHeaderBackgroundDisabled`. An undefined DynamicResource resolves to nothing. Use theme tokens.
- "Home page" is left-aligned for the same reason as ContentPage (task 9: content alignment set to Left); re-shoot after task 9 before changing alignment here.
- The selected underline spans the whole tab width and a full-width line runs under the strip: make the indicator match TabItem's after task 53 (same stage, thickness, length rule), and decide if the full-width divider stays (as `{color:BorderDefault}`, `{size:ThicknessSm}` via the double ladder of task 4).
- Four `Theme Exception:` lines already exist in this file: read their reasons before touching those setters.

**TextBox (task 58)**
- Preview widths: every TextBox sizes to its text, so "Center aligned" cannot show centring. Give all of them one fixed Width.
- The floating placeholder changes the layout: `TextBlock#PART_FloatingPlaceholder` sits in its own `Auto` row above the frame and is only made visible once the box has text (`^[UseFloatingPlaceholder=true]:not(:empty)`), so the control grows by a row the moment you type. Reserve the row whenever `UseFloatingPlaceholder` is true (show and hide with Opacity plus `IsHitTestVisible="False"`, not IsVisible), or float the label over the frame's top edge without taking layout space. Measure the control's height empty and filled; they must be equal.
- The reveal-password and clear buttons are full themed buttons in `InnerRightContent` (set by the `revealPasswordButton` and `clearButton` class styles), so they bring Button or ToggleButton padding (`ContentPaddingWide`), a frame, and when revealed a dark checked fill. Give them a small frameless icon-button sub-theme (Rule 7) with an inset equal to the field's padding. The icons are already the theme's (`{icons:Eye}`, `{icons:EyeOff}`).
- The "Accent focused" example shows the magenta ring on a Warning field: after task 3 it should keep the Warning border.

**TextSelectionHandle (task 59)**
- The handle is a drop shape filled with `{color:AccentPrimaryBrush}` (a pinned seed); use the hue stage the TextBox selection uses (`{color:AccentBrush Dark3}`, matching task 48's selection colour) so handles and selection agree and follow the hue.
- Its geometry and drawings are local resources (`TextSelectionHandlePath`, `TextCaretHandlePath`), read with DynamicResource; those keys ARE defined in this file, so they work, unlike the stock keys elsewhere. Check the literal sizes (2 literal values) and the existing `Theme Exception:` line before changing them.
- Preview: one handle on its own. Show the caret handle and both selection handles, ideally beside a TextBox with a pinned selection so their size relative to text is visible.

**ThemeVariantScope (task 60)**
- Luke decided (18 Oct 2026): if ThemeVariantScope is an old-theme mechanism this theme replaces, delete it. This theme switches light and dark with the engine's own `theme:ThemeRegion.IsDark="True"` (see LightDarkPreview.axaml), not Avalonia's ThemeVariant.
- First prove it: put a Button inside `ThemeVariantScope RequestedThemeVariant="Dark"` in a light pane and measure whether any brush changes in inspection.json. If nothing changes, delete `Controls/ThemeVariantScope.axaml` and remove whatever includes or lists it (search the theme and the lab for `ThemeVariantScope`), so the gallery no longer offers it.
- Trap: ThemeRules Rule 13 names ThemeVariantScope as a text root (it sets Foreground and fonts). Before deleting, check nothing in the theme or Avalonia's own popups relies on that ControlTheme for text properties; if something does, report it instead of deleting. Report the Rule 13 line so the orchestrator can update ThemeRules.md.
- If the scope DOES flip this theme, do not delete: report the measurement to the orchestrator and stop.

**ToggleSwitch (task 61)**
- The background fills around each item: the ToggleSwitch theme sets `Background` `{color:BackgroundDefault}`, painted by the template's root Grid behind the header and the switch, so every example sits on a pale rectangle (accented ones on a tinted one). A switch has no surface: make the control Background Transparent and keep fills on the track only.
- The checked track is `{color:AccentPrimaryBrush}` (pinned seed), so Warning stays blue. Use `{color:AccentBrush Dark3}` with the knob `Light10`. Hover fills use `{color:OverlayWeakBrush}` (forbidden): use the outlined hover stage.
- On and off content are swapped by many Opacity setters (the primer's Opacity trap); keep them but make sure the hidden one is not hit-testable.
- 13 literal values: replace each with a token.

**TransitioningContentControl (task 62)**
- Nothing transitions because the content never changes: one fixed TextBlock. Without a view model, drive it from another control: a small `ListBox x:Name="Pick"` of three strings and `<TransitioningContentControl Content="{Binding #Pick.SelectedItem}" />` beside it, so clicking an item cross-fades (the crawler clicks list items). Check `PageTransition` uses a token duration (`{theme:AnimationNormalMs}`).
- Give the presenter a visible frame so the area being transitioned is obvious.
- Rule 18 notes its root panel deliberately clips locally (the slide must stay inside): keep that.

**TreeViewItem (task 63)**
- Not stretched: `HorizontalAlignment` is `{theme:ControlHorizontalAlignment}` (Left), so each row ends at its text. A row is a container: `{theme:ContainerHorizontalAlignment}`, as ListBoxItem does.
- No padding on the right: `Padding` is Horizontal 4 (`SpacingMd`), Vertical 2. Use `{size:ContentPadding}` like ListBoxItem.
- Too much space around the chevron: its area has `Margin` Horizontal 16 (`SpacingXxl`) on each side, and the chevron's `Width` is `{size:FontSizeSm}` (a font size used as a width). Use the icon size token and the same small gap Button uses between icon and label.
- Selected, hover and disabled: Luke decided (18 Oct 2026): selected rows match ButtonSpinner's outlined look, not the filled column. Selected is `{color:AccentBrush Light6}` fill with `{color:TextDefault}` text (ButtonSpinner's pressed depth, so it reads as chosen without going dark); selected and hovered `Light5`, selected and pressed `Light4`; disabled, selected or not, is the whole row at `{size:DisabledOpacity}` like ButtonSpinner. Measure contrast of the text on the fill in both variants and nudge the stage one step if it falls below the outlined text figures in ThemeRules. Use exactly the stages ComboBoxItem (task 12) settled on. Match ListBoxItem's focus pattern (`FocusAdorner` null, inset focus border).
- Hover is `HoverBackgroundDefault` and a separate style also uses it; check there is a distinct pressed stage.

**TreeView (task 64)**
- Re-shoot after task 63 first: most of what Luke saw (left-aligned rows, padding, chevron gaps, state colours) lives in TreeViewItem. Then fix only what remains at tree level: the frame (`{color:BorderDefault}`), the scroller inset (content Margin, not ScrollViewer Padding, per the primer), and its disabled look.

### Defect list

Luke's findings from the running gallery, 18 Oct 2026. Each entry is one worker's brief. Entry names are the gallery names.

**Theme-wide (tasks 2 and 3)**
- Focus ring: the magenta focus border (`AccentFocus`) should only appear on neutral, non-accented controls. Add a theme boolean `FocusColorOnAccents`, default false. When false, a Warn, Success or other accented control keeps its own accent border when focused; a neutral control still gets the focus colour. When true, today's behaviour.
- Preview header: the control title ("AutoCompleteBox - Light") sits above the row of accent swatches, not to the left of them.
- Dark-pane text: ContentControl, GroupBox, LightDarkPreview and others render black text on the dark pane, so foreground is not inherited from the theme there. Find the shared cause and fix it once.

**LightDarkPreview** - its own preview is broken (black text on dark) and shows nothing useful. Show a few real controls: a button, some text, a checkbox.

**Button** - the bottom "Error" example is a plain grey button with no error adorner. Make it show the error state like ButtonSpinner's Error example.

**ButtonSpinner** - preview only: give every example one fixed width, slightly wider than the longest ("Ever Destructive"), so they are not staggered.

**CalendarView**
- Clicking the header (e.g. "February 2022") changes the calendar's width. Height is already fixed; width must hold too, by default, while staying settable. The earlier approach was to keep the hidden views laid out at opacity 0 rather than invisible, so the calendar sizes to its largest view (days, months or years).
- The drop-down date example: the focus border sits inside the normal border, so 1px of grey shows outside it; it must overlay the border centrally like other controls. Its text has no left padding, unlike every other text field, and the selected date sits slightly below vertical centre.

**Carousel**
- In the gallery, light and dark sit side by side so the carousel collapses too small; at the default window size the second slide overhangs the light pane's background by a few pixels (padding or clipping). For this control stack light above dark instead.
- Slide 1's accent background is far too pale (very light in light mode); move it to a more contrasting stage of the hue ramp, in both variants.
- Both slides say "Slide 1"; the second must say "Slide 2".

**CarouselPage** - "First Page" is left-aligned while Carousel's "Slide 1" is centred. Match it, and the centring should come from the theme, not be set on this one control. Bind it to the zoom on the theme control (there is a binding that is not wired).

**ContentPage** - page background too pale, same problem as Carousel: use exactly the stage Carousel ends up with. Content is left-aligned; centre it.

**CheckBox**
- The tick is too thin, the wrong colour and does not follow the accent; it needs more space between it and the box edge.
- The disabled tick does not use the disabled colour (should follow once the tick takes its colour from the theme).
- On press the box shrinks but the label moves up instead of shrinking about its centre; alignment is off.

**ComboBoxItem**
- States do not match the rest of the theme: Warn and Success are darker than anywhere else, Disabled and Selected Disabled look like no other disabled control, hover colour, brightness and borders do not match Button.
- Items in a drop-down do not stretch horizontally and have less left padding than top, bottom and right.
- Preview: examples are different widths and left-aligned; give them one fixed width.

**ComboBox**
- Nobody can tell what the many previews show: they all say 1 or 2 in odd colours. Review and cut to a set where each example is a nameable state.
- The 6th, 7th and 8th examples have a blue background on the box while the drop-down is themed; drop that custom-background demo.
- The second-to-last example (error adorner) is shorter than the others; it has lost its top and bottom padding.

**CommandBar**
- Hover highlight has arbitrary padding and no theme corner radius; the bar's own corner radius does not match the theme.
- Text and icons are left-aligned; text colour is near black and not the theme's.
- The bottom bar's icons (minus, reload, store badge) are not centred. The reload icon is malformed: pixels missing where the arrowhead should be, no arrowhead (fix the icon in `Avalonia.Themes.Default/Icons`).
- The favourite toggle does not look like any toggle: reuse the theme's ToggleButton look, styled to sit in a command bar. Unselected hover turns an unrelated orange.
- The selected "Done" in the second bar stays blue regardless of theme.

**ToggleButton** - does not match Button or ButtonSpinner styling at all. Preview: vertical stack, fixed width.

**ContentControl** - the preview is just "with content" at top centre. Give it a visible frame (margin, padding, border) with the content centred both ways. Black text on dark (see theme-wide).

**MenuItem**
- No visual reference for where the menu is: white on white. Items are different widths; fix one width in the preview.
- Sub-menu does nothing when opened; the sub-menu arrow has a huge gap on its right.
- Icon to text gap is wrong; text is off vertical centre. Disabled and selected styles do not match the theme.

**MenuFlyoutPresenter** - wrong padding; items do not stretch horizontally.

**ContextMenu**
- Item margin and padding inside the rounded container are inconsistent; items are left-aligned, not stretched.
- The item with an emoji-like icon has its icon's right side clipped.
- "With checkbox" shows a grey dot, not vertically centred, that cannot be checked. Make it a real, theme-styled checkable item.

**Menu** - looks nothing like a menu. Padding too small and unlike the theme; selected not styled; clicking does nothing (it should open a menu).

**DatePicker / DateTimePickerShared**
- Wrong padding all round; text not vertically centred.
- Six examples, unclear what each is. Default, disabled and error are guessable; the others look like a warning and a disabled success. Make each a nameable state that matches the other controls' accent and disabled styling.
- The pop-up is broken: it grows to the full screen height, inner padding and margins are wrong, and hovering any day, month or year makes the centred (selected) value vanish. It should feel like the rest of the app's pop-ups.

**TimePicker** - padding, alignment, margins and colours all off the theme. Clicking opens a full-screen overlay that hides everything. Same family as DatePicker; reuse what that fix establishes.

**DrawerPage** - almost nothing follows the theme: the nav open button is invisible with an arbitrary hover, all colours are wrong. Functionally it opens as a side overlay. Restyle it fully to theme values.

**DropDownButton** - no padding to the right of the arrow; clicking drops nothing (show a flyout so the preview demonstrates it); the disabled example runs off and clips. Preview: fixed widths, vertical stack like Button.

**EmbeddableControlRoot** - navigating to it crashes: `System.InvalidOperationException: The control EmbeddableControlRoot (Content = StackPanel) already has a visual parent TopLevelHost while trying to add it as a child of ContentPresenter (Name = PART_ContentPresenter, Host = ContentControl).` The gallery entry must open without throwing.

**Expander**
- Hover shows only a background rectangle on the right; that should be the theme's expand/collapse chevron icon, in theme colours.
- Hover, pressed and disabled all look like default.
- The up-collapsed and left-expanded examples shift the whole layout; put each in a fixed container so they read correctly.

**FlyoutPresenter** - a rounded rectangle with clipped corners and two plain texts. Show a real flyout presenter with correct corners and padding.

**GridSplitter** - a small dot and nothing else; dragging does nothing. Show two panes with a working splitter between them.

**GroupBox** - header and content text is black on light and does not follow the theme foreground (see theme-wide).

**HeaderedContentControl** - a rounded rectangle with no inner padding and two plain texts. Make it a usable, padded header and body.

**HyperlinkButton** - text colour is fixed, not themed. Remove the underline. No background fill or border: it should look like text. Hover and pressed change the accent stage like other controls; accents change the text colour (Success turns the text green). Warning is far too saturated compared with ButtonSpinner.

**ItemsControl** - padding does not match the theme; two items look like plain text. Show something that reads as a list of items.

**Label** - two texts ("File", "Plain text") with nothing to tell them apart. Show labels in all the states Button shows: normal, success, warning, disabled and so on.

**ListBoxItem** - Selected and every other state is much more saturated than ButtonSpinner; disabled states wrong. Every state off theme.

**ListBox** - items run off screen horizontally; stack vertically. Disabled and Selected Disabled do not match the theme (ButtonSpinner).

**ManagedFileChooser** - entirely broken: every part wrong, overlapping, missing, and nothing linked to the theme. Rebuild its theme from tokens.

**NavigationPage** - completely empty, nothing renders.

**NotificationCard** - all identical and all wrong: top-left plain text on a light background, no styles, padding, margin or accents. Each card should show its kind (information, success, warning, error) themed.

**NumericUpDown** - totally broken. Should match ButtonSpinner visually exactly, only the content differs.

**PathIcon** - random-sized stars that ignore the theme's icon size and accent colour. Show icons at theme sizes in the accent colours.

**PipsPager** - hovering a dot makes it vanish; the selected dot vanishes. Right arrow padding differs from left. Background does not match the theme's contrast standard, has no theme corner radius, and covers only the dots, not the arrows.

**ProgressBar** - ignores theme sizes and colours: text clips inside bars of the same colour, bar heights are not theme values. The four thin bars look identical except one is fuller; make each a nameable state.

**RadioButton** - paddings and alignment broken; every state and every accent broken.

**RefreshVisualizer** - a blank white page.

**RefreshContainer** - "pull down to refresh" has no visual indicator and pulling does nothing; padding wrong.

**RepeatButton** - looks like a plain button; show what makes it a repeat button (e.g. a counter it increments while held). Preview: vertical stack, fixed width like ButtonSpinner.

**ScrollBar** - runs off screen horizontally; lay out vertically. Ignores the theme corner radius; background does not match accent levels; the first scrollbar has a squashed thumb; padding and margins off theme.

**ScrollViewer** - the scrollbar covers the content in every state; content needs room beside it. Preview stacked vertically.

**SelectableTextBlock** - selection colours do not follow the theme; background and foreground do not match the accent levels; padding and margin off.

**Separator** - shows nothing.

**Slider** - a screen of random coloured dots. Rebuild to a proper themed slider: track, filled part, thumb, accents, disabled.

**SplitButton** - spacing is fine but the accent styling does not match ButtonSpinner.

**SplitView** - colours, margins, paddings and layout all off theme.

**TabItem** - cropped; lay out vertically so each shows. All look like plain text with no theme sizes or styling.

**TabControl** - stray lines through the headers; selected content lacks bottom padding and ignores theme colours; hover, pressed and off states off theme. The Page A and Page C examples (a letter A, a blue bar on the left, "Page A" top right) make no sense: replace with a clear demo.

**TabStripItem** - same problems as TabStrip below, at item level.

**TabStrip** - double rounded borders (outer and inner), an underline rectangle matching nothing else in the theme, padding, margins and colours invented.

**TabbedPage** - works, but alignment, padding and margins are off theme; underlines of random sizes and a full-width line that ignores theme values; content alignment off.

**TextBox**
- Preview: one fixed width so alignment examples (centre-aligned) are visible.
- The floating placeholder jumps outside the box and changes its height; it must not shift the layout around it.
- Hide and reveal password has a huge right padding and may not use theme icons.

**TextSelectionHandle** - colours, size, padding and borders ignore the theme.

**ThemeVariantScope** - only shows text. Show a scope flipping variant around a few real controls.

**ToggleSwitch** - arbitrary background fills around each item; colours, padding and margins off theme.

**TransitioningContentControl** - a box saying "Cross-faded content.". Show an actual transition (content that changes and cross-fades).

**TreeViewItem** - disabled, selected and warning do not match the theme. Items left-aligned rather than stretched; no padding right of the content; padding either side of the arrow larger than elsewhere in the theme.

**TreeView** - the same issues seen at tree level.

## Tasks
- [ ] 1. Add a proof view to ai-mecca's ThemeLabProofs that renders one gallery entry exactly as the lab shows it, a shot script that captures it before or after a fix with builds serialised, and a proof page builder. Nothing goes into the public lab. [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh CheckBox before` exits 0 and writes AvaloniaThemeLab/Scratch/ThemeLabProof/CheckBox/before/inspection.json whose root holds a CheckBox, with a refusal case: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh NoSuchControl before` exits non-zero naming the unknown entry
    - Owns: ../ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/ThemeLabProofs/Views/GalleryEntryView.cs, ../ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh, ../ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-proof-page.sh, AgentDocumentation/DemoRun.md
    - Reached from: guard xaml inspect --view ThemeLabProofs.GalleryEntryView, run by Scripts/theme-lab-shot.sh
- [ ] 2. Fix the preview header (title above the swatches) and the shared cause of black text on the dark pane, let a control's preview ask for stacked light-over-dark panes, and give LightDarkPreview's own preview a few real controls. (needs 1) [risk: high]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh ContentControl after` exits 0 and its inspection.json shows dark-pane text resolved to a light colour and the title above the first swatch, with a refusal case: a TextBlock given an explicit Foreground in the preview keeps that colour, not the inherited one
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/LightDarkPreview.axaml, AvaloniaThemeLab/Avalonia.Themes.Default/Theme/LightDarkPreview.cs, AvaloniaThemeLab/AvaloniaThemeLab/Views/MainWindow.axaml, AvaloniaThemeLab/AvaloniaThemeLab/ValueConverters, AvaloniaThemeLab/Avalonia.Themes.Default/ThemeRules.md
- [ ] 3. Add the FocusColorOnAccents theme switch, default false, so a focused accented control keeps its accent border and only neutral controls get the focus colour. (needs 2) [risk: high]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh AutoCompleteBox after` exits 0 and its inspection.json shows the focused neutral box border in AccentFocus, with a refusal case: the focused Warn box must fail to take AccentFocus while FocusColorOnAccents is false and take it again once a test theme sets it true
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Theme/DefaultTheme.cs, AvaloniaThemeLab/Avalonia.Themes.Default/Theme/DefaultThemeStyles.cs, AvaloniaThemeLab/Avalonia.Themes.Default/ThemeRules.md, AvaloniaThemeLab/Avalonia.Themes.Default/Controls/
    - Reached from: App.axaml loads DefaultTheme, whose styles every control theme reads
- [ ] 4. Give the Thickness size ladder a double twin, as Radius has, and switch every control that uses a Thickness token as a plain number (a width, height or stroke) to it, so tracks, dots, separators and rings stop rendering at 0. (needs 3) [risk: high]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh Separator after` and `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh Slider after` exit 0 and inspection.json shows the separator and the slider track at least 1px high, with a refusal case: `grep -nE '(Width|Height|StrokeThickness).{0,12}size:Thickness(Sm|Md|Lg|Xl|Xxl)\}' AvaloniaThemeLab/Avalonia.Themes.Default/Controls/*.axaml` must find nothing (it finds 26 lines today)
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Theme/DefaultTheme.cs, AvaloniaThemeLab/Avalonia.Themes.Default/ThemeRules.md, AvaloniaThemeLab/Avalonia.Themes.Default/Controls/
    - Reached from: DefaultTheme's [SizeScale] generator emits the tokens every control theme reads
- [ ] 5. Make Button's Error preview example show the error state it is named for. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh Button after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/Button/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/Button/after` passes with one row per Button bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/Button.axaml
- [ ] 6. Give ButtonSpinner's preview examples one fixed width so they stop staggering. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh ButtonSpinner after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/ButtonSpinner/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/ButtonSpinner/after` passes with one row per ButtonSpinner bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/ButtonSpinner.axaml
- [ ] 7. Keep CalendarView's width fixed when switching to month or year view, and fix the drop-down date example's focus border, left padding and text centring. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh CalendarView after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/CalendarView/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/CalendarView/after` passes with one row per CalendarView bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/CalendarView.axaml
- [ ] 8. Stack Carousel's light and dark panes vertically, darken slide 1's background a ramp stage, label the second slide Slide 2, and stop it clipping at default size. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh Carousel after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/Carousel/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/Carousel/after` passes with one row per Carousel bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/Carousel.axaml
- [ ] 9. Give ContentPage the same background stage Carousel ends up with, and centre its content. (needs 4, 8) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh ContentPage after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/ContentPage/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/ContentPage/after` passes with one row per ContentPage bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/ContentPage.axaml
- [ ] 10. Centre CarouselPage content the way Carousel does, from the theme rather than locally, and wire its missing zoom binding. (needs 4, 8, 9) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh CarouselPage after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/CarouselPage/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/CarouselPage/after` passes with one row per CarouselPage bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/CarouselPage.axaml
- [ ] 11. Rework the CheckBox tick (weight, inset, accent and disabled colour) and make the press animation shrink about the centre. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh CheckBox after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/CheckBox/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/CheckBox/after` passes with one row per CheckBox bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/CheckBox.axaml
- [ ] 12. Bring ComboBoxItem states in line with Button, stretch items with even padding, and give the preview one fixed width. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh ComboBoxItem after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/ComboBoxItem/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/ComboBoxItem/after` passes with one row per ComboBoxItem bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/ComboBoxItem.axaml
- [ ] 13. Cut the ComboBox preview to nameable states, drop the custom blue backgrounds, and restore the error example's vertical padding. (needs 4, 12) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh ComboBox after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/ComboBox/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/ComboBox/after` passes with one row per ComboBox bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/ComboBox.axaml
- [ ] 14. Restyle ToggleButton to match Button and ButtonSpinner, with a vertical fixed-width preview. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh ToggleButton after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/ToggleButton/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/ToggleButton/after` passes with one row per ToggleButton bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/ToggleButton.axaml
- [ ] 15. Bring CommandBar onto the theme: radii, alignment, text colour, a ToggleButton-style favourite, and a correct reload icon. (needs 4, 14) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh CommandBar after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/CommandBar/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/CommandBar/after` passes with one row per CommandBar bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/CommandBar.axaml, AvaloniaThemeLab/Avalonia.Themes.Default/Icons
- [ ] 16. Make the ContentControl preview a visible padded frame with centred content and themed text. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh ContentControl after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/ContentControl/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/ContentControl/after` passes with one row per ContentControl bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/ContentControl.axaml
- [ ] 17. Fix MenuItem and MenuFlyoutPresenter: visible menu surface, one item width, a working sub-menu, theme spacing and states. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh MenuItem after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/MenuItem/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/MenuItem/after` passes with one row per MenuItem bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/MenuItem.axaml, AvaloniaThemeLab/Avalonia.Themes.Default/Controls/MenuFlyoutPresenter.axaml
- [ ] 18. Even out ContextMenu item spacing and stretch, fix the clipped icon, and make the checkbox item really checkable. (needs 4, 17) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh ContextMenu after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/ContextMenu/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/ContextMenu/after` passes with one row per ContextMenu bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/ContextMenu.axaml
- [ ] 19. Make Menu look and behave like a menu: theme padding, styled selection, and a menu that opens on click. (needs 4, 17) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh Menu after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/Menu/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/Menu/after` passes with one row per Menu bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/Menu.axaml
- [ ] 20. Fix the DatePicker and its shared pop-up: padding, centring, nameable state examples, and a pop-up sized and styled like the rest of the app. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh DatePicker after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/DatePicker/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/DatePicker/after` passes with one row per DatePicker bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/DatePicker.axaml, AvaloniaThemeLab/Avalonia.Themes.Default/Controls/DateTimePickerShared.axaml
- [ ] 21. Fix TimePicker's padding, alignment and colours, and replace its full-screen overlay with a properly sized pop-up. (needs 4, 20) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh TimePicker after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/TimePicker/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/TimePicker/after` passes with one row per TimePicker bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/TimePicker.axaml
- [ ] 22. Restyle DrawerPage to theme values, including a visible nav button with theme hover. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh DrawerPage after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/DrawerPage/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/DrawerPage/after` passes with one row per DrawerPage bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/DrawerPage.axaml
- [ ] 23. Add DropDownButton's arrow padding, give the preview a working flyout, and stack it vertically at fixed width. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh DropDownButton after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/DropDownButton/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/DropDownButton/after` passes with one row per DropDownButton bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/DropDownButton.axaml
- [ ] 24. Stop the EmbeddableControlRoot gallery entry crashing on navigation, and give it a preview that shows something. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh EmbeddableControlRoot after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/EmbeddableControlRoot/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/EmbeddableControlRoot/after` passes with one row per EmbeddableControlRoot bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/EmbeddableControlRoot.axaml, AvaloniaThemeLab/AvaloniaThemeLab/ViewModels/GalleryCatalogue.cs
- [ ] 25. Give Expander a themed chevron, distinct hover, pressed and disabled states, and a preview where each direction sits in its own frame. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh Expander after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/Expander/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/Expander/after` passes with one row per Expander bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/Expander.axaml
- [ ] 26. Show a real FlyoutPresenter with correct corners and padding instead of clipped corners and two texts. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh FlyoutPresenter after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/FlyoutPresenter/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/FlyoutPresenter/after` passes with one row per FlyoutPresenter bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/FlyoutPresenter.axaml
- [ ] 27. Show GridSplitter between two panes so it is visible, themed and draggable. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh GridSplitter after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/GridSplitter/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/GridSplitter/after` passes with one row per GridSplitter bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/GridSplitter.axaml
- [ ] 28. Make GroupBox header and content text follow the theme foreground in both variants. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh GroupBox after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/GroupBox/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/GroupBox/after` passes with one row per GroupBox bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/GroupBox.axaml
- [ ] 29. Give HeaderedContentControl a padded, readable header and body. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh HeaderedContentControl after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/HeaderedContentControl/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/HeaderedContentControl/after` passes with one row per HeaderedContentControl bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/HeaderedContentControl.axaml
- [ ] 30. Make HyperlinkButton plain text with no fill, border or underline, with accent-coloured text and ramp-stage hover and pressed. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh HyperlinkButton after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/HyperlinkButton/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/HyperlinkButton/after` passes with one row per HyperlinkButton bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/HyperlinkButton.axaml
- [ ] 31. Make the ItemsControl preview read as a list with theme padding. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh ItemsControl after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/ItemsControl/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/ItemsControl/after` passes with one row per ItemsControl bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/ItemsControl.axaml
- [ ] 32. Show Label in every state Button shows: normal, each accent, disabled. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh Label after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/Label/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/Label/after` passes with one row per Label bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/Label.axaml
- [ ] 33. Tone ListBoxItem selected, hover and disabled states down to ButtonSpinner's levels. (needs 4, 12) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh ListBoxItem after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/ListBoxItem/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/ListBoxItem/after` passes with one row per ListBoxItem bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/ListBoxItem.axaml
- [ ] 34. Stack ListBox items vertically and fix its disabled and selected-disabled looks. (needs 4, 33) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh ListBox after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/ListBox/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/ListBox/after` passes with one row per ListBox bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/ListBox.axaml
- [ ] 35. Rebuild ManagedFileChooser's theme from tokens so nothing overlaps, goes missing or ignores the theme. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh ManagedFileChooser after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/ManagedFileChooser/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/ManagedFileChooser/after` passes with one row per ManagedFileChooser bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/ManagedFileChooser.axaml
- [ ] 36. Make NavigationPage render: its preview is currently empty. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh NavigationPage after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/NavigationPage/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/NavigationPage/after` passes with one row per NavigationPage bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/NavigationPage.axaml
- [ ] 37. Theme NotificationCard so each kind shows its accent, padding and layout. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh NotificationCard after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/NotificationCard/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/NotificationCard/after` passes with one row per NotificationCard bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/NotificationCard.axaml
- [ ] 38. Make NumericUpDown match ButtonSpinner visually in every state. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh NumericUpDown after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/NumericUpDown/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/NumericUpDown/after` passes with one row per NumericUpDown bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/NumericUpDown.axaml
- [ ] 39. Show PathIcon at theme icon sizes in the accent colours. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh PathIcon after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/PathIcon/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/PathIcon/after` passes with one row per PathIcon bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/PathIcon.axaml
- [ ] 40. Fix PipsPager dots vanishing on hover and selection, even the arrow padding, and give it a themed rounded background around arrows and dots. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh PipsPager after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/PipsPager/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/PipsPager/after` passes with one row per PipsPager bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/PipsPager.axaml
- [ ] 41. Put ProgressBar on theme heights and colours so text never clips, and make each preview bar a nameable state. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh ProgressBar after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/ProgressBar/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/ProgressBar/after` passes with one row per ProgressBar bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/ProgressBar.axaml
- [ ] 42. Fix RadioButton padding, alignment, states and accents. (needs 4, 11) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh RadioButton after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/RadioButton/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/RadioButton/after` passes with one row per RadioButton bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/RadioButton.axaml
- [ ] 43. Make RefreshVisualizer render something visible and themed instead of a blank page. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh RefreshVisualizer after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/RefreshVisualizer/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/RefreshVisualizer/after` passes with one row per RefreshVisualizer bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/RefreshVisualizer.axaml
- [ ] 44. Give RefreshContainer a visible pull indicator that responds, and fix its padding. (needs 4, 43) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh RefreshContainer after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/RefreshContainer/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/RefreshContainer/after` passes with one row per RefreshContainer bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/RefreshContainer.axaml
- [ ] 45. Make the RepeatButton preview show repeating behaviour, stacked vertically at fixed width. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh RepeatButton after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/RepeatButton/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/RepeatButton/after` passes with one row per RepeatButton bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/RepeatButton.axaml
- [ ] 46. Stack the ScrollBar preview vertically and put radius, background, thumb and spacing on theme values. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh ScrollBar after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/ScrollBar/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/ScrollBar/after` passes with one row per ScrollBar bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/ScrollBar.axaml
- [ ] 47. Stop ScrollViewer's scrollbar covering content and stack its preview vertically. (needs 4, 46) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh ScrollViewer after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/ScrollViewer/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/ScrollViewer/after` passes with one row per ScrollViewer bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/ScrollViewer.axaml
- [ ] 48. Theme SelectableTextBlock's selection colours, background, foreground and spacing. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh SelectableTextBlock after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/SelectableTextBlock/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/SelectableTextBlock/after` passes with one row per SelectableTextBlock bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/SelectableTextBlock.axaml
- [ ] 49. Make Separator render a visible themed line. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh Separator after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/Separator/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/Separator/after` passes with one row per Separator bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/Separator.axaml
- [ ] 50. Rebuild Slider as a themed track, fill and thumb with accent and disabled states. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh Slider after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/Slider/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/Slider/after` passes with one row per Slider bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/Slider.axaml
- [ ] 51. Match SplitButton's accent styling to ButtonSpinner. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh SplitButton after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/SplitButton/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/SplitButton/after` passes with one row per SplitButton bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/SplitButton.axaml
- [ ] 52. Restyle SplitView colours, spacing and layout to theme values. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh SplitView after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/SplitView/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/SplitView/after` passes with one row per SplitView bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/SplitView.axaml
- [ ] 53. Lay TabItem out vertically so none are cropped, and style it with theme sizes and states. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh TabItem after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/TabItem/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/TabItem/after` passes with one row per TabItem bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/TabItem.axaml
- [ ] 54. Remove TabControl's stray header lines, theme its content and states, and replace the Page A and C demo with a clear one. (needs 4, 53) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh TabControl after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/TabControl/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/TabControl/after` passes with one row per TabControl bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/TabControl.axaml
- [ ] 55. Remove TabStripItem's invented borders and underline and put it on theme values. (needs 4, 53) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh TabStripItem after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/TabStripItem/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/TabStripItem/after` passes with one row per TabStripItem bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/TabStripItem.axaml
- [ ] 56. Give TabStrip a single themed border and remove the invented underline, padding and colours. (needs 4, 55) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh TabStrip after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/TabStrip/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/TabStrip/after` passes with one row per TabStrip bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/TabStrip.axaml
- [ ] 57. Put TabbedPage alignment, padding and underlines on theme values. (needs 4, 53, 9) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh TabbedPage after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/TabbedPage/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/TabbedPage/after` passes with one row per TabbedPage bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/TabbedPage.axaml
- [ ] 58. Give the TextBox preview fixed widths, stop the floating placeholder changing the box height, and fix the reveal-password button padding and icons. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh TextBox after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/TextBox/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/TextBox/after` passes with one row per TextBox bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/TextBox.axaml
- [ ] 59. Put TextSelectionHandle colours, size and borders on theme values. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh TextSelectionHandle after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/TextSelectionHandle/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/TextSelectionHandle/after` passes with one row per TextSelectionHandle bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/TextSelectionHandle.axaml
- [ ] 60. Prove ThemeVariantScope does nothing in this theme, which uses its own dark regions, and if so delete its theme file and every reference to it; if it does flip the theme, report instead. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh Button after` exits 0 with a Button inside a Dark-requesting ThemeVariantScope measuring the same brushes as one outside it, and afterwards `grep -rln ThemeVariantScope AvaloniaThemeLab/Avalonia.Themes.Default/Controls` lists no theme file for it and `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh --all after` still exits 0
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/ThemeVariantScope.axaml, any file that includes or lists ThemeVariantScope
- [ ] 61. Remove ToggleSwitch's stray background fills and put its colours and spacing on theme values. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh ToggleSwitch after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/ToggleSwitch/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/ToggleSwitch/after` passes with one row per ToggleSwitch bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/ToggleSwitch.axaml
- [ ] 62. Make the TransitioningContentControl preview actually cross-fade changing content. (needs 4) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh TransitioningContentControl after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/TransitioningContentControl/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/TransitioningContentControl/after` passes with one row per TransitioningContentControl bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/TransitioningContentControl.axaml
- [ ] 63. Stretch TreeViewItem, fix its arrow and content padding, and bring its states in line with the theme. (needs 4, 12) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh TreeViewItem after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/TreeViewItem/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/TreeViewItem/after` passes with one row per TreeViewItem bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/TreeViewItem.axaml
- [ ] 64. Bring TreeView's own states and layout in line with the fixed TreeViewItem. (needs 4, 63) [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh TreeView after` exits 0 and `guard xaml verify AvaloniaThemeLab/Scratch/ThemeLabProof/TreeView/verification.json --capture AvaloniaThemeLab/Scratch/ThemeLabProof/TreeView/after` passes with one row per TreeView bullet in the Defect list and the Task brief
    - Owns: AvaloniaThemeLab/Avalonia.Themes.Default/Controls/TreeView.axaml
- [ ] 65. Final gate, once every control task is ticked: shoot every gallery entry in light and dark and run the theme analyser over the whole theme. [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh --all after` exits 0 with no entry throwing, and `guard xaml analyze AvaloniaThemeLab/AvaloniaThemeLab/AvaloniaThemeLab.csproj` reports no errors
    - Owns: none, read-only
- [ ] 66. Luke reviews the proof page that task 1 creates and signs off or names what is still wrong. (needs 65) [risk: low]
    - Done when: Needs a person: Luke opens AvaloniaThemeLab/Scratch/ThemeLabProof/index.html and signs off, and every pair he rejects becomes a new task in this plan

## Feedback notes
{{Optional: consolidate live testing feedback here before folding it back into tasks/decisions.}}

## Decisions and trade-offs
- Plan style is solo, not wave: tasks name intent and the defects, not diffs. Parallel safety comes from the Owns line on each task (one owner per file) and the Worker rules, not from frozen detail.
- One worker per control file. Where an item and its parent are coupled (ComboBoxItem then ComboBox, ListBoxItem then ListBox, TabItem then TabControl, TabStripItem then TabStrip, TreeViewItem then TreeView, MenuItem then Menu and ContextMenu, DatePicker then TimePicker, ToggleButton then CommandBar, Carousel then CarouselPage and ContentPage, RefreshVisualizer then RefreshContainer, ScrollBar then ScrollViewer) the parent waits for the item.
- The lab is public and ships only the theme and its demo app; every harness, script and proof over it lives in ai-mecca's `Tools/XamlAnalyzer/ThemeLabProofs/` (ai-mecca `Documentation/AvaloniaInspection.md`, "Where the lab proofs live"). So the shot harness (task 1) is a ThemeLabProofs view plus scripts, proof images go to the lab's gitignored `Scratch/`, and this plan sits in the youtube repo's `Plans/`, outside the lab.
- A headless shot of the whole gallery window works (`guard shot avalonia` on `AvaloniaThemeLab.Views.InspectionMainWindow`, about 11 seconds, measured 18 Oct 2026), but nothing can pick an entry, and `guard shot avalonia` on a bare control .axaml refuses because theme dictionaries have no x:Class. Hence task 1.
- The gallery hard-codes side-by-side panes for every entry (`MainWindow.axaml`), so stacking Carousel's light and dark panes needs a per-preview option; task 2 adds it.
- All workers build one project. The shot script serialises builds so four workers never fight over obj/ (task 1), and the Worker rules make each worker keep its file compiling.
- AutoCompleteBox has no defects of its own; task 3 covers its focus ring.
- Workers are local Coder (Rexxy Qwen 27B) only, four at a time, at Luke's request.
- Decided by Luke (18 Oct 2026): selected rows in ListBoxItem, ComboBoxItem and TreeViewItem match ButtonSpinner's contrast and layout (outlined column: a light fill stage with TextDefault text, disabled as whole-row DisabledOpacity), replacing ThemeRules' filled column for those three. Task 12 sets the stages; tasks 33 and 63 copy them. Toggles (ToggleButton, SplitButton, CommandBar checked) keep the filled column.
- HyperlinkButton's Foreground is currently pinned to the Primary hue, which Rule 13 lists as deliberate; Luke's note (text follows the accent) overrides it, so task 30 moves it to `{color:AccentBrush ...}` and updates that line of the rule.
- Before the tasks were written (18 Oct 2026), every gallery entry was rendered headless from a throwaway host in /tmp (outside every repo): it built the lab's App, set `MainWindowViewModel.SelectedEntry` to each entry in turn on a 1200x900 MainWindow, and saved `CaptureRenderedFrame()`. All 65 entries rendered in about 9 seconds in one process (EmbeddableControlRoot threw). That is far faster than one `guard shot avalonia` per entry (about 11 seconds each), so task 1 should copy the approach. The findings are in Task briefs.
- The Thickness ladder is the root cause of several "shows nothing" bugs: `[SizeScale(... Types = new[] { typeof(Thickness) })]` in DefaultTheme.cs emits only Thickness tokens, and 26 sites in 12 control files use them as plain numbers, which become 0. Radius already solves this with a double twin (`TypeSuffixes = ["", "Double"]`); task 4 does the same for Thickness and swaps those 26 sites.
- GroupBox text (Luke left it to the planner): body text stays `TextDefault` like every control, so near-black in light is correct; the header label takes the hue at `HoverTextDefault` (Dark7) so an accented group box is recognisable. `TextDefault` itself is not changed.
- Decided by Luke (18 Oct 2026): ThemeVariantScope is deleted if, as expected, it does nothing in this theme (dark mode is the engine's `ThemeRegion.IsDark`). Task 60 proves that first and deletes; if the scope does flip the theme, it reports instead.
- ThemeRules.md has one writer at a time: the foundation tasks (2, 3, 4) edit it themselves while they run alone; control workers never edit it and instead report the rule change, which the orchestrator applies (task 12's selection stages, task 30's HyperlinkButton line, task 53's TabItem font size, task 60's Rule 13 root list).

## Pinned terms

## Agent activity log
- 2026-10-18: Task 4 edited by `guard plan edit-task`.
- 2026-10-18: Guard review receipts cleared by `guard plan set-section` because section `Decisions and trade-offs` changed.
- 2026-10-18: Guard review receipts cleared by `guard plan set-section` because section `The plan` changed.
- 2026-10-18: Status changed from `BRAINSTORMING` to `READY` by `guard plan set-status`.
- 2026-10-18: Added 66 task(s) by `guard plan add-tasks`.

## Model authorship
- 2026-10-18: claude-opus-5-5 — created plan, added 66 tasks, set status to READY, set section "The plan", set section "Decisions and trade-offs", edited task 4
