# Verification rules: repo

Governs: .

Prove command: ["node","checker/prove.mjs"]
Fault manifest: faults.json

Every peer review of a change under this repo carries these rules verbatim, and the reviewer reports each one by its id as a row in its `## Verification` section. Each rule names the SURFACE to drive, the ACTION to perform, what it MUST do, what it MUST NOT do, and the ENVIRONMENT it must hold in. Ids are stable and never reused. Add a rule with `guard docs verification-rules <repo> --add --folder <dir> --surface … --action … --must … --must-not … --env …`; never hand-format one.

Last id: R7

## Rules

R1. Surface: Lab theme classes and example apps (DefaultTheme, Minimal, ThemePlayground)
   Action: Build the lab solution against the locally packed renamed engine and run its tests after the stage-vocabulary sweep and Defaults-first region regroup
   Must: dotnet build of AvaloniaThemeLab.sln exits 0 against the local engine feed; existing tests stay green; every theme class reads Defaults first, then Seeds, then the token regions; XAML tokens like {color:BackgroundDefault} still resolve per hue
   Must not: Any AccentRole, RampStage, region Roles or region Palette left in lab C#; any theme value changed by the regroup; any hard-coded colour introduced
   Environment: desktop; origin=local build; auth=none; request=392cbdd98dc540009d322e8d797a9da1

R2. Surface: develop branch of the youtube repository: the idle files AvaloniaThemeLab/Examples/AvaloniaThemeLab.Minimal/Theme/MinimalTheme.cs and AvaloniaThemeLab/Themes/AngelSix.Themes.Default/Theme/DefaultTheme.cs
   Action: Commit the idle work in /Users/lukemalpass/Documents/GitHub/youtube with no help from Luke: commit the idle files that belong together, in sensible commits
   Must: Both idle files are committed on develop on top of 5f9f1f6, no longer show as uncommitted, and every change of the idle work (the Auto Git backup's changes.patch) is in what was committed
   Must not: Delete or revert anybody's work: an idle change dropped or reverted, an idle file left uncommitted, or 5f9f1f6 and the unpushed commits rewritten out of develop
   Environment: git working copy; origin=file:///Users/lukemalpass/Documents/GitHub/youtube; auth=none; request=b3db58e842394721bf0f511146fc6bdf
   Proof origin: file:///Users/lukemalpass/Documents/GitHub/youtube
   Proof auth: none
   Mutates: yes
   Proof write: commit the idle files

R3. Surface: the files in the new develop commits after 5f9f1f6
   Action: Commit with plain git commit -m MSG -- <files>, naming the files (never git commit -a); leave out a file another live chat is still changing
   Must: The new commits contain only the idle files (plus at most a narrow .gitignore entry); every other change in the tree is left exactly as it was
   Must not: Commit secrets (keys, tokens, passwords, .env files, credentials), build output, logs, scratch or temporary files, or any file other than the idle work
   Environment: git working copy; origin=file:///Users/lukemalpass/Documents/GitHub/youtube; auth=none; request=b3db58e842394721bf0f511146fc6bdf
   Proof origin: file:///Users/lukemalpass/Documents/GitHub/youtube
   Proof auth: none
   Mutates: yes
   Proof write: commit the idle files

R4. Surface: the messages and author of the new develop commits after 5f9f1f6
   Action: Commit the idle files in sensible commits with plain, specific messages; commit as Luke
   Must: Each new commit is authored by Luke Malpass and its subject plainly names what changed in the theme files, not a placeholder such as wip, update or auto commit
   Must not: AI attribution of any kind in a message: no Co-Authored-By, no Generated with, no robot emoji, no Claude, Codex, AI or agent mention
   Environment: git working copy; origin=file:///Users/lukemalpass/Documents/GitHub/youtube; auth=none; request=b3db58e842394721bf0f511146fc6bdf
   Proof origin: file:///Users/lukemalpass/Documents/GitHub/youtube
   Proof auth: none
   Mutates: yes
   Proof write: commit the idle files

R5. Surface: the repository's pre-commit hook (guard repo staged) on the new develop commits
   Action: Commit the idle files with the pre-commit hook passing; if the pre-commit hook refuses, fix the cause in the files being committed and commit again
   Must: Each new commit's change, staged on its parent, is accepted by the repository's own pre-commit hook
   Must not: A commit the pre-commit hook refuses, or the hook skipped (--no-verify) to land a change it would block
   Environment: git working copy; origin=file:///Users/lukemalpass/Documents/GitHub/youtube; auth=none; request=b3db58e842394721bf0f511146fc6bdf
   Proof origin: file:///Users/lukemalpass/Documents/GitHub/youtube
   Proof auth: none
   Mutates: yes
   Proof write: commit the idle files

R6. Surface: develop on the GitHub remote https://github.com/angelsix/youtube.git
   Action: Do not push and do not run guard repo all: Auto push is off for this folder; force pushes stay refused
   Must: The new commits sit on local develop only: the remote develop does not contain them and still has 42bc9fd in its history
   Must not: A push or force push of develop, or the remote's history rewritten
   Environment: git working copy and its remote; origin=file:///Users/lukemalpass/Documents/GitHub/youtube; auth=none; request=b3db58e842394721bf0f511146fc6bdf
   Proof origin: file:///Users/lukemalpass/Documents/GitHub/youtube
   Proof auth: none
   Mutates: no

R7. Surface: the ToggleSwitch control surface, Themes/AngelSix.Themes.Default/Controls/ToggleSwitch.axaml, shot through the lab's own preview of the control (theme-lab-shot.sh ToggleSwitch) with nothing added
   Action: Look at the switch's knob in the unchecked and the checked state, at ThicknessScale 1 and 6, in light and dark, and build plus test the lab solution
   Must: The knob shows exactly one clean edge in both states at every scale: one stroke or one fill meeting the track, no second concentric arc round the knob, no backdrop colour visible between the knob and the track edge; dot and track keep their existing geometry (40x20 track, 10x10 dot at scale 1) and the lab builds with 0 errors and its tests stay green; every part name still present and addressable (OuterBorder, SwitchKnobBounds, PART_KnobContent, SwitchKnobOn, SwitchKnobOff, PART_SwitchKnob)
   Must not: Any layout hack or restyle in a consumer app or the demo to hide it, any part deleted or renamed to dodge the styles, or any other control's appearance changed
   Environment: desktop; origin=local build of AvaloniaThemeLab.sln against the local theme-feed pack; auth=none
