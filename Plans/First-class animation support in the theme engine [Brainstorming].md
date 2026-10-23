# First-class animation support in the theme engine
Status: BRAINSTORMING
Priority: 3/5
Plan type: code
Plan style: solo
Several Theme Lab controls carry ad-hoc animations today: NotificationCard's bounce-in entrance and slide-out dismissal, ProgressBar's looping indeterminate sweep, the DatePicker/TimePicker wheel glide, and twenty-two control themes declaring their own Transitions and KeyFrames with mixed literal and token durations. Luke wants animation to become a first-class concept in the theme engine, the way colours and sizes already are: tokens for durations, easings and transform presets, so a control declares WHAT moves and WHEN, and the theme decides HOW FAST and WITH WHICH CURVE. The engine lives in the AngelSix.ThemeEngine NuGet package (private repo). This plan inventories every animation the Default theme ships today, agrees the token model with Luke, then lands the engine support and migrates each control onto it.
## The idea

## Rough approach
**The plan:**
Today the Default theme expresses motion three different ways, none of them first-class. Colours and sizes became engine concepts long ago: `{color:...}` and `{size:...}` tokens defined once in `Theme/DefaultTheme.cs` and consumed everywhere. Animation never got the same treatment. Durations ride on a single `[SizeScale]` triple (`AnimationFastMs` 75 / `AnimationNormalMs` 150 / `AnimationSlowMs` 300, generated from one base number in `Theme/DefaultTheme.cs`), easings appear as bare strings (`QuadraticEaseIn`, `CubicEaseOut`) or inline spline resources, and transform presets do not exist at all: a press-scale is spelled out per control, and multi-keyframe motions (NotificationCard's bounce-in, ProgressBar's indeterminate sweep) are written as literal `KeyFrame` blocks with hard-coded timescales. Two behaviours (`Theme/WheelSnapBehavior.cs`, `Theme/ScrollRevealBehavior.cs`) drive motion entirely in C# because nothing declarative could express it, and their timings are private constants the theme cannot tune.

Luke wants animation to join colours and sizes as a first-class concept in the theme engine: tokens for durations, easings and transform presets, so a control declares WHAT moves and WHEN (hover, press, expand, entrance, loop) and the theme decides HOW FAST and WITH WHICH CURVE. Several controls already carry ad-hoc animations today, notably RefreshContainer's pull indicator and NotificationCard's entrance and dismissal. The engine lives in `/Users/lukemalpass/Documents/GitHub/angelsix-consulting/Avalonia Themes` (`AngelSix.ThemeEngine`, shipped as a NuGet package): the lab consumes it, so engine changes publish and bump the version before the lab migrates onto them.

This plan does four things in order: inventories every animation the Default theme ships today (below), agrees the token model with Luke, lands the engine support with tests, and migrates each control off its ad-hoc declaration onto the engine. Until that migration runs, the existing ad-hoc animations stay untouched: plan 01's note on task 130 says so explicitly, and the animations requested in its tasks 106-128 depend on them.

### Inventory

Floor check: `grep -lE "Transitions|Animation "` over `Avalonia.Themes.Default/Controls/*.axaml` lists exactly the 23 `.axaml` files below; every one appears here. Plus the two `Theme/*.cs` behaviours named in the brief. Timings read from the code as of this writing; "token" means the engine-generated `{theme:Animation*Ms}` trio, "literal" means a hard-coded timescale or constant. The list was re-run against the live tree at authoring time; other workers on plan 01 may add more before this plan starts, so re-run the floor check then.

**AXAML control themes (23 files)**

