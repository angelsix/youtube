# Verification rules: repo

Governs: .

Every peer review of a change under this repo carries these rules verbatim, and the reviewer reports each one by its id as a row in its `## Verification` section. Each rule names the SURFACE to drive, the ACTION to perform, what it MUST do, what it MUST NOT do, and the ENVIRONMENT it must hold in. Ids are stable and never reused. Add a rule with `guard docs verification-rules <repo> --add --folder <dir> --surface … --action … --must … --must-not … --env …`; never hand-format one.

Last id: R1

## Rules

R1. Surface: Lab theme classes and example apps (DefaultTheme, Minimal, ThemePlayground)
   Action: Build the lab solution against the locally packed renamed engine and run its tests after the stage-vocabulary sweep and Defaults-first region regroup
   Must: dotnet build of AvaloniaThemeLab.sln exits 0 against the local engine feed; existing tests stay green; every theme class reads Defaults first, then Seeds, then the token regions; XAML tokens like {color:BackgroundDefault} still resolve per hue
   Must not: Any AccentRole, RampStage, region Roles or region Palette left in lab C#; any theme value changed by the regroup; any hard-coded colour introduced
   Environment: desktop; origin=local build; auth=none; request=392cbdd98dc540009d322e8d797a9da1
