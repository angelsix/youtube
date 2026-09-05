# Remake the calendar as one custom control
Status: DONE
Priority: 4/5
Plan type: code
Plan style: solo
Replace the five legacy calendar ControlThemes with one custom CalendarView control: a color-calendar-styled month view with range selection, event dots and an optional built-in picker mode.
## The plan
**The idea:**
The theme library currently ships five ControlThemes for Avalonia's built-in calendar family (Calendar, CalendarItem, CalendarButton, CalendarDayButton, CalendarDatePicker). That five-way split is forced by Avalonia itself: its CalendarItem code instantiates the day and month cells in C# and resolves their look by implicit type lookup, the helper classes are sealed, their state properties are internal, the grid geometry is hard-coded, and nothing in the family understands time of day. Re-theming can never fix any of that.

So we are replacing the lot with one custom control of our own, provisionally named CalendarView (the name Calendar would clash with Avalonia.Controls.Calendar in XAML). One C# TemplatedControl plus one AXAML ControlTheme in the prototype library, following ThemeRules.md throughout. It is the library's first custom control class, and that is deliberate.

The design steals from the best of the current field, researched Sep 2026:

- Visuals: color-calendar (https://github.com/PawanKolhe/color-calendar) - accent-filled selected day, event dots under the date number, header that opens month and year picker overlays. This is the look Luke wants.
- Range UX: shadcn/ui Calendar visuals plus the MUI X hover-preview state machine - solid accent endpoints with rounded outer corners, a continuous muted band between them, and a ghost preview band from the committed start to the hovered day before the second click.
- API shape: react-day-picker and Mantine Dates - a selection mode, a disabled-date predicate, min/max display dates, level-based month/year/decade zoom via the header.
- Event markers: WinUI CalendarView's density-bar idea generalised - a bindable per-day markers source rendered as dots now, with richer event display left to a future separate control binding the same data. WinUI's sixty flat styling properties are the anti-pattern; our token system replaces them.

Picker mode is baked in as Luke described: a single property flips the control from an inline calendar to a textbox-plus-button presentation whose popup hosts the same calendar - one control, one XAML file.

The five legacy AXAML files are deleted at the end; the theme's generated include list picks that up automatically. The library simply stops theming the built-in calendar family. The spinner-style DatePicker and TimePicker themes are a separate lineage and are untouched.

A sparse clone of the Avalonia source sits at /tmp/avalonia-src for cribbing structural details (date maths, week layout) during implementation.

Settled while brainstorming:

- New custom control rather than re-theming: Avalonia creates the calendar cell buttons in sealed C# with internal state, so a single-control calendar is impossible any other way. This is the library's first custom control class, accepted deliberately.
- Provisional name CalendarView, because Calendar clashes with Avalonia.Controls.Calendar in XAML. Open question 1 confirms or changes it.
- The API binds DateTime values (not DateOnly) so time of day can arrive later without a breaking change, but no time-editing UI in this plan - see open question 2.
- Event display stays dots-only here. Richer event rendering is a future separate control binding the same markers data - a separate concern by design.
- The library stops theming the built-in calendar family entirely. The spinner DatePicker and TimePicker themes are a different lineage and stay.
- There is no test project in this solution, so behavioural proof runs through the live lab app driven by FeedbackCLI. Adding a unit test suite for the date and range logic is worthwhile but is its own job per the unit-testing skill - see open question 3.

**Open questions:**
1. RESOLVED: the control is named CalendarView - matches the WinUI precedent and avoids the XAML clash with Avalonia.Controls.Calendar. Built by task 1.
2. RESOLVED: no time-of-day UI in v1. Task 3 binds DateTime-typed SelectedDate and SelectedRange so time support can arrive later without a breaking change; a time-editing row is a follow-up plan.
3. RESOLVED: yes, the date and range logic gets unit tests - task 10 stands up the test project and covers the range state machine.

## Tasks
- [x] 1. Create the CalendarView TemplatedControl in the prototype library with a month grid honouring FirstDayOfWeek, a header label, previous and next navigation and a today highlight, plus a demo page in the lab app. [risk: low]
    - Done when: an fb screenshot of the lab shows the current month with today highlighted on the correct weekday, and the nav arrows move the displayed month
    - Evidence (2026-09-05): fb screenshot /tmp/cal-13.png: September 2026 grid with today (Sat 5) bold and Sep 1 on Tuesday; screenshot /tmp/cal-nav.png shows the next arrow moved the header to October 2026, prev arrow returned it
- [x] 2. Add header zoom: clicking the header climbs month to year to decade, and picking drills back down. [risk: low]
    - Done when: fb clicks the header twice capturing the year then decade grids, and picking a year then month lands the month view on the chosen month
    - Evidence (2026-09-05): fb: header click -> 2026 year grid (/tmp/cal-zoom5.png), second click -> 2020-2029 decade (/tmp/cal-decade2.png), 2027 -> Mar -> March 2027 month view with 1st on Monday (/tmp/cal-mar27.png)
- [x] 3. Add selection: a SelectionMode of None, Single or Range with bindable SelectedDate and SelectedRange, using the hover-preview range state machine with pseudo-classes for range start, range end, in-range and preview. [risk: high]
    - Done when: fb picks a start day, hovers a later day capturing the preview band, clicks to commit, and a bound label in the demo page shows the committed range
    - Evidence (2026-09-05): fb: start 10, hover 18 shows continuous preview band (/tmp/cal-preview2.png), click 17 commits accent endpoints + band with labels From: 10/09/2026 To: 17/09/2026 (/tmp/cal-commit.png); engine tests 14/14
- [x] 4. Add constraints: MinDate and MaxDate plus a disabled-date predicate, with disabled days dimmed and inert. [risk: low]
    - Done when: with a weekend-disabling predicate on the demo page, fb clicks a disabled day and the bound selection is unchanged while the screenshot shows the dimmed cell
    - Evidence (2026-09-05): fb screenshot /tmp/cal-sel-label.png: after clicking disabled Saturday 12 the bound Selected label is empty, weekends render dimmed; IsSelectable min/max/predicate covered by unit tests
- [x] 5. Add event markers: a bindable per-day markers source rendered as dots under the day number through a marker template. [risk: low]
    - Done when: the demo page binds markers for three dates and a screenshot shows dots under exactly those three days and no others
    - Evidence (2026-09-05): fb screenshot /tmp/cal-final-crop.png: crimson dot under 6, green and blue pair under 8, theme-accent fallback dot under 15, no dots on any other day
- [x] 6. Add keyboard support: arrows move day focus, Enter selects, PageUp and PageDown change month. [risk: low]
    - Done when: fb drives arrow keys then Enter and the bound selected date matches the day travelled to
    - Evidence (2026-09-05): fb: click 9 -> Selected 09/09/2026, key Right + Return -> Selected 10/09/2026 (/tmp/cal-sel3.png)
- [x] 7. Add picker mode: one property flips the control to a textbox and calendar-icon button whose popup hosts the same calendar. [risk: high]
    - Done when: fb enables picker mode, opens the popup, picks a date, and the textbox shows that date with the popup closed
    - Evidence (2026-09-05): fb: picker chevron opened popup calendar (/tmp/cal-popup.png), clicking 24 closed it and textbox shows 24/09/2026 (/tmp/cal-picked.png)
- [x] 8. Theme the control: a single ControlTheme obeying ThemeRules.md with the color-calendar look, hue-agnostic AccentBrush and role tokens throughout. [risk: low]
    - Done when: guard xaml analyze reports no findings on the new AXAML and light and dark fb screenshots show the calendar fully restyled in both variants
    - Evidence (2026-09-05): guard xaml analyze CalendarView.axaml: scanned 1 file, no violations; light /tmp/cal-13.png and dark /tmp/cal-dark.png (BrandTheme IsDark=true) both fully restyled, dark canvas takes the DarkSeed red
- [x] 9. Delete the five legacy calendar AXAML files from the prototype library. [risk: low]
    - Done when: the solution builds clean with the files gone and an fb screenshot shows the lab window still rendering the new calendar
    - Evidence (2026-09-05): five legacy calendar axaml files deleted; dotnet build AvaloniaThemeLab.sln 0 errors 0 warnings; fb screenshot /tmp/cal-final-crop.png shows the lab window still rendering the new calendar
- [x] 10. Stand up a unit test project for the prototype library and cover the range-selection state machine and month-grid date maths. [risk: low]
    - Done when: guard test prove goes red when the range-commit logic is reverted, with named tests covering preview, commit and disabled-date cases
    - Evidence (2026-09-05): dotnet test: 14/14 pass; inverting range-commit swap -> Failed: 3 (incl Clicking_backwards_swaps_the_endpoints); reverted -> 14/14 green

## Feedback notes
{{Optional: consolidate live testing feedback here before folding it back into tasks/decisions.}}

## Decisions and trade-offs

## Pinned terms

## Agent activity log
- 2026-09-05: Task 9 evidence re-proofed by `guard plan mark-done`.
- 2026-09-05: Task 5 evidence re-proofed by `guard plan mark-done`.
- 2026-09-05: Task 4 evidence re-proofed by `guard plan mark-done`.
- 2026-09-05: Task 1 evidence re-proofed by `guard plan mark-done`.
- 2026-09-05: Status changed from `IN PROGRESS` to `DONE` by `guard plan set-status`.
- 2026-09-05: Tasks marked done by `guard plan mark-done`.
- 2026-09-05: Tasks marked done by `guard plan mark-done`.
- 2026-09-05: Tasks marked done by `guard plan mark-done`.
- 2026-09-05: Tasks marked done by `guard plan mark-done`.
- 2026-09-05: Tasks marked done by `guard plan mark-done`.
- 2026-09-05: Tasks marked done by `guard plan mark-done`.
- 2026-09-05: Tasks marked done by `guard plan mark-done`.
- 2026-09-05: Tasks marked done by `guard plan mark-done`.
- 2026-09-05: Tasks marked done by `guard plan mark-done`.
- 2026-09-05: Tasks marked done by `guard plan mark-done`.
- 2026-09-05: Status changed from `READY` to `IN PROGRESS` by `guard plan set-status`.
- 2026-09-05: Status changed from `BRAINSTORMING` to `READY` by `guard plan set-status`.
- 2026-09-05: Guard review receipts cleared by `guard plan set-section` because section `Open questions` changed.
- 2026-09-05: Task added by `guard plan add-task`.
- 2026-09-05: Guard review receipts cleared by `guard plan set-section` because section `The idea` changed.
- 2026-09-05: Priority set to 3/5 by `guard plan set-priority`.
- 2026-09-05: Guard review receipts cleared by `guard plan set-section` because section `Open questions` changed.
- 2026-09-05: Added 9 task(s) by `guard plan add-tasks`.
- 2026-09-05: Guard review receipts cleared by `guard plan set-section` because section `The idea` changed.

## Model authorship
- 2026-09-05: claude-fable-5 — created plan, set section "The idea", added 9 tasks, set section "Open questions", set priority to 3/5, added task, set status to READY, set status to IN PROGRESS, marked tasks done, set status to DONE, re-proofed task evidence