| File | Control / part | Property | Duration | Easing | Trigger |
|---|---|---|---|---|---|
| Button.axaml | Button face | RenderTransform (press scale) | token Fast | default | pressed |
| ButtonSpinner.axaml | Spinner face | RenderTransform (press scale) | token Fast | default | pressed |
| CalendarView.axaml | Day cell | RenderTransform (press scale) | token Fast | default | pressed |
| CheckBox.axaml | Box | RenderTransform (press scale) | token Fast | default | pressed |
| CommandBar.axaml | Toolbar item (two sub-themes) | RenderTransform (press scale) | token Fast | default | pressed |
| DatePicker.axaml | Picker button face | RenderTransform (press scale) | token Fast | default | pressed |
| DateTimePickerShared.axaml | Wheel row (ListBoxItem) | Background | token Fast | default | pointerover tint |
| DateTimePickerShared.axaml | Stepper chip (two sub-themes) | Opacity + RenderTransform | token Fast | default | pointerover grow / pressed push |
| DrawerPage.axaml | Drawer handle | RenderTransform (press scale) | token Fast | default | pressed |
| Expander.axaml | Chevron | RotateTransform.Angle 0 to 180 (two KeyFrame Animations) | token Fast | default | expanded / collapsed Tag |
| Expander.axaml | Chevron | RenderTransform (grow/shrink glide) | token Fast | default | pointerover (+ pinned Hover example) |
| Expander.axaml | Body card (#ExpanderContent) | MaxWidth + MaxHeight + Opacity | token Fast | default | expand/collapse (plays forwards/backwards) |
| HyperlinkButton.axaml | Link face | RenderTransform (press scale) | token Fast | default | pressed |
| NotificationCard.axaml | Card entrance | Opacity 0 to 1, TranslateTransform.Y 20 to -20 to 0, ScaleTransform 0.85 to 1 (three KeyFrames) | literal 0:0:0.45 | QuadraticEaseIn | entrance (ControlTheme.Animations) |
| NotificationCard.axaml | Card dismissal | TranslateTransform.X 0 to 800, ScaleTransform.ScaleY 1 to 0 (three KeyFrames) | literal 0:0:0.75 | QuadraticEaseOut | IsClosing=true (dismiss) |
| NotificationCard.axaml | Card close commit | IsClosed to True at cue 100% | literal 0:0:1.25 | QuadraticEaseOut | IsClosing=true (delayed close) |
| PipsPager.axaml | Pip background ellipse | Fill + Stroke brushes | token Normal | default | page selected |
| PipsPager.axaml | Active pip | Width + Height + Fill | token Normal | default | page selected |
| ProgressBar.axaml | Determinate indicator (two orientations) | Opacity | literal 0:0:0.15 | default | value/state change |
| ProgressBar.axaml | Indeterminate sweep (four styles: h/v, containers 1+2) | TranslateTransform.X/Y, Infinite iteration, positions from TemplateSettings bindings | literal 0:0:2 | KeySpline 0.4,0,0.6,1 | indeterminate (loop) |
| RadioButton.axaml | Radio circle | RenderTransform (press scale) | token Fast | default | pressed |
| RepeatButton.axaml | Face | RenderTransform (press scale) | token Fast | default | pressed |
| ScrollBar.axaml | Thumb | CornerRadius + RenderTransform | token Fast | default | hover round-out / pressed scale |
| ScrollViewer.axaml | Auto-hide bars (H/V) + separator panel | Opacity | token Normal | default | reveal/hide (auto-hide) |
| SplitView.axaml | Pane (open, four orientation styles) | Width or Height | literal resource 00:00:00.2 | default | pane opens |
| SplitView.axaml | Pane (close, four orientation styles) | Width or Height | literal resource 00:00:00.1 | resource spline 0.1,0.9,0.2,1.0 | pane closes |
| TimePicker.axaml | Picker button face | RenderTransform (press scale) | token Fast | default | pressed |
| ToggleButton.axaml | Face | RenderTransform (press scale) | token Fast | default | pressed |
| ToggleSwitch.axaml | Knob (Canvas.Left) | DoubleTransition | token Normal | CubicEaseOut | toggled |
| ToolTip.axaml | Tip root | Opacity 0 to shown | token Normal | default | tip shown |
| TextBox.axaml | Field face | RenderTransform (press scale) | token Fast | default | pressed |

**Behaviour code (Theme/\*.cs, 2 files)**

| File | Behaviour | Property driven | Timing | Curve | Trigger |
|---|---|---|---|---|---|
| WheelSnapBehavior.cs | Date/time wheel glide (attached as `{default:WheelSnap.Apply}` from DatePicker.axaml and TimePicker.axaml) | DateTimePickerPanel.Offset.Y via DispatcherTimer ticks | literal 150 ms glide; 1 s idle delay; 400 ms settle | manual smoothstep ease-in-out (t squared times 3 minus 2t) | row tapped (glide + commit-at-end); gesture ended (interrupt-settle or idle snap) |
| ScrollRevealBehavior.cs | Scrollbar reveal (attached as `{default:ScrollReveal.Apply}` from ScrollViewer.axaml) | Adds/removes the `scrolling` style class that drives the bar-opacity transitions above | literal 1 s hide delay (stock grace) | n/a (class arming; the fade itself is the XAML transition) | any non-zero OffsetDelta (wheel, trackpad, thumb drag, keyboard) |

**Token source**: `Theme/DefaultTheme.cs` carries `[SizeScale(Multipliers=[0.5,1,2], Labels=["FastMs","NormalMs","SlowMs"], Types=[typeof(TimeSpan)])] public virtual double Animation => 150;`, which generates the only motion tokens the theme has today. No easing tokens, no transform-preset tokens, no loop or delay primitives.

**Named in the brief but declaring no animation of its own**: `RefreshContainer.axaml` hosts the pull indicator (a `RefreshVisualizer`) whose refresh motion stays owned by the stock control; the theme only cancels the idle lift (`RenderTransform = {x:Null}` in RefreshVisualizer.axaml). Listed here so the inventory provably covers the examples Luke cited; if the engine gains a spin/rotate preset, the themed refresh arrow becomes a candidate consumer.

**Feedback notes:**
{{Optional: consolidate live testing feedback here before folding it back into tasks/decisions.}}

## Findings

## Open questions

## Resume notes

## Tasks
- [ ] 1. Agree the animation token model with Luke: duration ladder, easing vocabulary, transform-preset set and trigger mapping. Record the agreed model in this plan's Decisions section. [risk: low]
    - Done when: Needs a person: Luke signs off the token model and the agreed decisions are written into this plan's Decisions and trade-offs section
- [ ] 2. Land the engine support in AngelSix.ThemeEngine (private repo): token generation for durations, easings and transform presets per the agreed model, plus the markup surface controls consume. Publish a new package version. [risk: high]
    - Done when: `dotnet test` on the AngelSix.ThemeEngine test project filtered to the new token tests exits 0, proving a generated duration token resolves to its expected TimeSpan, an easing token yields the expected curve, a transform preset produces the expected operation list, and an unknown easing name is refused with a thrown error
- [ ] 3. Remove the existing ad-hoc animations from the Default theme: strip the literal KeyFrame blocks, inline spline resources and hard-coded timescales listed in the Inventory, leaving each control's static states intact. [risk: medium]
    - Done when: `bash -c '! grep -RE "KeyFrame|IterationCount" AvaloniaThemeLab/Avalonia.Themes.Default/Controls/*.axaml'` exits 0 (positive control: run the same grep on a copy of the pre-change files and record that it fired) and `dotnet build` on the lab project exits 0
- [ ] 4. Re-add each control's animations from the Inventory through the new engine tokens: durations, easings and transform presets replace every literal, and the two Theme behaviours adopt token-driven timings. [risk: medium]
    - Done when: `guard xaml verify` on a verification.json covering one representative entry per animation family (pressed scale, entrance, indeterminate loop, wheel glide) exits 0, with no KeyFrame or IterationCount left in Controls/
- [ ] 5. Re-shoot every affected gallery entry and assemble the proof page so Luke can compare old and new motion side by side. [risk: low]
    - Done when: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-proof-page.sh` exits 0 and the regenerated index.html under AvaloniaThemeLab/Scratch/ThemeLabProof/ carries an after capture for every Inventory entry

## Agent activity log
- 2026-10-20: Status changed from `READY` to `BRAINSTORMING` by `guard plan set-status`.
- 2026-10-20: Guard review receipts cleared by `guard plan set-section` because section `The plan` changed.
- 2026-10-20: Task 2 edited by `guard plan edit-task`.
- 2026-10-20: Task 5 edited by `guard plan edit-task`.
- 2026-10-20: Task 4 edited by `guard plan edit-task`.
- 2026-10-20: Task 3 edited by `guard plan edit-task`.
- 2026-10-20: Task 5 edited by `guard plan edit-task`.
- 2026-10-20: Task 4 edited by `guard plan edit-task`.
- 2026-10-20: Task 3 edited by `guard plan edit-task`.
- 2026-10-20: Task 5 edited by `guard plan edit-task`.
- 2026-10-20: Task 3 edited by `guard plan edit-task`.
- 2026-10-20: Task 2 moved up by `guard plan move-task`.
- 2026-10-20: Task 3 moved up by `guard plan move-task`.
- 2026-10-20: Task 4 moved up by `guard plan move-task`.
- 2026-10-20: Task 5 moved up by `guard plan move-task`.
- 2026-10-20: Task added by `guard plan add-task`.
- 2026-10-20: Added 2 task(s) by `guard plan add-tasks`.
- 2026-10-20: Task 1 edited by `guard plan edit-task`.
- 2026-10-20: Task added by `guard plan add-task`.
- 2026-10-20: Task added by `guard plan add-task`.
- 2026-10-20: Guard review receipts cleared by `guard plan set-section` because section `The plan` changed.

## Model authorship
- 2026-10-20: claude-opus-5-5 — set status to BRAINSTORMING
